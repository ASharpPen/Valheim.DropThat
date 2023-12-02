using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DropThat.Drop.DropTableSystem.Models;
using DropThat.Drop.DropTableSystem.Services;
using DropThat.Drop.DropTableSystem.Wrapper;
using DropThat.Drop.Options;
using ThatCore.Cache;
using ThatCore.Extensions;
using ThatCore.Logging;
using UnityEngine;

namespace DropThat.Drop.DropTableSystem.Managers;

/// <summary>
/// Logic for general workflow surrounding configuring
/// drop table, running conditions and applying modifiers.
/// </summary>
internal static class DropTableManager
{
    public static ManagedCache<GameObject> DropTableInstances { get; } = new();

    private static ConditionalWeakTable<DropTable, GameObject> SourceLinkTable { get; } = new();
    private static ConditionalWeakTable<DropTable, DropTableTemplate> TemplateLinkTable { get; } = new();
    private static ConditionalWeakTable<DropTable, List<DropTableDrop>> DropsByTable { get; } = new();

    /// <summary>
    /// Initialize references from drop table to source.
    /// </summary>
    public static void Initialize(MonoBehaviour source, DropTable dropTable)
    {
        try
        {
            if (dropTable is null ||
                source.IsNull())
            {
                Log.Development?.Log("DropTable or Source is null");
                return;
            }

            if (SourceLinkTable.TryGetValue(dropTable, out _))
            {
                return;
            }

            DropTableInstances.Set(source.gameObject, source.gameObject);
            SourceLinkTable.Add(dropTable, source.gameObject);

            if (DropTableTemplateManager.TryGetTemplate(source.GetCleanedName(), out var template))
            {
                TemplateLinkTable.Remove(dropTable);
                TemplateLinkTable.Add(dropTable, template);

                PrepareTable(dropTable);
            }
        }
        catch (Exception e)
        {
            Log.Error?.Log($"Error while attempting to store reference from drop table to its source.", e);
        }
    }

    public static bool HasChanges(DropTable dropTable)
    {
        return TemplateLinkTable.TryGetValue(dropTable, out _);
    }

    /// <summary>
    /// Overhaul of vanilla drop generation for GetItemDrops.
    /// </summary>
    public static List<ItemDrop.ItemData> GenerateItemDrops(DropTable dropTable)
    {
        if (dropTable is null)
        {
            return new();
        }

        List<DropTableDrop> drops;

        if (!DropsByTable.TryGetValue(dropTable, out drops))
        {
            Log.Warning?.Log("Attempted to generate drops without having prepared DropTable. Attempting recovery, but something is wrong.");

            drops = PrepareTable(dropTable);
        }

        // Roll drops
        if (!SourceLinkTable.TryGetValue(dropTable, out var source))
        {
            // Something is wrong. We shouldn't be trying to overhaul drop generation without droptable source being linked.
            return new();
        }

        var rolledDrops = DropRollerService.RollDrops(dropTable, source, drops);

        if (Log.TraceEnabled)
        {
            Log.Trace?.Log($"Dropping {drops.Count} items:");
            foreach (var drop in drops)
            {
                Log.Trace?.Log($"\t{drop.DropData.m_item.name}");
            }
        }

        // Apply modifiers, roll/scale drop amount and finalize results as ItemData.
        var convertedDrops = drops.SelectMany(drop =>
            DropScalerService.ScaleDropsAsItemData(source, drop));

        return convertedDrops
            .Where(x => x is not null)
            .ToList();
    }

    /// <summary>
    /// Overhaul of vanilla drop generation for GetDrops.
    /// </summary>
    public static List<GameObject> GenerateDrops(DropTable dropTable)
    {
        if (dropTable is null)
        {
            return new();
        }

        List<DropTableDrop> drops;

        if (!DropsByTable.TryGetValue(dropTable, out drops))
        {
            Log.Warning?.Log("Attempted to generate drops without having prepared DropTable. Attempting recovery, but something is wrong.");

            drops = PrepareTable(dropTable);
        }

        // Roll drops
        if (!SourceLinkTable.TryGetValue(dropTable, out var source))
        {
            // Something is wrong. We shouldn't be trying to overhaul drop generation without droptable source being linked.
            return new();
        }

        var rolledDrops = DropRollerService.RollDrops(dropTable, source, drops);

        // Convert to GameObject.
        // In vanilla, these are the prefabs referenced by the ItemDrop.
        var convertedDrops = rolledDrops.SelectMany(DropScalerService.ScaleDropsAsGameObjects);

        return convertedDrops
            .Where(x => x is not null)
            .ToList();
    }

    private static List<DropTableDrop> PrepareTable(DropTable dropTable)
    {
        // Find configs
        DropTableTemplate template;

        if (!TemplateLinkTable.TryGetValue(dropTable, out template))
        {
            Log.Warning?.Log("Attempted to generate drops without having template linked to DropTable.");
            // Something is wrong. We shouldn't be trying to overhaul drop generation without a template with changes being linked.
            return new(0);
        }

        // Configure table
        ConfigureDropTableService.ConfigureTable(dropTable, template);

        // Create list of configured drops for table.
        var drops = ConfigureDropTableService.CreateDropList(dropTable, template);

        DropsByTable.Remove(dropTable);
        DropsByTable.Add(dropTable, drops);

        return drops;
    }

    /// <summary>
    /// Short-term state between <see cref="UnwrapDrop(GameObject)"/> and <see cref="Modify"/>.
    /// </summary>
    private static GameObject _currentWrapped;

    /// <summary>
    /// Unwrap GameObject is possible, in preparation for instantiation of drop.
    /// 
    /// Wrapping is done during generation of drop list, and consists of wrapping up the prefab
    /// that is intended to get dropped, in a custom GameObject that we can trace through
    /// the code flow. Unnwrapping involves replacing said custom GameObject on the stack with
    /// the prefab it wraps, while storing the trackable reference of the wrapper for operations
    /// slighty further ahead in the workflow.
    /// </summary>
    public static GameObject UnwrapDrop(GameObject wrappedDrop)
    {
        try
        {
            _currentWrapped = wrappedDrop;

            return wrappedDrop.Unwrap();
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error while attempting to unwrap drop", e);
            return wrappedDrop;
        }
    }

    /// <summary>
    /// Modify dropped object after it has been instantiated.
    /// </summary>
    public static void ModifyInstantiatedDrop(GameObject drop)
    {
        try
        {
            if (WrapperCache.TryGet(_currentWrapped, out var cache) &&
                cache.Wrapper.Drop?.DropTemplate is not null)
            {
                ItemModifierContext<GameObject> dropContext = new()
                {
                    Item = drop,
                    Position = drop.transform.position,
                };

                cache.Wrapper.Drop.DropTemplate.ItemModifiers.ForEach(modifier =>
                {
                    try
                    {
                        modifier.Modify(dropContext);
                    }
                    catch (Exception e)
                    {
                        Log.Error?.Log($"Error while attempting to apply modifier '{modifier.GetType().Name}' to drop '{drop}'. Skipping modifier.", e);

                    }
                });
            }
        } 
        catch (Exception e)
        {
            Log.Error?.Log($"Error while preparing to modify drop '{drop}'. Skipping modifiers.", e);
        }
    }
}

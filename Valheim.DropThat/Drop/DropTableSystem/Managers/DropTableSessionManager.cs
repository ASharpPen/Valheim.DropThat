using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DropThat.Drop.DropTableSystem.Models;
using DropThat.Drop.DropTableSystem.Services;
using DropThat.Drop.Options;
using ThatCore.Cache;
using ThatCore.Extensions;
using ThatCore.Logging;
using UnityEngine;
using static CharacterDrop;

namespace DropThat.Drop.DropTableSystem.Managers;

/// <summary>
/// Logic for general workflow surrounding configuring
/// drop table, running conditions and applying modifiers.
/// </summary>
public static class DropTableSessionManager
{
    public static ManagedCache<GameObject> DropTableInstances { get; } = new();

    private static ConditionalWeakTable<DropTable, GameObject> SourceLinkTable { get; } = new();
    private static ConditionalWeakTable<DropTable, DropTableTemplate> TemplateLinkTable { get; } = new();
    private static ConditionalWeakTable<DropTable, List<DropTableDrop>> DropsByTable { get; } = new();

    private static ConditionalWeakTable<DropTable, List<DropTableDrop>> SessionDrops { get; } = new();

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
            Log.Trace?.Log($"Dropping {rolledDrops.Count} items:");
            foreach (var drop in rolledDrops)
            {
                Log.Trace?.Log($"\t{drop.DropData.m_item.name}");
            }
        }

        // Apply modifiers, roll/scale drop amount and finalize results as ItemData.
        var convertedDrops = rolledDrops
            .SelectMany(drop => DropScalerService.ScaleDropsAsItemData(source, drop))
            .Where(x => x is not null)
            .ToList();

        return convertedDrops;
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
        List<DropTableDrop> dropConfigs = [];

        var convertedDrops = rolledDrops
            .SelectMany(DropScalerService.ScaleDropsAsGameObjects)
            .Where(x => x is not null)
            .ToList();

        if (SessionDrops.TryGetValue(dropTable, out _))
        {
            // This is probably not likely, but just in case the same droptable is rolled multiple times, make sure to clean up cache.
            SessionDrops.Remove(dropTable);
        }

        SessionDrops.Add(dropTable, dropConfigs);

        return convertedDrops;
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
    /// Modify dropped object after it has been instantiated.
    /// Note, this is not relevant when using ItemDrop.ItemData, since the modifiers are already applied for the ItemData itself.
    /// </summary>
    public static void ModifyInstantiatedObjectDrop(GameObject drop, DropTable dropTable, int index)
    {
        try
        {
            if (index >= 0 &&
               SessionDrops.TryGetValue(dropTable, out var configs) &&
               index < configs.Count)
            {
                var config = configs[index];

                ItemModifierContext<GameObject> dropContext = new()
                {
                    Item = drop,
                    Position = drop.transform.position,
                };

                config.DropTemplate?.ItemModifiers?.ForEach(modifier =>
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

    public static void Cleanup(DropTable dropTable)
    {
        try
        {
            if (dropTable is not null)
            {
                SessionDrops.Remove(dropTable);
            }
        }
        catch (Exception e)
        {
            Log.Error?.Log($"Error while cleaning up DropTable.", e);
        }
    }
}

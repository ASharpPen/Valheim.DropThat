using System;
using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Models;
using DropThat.Drop.DropTableSystem.Services;
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
public static class DropTableSessionManager
{
    internal static ManagedCache<GameObject> DropTableInstances { get; } = new();
    private static ManagedCache<DropSession> Sessions { get; } = new();
    private static MonoBehaviour SessionEntity { get; set; }
    private static List<DropTableDrop> SessionDrops { get; set; }

    private class DropSession
    {
        public DropTableTemplate Template { get; set; }

        public List<DropTableDrop> Table { get; set; }
    }

    /// <summary>
    /// Initialize and prepare table.
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

            DropTableInstances.Set(source.gameObject, source.gameObject);

            if (DropTableTemplateManager.TryGetTemplate(source.GetCleanedName(), out var template))
            {
                Sessions.Set(source, new()
                {
                    Template = template,
                    Table = PrepareTable(dropTable, template)
                });
            }
        }
        catch (Exception e)
        {
            Log.Error?.Log($"Error while attempting to initialize DropTable configuration.", e);
        }
    }

    public static bool HasChanges()
    {
        if (SessionEntity.IsNotNull())
        {
            return Sessions.TryGet(SessionEntity, out _);
        }

        return false;
    }

    public static void StartSession(MonoBehaviour source)
    {
        SessionEntity = source;
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

        if (SessionEntity.IsNull())
        {
            // Something is wrong. We shouldn't be trying to overhaul drop generation without droptable source being linked.
            return new();
        }

        List<DropTableDrop> drops = Sessions.TryGet(SessionEntity, out var session)
            ? session.Table
            : null;

        if (drops is null)
        {
            Log.Warning?.Log($"{SessionEntity.GetCleanedName()}: Attempted to generate drops without having prepared DropTable. Attempting recovery, but something is wrong.");

            if (DropTableTemplateManager.TryGetTemplate(SessionEntity.GetCleanedName(), out var template))
            {
                Sessions.Set(
                    SessionEntity,
                    new DropSession()
                    {
                        Template = template,
                        Table = drops = PrepareTable(dropTable, template)
                    });
            }
            else
            {
                // Recovery failed or no templates found. Prepare using default drops in table.
                drops = PrepareTable(dropTable, new());
            }
        }

        // Roll drops
        var rolledDrops = DropRollerService.RollDrops(dropTable, SessionEntity.gameObject, drops);

        if (Log.TraceEnabled)
        {
            Log.Trace?.Log($"{SessionEntity.GetCleanedName()}: Dropping {rolledDrops.Count} items");
            foreach (var drop in rolledDrops)
            {
                Log.Trace?.Log($"\t{drop.DropData.m_item.name}");
            }
        }

        // Apply modifiers, roll/scale drop amount and finalize results as ItemData.
        var convertedDrops = rolledDrops
            .SelectMany(drop => DropScalerService.ScaleDropsAsItemData(SessionEntity.gameObject, drop))
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

        if (SessionEntity.IsNull())
        {
            // Something is wrong. We shouldn't be trying to overhaul drop generation without droptable source being linked.
            return new();
        }

        List<DropTableDrop> drops = Sessions.TryGet(SessionEntity, out var session)
            ? session.Table
            : null;

        if (drops is null)
        {
            Log.Warning?.Log($"{SessionEntity.GetCleanedName()}: Attempted to generate drops without having prepared DropTable. Attempting recovery, but something is wrong.");

            if (DropTableTemplateManager.TryGetTemplate(SessionEntity.GetCleanedName(), out var template))
            {
                Sessions.Set(
                    SessionEntity,
                    new()
                    {
                        Template = template,
                        Table = drops = PrepareTable(dropTable, template)
                    });
            }
            else
            {
                // Recovery failed or no templates found. Prepare using default drops in table.
                drops = PrepareTable(dropTable, new());
            }
        }

        // Roll drops
        var rolledDrops = DropRollerService.RollDrops(dropTable, SessionEntity.gameObject, drops);

        // Convert to GameObject.
        var convertedDrops = rolledDrops
            .SelectMany(DropScalerService.ScaleDropsAsGameObjects)
            .Where(x => x.IsNotNull())
            .ToList();

        if (Log.TraceEnabled)
        {
            Log.Trace?.Log($"Dropping {rolledDrops.Count} items:");
            foreach (var drop in rolledDrops)
            {
                Log.Trace?.Log($"\t{drop.DropData.m_item.name}");
            }
        }

        SessionDrops = rolledDrops;

        return convertedDrops;
    }

    private static List<DropTableDrop> PrepareTable(DropTable dropTable, DropTableTemplate template)
    {
        // Configure table
        ConfigureDropTableService.ConfigureTable(dropTable, template);

        // Create list of configured drops for table.
        var drops = ConfigureDropTableService.CreateDropList(dropTable, template);

        return drops;
    }

    /// <summary>
    /// Modify dropped object after it has been instantiated.
    /// Note, this is not relevant when using ItemDrop.ItemData, since the modifiers are already applied for the ItemData itself.
    /// </summary>
    /// <param name="drop">New instantiated instance of the drop.</param>
    /// <param name="index">Index of drop in the list returned from DropTable.GetDropList.</param>
    public static void ModifyDrop(GameObject drop, int index)
    {
        try
        {
            if (index >= 0 &&
               index < SessionDrops?.Count)
            {
                var config = SessionDrops[index];

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

    public static void EndSession()
    {
        try
        {
            SessionEntity = null;
            SessionDrops = null;
        }
        catch (Exception e)
        {
            Log.Error?.Log($"Error while cleaning up DropTable.", e);
        }
    }
}

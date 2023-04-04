using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropThat.Drop.DropTableSystem.Caches;
using DropThat.Drop.DropTableSystem.Models;
using DropThat.Drop.DropTableSystem.Services;
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
    private static Dictionary<DropTable, GameObject> SourceLinkTable { get; } = new();
    private static Dictionary<DropTable, DropTableTemplate> TemplateLinkTable { get; } = new();

    /// <summary>
    /// Step 1 - Initialize references from table to source.
    /// </summary>
    public static void Initialize(MonoBehaviour source, DropTable dropTable)
    {
        try
        {
            if (dropTable is null ||
                source.IsNull())
            {
                return;
            }

            SourceLinkTable[dropTable] = source.gameObject;

            if (DropTableTemplateManager.TryGetTemplate(source.GetCleanedName(), out var template))
            {
                TemplateLinkTable[dropTable] = template;
            }
        }
        catch (Exception e)
        {
            Log.Error?.Log($"Error while attempting to store reference from drop table to its source.", e);
        }
    }

    public static bool HasChanges(DropTable dropTable)
    {
        return TemplateLinkTable.ContainsKey(dropTable);
    }

    /// <summary>
    /// Step 2a - Overhaul of vanilla drop generation for GetItemDrops.
    /// </summary>
    public static List<ItemDrop.ItemData> GenerateItemDrops(DropTable dropTable)
    {
        if (dropTable is null)
        {
            return new();
        }

        PrepareTable(dropTable);

        // Roll drops
        if (!SourceLinkTable.TryGetValue(dropTable, out var source))
        {
            // Something is wrong. We shouldn't be trying to overhaul drop generation without droptable source being linked.
            return new();
        }

        return DropRollerService.RollItemDrops(dropTable, source);
    }

    /// <summary>
    /// Step 2b - Overhaul of vanilla drop generation for GetDrops.
    /// </summary>
    public static List<GameObject> GenerateDrops(DropTable dropTable)
    {
        if (dropTable is null)
        {
            return new();
        }

        PrepareTable(dropTable);

        // Roll drops
        if (!SourceLinkTable.TryGetValue(dropTable, out var source))
        {
            // Something is wrong. We shouldn't be trying to overhaul drop generation without droptable source being linked.
            return new();
        }

        return DropRollerService.RollDrops(dropTable, source);
    }

    private static void PrepareTable(DropTable dropTable)
    {
        // Find configs
        DropTableTemplate template;

        if (!TemplateLinkTable.TryGetValue(dropTable, out template))
        {
            // Something is wrong. We shouldn't be trying to overhaul drop generation without a template with changes being linked.
            return;
        }

        // Configure table
        ConfigureDropTableService.ConfigureTable(dropTable, template);

        // Configure drops
        ConfigureDropTableService.ConfigureDrops(dropTable, template);
    }

    /// <summary>
    /// Step 3 - Apply modifiers to created container items.
    /// </summary>
    public static ItemDrop.ItemData ModifyContainerItem(ItemDrop.ItemData item, Container container)
    {
        // TODO: PLACEHOLDER
        return item;
    }

    /// <summary>
    /// Step 4 - Cleanup references
    /// </summary>
    public static void Cleanup(MonoBehaviour source, DropTable dropTable)
    {
        try
        {
            SourceLinkTable.Remove(dropTable);
            TemplateLinkTable.Remove(dropTable);
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error while attempting to clean up reference from drop table to its source.", e);
        }
    }
}

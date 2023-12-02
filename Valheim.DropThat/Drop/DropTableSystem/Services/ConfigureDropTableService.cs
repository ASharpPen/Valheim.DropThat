using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Models;
using DropThat.Utilities;
using ThatCore.Logging;

namespace DropThat.Drop.DropTableSystem.Services;

internal static class ConfigureDropTableService
{
    public static void ConfigureTable(DropTable table, DropTableTemplate template)
    {
        if (template.DropMin is not null)
        {
            table.m_dropMin = template.DropMin.Value;
        }

        if (template.DropMax is not null)
        {
            table.m_dropMax = template.DropMax.Value;
        }

        if (template.DropChance is not null)
        {
            table.m_dropChance = template.DropChance.Value / 100f;
        }

        if (template.DropOnlyOnce is not null)
        {
            table.m_oneOfEach = template.DropOnlyOnce.Value;
        }
    }

    public static List<DropTableDrop> CreateDropList(DropTable table, DropTableTemplate template)
    {
        // Initialize with default drops.
        var defaultDrops = table.m_drops ?? new();

        List<DropTableDrop> drops = defaultDrops
            .Select((x, i) =>
                new DropTableDrop()
                {
                    DropTableIndex = i,
                    CurrentIndex = i,
                    DropData = x,
                    TableData = table,                    
                })
            .ToList();

        foreach (var dropTemplate in template.Drops.OrderBy(x => x.Key))
        {
            if (dropTemplate.Value.TemplateEnabled is not null &&
                dropTemplate.Value.TemplateEnabled == false)
            {
                continue;
            }

            if (dropTemplate.Key < drops.Count &&
                dropTemplate.Key >= 0)
            {
                var existingDropPrefab = defaultDrops[dropTemplate.Key].m_item.GetCleanedName();

                if (!string.IsNullOrEmpty(dropTemplate.Value.PrefabName) &&
                    existingDropPrefab == template.PrefabName)
                {
                    Log.Trace?.Log($"Configuring existing drop '{dropTemplate.Key}:{existingDropPrefab}'.");
                }
                else
                {
                    Log.Trace?.Log($"Configuring and changing existing drop '{dropTemplate.Key}:{existingDropPrefab}' to {dropTemplate.Value.PrefabName}.");
                }

                var drop = drops[dropTemplate.Key];

                drop.DropData = ConfigureDrop(
                     defaultDrops[dropTemplate.Key], 
                     template,
                     dropTemplate.Value);

                drop.TableTemplate = template;
                drop.DropTemplate = dropTemplate.Value;
            }
            else if (dropTemplate.Value.Enabled != false)
            {
                Log.Trace?.Log($"Inserting drop '{dropTemplate.Value.PrefabName}'.");

                if (TryCreateDrop(template, dropTemplate.Value, out var newDrop))
                {
                    drops.Add(new()
                    {
                        DropTableIndex = drops.Count,
                        CurrentIndex = drops.Count,
                        DropData = newDrop,
                        TableData = table,
                        DropTemplate = dropTemplate.Value,
                        TableTemplate = template,
                    });
                }
            }
        }

        return drops;
    }

    private static bool TryCreateDrop(
        DropTableTemplate tableTemplate,
        DropTableDropTemplate dropTemplate,
        out DropTable.DropData drop)
    {
        drop = new()
        {
            m_stackMax = 1,
            m_stackMin = 1,
            m_weight = 1
        };

        if (!string.IsNullOrWhiteSpace(dropTemplate.PrefabName))
        {
            // Try find object
            var prefab = ZNetScene.instance.GetPrefab(dropTemplate.PrefabName);

            if (prefab is null)
            {
                Log.Warning?.Log(
                    $"Unable to find prefab '{dropTemplate.PrefabName}' " +
                    $"for config '{tableTemplate.PrefabName}.{dropTemplate.Id}'. Disabling config.");
                dropTemplate.TemplateEnabled = false;

                return false;
            }

            drop.m_item = prefab;
        }

        if (dropTemplate.AmountMin is not null)
        {
            drop.m_stackMin = dropTemplate.AmountMin.Value;
        }

        if (dropTemplate.AmountMax is not null)
        {
            drop.m_stackMax = dropTemplate.AmountMax.Value;
        }

        if (dropTemplate.Weight is not null)
        {
            drop.m_weight = dropTemplate.Weight.Value;
        }

        if (dropTemplate.DisableResourceModifierScaling is not null)
        {
            drop.m_dontScale = dropTemplate.DisableResourceModifierScaling.Value;
        }

        return true;
    }

    private static DropTable.DropData ConfigureDrop(
        DropTable.DropData drop, 
        DropTableTemplate tableTemplate, 
        DropTableDropTemplate dropTemplate)
    {
        if (!string.IsNullOrWhiteSpace(dropTemplate.PrefabName))
        {
            // Try find object
            var prefab = ZNetScene.instance.GetPrefab(dropTemplate.PrefabName);

            if (prefab is null)
            {
                Log.Warning?.Log(
                    $"Unable to find prefab '{dropTemplate.PrefabName}' " +
                    $"for config '{tableTemplate.PrefabName}.{dropTemplate.Id}'. Disabling config.");
                dropTemplate.TemplateEnabled = false;

                return drop;
            }

            drop.m_item = prefab;
        }

        if (dropTemplate.AmountMin is not null)
        {
            drop.m_stackMin = dropTemplate.AmountMin.Value;
        }

        if (dropTemplate.AmountMax is not null)
        {
            drop.m_stackMax = dropTemplate.AmountMax.Value;
        }

        if (dropTemplate.Weight is not null)
        {
            drop.m_weight = dropTemplate.Weight.Value;
        }

        if (dropTemplate.DisableResourceModifierScaling is not null)
        {
            drop.m_dontScale = dropTemplate.DisableResourceModifierScaling.Value;
        }

        return drop;
    }
}

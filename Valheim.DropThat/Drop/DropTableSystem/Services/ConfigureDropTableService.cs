using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Caches;
using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Logging;

namespace DropThat.Drop.DropTableSystem.Services;

internal static class ConfigureDropTableService
{
    public static void ConfigureTable(DropTable table, DropTableTemplate template)
    {
        if (template.DropMin.IsSet &&
            template.DropMin.Value is not null)
        {
            table.m_dropMin = template.DropMin.Value.Value;
        }

        if (template.DropMax.IsSet &&
            template.DropMax.Value is not null)
        {
            table.m_dropMax = template.DropMax.Value.Value;
        }

        if (template.DropChance.IsSet &&
            template.DropChance.Value is not null)
        {
            table.m_dropChance = template.DropChance.Value.Value / 100f;
        }

        if (template.DropOnlyOnce.IsSet &&
            template.DropOnlyOnce.Value is not null)
        {
            table.m_oneOfEach = template.DropOnlyOnce.Value.Value;
        }
    }

    public static List<DropTableDrop> CreateDropList(DropTable table, DropTableTemplate template)
    {
        // Initialize with default drops.
        List<DropTableDrop> drops = table.m_drops
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
                var drop = drops[dropTemplate.Key];

                drop.DropData = ModifyDrop(
                     table.m_drops[dropTemplate.Key], 
                     template,
                     dropTemplate.Value);

                drop.TableTemplate = template;
                drop.DropTemplate = dropTemplate.Value;

                DropLinkCache.SetLink(table, dropTemplate.Key, new()
                {
                    Table = template,
                    Drop = dropTemplate.Value
                });
            }
            else
            {
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

                DropLinkCache.SetLink(table, table.m_drops.Count - 1, new()
                {
                    Table = template,
                    Drop = dropTemplate.Value
                });
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

        if (dropTemplate.PrefabName.IsSet &&
            !string.IsNullOrWhiteSpace(dropTemplate.PrefabName))
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

        if (dropTemplate.AmountMin.IsSet &&
            dropTemplate.AmountMin.Value is not null)
        {
            drop.m_stackMin = dropTemplate.AmountMin.Value.Value;
        }

        if (dropTemplate.AmountMax.IsSet &&
            dropTemplate.AmountMax.Value is not null)
        {
            drop.m_stackMax = dropTemplate.AmountMax.Value.Value;
        }

        if (dropTemplate.Weight.IsSet &&
            dropTemplate.Weight.Value is not null)
        {
            drop.m_weight = dropTemplate.Weight.Value.Value;
        }

        return true;
    }

    private static DropTable.DropData ModifyDrop(
        DropTable.DropData drop, 
        DropTableTemplate tableTemplate, 
        DropTableDropTemplate dropTemplate)
    {
        if (dropTemplate.PrefabName.IsSet &&
            !string.IsNullOrWhiteSpace(dropTemplate.PrefabName))
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

        if (dropTemplate.AmountMin.IsSet &&
            dropTemplate.AmountMin.Value is not null)
        {
            drop.m_stackMin = dropTemplate.AmountMin.Value.Value;
        }

        if (dropTemplate.AmountMax.IsSet &&
            dropTemplate.AmountMax.Value is not null)
        {
            drop.m_stackMax = dropTemplate.AmountMax.Value.Value;
        }

        if (dropTemplate.Weight.IsSet &&
            dropTemplate.Weight.Value is not null)
        {
            drop.m_weight = dropTemplate.Weight.Value.Value;
        }

        return drop;
    }
}

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

    public static void ConfigureDrops(DropTable table, DropTableTemplate template)
    {
        table.m_drops ??= new();

        foreach (var drop in template.Drops.OrderBy(x => x.Key))
        {
            if (drop.Value.TemplateEnabled is not null &&
                drop.Value.TemplateEnabled == false)
            {
                continue;
            }

            if (drop.Key < table.m_drops.Count &&
                drop.Key >= 0)
            {
                 table.m_drops[drop.Key] = ModifyDrop(
                     table.m_drops[drop.Key], 
                     template,
                     drop.Value);

                DropLinkCache.SetLink(table, drop.Key, new()
                {
                    Table = template,
                    Drop = drop.Value
                });
            }
            else
            {
                var newDrop = CreateDrop(template, drop.Value);

                if (newDrop is not null)
                {
                    table.m_drops.Add(newDrop.Value);
                }

                DropLinkCache.SetLink(table, table.m_drops.Count - 1, new()
                {
                    Table = template,
                    Drop = drop.Value
                });
            }
        }
    }

    private static DropTable.DropData? CreateDrop(
        DropTableTemplate tableTemplate,
        DropTableDropTemplate dropTemplate)
    {
        DropTable.DropData drop = new()
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

                return null;
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

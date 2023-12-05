using System;
using System.Collections.Generic;
using System.Linq;
using DropThat.Caches;
using DropThat.Drop.DropTableSystem.Managers;
using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Extensions;
using UnityEngine;

namespace DropThat.Drop.DropTableSystem.Services;

internal static class DropTableCompiler
{
    public static List<DropTableTemplate> CompileAllDrops(
        bool applyTemplate = true)
    {
        if (ZNetScene.instance.IsNull())
        {
            throw new InvalidOperationException("Too early. Trying to get expected drops before ZnetScene is instantiated.");
        }

        List<DropTableTemplate> results = new();

        // Scan ZNetScene for prefabs with drop tables.
        foreach (var prefab in ZNetScene.instance.m_prefabs)
        {
            if (TryCompileDrops(prefab, applyTemplate, out var result))
            {
                results.Add(result);
            }
        }

        return results;
    }

    public static bool TryCompileDrops(
        string prefabName, 
        bool applyTemplate,
        out DropTableTemplate compiledDrops)
    {
        if (ZNetScene.instance.IsNull())
        {
            throw new InvalidOperationException("Too early. Trying to get expected drops before ZnetScene is instantiated.");
        }

        var prefab = ZNetScene.instance.GetPrefab(prefabName);

        if (prefab.IsNull())
        {
            compiledDrops = null;
            return false;
        }

        return TryCompileDrops(prefab, applyTemplate, out compiledDrops);
    }

    public static bool TryCompileDrops(
        GameObject prefab, 
        bool applyTemplate,
        out DropTableTemplate compiledDrops)
    {
        DropTable dropTable = null;

        if (ComponentCache.TryGet<Container>(prefab, out var container))
        {
            dropTable = container.m_defaultItems;
        }
        else if (ComponentCache.TryGet<DropOnDestroyed>(prefab, out var dropOnDestroyed))
        {
            dropTable = dropOnDestroyed.m_dropWhenDestroyed;
        }
        else if (ComponentCache.TryGet<LootSpawner>(prefab, out var lootSpawner))
        {
            dropTable = lootSpawner.m_items;
        }
        else if (ComponentCache.TryGet<TreeBase>(prefab, out var treeBase))
        {
            dropTable = treeBase.m_dropWhenDestroyed;
        }
        else if (ComponentCache.TryGet<TreeLog>(prefab, out var treeLog))
        {
            dropTable = treeLog.m_dropWhenDestroyed;
        }
        else if (ComponentCache.TryGet<MineRock>(prefab, out var mineRock))
        {
            dropTable = mineRock.m_dropItems;
        }
        else if (ComponentCache.TryGet<MineRock5>(prefab, out var mineRock5))
        {
            dropTable = mineRock5.m_dropItems;
        }

        if (dropTable is null)
        {
            compiledDrops = null;
            return false;
        }

        compiledDrops = CompileDrops(prefab.GetCleanedName(), applyTemplate, dropTable);
        return true;
    }

    private static DropTableTemplate CompileDrops(
        string prefabName,
        bool applyTemplate,
        DropTable table)
    {
        DropTableTemplate resultTemplate = new DropTableTemplate
            {
                PrefabName = prefabName,
                DropChance = table.m_dropChance,
                DropMin = table.m_dropMin,
                DropMax = table.m_dropMax,
                DropOnlyOnce = table.m_oneOfEach,
                Drops = table.m_drops
                    .Select((x, i) =>
                        new DropTableDropTemplate()
                        {
                            Id = i,
                            PrefabName = x.m_item.GetCleanedName(),
                            AmountMin = x.m_stackMin,
                            AmountMax = x.m_stackMax,
                            Weight = x.m_weight,
                            DisableResourceModifierScaling = x.m_dontScale,
                        })
                    .ToDictionary(x => x.Id)
            };

        if (!applyTemplate ||
            !DropTableTemplateManager.TryGetTemplate(prefabName, out var template))
        {
            return resultTemplate;
        }

        foreach (var entry in template.Drops.OrderBy(x => x.Key))
        {
            var dropTemplate = entry.Value;

            if (dropTemplate.TemplateEnabled == false)
            {
                continue;
            }

            if (resultTemplate.Drops.TryGetValue(entry.Key, out var existing))
            {
                // Update existing
                existing.PrefabName = dropTemplate.PrefabName ?? existing.PrefabName;
                existing.Enabled = dropTemplate.Enabled ?? existing.Enabled;
                existing.AmountMin = dropTemplate.AmountMin ?? existing.AmountMin;
                existing.AmountMax = dropTemplate.AmountMax ?? existing.AmountMax;
                existing.Weight = dropTemplate.Weight ?? existing.Weight;
                existing.DisableResourceModifierScaling = dropTemplate.DisableResourceModifierScaling ?? existing.DisableResourceModifierScaling;

                existing.ItemModifiers = dropTemplate.ItemModifiers.ToList();
                existing.Conditions = dropTemplate.Conditions.ToList();
            }
            else if (dropTemplate.Enabled != false)
            {
                // Add new
                resultTemplate.Drops[entry.Key] = new()
                {
                    PrefabName = dropTemplate.PrefabName,
                    AmountMin = dropTemplate.AmountMin,
                    AmountMax = dropTemplate.AmountMax,
                    Weight = dropTemplate.Weight,
                    DisableResourceModifierScaling = dropTemplate.DisableResourceModifierScaling,

                    ItemModifiers = dropTemplate.ItemModifiers.ToList(),
                    Conditions = dropTemplate.Conditions.ToList(),
                };
            }
        }

        return resultTemplate;
    }
}

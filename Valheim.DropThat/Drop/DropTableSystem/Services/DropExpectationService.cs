using System;
using System.Collections.Generic;
using System.Linq;
using DropThat.Caches;
using DropThat.Drop.DropTableSystem.Managers;
using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Extensions;
using UnityEngine;

namespace DropThat.Drop.DropTableSystem.Services;

/// <summary>
/// Service for estimating expected drops from a prefab,
/// given the currently loaded templates.
/// 
/// This might differ from the actual drops applied at run-time,
/// since mods might be adding drops on awake / start that would
/// not be caught here.
/// 
/// This is mostly implemented with VNEI integration in mind,
/// to allow for an easier time of extracting the drop tables.
/// </summary>
internal static class DropExpectationService
{
    public static Dictionary<string, ExpectedDropTable> AllExpectedDrops()
    {
        if (ZNetScene.instance.IsNull())
        {
            throw new InvalidOperationException("Too early. Trying to get expected drops before ZnetScene is instantiated.");
        }

        Dictionary<string, ExpectedDropTable> expectedDropsByPrefab = new();

        // Scan ZNetScene for prefabs with drop tables.
        foreach (var prefab in ZNetScene.instance.m_prefabs)
        {
            if (TryGetExpectedDrops(prefab, out var result))
            {
                expectedDropsByPrefab[prefab.GetCleanedName()] = result;
            }
        }

        return expectedDropsByPrefab;
    }

    public static bool TryGetExpectedDrops(string prefabName, out ExpectedDropTable expectedDropTable)
    {
        if (ZNetScene.instance.IsNull())
        {
            throw new InvalidOperationException("Too early. Trying to get expected drops before ZnetScene is instantiated.");
        }

        var prefab = ZNetScene.instance.GetPrefab(prefabName);

        if (prefab.IsNull())
        {
            expectedDropTable = null;
            return false;
        }

        return TryGetExpectedDrops(prefab, out expectedDropTable);
    }

    public static bool TryGetExpectedDrops(GameObject prefab, out ExpectedDropTable expectedDropTable)
    {
        DropTable? dropTable = null;

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
            expectedDropTable = null;
            return false;
        }

        var prefabName = prefab.GetCleanedName();

        expectedDropTable = DropTableTemplateManager.TryGetTemplate(prefabName, out var template)
            ? GenerateExpectedDrops(prefabName, dropTable, template)
            : GenerateDefaultDrops(prefabName, dropTable);

        return true;
    }

    private static ExpectedDropTable GenerateExpectedDrops(
        string prefabName, 
        DropTable table, 
        DropTableTemplate template)
    {
        List<DropTable.DropData> drops = table.m_drops?.ToList() ?? new();

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
                var existingDropPrefab = drops[dropTemplate.Key].m_item.GetCleanedName();

                var drop = drops[dropTemplate.Key];

                if (TryCreateDrop(
                     drops[dropTemplate.Key],
                     dropTemplate.Value,
                     out var newDrop))
                {
                    drops[dropTemplate.Key] = newDrop;
                }
            }
            else if (dropTemplate.Value.Enabled != false)
            {
                if (TryCreateDrop(
                    dropTemplate.Value, 
                    out var newDrop))
                {
                    drops.Add(newDrop);
                }
            }
        }

        return new ExpectedDropTable()
        {
            PrefabName = prefabName,
            DropChance = template.DropChance ?? table.m_dropChance,
            DropMax = template.DropMax ?? table.m_dropMax,
            DropMin = template.DropMin ?? table.m_dropMin,
            OneOfEach = template.DropOnlyOnce ?? table.m_oneOfEach,
            Drops = drops,
            IsModified = true,
        };
    }

    private static ExpectedDropTable GenerateDefaultDrops(
        string prefabName, 
        DropTable table)
    {
        return new ExpectedDropTable()
        {
            PrefabName = prefabName,
            DropChance = table.m_dropChance,
            DropMax = table.m_dropMax,
            DropMin = table.m_dropMin,
            OneOfEach = table.m_oneOfEach,
            Drops = table.m_drops?.ToList() ?? new(0),
        };
    }

    private static bool TryCreateDrop(
        DropTableDropTemplate dropTemplate,
        out DropTable.DropData drop)
    {
        drop = new()
        {
            m_stackMin = dropTemplate.AmountMin ?? 1,
            m_stackMax = dropTemplate.AmountMax ?? 1,
            m_weight = dropTemplate.Weight ?? 1
        };

        if (dropTemplate.PrefabName is null)
        {
            return false;
        }

        // Try find object
        var prefab = ZNetScene.instance.GetPrefab(dropTemplate.PrefabName);

        if (prefab.IsNull())
        {
            return false;
        }

        drop.m_item = prefab;

        if (dropTemplate.DisableResourceModifierScaling is not null)
        {
            drop.m_dontScale = dropTemplate.DisableResourceModifierScaling.Value;
        }

        return true;
    }

    private static bool TryCreateDrop(
        DropTable.DropData drop,
        DropTableDropTemplate dropTemplate,
        out DropTable.DropData newDrop)
    {
        newDrop = new()
        {
            m_stackMin = dropTemplate.AmountMin ?? drop.m_stackMin,
            m_stackMax = dropTemplate.AmountMax ?? drop.m_stackMax,
            m_weight = dropTemplate.Weight ?? drop.m_weight,
            m_dontScale = dropTemplate.DisableResourceModifierScaling ?? drop.m_dontScale
        };

        if (!string.IsNullOrWhiteSpace(dropTemplate.PrefabName))
        {
            // Try find object
            var prefab = ZNetScene.instance.GetPrefab(dropTemplate.PrefabName);

            if (prefab.IsNull())
            {
                return false;
            }

            drop.m_item = prefab;
        }

        return true;
    }
}

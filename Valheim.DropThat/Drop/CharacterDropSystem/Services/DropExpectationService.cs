using System;
using System.Collections.Generic;
using System.Linq;
using DropThat.Caches;
using DropThat.Configuration;
using DropThat.Drop.CharacterDropSystem.Managers;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;
using UnityEngine;

namespace DropThat.Drop.CharacterDropSystem.Services;

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
    public static Dictionary<string, ExpectedCharacterDrop> AllExpectedDrops()
    {
        if (ZNetScene.instance.IsNull())
        {
            throw new InvalidOperationException("Too early. Trying to get expected drops before ZnetScene is instantiated.");
        }

        Dictionary<string, ExpectedCharacterDrop> expectedDropsByPrefab = new();

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

    public static bool TryGetExpectedDrops(string prefabName, out ExpectedCharacterDrop expectedDrops)
    {
        if (ZNetScene.instance.IsNull())
        {
            throw new InvalidOperationException("Too early. Trying to get expected drops before ZnetScene is instantiated.");
        }

        var prefab = ZNetScene.instance.GetPrefab(prefabName);

        if (prefab.IsNull())
        {
            expectedDrops = null;
            return false;
        }

        return TryGetExpectedDrops(prefab, out expectedDrops);
    }

    public static bool TryGetExpectedDrops(GameObject prefab, out ExpectedCharacterDrop expectedDrops)
    {
        if (!ComponentCache.TryGet<CharacterDrop>(prefab, out var dropTable))
        {
            expectedDrops = null;
            return false;
        }

        var prefabName = prefab.GetCleanedName();

        if (CharacterDropTemplateManager.TryGetTemplate(prefabName, out var template))
        {
            expectedDrops = new()
            {
                PrefabName = prefabName,
                Drops = GenerateExpectedDrops(dropTable, template),
                IsModified = true,
            };
            return true;
        }

        if (GeneralConfigManager.Config?.ClearAllExistingCharacterDrops)
        {
            expectedDrops = new ExpectedCharacterDrop
            {
                PrefabName = prefabName,
                Drops = new(0),
                IsModified = true,
            };
        }
        else
        {
            expectedDrops = new ExpectedCharacterDrop
            {
                PrefabName = prefabName,
                Drops = dropTable.m_drops?
                    .Select(Clone)
                    .ToList() ?? new()
            };
        }

        return true;
    }

    private static List<CharacterDrop.Drop> GenerateExpectedDrops(
        CharacterDrop table,
        CharacterDropMobTemplate tableTemplate)
    {
        var drops = table.m_drops?
            .Select(Clone)
            .ToList() ?? new();

        var dropTemplates = tableTemplate.Drops
            .Where(x => x.Value.TemplateEnabled ?? true)
            .OrderBy(x => x.Key)
            .Select(x => (x.Key, x.Value))
            .ToList();

        if (GeneralConfigManager.Config?.ClearAllExistingCharacterDrops ||
            GeneralConfigManager.Config?.ClearAllExistingCharacterDropsWhenModified &&
            dropTemplates.Count != 0)
        {
            drops.Clear();
        }

        List<int> indexesToRemove = new();

        foreach ((int id, CharacterDropDropTemplate template) in dropTemplates)
        {
            if (id >= drops.Count ||
                GeneralConfigManager.Config?.AlwaysAppendCharacterDrops == true)
            {
                if (template.Enabled == false)
                {
                    continue;
                }

                if (TryGenerateNewDrop(template, out var newDrop))
                {
                    drops.Add(newDrop);
                }
            }
            else
            {
                var existingDrop = drops[id];
                var existingDropPrefab = existingDrop.m_prefab.GetCleanedName();

                if (template.Enabled == false)
                {
                    indexesToRemove.Add(id);
                    continue;
                }

                if (TryGenerateNewDrop(existingDrop, template, out var newDrop))
                {
                    drops[id] = newDrop;
                }
            }
        }

        for (int i = indexesToRemove.Count; i > 0; --i)
        {
            drops.RemoveAt(indexesToRemove[i - 1]);
        }

        return drops;
    }

    private static bool TryGenerateNewDrop(
        CharacterDrop.Drop drop,
        CharacterDropDropTemplate template,
        out CharacterDrop.Drop newDrop)
    {
        GameObject prefab;

        if (!DropConfigurationService.TryFindPrefab(template.PrefabName, out prefab))
        {
            newDrop = null;
            return false;
        }
        else
        {
            prefab = drop.m_prefab;
        }

        newDrop = new CharacterDrop.Drop
        {
            m_prefab = prefab,
            m_amountMin = template.AmountMin ?? drop.m_amountMin,
            m_amountMax = template.AmountMax ?? drop.m_amountMax,
            m_chance = template.ChanceToDrop ?? drop.m_chance,
            m_levelMultiplier = template.ScaleByLevel ?? drop.m_levelMultiplier,
            m_onePerPlayer = template.DropOnePerPlayer ?? drop.m_onePerPlayer,
            m_dontScale = template.DisableResourceModifierScaling ?? drop.m_dontScale,
        };

        return true;
    }

    private static bool TryGenerateNewDrop(
        CharacterDropDropTemplate template,
        out CharacterDrop.Drop newDrop)
    {
        GameObject prefab;

        if (!DropConfigurationService.TryFindPrefab(template.PrefabName, out prefab))
        {
            newDrop = null;
            return false;
        }

        newDrop = new CharacterDrop.Drop
        {
            m_prefab = prefab,
            m_amountMin = template.AmountMin ?? 1,
            m_amountMax = template.AmountMax ?? 1,
            m_chance = template.ChanceToDrop ?? 1,
            m_levelMultiplier = template.ScaleByLevel ?? true,
            m_onePerPlayer = template.DropOnePerPlayer ?? false,
            m_dontScale = template.DisableResourceModifierScaling ?? false,
        };

        return true;
    }

    private static CharacterDrop.Drop Clone(CharacterDrop.Drop drop) =>
        new CharacterDrop.Drop
        {
            m_prefab = drop.m_prefab,
            m_amountMax = drop.m_amountMax,
            m_amountMin = drop.m_amountMin,
            m_chance = drop.m_chance,
            m_dontScale = drop.m_dontScale,
            m_levelMultiplier = drop.m_levelMultiplier,
            m_onePerPlayer = drop.m_onePerPlayer,
        };
}

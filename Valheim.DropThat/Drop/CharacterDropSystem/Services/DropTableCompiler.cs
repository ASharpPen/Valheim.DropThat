using System;
using System.Collections.Generic;
using System.Linq;
using DropThat.Caches;
using DropThat.Configuration;
using DropThat.Drop.CharacterDropSystem.Managers;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Services;

internal static class DropTableCompiler
{
    public static List<CharacterDropMobTemplate> CompileAllDrops(bool applyTemplate = true)
    {
        if (ZNetScene.instance.IsNull())
        {
            throw new InvalidOperationException("Too early. Trying to get expected drops before ZnetScene is instantiated.");
        }

        List<CharacterDropMobTemplate> results = new();

        // Scan ZNetScene for prefabs with drop tables.
        foreach (var prefab in ZNetScene.instance.m_prefabs)
        {
            if (ComponentCache.TryGet<CharacterDrop>(prefab, out var characterDrop))
            {
                results.Add(CompileDrops(prefab.GetCleanedName(), characterDrop, applyTemplate));
            }
        }

        return results;
    }

    public static bool TryCompileDrops(
        string prefabName, 
        bool applyTemplate,
        out CharacterDropMobTemplate compiledDrops)
    {
        if (ZNetScene.instance.IsNull())
        {
            throw new InvalidOperationException("Too early. Trying to get expected drops before ZnetScene is instantiated.");
        }

        var prefab = ZNetScene.instance.GetPrefab(prefabName);


        if (prefab.IsNull() ||
            !ComponentCache.TryGet<CharacterDrop>(prefab, out var characterDrop))
        {
            compiledDrops = null;
            return false;
        }

        compiledDrops = CompileDrops(prefab.GetCleanedName(), characterDrop, applyTemplate);
        return true;
    }

    private static CharacterDropMobTemplate CompileDrops(
        string prefabName, 
        CharacterDrop prefab,
        bool applyTemplate = true)
    {
        // Prepare drops.
        CharacterDropMobTemplate resultMob;

        bool hasTemplate = CharacterDropTemplateManager.TryGetTemplate(prefabName, out var mobTemplate);

        if (GeneralConfigManager.Config?.ClearAllExistingCharacterDrops ||
            GeneralConfigManager.Config?.ClearAllExistingCharacterDropsWhenModified && hasTemplate)
        {
            resultMob = new()
            {
                PrefabName = prefabName,
            };
        }
        else
        {
            resultMob = new()
            {
                PrefabName = prefabName,
                Drops = prefab.m_drops?
                    .Select((x, i) =>
                        new CharacterDropDropTemplate
                        {
                            Id = i,
                            PrefabName = x.m_prefab.name,
                            AmountMin = x.m_amountMin,
                            AmountMax = x.m_amountMax,
                            ChanceToDrop = x.m_chance,
                            DropOnePerPlayer = x.m_onePerPlayer,
                            ScaleByLevel = x.m_levelMultiplier,
                            DisableResourceModifierScaling = x.m_dontScale
                        })
                    .ToDictionary(x => x.Id) ?? new()
            };
        }

        if (!applyTemplate ||
            !hasTemplate)
        {
            return resultMob;
        }

        List<int> idsToRemove = new();

        foreach ((int id, CharacterDropDropTemplate template) in mobTemplate.Drops
            .Select(x => (x.Key, x.Value))
            .ToList())
        {
            if (resultMob.Drops.TryGetValue(id, out var existingDrop))
            {
                Map(template, existingDrop);

                if (existingDrop.Enabled == false)
                {
                    idsToRemove.Add(id);
                }
            }
            else if(template.Enabled != false)
            {
                resultMob.Drops[id] = template;
            }
        }

        // Clean up disabled drops
        foreach (var id in idsToRemove)
        {
            resultMob.Drops.Remove(id);
        }

        return resultMob;

        static void Map(CharacterDropDropTemplate source, CharacterDropDropTemplate destination)
        {
            destination.Conditions = source.Conditions;
            destination.ItemModifiers = source.ItemModifiers;

            if (!string.IsNullOrWhiteSpace(source.PrefabName))
            {
                destination.PrefabName = source.PrefabName;
            }

            if (source.Enabled is not null)
            {
                destination.Enabled = source.Enabled;
            }

            if (source.TemplateEnabled is not null)
            {
                destination.TemplateEnabled = source.TemplateEnabled;
            }

            if (source.AutoStack is not null)
            {
                destination.AutoStack = source.AutoStack;
            }

            if (source.AmountLimit is not null)
            {
                destination.AmountLimit = source.AmountLimit;
            }

            if (source.AmountMin is not null)
            {
                destination.AmountMin = source.AmountMin;
            }

            if (source.AmountMax is not null)
            {
                destination.AmountMax = source.AmountMax;
            }

            if (source.ChanceToDrop is not null)
            {
                destination.ChanceToDrop = source.ChanceToDrop;
            }

            if (source.DropOnePerPlayer is not null)
            {
                destination.DropOnePerPlayer = source.DropOnePerPlayer;
            }

            if (source.ScaleByLevel is not null)
            {
                destination.ScaleByLevel = source.ScaleByLevel;
            }

            if (source.DisableResourceModifierScaling is not null)
            {
                destination.DisableResourceModifierScaling = source.DisableResourceModifierScaling;
            }
        }
    }
}

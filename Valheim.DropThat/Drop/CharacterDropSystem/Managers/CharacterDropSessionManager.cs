﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DropThat.Configuration;
using DropThat.Drop.CharacterDropSystem.Caches;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Drop.CharacterDropSystem.Services;
using DropThat.Drop.Options;
using ThatCore.Cache;
using ThatCore.Extensions;
using ThatCore.Logging;
using UnityEngine;

namespace DropThat.Drop.CharacterDropSystem.Managers;

/// <summary>
/// Logic for general workflow surrounding configuring
/// drop table, running conditions and applying modifiers.
/// </summary>
internal static class CharacterDropSessionManager
{
    public static ManagedCache<CharacterDrop> CharacterDropInstances { get; } = new();

    public static ConditionalWeakTable<CharacterDrop.Drop, DropConfigInfo> DropInstanceTable { get; } = new();

    public static void Initialize(CharacterDrop droptable)
    {
        try
        {
            Log.Development?.Log($"CharacterDrop for '{droptable.GetName()}' initializing.");

            CharacterDropInstances.Set(droptable, droptable);

            // Record spawn data if missing.
            CharacterDropEventManager.DropTableInitialize(droptable);

            // Insert or configure drops.
            var existingDrops = droptable.m_drops ??= new();

            var sourceName = droptable.GetCleanedName();
            var config = CharacterDropTemplateManager.GetTemplate(sourceName);

            if (config is null)
            {
                return;
            }

            var dropTemplates = config.Drops
                .Where(x => x.Value.TemplateEnabled ?? true)
                .OrderBy(x => x.Key)
                .Select(x => (x.Key, x.Value))
                .ToList();

            if (GeneralConfigManager.Config?.ClearAllExistingCharacterDrops ||
                GeneralConfigManager.Config?.ClearAllExistingCharacterDropsWhenModified &&
                dropTemplates.Count != 0)
            {
                Log.Trace?.Log($"[{sourceName}] Clearing '{existingDrops.Count}' drops.");
                existingDrops.Clear();
            }

            Log.Development?.Log($"[{sourceName}] Found {dropTemplates.Count} templates.");

            if (dropTemplates.Count == 0)
            {
                return;
            }

            List<int> indexesToRemove = new();

            foreach ((int id, CharacterDropDropTemplate template) in dropTemplates)
            {
                var dropInfo = new DropConfigInfo()
                {
                    MobTemplate = config,
                    DropTemplate = template,
                };

                if (id >= existingDrops.Count)
                {
                    if (dropInfo.DropTemplate.Enabled == false)
                    {
                        continue;
                    }

                    Log.Trace?.Log($"{dropInfo.DisplayName} Inserting drop '{template.PrefabName}'");

                    if (DropConfigurationService.TryConfigureNewDrop(dropInfo, out var newDrop))
                    {
                        existingDrops.Add(newDrop);

                        dropInfo.Index = existingDrops.Count;
                        DropInstanceTable.Add(newDrop, dropInfo);
                    }
                }
                else
                {
                    var existingDrop = existingDrops[id];
                    var existingDropPrefab = existingDrop.m_prefab.GetCleanedName();

                    if (dropInfo.DropTemplate.Enabled == false)
                    {
                        Log.Trace?.Log($"{dropInfo.DisplayName} Disabling existing drop '{existingDropPrefab}'.");

                        indexesToRemove.Add(id);
                        continue;
                    }

                    if (existingDropPrefab == template.PrefabName)
                    {
                        Log.Trace?.Log($"{dropInfo.DisplayName} Configuring existing drop '{existingDropPrefab}'.");
                    }
                    else
                    {
                        Log.Trace?.Log($"{dropInfo.DisplayName} Configuring and changing existing drop '{existingDropPrefab}' to '{template.PrefabName}'.");
                    }

                    DropConfigurationService.ConfigureExistingDrop(dropInfo, existingDrop);

                    dropInfo.Index = id;
                    DropInstanceTable.Add(existingDrop, dropInfo);
                }
            }

            for(int i = indexesToRemove.Count; i > 0; --i)
            {
                existingDrops.RemoveAt(indexesToRemove[i - 1]);
            }

            CharacterDropEventManager.DropTableConfigured(droptable);
        }
        catch (Exception e)
        {
            Log.Error?.Log($"Error while initializing drops for '{droptable.name}'", e);
        }
    }

    public static List<CharacterDrop.Drop> FilterDrops(CharacterDrop characterDrop)
    {
        try
        {
            if (characterDrop.IsNull())
            {
                return new();
            }

            // Run conditions on list
            List<CharacterDrop.Drop> validDrops = new List<CharacterDrop.Drop>();

            foreach (var drop in characterDrop.m_drops)
            {
                if (!DropInstanceTable.TryGetValue(drop, out var dropInfo) ||
                    dropInfo?.DropTemplate is null)
                {
                    // No settings for drop.
                    validDrops.Add(drop);
                    continue;
                }

                if (dropInfo.DropTemplate.Enabled == false)
                {
                    // Drop is disabled. Skip it.
                    continue;
                }

                var context = new DropContext(characterDrop)
                {
                    DropInfo = dropInfo,
                };

                var isValid = dropInfo.DropTemplate.Conditions?.All(x =>
                {
                    try
                    {
                        return x.IsValid(context);
                    }
                    catch (Exception e)
                    {
                        Log.Warning?.Log($"{dropInfo.DisplayName} Error while checking condition '{x.GetType().Name}'. Disabling drop.", e);
                        return false;
                    }
                });

                if (isValid is null ||
                    isValid == true)
                {
                    validDrops.Add(drop);
                }
            }

            return validDrops;
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error while checking drop conditions.", e);
            return new();
        }
    }

    /// <summary>
    /// Clamp each drop to configured max limit.
    /// </summary>
    public static void LimitDropAmounts(CharacterDrop droptable, List<KeyValuePair<GameObject, int>> drops)
    {
        try
        {
            if (GeneralConfigManager.Config is null)
            {
                return;
            }

            for (int i = 0; i < drops.Count; ++i)
            {
                var item = drops[i];
                if (GeneralConfigManager.Config.DropLimit > 0 && item.Value > GeneralConfigManager.Config.DropLimit)
                {
                    Log.Trace?.Log($"Limiting {item.Key.name}:{item.Value} to {GeneralConfigManager.Config.DropLimit}");

                    drops[i] = Limit(item, GeneralConfigManager.Config.DropLimit);
                    continue;
                }

                var config = TempDropListCache.GetDrop(droptable, i);

                if (config?.DropTemplate?.AmountLimit >= 0 &&
                    item.Value > config.DropTemplate.AmountLimit)
                {
                    Log.Trace?.Log($"{config.DisplayName} Limiting drop amount from '{item.Value}' to '{config.DropTemplate.AmountLimit}'");

                    drops[i] = Limit(item, config.DropTemplate.AmountLimit.Value);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error while checking and applying drop amount limits.", e);
        }

        static KeyValuePair<GameObject, int> Limit(KeyValuePair<GameObject, int> drop, int limit)
        {
            return new KeyValuePair<GameObject, int>(drop.Key, Math.Min(drop.Value, limit));
        }
    }

    public static void ModifyDrop(GameObject drop, List<KeyValuePair<GameObject, int>> drops, int index)
    {
        try
        {
            var dropPair = drops[index];

            var configInfo = TempDropListCache.GetDrop(drops, index);

            if (configInfo is null)
            {
                return;
            }

            ItemModifierContext<GameObject> dropContext = new()
            {
                Item = drop,
                Position = drop.transform.position
            };

            foreach (var modifier in configInfo.DropTemplate.ItemModifiers)
            {
                try
                {
                    modifier.Modify(dropContext);
                }
                catch (Exception e)
                {
                    Log.Error?.Log($"Error while attempting to apply modifier '{modifier.GetType().Name}' to drop '{drop.GetName()}'.", e);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error during drop modification.", e);
        }
    }
}

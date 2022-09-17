using HarmonyLib;
using System.Linq;
using UnityEngine;
using System;
using DropThat.Configuration;
using DropThat.Core;
using DropThat.Drop.CharacterDropSystem.Services;
using DropThat.Drop.CharacterDropSystem.Caches;
using DropThat.Utilities;
using DropThat.Drop.CharacterDropSystem.Configurations;
using DropThat.Configuration.ConfigTypes;

namespace DropThat.Drop.CharacterDropSystem.Patches;

// TODO: This needs an overhaul. Fields should be set based on TomlConfigEntry.IsSet, meaning no unintentional overrides are performed.
// TODO: Refactor so that patches become lifecycle hooks, and the rest of the code is contained elsewhere.
[HarmonyPatch(typeof(CharacterDrop), nameof(CharacterDrop.Start))]
public static class Patch_CharacterDrop_Start
{
    private static GeneralConfiguration GeneralConfig => ConfigurationManager.GeneralConfig;
    private static CharacterDropConfigurationFile DropConfig => CharacterDropConfigurationFileManager.CharacterDropConfig;
    private static CharacterDropListConfigurationFile DropListConfig => CharacterDropConfigurationFileManager.CharacterDropListConfig;

    [HarmonyPriority(Priority.Last)]
    private static void Postfix(CharacterDrop __instance)
    {
        try
        {
            if (DropConfig is null)
            {
                Log.LogDebug("Loading drop tables");
                CharacterDropConfigurationFileManager.LoadAllConfigurations();
            }

            string name = __instance.gameObject.name;

            CharacterDropMobConfiguration configMatch = FindConfigMatch(name);

            // Find drop list
            string dropListName = configMatch?.UseDropList?.Value;

            CharacterDropListConfiguration listConfig = null;

            if (!string.IsNullOrWhiteSpace(dropListName) &&
                DropListConfig is not null &&
                DropListConfig.TryGet(dropListName, out CharacterDropListConfiguration dropList))
            {
                listConfig = dropList;
            }

            bool skipExisting = false;

            if (GeneralConfig.ClearAllExistingCharacterDrops ||
                GeneralConfig.ClearAllExistingCharacterDropsWhenModified &&
                (configMatch?.Subsections?.Any(x => x.Value.EnableConfig) == true ||
                listConfig?.Subsections?.Any(x => x.Value.EnableConfig) == true))
            {
                skipExisting = true;
            }

            if (skipExisting && __instance.m_drops.Count > 0)
            {
                Log.LogTrace($"[{name}]: Clearing '{__instance.m_drops.Count}'");
                __instance.m_drops.Clear();
            }

            // Merge list and mob config
            var configs = MobDropInitializationService.PrepareInsertion(listConfig, configMatch);

            foreach (var config in configs)
            {
                InsertDrops(__instance, config);
            }

            __instance.m_drops = ConditionChecker.FilterOnStart(__instance);
        }
        catch (Exception e)
        {
            Log.LogError("Error while attempting to configure creature drops.", e);
        }
    }

    private static void InsertDrops(CharacterDrop instance, CharacterDropItemConfiguration dropConfig)
    {
        GameObject item = ObjectDB.instance.GetItemPrefab(dropConfig.PrefabName?.Value);

        if (item.IsNull())
        {
            Log.LogWarning($"[{dropConfig.SectionPath}]: No item '{dropConfig.PrefabName}' exists.");
            return;
        }

        CharacterDrop.Drop newDrop = new CharacterDrop.Drop
        {
            m_prefab = item,
            m_amountMax = dropConfig.SetAmountMax.Value,
            m_amountMin = dropConfig.SetAmountMin.Value,
            m_chance = dropConfig.SetChanceToDrop.Value / 100f,
            m_levelMultiplier = dropConfig.SetScaleByLevel.Value,
            m_onePerPlayer = dropConfig.SetDropOnePerPlayer.Value,
        };

        DropExtended.Set(newDrop, dropConfig);

        if (!GeneralConfig.AlwaysAppendCharacterDrops.Value)
        {
            int index = dropConfig.Index;

            if (instance.m_drops.Count > index)
            {
                Log.LogDebug($"[{dropConfig.SectionPath}]: Removing overriden item '{instance.m_drops[index].m_prefab.name}' at index '{index}'.");

                instance.m_drops.RemoveAt(index);
            }
        }

        Insert(instance, dropConfig, newDrop);
    }

    private static void Insert(CharacterDrop __instance, CharacterDropItemConfiguration config, CharacterDrop.Drop drop)
    {
        int index = config.Index;
        if (index >= 0 && __instance.m_drops.Count >= index && !ConfigurationManager.GeneralConfig.AlwaysAppendCharacterDrops.Value)
        {
            Log.LogDebug($"[{__instance.gameObject.name}]: Inserting drop {config.PrefabName.Value} at index '{index}'.");

            __instance.m_drops.Insert(index, drop);
        }
        else
        {
            Log.LogDebug($"[{__instance.gameObject.name}]: Adding item {config.PrefabName.Value}.");
            __instance.m_drops.Add(drop);
        }
    }

    private static CharacterDropMobConfiguration FindConfigMatch(string prefabName)
    {
        if ((DropConfig?.Subsections?.Count ?? 0) == 0)
        {
#if DEBUG
            Log.LogDebug("No drop configs found to match.");
#endif
            return null;
        }

        var cleanedName = prefabName
            .Split(new[] { '(' }, StringSplitOptions.RemoveEmptyEntries)?
            .FirstOrDefault();

        if ((cleanedName?.Length ?? 0) == 0)
        {
#if DEBUG
            Log.LogDebug($"Prefabname {prefabName} was empty after cleaning.");
#endif

            return null;
        }

        if (DropConfig.Subsections.TryGetValue(cleanedName, out CharacterDropMobConfiguration mobConfig))
        {
            return mobConfig;
        }

#if DEBUG
        Log.LogDebug($"Unable to find config for {cleanedName}.");
#endif

        return null;
    }
}

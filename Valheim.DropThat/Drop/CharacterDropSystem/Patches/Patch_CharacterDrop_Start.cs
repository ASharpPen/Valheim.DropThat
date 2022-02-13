using HarmonyLib;
using System.Linq;
using UnityEngine;
using Valheim.DropThat.Configuration.ConfigTypes;
using System;
using Valheim.DropThat.Configuration;
using Valheim.DropThat.Core;
using Valheim.DropThat.Drop.CharacterDropSystem.Services;
using Valheim.DropThat.Drop.CharacterDropSystem.Caches;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Drop.CharacterDropSystem.Patches
{
    [HarmonyPatch(typeof(CharacterDrop), "Start")]
    public static class Patch_CharacterDrop_Start
    {
        private static GeneralConfiguration GeneralConfig => ConfigurationManager.GeneralConfig;

        [HarmonyPriority(Priority.Last)]
        private static void Postfix(CharacterDrop __instance)
        {
            try
            {
                if (ConfigurationManager.CharacterDropConfigs == null)
                {
                    Log.LogDebug("Loading drop tables");
                    ConfigurationManager.LoadAllCharacterDropConfigurations();
                }

                string name = __instance.gameObject.name;

                CharacterDropMobConfiguration configMatch = FindConfigMatch(name);

                // Find drop list
                string dropListName = configMatch?.UseDropList?.Value;

                CharacterDropListConfiguration listConfig = null;

                if (!string.IsNullOrWhiteSpace(dropListName) &&
                    ConfigurationManager.CharacterDropLists is not null &&
                    ConfigurationManager.CharacterDropLists.TryGet(dropListName, out CharacterDropListConfiguration dropList))
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
            //Sanity checks
            if (!dropConfig.IsValid())
            {
#if DEBUG
                Log.LogDebug($"Drop config {dropConfig.SectionKey} is not valid or enabled.");
#endif
                return;
            }

            GameObject item = ObjectDB.instance.GetItemPrefab(dropConfig.PrefabName?.Value);

            if (item.IsNull())
            {
                item = ZNetScene.instance.GetPrefab(dropConfig.PrefabName.Value);
            }

            if (item.IsNull())
            {
                Log.LogWarning($"[{dropConfig.SectionKey}]: No item '{dropConfig.PrefabName}' exists");
                return;
            }

            CharacterDrop.Drop newDrop = new CharacterDrop.Drop
            {
                m_prefab = item,
                m_amountMax = dropConfig.SetAmountMax.Value,
                m_amountMin = dropConfig.SetAmountMin.Value,
                m_chance = dropConfig.SetChanceToDrop.Value / 100,
                m_levelMultiplier = dropConfig.SetScaleByLevel.Value,
                m_onePerPlayer = dropConfig.SetDropOnePerPlayer.Value,
            };

            DropExtended.Set(newDrop, dropConfig);

            if (!GeneralConfig.AlwaysAppendCharacterDrops.Value)
            {
                int index = dropConfig.Index;

                if (instance.m_drops.Count > index)
                {
                    Log.LogDebug($"[{dropConfig.SectionKey}]: Removing overriden item '{instance.m_drops[index].m_prefab.name}' at index '{index}'.");

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
            if ((ConfigurationManager.CharacterDropConfigs?.Subsections?.Count ?? 0) == 0)
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

            if (ConfigurationManager.CharacterDropConfigs.Subsections.TryGetValue(cleanedName, out CharacterDropMobConfiguration mobConfig))
            {
                return mobConfig;
            }

#if DEBUG
            Log.LogDebug($"Unable to find config for {cleanedName}.");
#endif

            return null;
        }
    }
}

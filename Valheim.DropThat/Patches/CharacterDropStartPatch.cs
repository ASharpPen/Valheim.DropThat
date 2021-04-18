using HarmonyLib;
using System.Linq;
using UnityEngine;
using Valheim.DropThat.Conditions;
using Valheim.DropThat.Configuration.ConfigTypes;
using System;
using Valheim.DropThat.Configuration;
using Valheim.DropThat.Core;
using Valheim.DropThat.Caches;

namespace Valheim.DropThat
{
    [HarmonyPatch(typeof(CharacterDrop), "Start")]
    public static class CharacterDropStartPatch
    {
        private static GeneralConfiguration GeneralConfig => ConfigurationManager.GeneralConfig;

        private static void Postfix(CharacterDrop __instance)
        {
            if(ConfigurationManager.DropConfigs == null)
            {
                Log.LogDebug("Loading drop tables");
                ConfigurationManager.LoadAllDropTableConfigurations();
            }

            string name = __instance.gameObject.name;

            var configMatch = FindConfigMatch(name);

            if (GeneralConfig.ClearAllExisting.Value && __instance.m_drops.Count > 0)
            {
                Log.LogDebug($"[{name}]: Clearing '{__instance.m_drops.Count}'");
                __instance.m_drops.Clear();
            }

            if (configMatch is not null)
            {
                if(GeneralConfig.ClearAllExistingWhenModified.Value && __instance.m_drops.Count > 0)
                {
                    Log.LogDebug($"[{name}]: Clearing '{__instance.m_drops.Count}'");
                    __instance.m_drops.Clear();
                }

                foreach (var dropEntry in configMatch.Subsections.OrderBy(x => x.Value.Index))
                {
                    var dropConfig = dropEntry.Value;

                    //Sanity checks
                    if (dropConfig is null || !dropConfig.IsValid())
                    {
#if DEBUG
                        Log.LogDebug($"Drop config {dropConfig.SectionKey} is not valid or enabled.");
#endif
                        continue;
                    }

                    GameObject item = ObjectDB.instance.GetItemPrefab(dropConfig.ItemName?.Value);

                    if (item == null)
                    {
                        Log.LogWarning($"Couldn't find item '{dropConfig.ItemName}' for configuration '{configMatch.SectionName}'");
                        continue;
                    }

                    CharacterDrop.Drop newDrop = new CharacterDrop.Drop
                    {
                        m_prefab = item,
                        m_amountMax = dropConfig.AmountMax.Value,
                        m_amountMin = dropConfig.AmountMin.Value,
                        m_chance = dropConfig.Chance.Value,
                        m_levelMultiplier = dropConfig.LevelMultiplier.Value,
                        m_onePerPlayer = dropConfig.OnePerPlayer.Value,
                    };

                    DropExtended.Set(newDrop, dropConfig);

                    Log.LogDebug($"[{name}]: {__instance.m_drops.Count} existing drops in table.");

                    if(!GeneralConfig.AlwaysAppend.Value)
                    {
                        int index = dropConfig.Index;

                        if (__instance.m_drops.Count > index)
                        {
                            Log.LogDebug($"[{configMatch.SectionName}]: Removing overriden item '{__instance.m_drops[index].m_prefab.name}' at index '{index}'.");
                            __instance.m_drops.RemoveAt(index);
                        }
                    }

                    Insert(__instance, dropConfig, newDrop);
                }
            }

            __instance.m_drops = ConditionChecker.FilterOnStart(__instance);
        }

        private static void Insert(CharacterDrop __instance, DropItemConfiguration config, CharacterDrop.Drop drop)
        {
            int index = config.Index;
            if (index >= 0 && __instance.m_drops.Count >= index && !ConfigurationManager.GeneralConfig.AlwaysAppend.Value)
            {
                Log.LogDebug($"[{__instance.gameObject.name}]: Inserting drop {config.ItemName.Value} at index '{index}'.");

                __instance.m_drops.Insert(index, drop);
            }
            else
            {
                Log.LogDebug($"[{__instance.gameObject.name}]: Adding item {config.ItemName.Value}.");
                __instance.m_drops.Add(drop);
            }
        }

        private static DropMobConfiguration FindConfigMatch(string prefabName)
        {
            if((ConfigurationManager.DropConfigs?.Subsections?.Count ?? 0) == 0)
            {
#if DEBUG
                Log.LogDebug("No drop configs found to match.");
#endif
                return null;
            }

            var cleanedName = prefabName
                .Split(new[] { '(' }, StringSplitOptions.RemoveEmptyEntries)?
                .FirstOrDefault();

            if((cleanedName?.Length ?? 0) == 0)
            {
#if DEBUG
                Log.LogDebug($"Prefabname {prefabName} was empty after cleaning.");
#endif

                return null;
            }

            if (ConfigurationManager.DropConfigs.Subsections.TryGetValue(cleanedName, out DropMobConfiguration mobConfig))
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

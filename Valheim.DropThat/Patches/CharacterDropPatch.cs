using HarmonyLib;
using System.Linq;
using UnityEngine;
using Valheim.DropThat.ConfigurationCore;
using Valheim.DropThat.ConfigurationTypes;
using System.Collections.Generic;
using Valheim.DropThat.Conditions;

namespace Valheim.DropThat
{
    [HarmonyPatch(typeof(CharacterDrop), "Start")]
    public static class ModifyDropTable
    {
        private static GeneralConfiguration GeneralConfig => ConfigurationManager.GeneralConfig;

        private static void Postfix(ref CharacterDrop __instance)
        {
            if(ConfigurationManager.DropConfigs == null)
            {
                Log.LogDebug("Loading drop tables");
                ConfigurationManager.LoadAllDropTableConfigurations();
            }

            string name = __instance.gameObject.name;

            string cleanedName = name.Split('(')[0].Trim().ToUpperInvariant();

            Log.LogDebug($"[{name}] CharacterDrop starting. Using cleaned name '{cleanedName}' for comparisons.");

            var configMatch = ConfigurationManager.DropConfigs.FirstOrDefault(x => x.Enabled.Value && cleanedName == x.EntityName);

            Log.LogTrace("Possible comparisons:");
            ConfigurationManager.DropConfigs.ForEach(x =>
            {
                Log.LogTrace(x.EntityName);

                x.DropConfigurations.ForEach(y => Log.LogTrace("\t" + y.SectionName + ":" + y.ItemName));
            });

            if(GeneralConfig.ClearAllExisting.Value && __instance.m_drops.Count > 0)
            {
                Log.LogDebug($"[{name}]: Clearing '{__instance.m_drops.Count}'");
                __instance.m_drops.Clear();
            }

            if (configMatch != null)
            {
                if(GeneralConfig.ClearAllExistingWhenModified.Value && __instance.m_drops.Count > 0)
                {
                    Log.LogDebug($"[{name}]: Clearing '{__instance.m_drops.Count}'");
                    __instance.m_drops.Clear();
                }

                foreach (var dropEntry in configMatch.DropConfigurations.OrderBy(x => x.Index))
                {
                    //Sanity checks
                    if (dropEntry == null || !dropEntry.IsValid())
                    {
                        continue;
                    }

                    GameObject item = ObjectDB.instance.GetItemPrefab(dropEntry.ItemName?.Value);

                    if (item == null)
                    {
                        Log.LogWarning($"Couldn't find item '{dropEntry.ItemName}' for configuration '{configMatch.EntityName}'");
                        continue;
                    }

                    DropExtended dropConfig = new DropExtended
                    {
                        m_prefab = item,
                        m_amountMax = dropEntry.AmountMax.Value,
                        m_amountMin = dropEntry.AmountMin.Value,
                        m_chance = dropEntry.Chance.Value,
                        m_levelMultiplier = dropEntry.LevelMultiplier.Value,
                        m_onePerPlayer = dropEntry.OnePerPlayer.Value,
                        Config = dropEntry,
                    };

                    Log.LogDebug($"[{name}]: {__instance.m_drops.Count} existing drops in table.");

                    if(!GeneralConfig.AlwaysAppend.Value)
                    {
                        int index = dropEntry.Index;

                        if (__instance.m_drops.Count > index)
                        {
                            Log.LogDebug($"[{configMatch.EntityName}]: Removing overriden item '{__instance.m_drops[index].m_prefab.name}' at index '{index}'.");
                            __instance.m_drops.RemoveAt(index);
                        }
                    }

                    Insert(__instance, dropEntry, dropConfig);
                }
            }

            if (!GeneralConfig.ApplyConditionsOnDeath?.Value ?? false)
            {
                __instance.m_drops = ConditionChecker.FilterByCondition(__instance);
            }
        }

        private static void Insert(CharacterDrop __instance, DropConfiguration config, CharacterDrop.Drop drop)
        {
            int index = config.Index;
            if (index >= 0 && __instance.m_drops.Count >= index)
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
    }
}

using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace Valheim.DropThat
{
    [HarmonyPatch(typeof(CharacterDrop), "Start")]
    public static class ModifyDropTable
    {
        private static void Postfix(ref CharacterDrop __instance)
        {
            string name = __instance.gameObject.name;

            string cleanedName = name.Split('(')[0].Trim().ToUpperInvariant();

            if (DropThatPlugin.DefaultConfiguration.DebugMode.Value) Debug.Log("CharacterDrop starting: " + name + "; " + cleanedName);

            var configMatch = DropThatPlugin.DropTables.FirstOrDefault(x => cleanedName == x.EntityName.Trim().ToUpperInvariant());

            if(DropThatPlugin.DefaultConfiguration.ClearAllExisting.Value && __instance.m_drops.Count > 0)
            {
                if (DropThatPlugin.DefaultConfiguration.DebugMode.Value) Debug.Log($"Clearing '{__instance.m_drops.Count}' drops from '{name}'.");
                __instance.m_drops.Clear();
            }

            if (configMatch != null)
            {
                if(DropThatPlugin.DefaultConfiguration.ClearAllExistingWhenModified.Value && __instance.m_drops.Count > 0)
                {
                    if (DropThatPlugin.DefaultConfiguration.DebugMode.Value) Debug.Log($"Clearing '{__instance.m_drops.Count}' drops from '{name}'.");
                    __instance.m_drops.Clear();
                }

                foreach (var dropEntry in configMatch.Drops)
                {
                    //Sanity checks
                    if (dropEntry == null || !dropEntry.IsValid())
                    {
                        continue;
                    }

                    GameObject item = ObjectDB.instance.GetItemPrefab(dropEntry.ItemName?.Value);

                    if (item == null)
                    {
                        Debug.LogWarning($"Couldn't find item '{dropEntry.ItemName}'");
                        continue;
                    }

                    CharacterDrop.Drop dropConfig = new CharacterDrop.Drop
                    {
                        m_prefab = item,
                        m_amountMax = dropEntry.AmountMax.Value,
                        m_amountMin = dropEntry.AmountMin.Value,
                        m_chance = dropEntry.Chance.Value,
                        m_levelMultiplier = dropEntry.LevelMultiplier.Value,
                        m_onePerPlayer = dropEntry.OnePerPlayer.Value
                    };

                    if (DropThatPlugin.DefaultConfiguration.DebugMode.Value) Debug.Log($"[{configMatch.EntityName}]: {__instance.m_drops.Count} existing drops in table.");

                    if (DropThatPlugin.DefaultConfiguration.AlwaysAppend.Value)
                    {
                        if (DropThatPlugin.DefaultConfiguration.DebugMode.Value) Debug.Log($"[{configMatch.EntityName}]: Adding {dropEntry.ItemName.Value} drop.");
                        __instance.m_drops.Insert(dropEntry.Index, dropConfig);
                    }
                    else
                    {
                        if (__instance.m_drops.Count > dropEntry.Index && dropEntry.Index >= 0)
                        {
                            if (DropThatPlugin.DefaultConfiguration.DebugMode.Value) Debug.Log($"[{configMatch.EntityName}]: Replacing {__instance.m_drops[dropEntry.Index].m_prefab.name} drop with {dropEntry.ItemName.Value}.");

                            //Replace existing entry
                            __instance.m_drops[dropEntry.Index] = dropConfig;
                        }
                        else
                        {
                            if (DropThatPlugin.DefaultConfiguration.DebugMode.Value) Debug.Log($"[{configMatch.EntityName}]: Adding {dropEntry.ItemName.Value} drop.");
                            __instance.m_drops.Insert(dropEntry.Index, dropConfig);
                        }
                    }
                }
            }
        }
    }
}

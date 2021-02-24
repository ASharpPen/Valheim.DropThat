using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Valheim.DropThat
{
    [BepInPlugin("asharppen.valheim.drop_that", "Drop That!", "0.0.1.0")]
    public class DropThatPlugin : BaseUnityPlugin
    {
        public static List<DropTableConfiguration> DropTables = new List<DropTableConfiguration>();

        public static ConfigEntry<bool> DebugMode { get; set; }

        // Awake is called once when both the game and the plug-in are loaded
        void Awake()
        {
            Logger.LogInfo("Loading configurations");

            string dropTableConfigFile = Path.Combine(Paths.ConfigPath, "drop_that.tables.cfg");

            ConfigFile generalConfig = new ConfigFile(Path.Combine(Paths.ConfigPath, "drop_that.cfg"), true);
            DebugMode = generalConfig.Bind<bool>("General", "EnableDebug", false, "Enable debug logging.");

            ConfigFile dropConfig;
            if (!File.Exists(dropTableConfigFile))
            {
                dropConfig = new ConfigFile(dropTableConfigFile, true);
                DropTableConfigurationLoader.InitializeExample(dropConfig);
            }
            else
            {
                dropConfig = new ConfigFile(dropTableConfigFile, true);
            }

            DropTableConfigurationLoader.ScanBindings(dropConfig);
            DropTables = DropTableConfigurationLoader.GroupConfigurations(dropConfig);

            Logger.LogInfo("Configuration loading complete.");

            var harmony = new Harmony("mod.drop_that");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(CharacterDrop), "Start")]
    public static class ModifyDropTable
    {
        private static void Postfix(ref CharacterDrop __instance)
        {
            string name = __instance.gameObject.name;

            string cleanedName = name.Split('(')[0].Trim().ToUpperInvariant();

            if (DropThatPlugin.DebugMode.Value) Debug.Log("CharacterDrop starting: " + name + "; " + cleanedName);

            var configMatch = DropThatPlugin.DropTables.FirstOrDefault(x => cleanedName == x.EntityName.Trim().ToUpperInvariant());

            if (configMatch != null)
            {
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

                    if (DropThatPlugin.DebugMode.Value) Debug.Log($"[{configMatch.EntityName}]: {__instance.m_drops.Count} existing drops in table.");

                    if (__instance.m_drops.Count > dropEntry.Index && dropEntry.Index >= 0)
                    {
                        if (DropThatPlugin.DebugMode.Value) Debug.Log($"[{configMatch.EntityName}]: Replacing {__instance.m_drops[dropEntry.Index].m_prefab.name} drop with {dropEntry.ItemName.Value}.");

                        //Replace existing entry
                        __instance.m_drops[dropEntry.Index] = dropConfig;
                    }
                    else
                    {
                        if (DropThatPlugin.DebugMode.Value) Debug.Log($"[{configMatch.EntityName}]: Adding {dropEntry.ItemName.Value} drop.");
                        __instance.m_drops.Insert(dropEntry.Index, dropConfig);
                    }
                }
            }
        }
    }
}

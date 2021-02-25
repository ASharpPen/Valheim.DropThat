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

        public static DefaultConfiguration DefaultConfiguration;

        // Awake is called once when both the game and the plug-in are loaded
        void Awake()
        {
            Logger.LogInfo("Loading configurations");

            string dropTableConfigFile = Path.Combine(Paths.ConfigPath, "drop_that.tables.cfg");

            ConfigFile generalConfig = new ConfigFile(Path.Combine(Paths.ConfigPath, "drop_that.cfg"), true);

            DefaultConfiguration = new DefaultConfiguration();
            DefaultConfiguration.Load(generalConfig);

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
}

using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Valheim.DropThat.ConfigurationCore;
using Valheim.DropThat.ConfigurationTypes;

namespace Valheim.DropThat
{
    public static class ConfigurationManager
    {
        public const string DefaultConfigFile = "drop_that.cfg";
        public const string DefaultDropFile = "drop_that.tables.cfg";
        public const string SupplementalPattern = "drop_that.supplemental.*";

        public static bool DebugOn => GeneralConfig?.DebugMode?.Value ?? false;

        public static GeneralConfiguration GeneralConfig = null;

        public static List<DropTableConfiguration> DropConfigs = null;

        public static void LoadAllConfigurations()
        {
            LoadGeneralConfigurations();

            LoadAllDropTableConfigurations();
        }

        public static void LoadGeneralConfigurations()
        {
            string generalConfig = Path.Combine(Paths.ConfigPath, DefaultConfigFile);

            Log.LogInfo($"Loading general configuration from {generalConfig}.");

            GeneralConfig = new GeneralConfiguration();
            GeneralConfig.Load(new ConfigFile(generalConfig, true));
        }

        public static void LoadAllDropTableConfigurations()
        {
            string configPath = Path.Combine(Paths.ConfigPath, DefaultDropFile);

            var configs = LoadDropTableConfig(configPath);

            if(GeneralConfig?.LoadSupplementalDropTables?.Value == true)
            {
                Log.LogInfo("Loading supplemental drop tables...");

                configs.AddRange(LoadSupplemental());
            }

            DropConfigs = configs;

            Log.LogDebug("Finished loading drop configurations");
        }

        
        public static List<DropTableConfiguration> LoadSupplemental()
        {
            var supplementalFiles = Directory.GetFiles(Paths.ConfigPath, SupplementalPattern);
            var supplementalConfigurations = new List<DropTableConfiguration>(supplementalFiles.Length);

            Log.LogDebug($"Found {supplementalFiles.Length} supplemental files");

            foreach (var file in supplementalFiles)
            {
                try
                {
                    supplementalConfigurations.AddRange(LoadDropTableConfig(file));
                }
                catch (Exception e)
                {
                    Log.LogWarning($"Failed to load supplemental raid {file}: {e.Message}");
                }
            }

            return supplementalConfigurations;
        }

        private static List<DropTableConfiguration> LoadDropTableConfig(string configPath)
        {
            Log.LogDebug($"Loading drop table configurations from {configPath}.");

            var configFile = new ConfigFile(configPath, true);

            if (GeneralConfig?.StopTouchingMyConfigs?.Value != null) configFile.SaveOnConfigSet = GeneralConfig.StopTouchingMyConfigs.Value;

            Dictionary<string, DropTableConfiguration> configurations = ConfigurationLoader.LoadConfigurationGroup<DropTableConfiguration, DropConfiguration>(configFile);

            return configurations.Values.ToList();
        }
    }
}

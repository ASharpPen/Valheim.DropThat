using BepInEx;
using BepInEx.Configuration;
using System;
using System.IO;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;
using Valheim.DropThat.Core.Configuration;

namespace Valheim.DropThat.Configuration
{
    public static class ConfigurationManager
    {
        public static GeneralConfiguration GeneralConfig = null;

        public static DropConfiguration DropConfigs = null;

        public const string DefaultConfigFile = "drop_that.cfg";
        public const string DefaultDropFile = "drop_that.tables.cfg";
        public const string SupplementalPattern = "drop_that.supplemental.*";

        public static void LoadAll()
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
            Log.LogInfo("Loading drop configurations");

            string configPath = Path.Combine(Paths.ConfigPath, DefaultDropFile);

            var configs = LoadDropTableConfig(configPath);

            if (GeneralConfig?.LoadSupplementalDropTables?.Value == true)
            {
                var supplementalFiles = Directory.GetFiles(Paths.ConfigPath, SupplementalPattern);
                Log.LogDebug($"Found {supplementalFiles.Length} supplemental files");

                foreach (var file in supplementalFiles)
                {
                    try
                    {
                        var supplementalConfig = LoadDropTableConfig(file);

                        supplementalConfig.MergeInto(configs);
                    }
                    catch(Exception e)
                    {
                        Log.LogError($"Failed to load supplemental config '{file}'.", e);
                    }
                }
            }

            DropConfigs = configs;

            Log.LogDebug("Finished loading drop configurations");
        }

        private static DropConfiguration LoadDropTableConfig(string configPath)
        {
            Log.LogDebug($"Loading drop table configurations from {configPath}.");

            var configFile = new ConfigFile(configPath, true);

            if (GeneralConfig?.StopTouchingMyConfigs?.Value != null) configFile.SaveOnConfigSet = !GeneralConfig.StopTouchingMyConfigs.Value;

            return ConfigurationLoader.LoadConfiguration<DropConfiguration>(configFile);
        }
    }
}

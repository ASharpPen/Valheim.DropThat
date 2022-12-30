using BepInEx;
using BepInEx.Configuration;
using System;
using System.IO;
using DropThat.Configuration.ConfigTypes;
using ThatCore.Logging;

namespace DropThat.Configuration;

public static class ConfigurationManager
{
    public static GeneralConfiguration GeneralConfig;

    public static CharacterDropConfiguration CharacterDropConfigs = new();

    public static DropTableConfiguration DropTableConfigs = new();

    public static CharacterDropListConfigurationFile CharacterDropLists = new();
    public static DropTableListConfigurationFile DropTableLists = new();

    public const string DefaultConfigFile = "drop_that.cfg";
    public const string DefaultDropFile = "drop_that.character_drop.cfg";
    public const string SupplementalPattern = "drop_that.character_drop.*.cfg";

    public const string DefaultDropTablesFile = "drop_that.drop_table.cfg";
    public const string SupplementalDropTablePattern = "drop_that.drop_table.*.cfg";

    public const string CharacterDropListsFiles = "drop_that.character_drop_list.*.cfg";
    public const string DropTableListsFiles = "drop_that.drop_table_list.*.cfg";

    public static void LoadAll()
    {
        LoadGeneralConfigurations();

        LoadAllCharacterDropLists();

        LoadAllCharacterDropConfigurations();

        LoadAllDropTableLists();

        LoadAllDropTableConfigurations();
    }

    public static void LoadGeneralConfigurations()
    {
        string generalConfig = Path.Combine(Paths.ConfigPath, DefaultConfigFile);

        Log.LogInfo($"Loading general configuration from {generalConfig}.");

        GeneralConfig = new GeneralConfiguration();
        GeneralConfig.Load(new ConfigFile(generalConfig, true));
    }

    public static void LoadAllCharacterDropLists()
    {
        Log.LogInfo("Loading character drop lists");

        var supplementalFiles = Directory.GetFiles(Paths.ConfigPath, CharacterDropListsFiles, SearchOption.AllDirectories);
        Log.LogDebug($"Found {supplementalFiles.Length} files");

        CharacterDropListConfigurationFile configs = new();

        foreach (var file in supplementalFiles)
        {
            try
            {
                var supplementalConfig = LoadConfigFile<CharacterDropListConfigurationFile>(file);

                supplementalConfig.MergeInto(configs);
            }
            catch (Exception e)
            {
                Log.LogError($"Failed to load config file '{file}'.", e);
            }
        }

        CharacterDropLists = configs;

        Log.LogDebug("Finished loading character drop list");
    }

    public static void LoadAllCharacterDropConfigurations()
    {
        Log.LogInfo("Loading character drop configurations");

        string configPath = Path.Combine(Paths.ConfigPath, DefaultDropFile);

        var configs = LoadConfigFile<CharacterDropConfiguration>(configPath);

        if (GeneralConfig?.LoadSupplementalDropTables?.Value == true)
        {
            var supplementalFiles = Directory.GetFiles(Paths.ConfigPath, SupplementalPattern, SearchOption.AllDirectories);
            Log.LogDebug($"Found {supplementalFiles.Length} supplemental files");

            foreach (var file in supplementalFiles)
            {
                try
                {
                    var supplementalConfig = LoadConfigFile<CharacterDropConfiguration>(file);

                    supplementalConfig.MergeInto(configs);
                }
                catch(Exception e)
                {
                    Log.LogError($"Failed to load supplemental config '{file}'.", e);
                }
            }
        }

        CharacterDropConfigs = configs;

        Log.LogDebug("Finished loading drop configurations");
    }

    public static void LoadAllDropTableLists()
    {
        Log.LogInfo("Loading drop table lists");

        var configs = new DropTableListConfigurationFile();

        if (GeneralConfig?.LoadSupplementalDropTables?.Value == true)
        {
            var supplementalFiles = Directory.GetFiles(Paths.ConfigPath, DropTableListsFiles, SearchOption.AllDirectories);
            Log.LogDebug($"Found {supplementalFiles.Length} files");

            foreach (var file in supplementalFiles)
            {
                try
                {
                    var supplementalConfig = LoadConfigFile<DropTableListConfigurationFile>(file);

                    supplementalConfig.MergeInto(configs);
                }
                catch (Exception e)
                {
                    Log.LogError($"Failed to load config file '{file}'.", e);
                }
            }
        }

        DropTableLists = configs;

        Log.LogDebug("Finished drop table lists");
    }

    public static void LoadAllDropTableConfigurations()
    {
        Log.LogInfo("Loading drop table configurations");

        string configPath = Path.Combine(Paths.ConfigPath, DefaultDropTablesFile);

        var configs = LoadConfigFile<DropTableConfiguration>(configPath);

        if (GeneralConfig?.LoadSupplementalDropTables?.Value == true)
        {
            var supplementalFiles = Directory.GetFiles(Paths.ConfigPath, SupplementalDropTablePattern, SearchOption.AllDirectories);
            Log.LogDebug($"Found {supplementalFiles.Length} supplemental files");

            foreach (var file in supplementalFiles)
            {
                try
                {
                    var supplementalConfig = LoadConfigFile<DropTableConfiguration>(file);

                    supplementalConfig.MergeInto(configs);
                }
                catch (Exception e)
                {
                    Log.LogError($"Failed to load supplemental config '{file}'.", e);
                }
            }
        }

        DropTableConfigs = configs;

        Log.LogDebug("Finished loading drop configurations");
    }


    private static TConfig LoadConfigFile<TConfig>(string configPath) where TConfig : Config, IConfigFile
    {
        Log.LogDebug($"Loading file {configPath}");

        var configFile = new ConfigFile(configPath, true);

        if (GeneralConfig?.StopTouchingMyConfigs?.Value != null)
        {
            configFile.SaveOnConfigSet = !GeneralConfig.StopTouchingMyConfigs;
        }

        return ConfigurationLoader.LoadConfiguration<TConfig>(configFile);
    }
}

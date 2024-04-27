using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BepInEx;
using DropThat.Configuration;
using ThatCore.Config.Toml;
using ThatCore.Config.Toml.Mapping;
using ThatCore.Config.Toml.Schema;
using ThatCore.Logging;

namespace DropThat.Drop.CharacterDropSystem.Configuration.Toml;

internal static partial class ConfigurationFileManager
{
    public const string MainDropFile = "drop_that.character_drop.cfg";
    public const string CharacterDropFiles = "drop_that.character_drop.*.cfg";
    public const string CharacterDropListsFiles = "drop_that.character_drop_list.*.cfg";

    public static ITomlSchemaLayer Schema;
    public static ITomlSchemaLayer ListSchema;

    private static ConfigToObjectMapper<CharacterDropSystemConfiguration> _configMapper;

    public static CharacterDropConfigMapper Mapper;

    internal static void Clear()
    {
        Mapper = null;
    }

    internal static CharacterDropConfigMapper PrepareMappings()
    {
        Mapper = new CharacterDropConfigMapper();

        RegisterMainMappings(Mapper);

        Schema = Mapper.BuildSchema();
        ListSchema = Mapper.BuildListSchema();

        return Mapper;
    }

    public static void LoadConfigs(CharacterDropSystemConfiguration configuration)
    {
        PrepareMappings();

        _configMapper = Mapper.CreateMapper(configuration);

        var listConfig = LoadAllCharacterDropLists();
        var mainConfig = LoadAllCharacterDropConfigurations();

        var combinedConfig = MergeLists(listConfig, mainConfig);

        _configMapper.Execute(combinedConfig);

        Log.Debug?.Log("Finished loading character_drop configs");
    }

    private static TomlConfig LoadAllCharacterDropLists()
    {
        var pluginFiles = Directory.GetFiles(Paths.PluginPath, CharacterDropListsFiles, SearchOption.AllDirectories);

        var configFiles = Directory.GetFiles(Paths.ConfigPath, CharacterDropListsFiles, SearchOption.AllDirectories);

        var supplementalFiles =
            pluginFiles
            .Concat(configFiles)
            .ToArray();

        Log.Debug?.Log($"Loading '{supplementalFiles.Length}' character_drop list files");

        TomlConfig[] configs = new TomlConfig[supplementalFiles.Length];

        Parallel.For(0, supplementalFiles.Length, (i) =>
        {
            try
            {
                configs[i] = TomlSchemaFileLoader.LoadFile(supplementalFiles[i], ListSchema);
            }
            catch (Exception e)
            {
                Log.Error?.Log($"Failed to load config file '{supplementalFiles[i]}'.", e);
            }
        });

        // Override based on order of occurence. Later loaded overrides earlier.
        var combinedConfig = configs.FirstOrDefault() ?? new();

        for (int i = 1; i < configs.Length; ++i)
        {
            TomlConfigMerger.Merge(configs[i], combinedConfig);
        }

        return combinedConfig;
    }

    private static TomlConfig LoadAllCharacterDropConfigurations()
    {
        var pluginFiles = Directory.GetFiles(Paths.PluginPath, CharacterDropFiles, SearchOption.AllDirectories);

        var configFiles = Directory.GetFiles(Paths.ConfigPath, CharacterDropFiles, SearchOption.AllDirectories);

        var supplementalFiles = 
            pluginFiles
            .Concat(configFiles)
            .ToArray();

        Log.Debug?.Log($"Loading '{supplementalFiles.Length + 1}' character_drop files");

        TomlConfig[] configs = new TomlConfig[supplementalFiles.Length];

        Parallel.For(0, supplementalFiles.Length, (i) =>
        {
            try
            {
                configs[i] = TomlSchemaFileLoader.LoadFile(supplementalFiles[i], Schema);
            }
            catch (Exception e)
            {
                Log.Error?.Log($"Failed to load config file '{supplementalFiles[i]}'.", e);
            }
        });

        // Override based on order of occurence. Later loaded overrides earlier.
        var combinedConfig = configs.FirstOrDefault() ?? new();

        foreach (var config in configs.Skip(1))
        {
            TomlConfigMerger.Merge(config, combinedConfig);
        }

        // Load and apply main config last
        var configPath = Path.Combine(Paths.ConfigPath, MainDropFile);

        if (!File.Exists(configPath))
        {
            CreateDefaultConfigFile(configPath);
        };

        var mainConfig = TomlSchemaFileLoader.LoadFile(configPath, Schema);

        TomlConfigMerger.Merge(mainConfig, combinedConfig);

        return combinedConfig;
    }

    internal static TomlConfig MergeLists(TomlConfig listConfigs, TomlConfig mainConfig)
    {
        TomlConfig mergedConfig = new();

        Dictionary<string, TomlConfig> listConfigsByName = listConfigs
            .Sections?
            .ToDictionary(x => x.Key.Name, x => x.Value) 
            ?? new();

        foreach (var section in mainConfig.Sections)
        {
            if (section.Value.Settings.TryGetValue("UseDropList", out var setting) && 
                setting.IsSet &&
                setting is TomlSetting<List<string>> useDropLists &&
                useDropLists.Value is not null)
            {
                foreach (var list in useDropLists.Value)
                {
                    if (listConfigsByName.TryGetValue(list, out var listConfig))
                    {
                        var temp = new TomlConfig();
                        temp.Sections.Add(section.Key, listConfig);

                        TomlConfigMerger.Merge(temp, mergedConfig);
                    }
                }
            }
        }

        TomlConfigMerger.Merge(mainConfig, mergedConfig);

        return mergedConfig;
    }

    private static void CreateDefaultConfigFile(string configPath)
    {
        using var file = File.Create(configPath);
        using var writer = new StreamWriter(file);

        writer.WriteLine("# Auto-generated file for adding CharacterDrop configurations.");
        writer.WriteLine("# This file is empty by default. It is intended to contains changes only, to avoid unintentional modifications as well as to reduce unnecessary performance cost.");
        writer.WriteLine("# Full documentation can be found at https://github.com/ASharpPen/Valheim.DropThat/wiki.");
        writer.WriteLine("# To get started: ");
        writer.WriteLine($"#     1. Generate default configs in BepInEx/Debug folder, by enabling {nameof(GeneralConfig.WriteCharacterDropsToFile)} in '{GeneralConfigManager.GeneralConfigFile}'.");
        writer.WriteLine($"#     2. Start game and enter a world, and wait a short moment for files to generate.");
        writer.WriteLine( "#     3. Go to generated file, and copy the drops you want to modify from there into this file");
        writer.WriteLine( "#     4. Make your changes.");
        writer.WriteLine($"# To find modded configs and change those, enable WriteLoadedConfigsToFile in '{GeneralConfigManager.GeneralConfigFile}', and do as described above.");
        writer.WriteLine();
    }
}

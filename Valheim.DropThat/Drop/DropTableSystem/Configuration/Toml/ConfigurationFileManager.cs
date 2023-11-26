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

namespace DropThat.Drop.DropTableSystem.Configuration.Toml;

internal static partial class ConfigurationFileManager
{
    public const string MainDropFile = "drop_that.drop_table.cfg";
    public const string DropTableFiles = "drop_that.drop_table.*.cfg";
    public const string DropTableListsFiles = "drop_that.drop_table_list.*.cfg";

    private static ITomlSchemaLayer _schema;
    private static ITomlSchemaLayer _listSchema;

    public static ConfigToObjectMapper<DropTableSystemConfiguration> ConfigObjectMapper;
    public static DropTableConfigMapper ConfigMapper { get; set; }
    public static DropTableListConfigMapper ConfigListMapper { get; set; }

    public static void LoadConfigs(DropTableSystemConfiguration configuration)
    {
        if (ConfigMapper is null)
        {
            ConfigMapper = RegisterMainMappings();
            _schema = ConfigMapper.BuildSchema();
        }

        if (ConfigListMapper is null)
        {
            ConfigListMapper = RegisterListMappings();
            _listSchema = ConfigListMapper.BuildSchema();
        }

        ConfigObjectMapper = ConfigMapper.CreateMapper(configuration);

        var listConfig = LoadAllDropTableLists();
        var mainConfig = LoadAllDropTables();

        var result = MergeListAndMain(listConfig, mainConfig);

        ConfigObjectMapper.Execute(result);

        Log.Debug?.Log("Finished loading drop_table configs");
    }

    private static TomlConfig LoadAllDropTableLists()
    {
        var pluginFiles = Directory.GetFiles(Paths.PluginPath, DropTableListsFiles, SearchOption.AllDirectories);

        var configFiles = Directory.GetFiles(Paths.ConfigPath, DropTableListsFiles, SearchOption.AllDirectories);

        var supplementalFiles =
            pluginFiles
            .Concat(configFiles)
            .ToArray();

        Log.Debug?.Log($"Loading '{supplementalFiles.Length}' drop_table list files.");

        TomlConfig[] configs = new TomlConfig[supplementalFiles.Length];

        Parallel.For(0, supplementalFiles.Length, (i) =>
        {
            try
            {
                configs[i] = TomlSchemaFileLoader.LoadFile(supplementalFiles[i], _listSchema);
            }
            catch (Exception e)
            {
                Log.Error?.Log($"Failed to load config file '{supplementalFiles[i]}'.", e);
            }
        });

        return MergeConfigs(configs);
    }

    private static TomlConfig LoadAllDropTables()
    {
        var pluginFiles = Directory.GetFiles(Paths.PluginPath, DropTableFiles, SearchOption.AllDirectories);

        var configFiles = Directory.GetFiles(Paths.ConfigPath, DropTableFiles, SearchOption.AllDirectories);

        var supplementalFiles = 
            pluginFiles
            .Concat(configFiles)
            .ToArray();

        Log.Debug?.Log($"Loading '{supplementalFiles.Length + 1}' drop_table files");

        List<TomlConfig> configs = new(supplementalFiles.Length + 1);

        Parallel.For(0, supplementalFiles.Length, (i) =>
        {
            try
            {
                configs.Add(TomlSchemaFileLoader.LoadFile(supplementalFiles[i], _schema));
            }
            catch (Exception e)
            {
                Log.Error?.Log($"Failed to load config file '{supplementalFiles[i]}'.", e);
            }
        });

        // Load and apply main config last
        var configPath = Path.Combine(Paths.ConfigPath, MainDropFile);

        if (!File.Exists(configPath))
        {
            CreateDefaultConfigFile(configPath);
        };

        var mainConfig = TomlSchemaFileLoader.LoadFile(configPath, _schema);

        configs.Add(mainConfig);

        return MergeConfigs(configs);
    }

    public static TomlConfig MergeConfigs(IList<TomlConfig> configs)
    {
        // Override based on order of occurence. Later loaded overrides earlier.
        var combinedConfig = new TomlConfig();

        foreach (var config in configs)
        {
            TomlConfigMerger.Merge(config, combinedConfig);
        }

        return combinedConfig;
    }

    public static TomlConfig MergeListAndMain(TomlConfig listToml, TomlConfig mainToml)
    {
        TomlConfig mergedConfig = new();

        Dictionary<string, TomlConfig> listConfigsByName = listToml
            .Sections?
            .ToDictionary(x => x.Key.Name, x => x.Value)
            ?? new();

        foreach (var section in mainToml.Sections)
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

        TomlConfigMerger.Merge(mainToml, mergedConfig);

        return mergedConfig;
    }

    private static void CreateDefaultConfigFile(string configPath)
    {
        using var file = File.Create(configPath);
        using var writer = new StreamWriter(file);

        writer.WriteLine("# Auto-generated file for adding DropTable configurations.");
        writer.WriteLine("# This file is empty by default. It is intended to contains changes only, to avoid unintentional modifications as well as to reduce unnecessary performance cost.");
        writer.WriteLine("# Full documentation can be found at https://github.com/ASharpPen/Valheim.DropThat/wiki.");
        writer.WriteLine("# To get started: ");
        writer.WriteLine($"#     1. Generate default configs in BepInEx/Debug folder, by enabling {nameof(GeneralConfig.WriteDropTablesToFiles)} in '{GeneralConfigManager.GeneralConfigFile}'.");
        writer.WriteLine($"#     2. Start game and enter a world, and wait a short moment for files to generate.");
        writer.WriteLine( "#     3. Go to generated file, and copy the drops you want to modify from there into this file");
        writer.WriteLine( "#     4. Make your changes.");
        writer.WriteLine($"# To find modded configs and change those, enable WriteLoadedConfigsToFile in '{GeneralConfigManager.GeneralConfigFile}', and do as described above.");
        writer.WriteLine();
    }
}

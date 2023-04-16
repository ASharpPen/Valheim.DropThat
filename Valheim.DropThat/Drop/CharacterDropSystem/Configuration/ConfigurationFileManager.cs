using System;
using System.IO;
using System.Threading.Tasks;
using BepInEx;
using DropThat.Configuration;
using ThatCore.Config.Toml;
using ThatCore.Config.Toml.Mapping;
using ThatCore.Config.Toml.Schema;
using ThatCore.Logging;

namespace DropThat.Drop.CharacterDropSystem.Configuration;

internal static class ConfigurationFileManager
{
    public const string MainDropFile = "drop_that.character_drop.cfg";
    public const string CharacterDropFiles = "drop_that.character_drop.*.cfg";
    public const string CharacterDropListsFiles = "drop_that.character_drop_list.*.cfg";

    private static TomlSchemaBuilder _schemaBuilder;
    private static TomlSchemaBuilder _listSchemaBuilder;

    private static ITomlSchemaLayer _schema;
    private static ITomlSchemaLayer _listSchema;

    private static ConfigToObjectMapper<CharacterDropSystemConfiguration> _listConfigMapper;
    private static ConfigToObjectMapper<CharacterDropSystemConfiguration> _configMapper;

    public static void Clear()
    {
        _schemaBuilder = null;
        _listSchemaBuilder = null;
        _schema = null;
        _listSchema = null;
    }

    public static void LoadConfigs(CharacterDropSystemConfiguration configuration)
    {
        if (_schema is null)
        {
            _schemaBuilder = CharacterDropSchemaGenerator.GenerateCfgSchema();
            _schema = _schemaBuilder.Build();
        }

        if (_listSchema is null)
        {
            _listSchemaBuilder = CharacterDropListSchemaGenerator.GenerateCfgSchema();
            _listSchema = _listSchemaBuilder.Build();
        }

        _configMapper = CharacterDropSchemaGenerator.GenerateConfigLoader(configuration);
        _listConfigMapper = CharacterDropListSchemaGenerator.GenerateConfigLoader(configuration);

        LoadAllCharacterDropLists();
        LoadAllCharacterDropConfigurations();

        Log.Debug?.Log("Finished loading character_drop configs");
    }

    private static void LoadAllCharacterDropLists()
    {
        var supplementalFiles = Directory.GetFiles(Paths.ConfigPath, CharacterDropListsFiles, SearchOption.AllDirectories);
        Log.Debug?.Log($"Loading '{supplementalFiles.Length}' character_drop list files");

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

        foreach (var config in configs)
        {
            _listConfigMapper.Execute(config);
        }
    }

    private static void LoadAllCharacterDropConfigurations()
    {
        var customFiles = Directory.GetFiles(Paths.ConfigPath, CharacterDropFiles, SearchOption.AllDirectories);
        Log.Debug?.Log($"Loading '{customFiles.Length + 1}' character_drop files");

        TomlConfig[] configs = new TomlConfig[customFiles.Length];

        Parallel.For(0, customFiles.Length, (i) =>
        {
            try
            {
                configs[i] = TomlSchemaFileLoader.LoadFile(customFiles[i], _schema);
            }
            catch (Exception e)
            {
                Log.Error?.Log($"Failed to load config file '{customFiles[i]}'.", e);
            }
        });

        foreach (var config in configs)
        {
            _configMapper.Execute(config);
        }

        // Load and apply main config last
        var configPath = Path.Combine(Paths.ConfigPath, MainDropFile);

        if (!File.Exists(configPath))
        {
            CreateDefaultConfigFile(configPath);
        };

        var mainConfig = TomlSchemaFileLoader.LoadFile(configPath, _schema);

        _configMapper.Execute(mainConfig);
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

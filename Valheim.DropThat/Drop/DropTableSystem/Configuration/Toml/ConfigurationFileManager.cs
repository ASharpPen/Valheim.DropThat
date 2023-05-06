using System;
using System.IO;
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

    private static ConfigToObjectMapper<DropTableSystemConfiguration> _configMapper;
    private static ConfigToObjectMapper<DropTableSystemConfiguration> _listConfigMapper;

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

        _configMapper = ConfigMapper.CreateMapper(configuration);
        _listConfigMapper = ConfigListMapper.CreateMapper(configuration);

        LoadAllDropTableLists();
        LoadAllDropTables();

        Log.Debug?.Log("Finished loading drop_table configs");
    }

    private static void LoadAllDropTableLists()
    {
        var supplementalFiles = Directory.GetFiles(Paths.ConfigPath, DropTableListsFiles, SearchOption.AllDirectories);

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

        foreach (var config in configs)
        {
            _listConfigMapper.Execute(config);
        }
    }

    private static void LoadAllDropTables()
    {
        var customFiles = Directory.GetFiles(Paths.ConfigPath, DropTableFiles, SearchOption.AllDirectories);
        Log.Debug?.Log($"Loading '{customFiles.Length + 1}' drop_table files");

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

using System;
using System.IO;
using System.Threading.Tasks;
using BepInEx;
using ThatCore.Config.Toml;
using ThatCore.Config.Toml.Mapping;
using ThatCore.Config.Toml.Schema;
using ThatCore.Logging;

namespace DropThat.Drop.DropTableSystem.Configuration.Toml;

internal static class ConfigurationFileManager
{
    public const string MainDropFile = "drop_that.drop_table.cfg";
    public const string DropTableFiles = "drop_that.drop_table.*.cfg";
    public const string DropTableListsFiles = "drop_that.drop_table_list.*.cfg";

    private static TomlSchemaBuilder _schemaBuilder;
    private static TomlSchemaBuilder _listSchemaBuilder;

    private static ITomlSchemaLayer _schema;
    private static ITomlSchemaLayer _listSchema;

    private static ConfigToObjectMapper<DropTableSystemConfiguration> _configMapper;
    private static ConfigToObjectMapper<DropTableSystemConfiguration> _listConfigMapper;

    public static void LoadConfigs(DropTableSystemConfiguration configuration)
    {
        if (_schema is null)
        {
            _schemaBuilder = DropTableSchemaGenerator.GenerateCfgSchema();
            _schema = _schemaBuilder.Build();
        }

        if (_listSchema is null)
        {
            _listSchemaBuilder = DropTableListSchemaGenerator.GenerateCfgSchema();
            _listSchema = _listSchemaBuilder.Build();
        }

        _configMapper = DropTableSchemaGenerator.GenerateConfigLoader(configuration);
        _listConfigMapper = DropTableListSchemaGenerator.GenerateConfigLoader(configuration);

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
        var mainConfig = TomlSchemaFileLoader.LoadFile(MainDropFile, _schema);

        _configMapper.Execute(mainConfig);
    }
}

using System;
using System.Diagnostics;
using System.IO;
using BepInEx;
using DropThat.Core;
using DropThat.Core.Toml;

namespace DropThat.Drop.CharacterDropSystem.Configurations;

internal static class CharacterDropConfigurationFileManager
{
    public const string CharacterDropFile = "drop_that.character_drop.cfg";
    public const string CharacterDropSupplementalPattern = "drop_that.character_drop.*.cfg";
    public const string CharacterDropListsPattern = "drop_that.character_drop_list.*.cfg";

    public static CharacterDropConfigurationFile CharacterDropConfig;
    public static CharacterDropListConfigurationFile CharacterDropListConfig;

    public static void LoadAllConfigurations()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        CharacterDropConfig = LoadCharacterDrops();
        CharacterDropListConfig = LoadCharacterDropLists();

        stopwatch.Stop();

        Log.LogInfo("Loading character drop configs took: " + stopwatch.Elapsed);
    }

    public static CharacterDropConfigurationFile LoadCharacterDrops()
    {
        Log.LogInfo($"Loading character drop configurations.");

        string configPath = Path.Combine(Paths.ConfigPath, CharacterDropFile);

        if (!File.Exists(configPath))
        {
            CreateDefaultCharacterDropFile(configPath);
        }

        CharacterDropConfigurationFile configs = new();

        var supplementalFiles = Directory.GetFiles(Paths.ConfigPath, CharacterDropSupplementalPattern, SearchOption.AllDirectories);
        Log.LogDebug($"Found {supplementalFiles.Length} supplemental character drop config files");

        foreach (var file in supplementalFiles)
        {
            try
            {
                var supplementalConfig = LoadCharacterDropConfig(file);

                supplementalConfig.MergeInto(configs);
            }
            catch (Exception e)
            {
                Log.LogError($"Failed to load supplemental config '{file}'.", e);
            }
        }

        var mainConfig = LoadCharacterDropConfig(configPath);
        mainConfig.MergeInto(configs);

        Log.LogDebug("Finished loading character drop configurations");

        return configs;
    }

    public static CharacterDropListConfigurationFile LoadCharacterDropLists()
    {
        Log.LogInfo($"Loading character drop list configurations.");

        var supplementalFiles = Directory.GetFiles(Paths.ConfigPath, CharacterDropListsPattern, SearchOption.AllDirectories);
        Log.LogDebug($"Found {supplementalFiles.Length} supplemental character drop config list files");

        CharacterDropListConfigurationFile configs = new();

        foreach (var file in supplementalFiles)
        {
            try
            {
                var supplementalConfig = LoadCharacterDropListConfig(file);

                supplementalConfig.MergeInto(configs);
            }
            catch (Exception e)
            {
                Log.LogError($"Failed to load supplemental config '{file}'.", e);
            }
        }

        return configs;
    }

    private static CharacterDropConfigurationFile LoadCharacterDropConfig(string configPath)
    {
        Log.LogDebug($"Loading character drop configurations from {configPath}.");

        return TomlLoader.LoadFile<CharacterDropConfigurationFile>(configPath);
    }

    private static CharacterDropListConfigurationFile LoadCharacterDropListConfig(string configPath)
    {
        Log.LogDebug($"Loading character drop list configurations from {configPath}.");

        return TomlLoader.LoadFile<CharacterDropListConfigurationFile>(configPath);
    }


    private static void CreateDefaultCharacterDropFile(string configPath)
    {
        using var file = File.Create(configPath);

        // TODO: Write intro
        /*
        using var writer = new StreamWriter(file);
        writer.WriteLine("# Auto-generated file for adding World Spawner configurations.");
        writer.WriteLine("# This file is empty by default. It is intended to contains changes only, to avoid unintentional modifications as well as to reduce unnecessary performance cost.");
        writer.WriteLine("# Full documentation can be found at https://asharppen.github.io/Valheim.SpawnThat.");
        writer.WriteLine("# To get started: ");
        writer.WriteLine($"#     1. Generate default configs in BepInEx/Debug folder, by enabling {nameof(GeneralConfiguration.WriteSpawnTablesToFileBeforeChanges)} in 'spawn_that.cfg'.");
        writer.WriteLine($"#     2. Start game and enter a world, and wait a short moment (ca. 10 seconds) for files to generate.");
        writer.WriteLine("#     3. Go to generated file, and copy the creatures you want to modify into this file");
        writer.WriteLine("#     4. Make your changes.");
        writer.WriteLine($"# To find modded configs and change those, enable {nameof(GeneralConfiguration.WriteSpawnTablesToFileAfterChanges)} in 'spawn_that.cfg', and do as described above.");
        writer.WriteLine();
        */
    }
}

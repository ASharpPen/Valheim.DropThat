﻿using System;
using ThatCore.Config.Toml;
using ThatCore.Config.Toml.Writers;
using DropThat.Drop.CharacterDropSystem.Managers;
using ThatCore.Config.Toml.Schema;
using DropThat.Debugging;
using DropThat.Drop.CharacterDropSystem.Configuration.Toml;
using DropThat.Configuration;
using ThatCore.Lifecycle;
using DropThat.Core;

namespace DropThat.Drop.CharacterDropSystem.Debug;

internal static class DebugWriter
{
    private static bool ConfigsLoaded = false;
    private static bool ZnetSceneStarted = false;

    public static void Configure()
    {
        LifecycleManager.OnWorldInit += () =>
        {
            ConfigsLoaded = false;
            ZnetSceneStarted = false;
        };

        LifecycleManager.OnFindSpawnPointFirstTime += () =>
        {
            if (GeneralConfigManager.Config?.WriteCreatureItemsToFile)
            {
                CreatureItemFileWriter.WriteToFile();
            }
        };

        DropThatLifecycleManager.OnZnetSceneStarted += () =>
        {
            ConfigsLoaded = true;

            TryWriteDebugFiles();
        };

        DropSystemConfigManager.OnConfigsLoaded += () =>
        {
            ZnetSceneStarted = true;

            TryWriteDebugFiles();
        };

        static void TryWriteDebugFiles()
        {
            if (!ConfigsLoaded ||
                !ZnetSceneStarted)
            {
                return;
            }

            if (GeneralConfigManager.Config?.WriteCharacterDropsToFile)
            {
                WriteExpectedPreChangesToDisk();
            }

            if (GeneralConfigManager.Config?.WriteLoadedCharacterDropsToFile)
            {
                WriteLoadedConfigsToDisk();
            }

            if (GeneralConfigManager.Config?.WriteCharacterDropsToFileAfterChanges)
            {
                WriteExpectedPostChangesToDisk();
            }
        }
    }

    public static void WriteExpectedPreChangesToDisk()
    {
        var drops = CharacterDropManager.CompileWithoutTemplates();

        if (ConfigurationFileManager.Mapper is null)
        {
            ConfigurationFileManager.PrepareMappings();
        }

        var config = ConfigurationFileManager.Mapper.MapToConfigFromTemplates(drops);

        var content = TomlConfigWriter.WriteToString(config, new()
        {
            AddComments = false,
            Header =
                $"# This file was auto-generated by Drop That {DropThatPlugin.Version} at {DateTimeOffset.UtcNow.ToString("u")}, with Valheim '{Version.CurrentVersion.m_major}.{Version.CurrentVersion.m_minor}.{Version.CurrentVersion.m_patch}'.\n" +
                $"# The file is intended for debugging and investigating the drops from entities with a CharacterDrop (the name of the drop tables mostly used by creatures).\n" +
                $"# The entries listed here show the expected drops before any Drop That configs are applied.\n" +
                $"# Note, the actual dropping is calculated for each entity at run-time, so this file might not perfectly reflect the game state.\n" +
                $"# This file is not scanned by Drop That, and any changes done will therefore have no effect. Copy sections into a CharacterDrop config in the configs folder if you want to change things.\n" +
                $"# See https://github.com/ASharpPen/Valheim.DropThat/wiki/CharacterDrop-Configuration for additional details."
        });

        DebugFileWriter.WriteFile(
            content,
            "drop_that.character_drop.before_changes.cfg",
            "expected CharacterDrop tables before Drop That changes are applied");
    }

    public static void WriteExpectedPostChangesToDisk()
    {
        var characterDrops = CharacterDropManager.CompileAllPrefabDrops();

        var characterDropConfig = ConfigurationFileManager.Mapper.MapToConfigFromTemplates(characterDrops);

        var content = TomlConfigWriter.WriteToString(characterDropConfig, new TomlWriterSettings()
        {
            AddComments = false,
            Header =
                $"# This file was auto-generated by Drop That {DropThatPlugin.Version} at {DateTimeOffset.UtcNow.ToString("u")}, with Valheim '{Version.CurrentVersion.m_major}.{Version.CurrentVersion.m_minor}.{Version.CurrentVersion.m_patch}'.\n" +
                $"# The file is intended for debugging and investigating the drops from entities with a CharacterDrop (the name of the drop tables mostly used by creatures).\n" +
                $"# The entries listed here show the expected result of applying the loaded Drop That configs. This will include any unchanged drops.\n" +
                $"# Note, the actual dropping is calculated for each entity at run-time, so this file might not perfectly reflect the game state.\n" +
                $"# This file is not scanned by Drop That, and any changes done will therefore have no effect. Copy sections into a CharacterDrop config in the configs folder if you want to change things.\n" +
                $"# See https://github.com/ASharpPen/Valheim.DropThat/wiki/CharacterDrop-Configuration for additional details."
        });

        DebugFileWriter.WriteFile(
            content, 
            "drop_that.character_drop.after_changes.cfg", 
            "expected CharacterDrops after Drop That changes are applied");
    }

    public static void WriteLoadedConfigsToDisk()
    {
        var templates = CharacterDropTemplateManager.GetTemplates();

        var tomlFile = ConfigurationFileManager.Mapper.MapToConfigFromTemplates(templates);

        var content = TomlConfigWriter.WriteToString(tomlFile, new TomlWriterSettings()
        {
            AddComments = false,
            Header =
                $"# This file was auto-generated by Drop That {DropThatPlugin.Version} at {DateTimeOffset.UtcNow.ToString("u")}, with Valheim '{Version.CurrentVersion.m_major}.{Version.CurrentVersion.m_minor}.{Version.CurrentVersion.m_patch}'.\n" +
                $"# The entries listed here were generated from the internally loaded CharacterDrop configurations.\n" +
                $"# This is intended to reveal the state of Drop That, after loading configs from all sources, and before applying them to their respective target drop tables.\n" +
                $"# This file is not scanned by Drop That, and any changes done will therefore have no effect. Copy sections into a CharacterDrop config in the configs folder if you want to change things."
        });

        DebugFileWriter.WriteFile(content, "drop_that.character_drop.loaded.cfg", "loaded CharacterDrop configs");
    }

    public static void WriteSchemaToDisk()
    {
        var content = TomlSchemaWriter.WriteToString(
            ConfigurationFileManager.Schema, 
            new TomlWriterSettings()
            {
                AddComments = true,
                Header =
                    $"# This file was auto-generated by Drop That {DropThatPlugin.Version} at {DateTimeOffset.UtcNow.ToString("u")}, with Valheim '{Version.CurrentVersion.m_major}.{Version.CurrentVersion.m_minor}.{Version.CurrentVersion.m_patch}'.\n" +
                    $"# This file is a description of all available settings for drop_that.character_drop configs, their default values and the expected structure.\n" +
                    $"# For more detailed documentation, see the Drop That wiki."
            });

        DebugFileWriter.WriteFile(content, "drop_that.character_drop.schema.cfg", "schema for CharacterDrop configs");

        var listContent = TomlSchemaWriter.WriteToString(
            ConfigurationFileManager.ListSchema,
            new TomlWriterSettings()
            {
                AddComments = true,
                Header =
                $"# This file was auto-generated by Drop That {DropThatPlugin.Version} at {DateTimeOffset.UtcNow.ToString("u")}, with Valheim '{Version.CurrentVersion.m_major}.{Version.CurrentVersion.m_minor}.{Version.CurrentVersion.m_patch}'.\n" +
                    $"# This file is a description of all available settings for drop_that.character_drop_list configs, their default values and the expected structure.\n" +
                    $"# For more detailed documentation, see the Drop That wiki."
            });

        DebugFileWriter.WriteFile(listContent, "drop_that.character_drop.schema_list.cfg", "schema for CharacterDrop list configs");
    }
}
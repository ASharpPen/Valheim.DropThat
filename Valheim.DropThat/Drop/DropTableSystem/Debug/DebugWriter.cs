﻿using System;
using DropThat.Configuration;
using DropThat.Core;
using DropThat.Debugging;
using DropThat.Drop.DropTableSystem.Configuration.Toml;
using DropThat.Drop.DropTableSystem.Managers;
using ThatCore.Config.Toml;
using ThatCore.Lifecycle;
using ThatCore.Logging;

namespace DropThat.Drop.DropTableSystem.Debug;

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

        DropThatLifecycleManager.OnZnetSceneStarted += () =>
        {
            ConfigsLoaded = true;

            TryWriteFiles();
        };

        DropSystemConfigManager.OnConfigsLoaded += () =>
        {
            ZnetSceneStarted = true;

            TryWriteFiles();
        };

        static void TryWriteFiles()
        {
            if (!ConfigsLoaded ||
                !ZnetSceneStarted)
            {
                return;
            }

            if (GeneralConfigManager.Config?.WriteLoadedDropTableDropsToFile)
            {
                WriteLoadedDropTablesToDisk();
            }

            if (GeneralConfigManager.Config?.WriteDropTablesToFileAfterChanges)
            {
                WriteExpectedPostChangesToDisk();
            }
        }
    }

    public static void WriteLoadedDropTablesToDisk()
    {
        var config = ConfigurationFileManager.ConfigMapper.MapToConfigFromTemplates(DropTableTemplateManager.Templates.Values);

        var content = TomlConfigWriter.WriteToString(config, new()
        {
            FileName = "drop_that.drop_table.loaded.cfg",
            Header =
                $"# This file was auto-generated by Drop That {DropThatPlugin.Version} at {DateTimeOffset.UtcNow.ToString("u")}, with Valheim '{Version.CurrentVersion.m_major}.{Version.CurrentVersion.m_minor}.{Version.CurrentVersion.m_patch}'.\n" +
                $"# The entries listed here were generated from the internally loaded DropTable configurations.\n" +
                $"# This is intended to reveal the state of Drop That, after loading configs from all sources, and before applying them to their respective target drop tables.\n" +
                $"# This file is not scanned by Drop That, and any changes done will therefore have no effect. Copy sections into a DropTable config in the configs folder if you want to change things."
        });

        DebugFileWriter.WriteFile(content, "drop_that.drop_table.loaded.cfg", "loaded DropTable configs");
    }

    public static void WriteExpectedPostChangesToDisk()
    {
        Log.Development?.Log("Attempting to write post changes for " + ZNetScene.instance.m_prefabs.Count + " prefabs");

        var drops = DropTableManager.CompileAllPrefabDrops();

        Log.Development?.Log("Compiled post changes for " + drops.Count + " drop tables");

        var config = ConfigurationFileManager.ConfigMapper.MapToConfigFromTemplates(drops);

        var content = TomlConfigWriter.WriteToString(config, new()
        {
            FileName = "drop_that.drop_table.after_changes.cfg",
            Header =
                $"# This file was auto-generated by Drop That {DropThatPlugin.Version} at {DateTimeOffset.UtcNow.ToString("u")}, with Valheim '{Version.CurrentVersion.m_major}.{Version.CurrentVersion.m_minor}.{Version.CurrentVersion.m_patch}'.\n" +
                $"# The file is intended for debugging and investigating the drops from entities with a DropTable (the name of the drop tables mostly used by objects and chests).\n" +
                $"# The entries listed here show the expected result of applying the loaded Drop That configs. This will include any unchanged drops.\n" +
                $"# Note, the actual dropping is calculated for each entity at run-time, so this file might not perfectly reflect the game state.\n" +
                $"# This file is not scanned by Drop That, and any changes done will therefore have no effect. Copy sections into a DropTable config in the configs folder if you want to change things." +
                $"# See https://github.com/ASharpPen/Valheim.DropThat/wiki/DropTable-Configuration for additional details."
        });

        DebugFileWriter.WriteFile(content, "drop_that.drop_table.after_changes.cfg", "expected drop tables after applying changes");
    }
}

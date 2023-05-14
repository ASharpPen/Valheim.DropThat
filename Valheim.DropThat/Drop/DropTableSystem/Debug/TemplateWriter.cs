﻿using System;
using DropThat.Debugging;
using DropThat.Drop.DropTableSystem.Configuration.Toml;
using DropThat.Drop.DropTableSystem.Managers;
using ThatCore.Config.Toml;

namespace DropThat.Drop.DropTableSystem.Debug;

internal static class TemplateWriter
{
    public static void PrintLoadedDropTables()
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
}

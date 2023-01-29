using System.Collections.Generic;
using System.IO;
using BepInEx;
using DropThat.Configuration;
using ThatCore.Extensions;
using ThatCore.Logging;
using ThatCore.Utilities.Files;

namespace DropThat.Debugging;

internal static class DebugFileWriter
{
    public static void WriteFile(string content, string filename, string fileDescription)
    {
        var filePath = Prepare(filename, fileDescription);

        File.WriteAllText(filePath, content);
    }

    public static void WriteFile(List<string> content, string filename, string fileDescription)
    {
        var filePath = Prepare(filename, fileDescription);

        File.WriteAllLines(filePath, content);
    }

    public static void WriteFile(byte[] content, string filename, string fileDescription)
    {
        var filePath = Prepare(filename, fileDescription);

        File.WriteAllBytes(filePath, content);
    }

    private static string Prepare(string filename, string fileDescription)
    {
        string debugDir = "Debug";

        if (ConfigurationManager.GeneralConfig?.DebugFileFolder is not null)
        {
            debugDir = Path.Combine(ConfigurationManager.GeneralConfig.DebugFileFolder.Value.SplitBySlash());
        }

        string filePath = Path.Combine(Paths.BepInExRootPath, debugDir, filename);

        FileUtils.EnsureDirectoryExistsForFile(filePath);

        Log.Info?.Log($"Writing {fileDescription} to file {filePath}.");

        return filePath;
    }
}

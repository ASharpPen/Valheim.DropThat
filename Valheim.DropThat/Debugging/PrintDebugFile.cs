using BepInEx;
using System.Collections.Generic;
using System.IO;
using DropThat.Configuration;
using DropThat.Core;
using ThatCore.Logging;

namespace DropThat.Debugging;

internal static class PrintDebugFile
{
    public static void PrintFile(string content, string filename, string fileDescription)
    {
        string directory = Path.Combine(Paths.BepInExRootPath, ConfigurationManager.GeneralConfig?.DebugFileFolder ?? "Debug");

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string filePath = Path.Combine(directory, filename);

        Log.Info?.Log($"Writing {fileDescription} to file {filePath}.");

        File.WriteAllText(filePath, content);
    }

    public static void PrintFile(List<string> content, string filename, string fileDescription)
    {
        string directory = Path.Combine(Paths.BepInExRootPath, ConfigurationManager.GeneralConfig?.DebugFileFolder ?? "Debug");

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string filePath = Path.Combine(directory, filename);

        Log.Info?.Log($"Writing {fileDescription} to file {filePath}.");

        File.WriteAllLines(filePath, content);
    }
}

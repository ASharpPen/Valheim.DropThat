using BepInEx;
using System.Collections.Generic;
using System.IO;
using Valheim.DropThat.Configuration;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Debugging
{
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

            Log.LogInfo($"Writing {fileDescription} to file {filePath}.");

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

            Log.LogInfo($"Writing {fileDescription} to file {filePath}.");

            File.WriteAllLines(filePath, content);
        }
    }
}

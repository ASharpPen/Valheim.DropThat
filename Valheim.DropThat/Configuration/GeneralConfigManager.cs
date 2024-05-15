using BepInEx;
using BepInEx.Configuration;
using System.IO;
using ThatCore.Logging;

namespace DropThat.Configuration;

public static class GeneralConfigManager
{
    public static GeneralConfig Config { get; private set; }

    public const string GeneralConfigFile = "drop_that.cfg";

    internal static void Load()
    {
        string generalConfig = Path.Combine(Paths.ConfigPath, GeneralConfigFile);

        Log.Info?.Log($"Loading general configuration from {generalConfig}.");

        Config = new GeneralConfig();
        Config.Load(new ConfigFile(generalConfig, true));

        Log.DebugEnabled = Config.EnableDebugLogging;
        Log.TraceEnabled = Config.EnableTraceLogging;
    }

    internal static void Set(GeneralConfig config)
    {
        Config = config;

        Log.DebugEnabled = Config.EnableDebugLogging;
        Log.TraceEnabled = Config.EnableTraceLogging;
    }
}

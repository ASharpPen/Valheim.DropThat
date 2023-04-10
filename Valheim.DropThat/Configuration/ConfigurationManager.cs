using BepInEx;
using BepInEx.Configuration;
using System.IO;
using DropThat.Configuration.ConfigTypes;
using ThatCore.Logging;

namespace DropThat.Configuration;

public static class ConfigurationManager
{
    public static GeneralConfiguration GeneralConfig;

    public const string DefaultConfigFile = "drop_that.cfg";

    public static void LoadAll()
    {
        LoadGeneralConfigurations();
    }

    public static void LoadGeneralConfigurations()
    {
        string generalConfig = Path.Combine(Paths.ConfigPath, DefaultConfigFile);

        Log.Info?.Log($"Loading general configuration from {generalConfig}.");

        GeneralConfig = new GeneralConfiguration();
        GeneralConfig.Load(new ConfigFile(generalConfig, true));
    }
}

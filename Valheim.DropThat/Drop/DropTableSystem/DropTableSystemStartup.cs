using DropThat.Drop.DropTableSystem.Configuration.Toml;
using DropThat.Drop.DropTableSystem.Debug;
using DropThat.Drop.DropTableSystem.Sync;

namespace DropThat.Drop.DropTableSystem;

internal static class DropTableSystemStartup
{
    public static void Setup()
    {
        // Setup sync
        DropTableConfigSyncManager.Configure();

        // Setup config file loading on configuration time.
        DropSystemConfigManager.OnConfigureLate += LoadFileConfigs;

        // Setup debug file outputs.
        DebugWriter.Configure();
    }

    private static void LoadFileConfigs(IDropSystemConfigCollection configCollection)
    {
        var configSystem = configCollection.GetDropSystemConfig<DropTableSystemConfiguration>();

        ConfigurationFileManager.LoadConfigs(configSystem);
    }
}

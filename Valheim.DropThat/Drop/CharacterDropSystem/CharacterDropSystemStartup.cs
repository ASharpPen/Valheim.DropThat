using DropThat.Drop.CharacterDropSystem.Configuration.Toml;
using DropThat.Drop.CharacterDropSystem.Debug;
using DropThat.Drop.CharacterDropSystem.Sync;

namespace DropThat.Drop.CharacterDropSystem;

internal static class CharacterDropSystemStartup
{
    public static void Setup()
    {
        // Setup sync
        CharacterDropConfigSyncManager.Configure();

        // Setup config file loading on configuration time.
        DropSystemConfigManager.OnConfigureLate += LoadFileConfigs;

        // Setup debug file outputs.
        DebugWriter.Configure();
    }

    private static void LoadFileConfigs(IDropSystemConfigCollection configCollection)
    {
        var configSystem = configCollection.GetDropSystemConfig<CharacterDropSystemConfiguration>();

        ConfigurationFileManager.LoadConfigs(configSystem);
    }
}

using DropThat.Drop.CharacterDropSystem.Configuration;
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

    }

    private static void LoadFileConfigs(IDropSystemConfigCollection configCollection)
    {
        var configSystem = configCollection.GetDropSystemConfig<CharacterDropSystemConfiguration>();

        ConfigurationFileManager.LoadConfigs(configSystem);
    }
}

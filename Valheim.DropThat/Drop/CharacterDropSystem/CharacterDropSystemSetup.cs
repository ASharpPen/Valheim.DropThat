using DropThat.Configuration;
using DropThat.Drop.CharacterDropSystem.Configuration;
using DropThat.Drop.CharacterDropSystem.Debug;
using DropThat.Drop.CharacterDropSystem.Managers;
using DropThat.Drop.CharacterDropSystem.Sync;
using ThatCore.Lifecycle;

namespace DropThat.Drop.CharacterDropSystem;

internal static class CharacterDropSystemSetup
{
    // TODO: Move config loading on dedicated to later in workflow. Give a bit of time for other mods like Expand World to load up and apply their changes.
    public static void PrepareModule()
    {
        ThatCore.Lifecycle.LifecycleManager.OnDedicatedServerInit += CharacterDropConfigSyncManager.Configure;
        ThatCore.Lifecycle.LifecycleManager.OnMultiplayerInit += CharacterDropConfigSyncManager.Configure;

        ThatCore.Lifecycle.LifecycleManager.OnSinglePlayerInit += LoadConfigs;
        ThatCore.Lifecycle.LifecycleManager.OnDedicatedServerInit += LoadConfigs;
    }

    private static void LoadConfigs()
    {
        var configuration = new CharacterDropSystemConfiguration();

        CharacterDropEventManager.ConfigurationsLoading();

        ConfigurationFileManager.LoadConfigs(configuration);

        configuration.Build();

        if (ConfigurationManager.GeneralConfig?.WriteCharacterDropsToFile)
        {
            TemplateWriter.WriteToDiskAsToml();
        }

        CharacterDropEventManager.ConfigurationsLoaded();
    }
}

using DropThat.Configuration;
using DropThat.Core;
using DropThat.Drop;
using DropThat.Drop.CharacterDropSystem;
using DropThat.Drop.DropTableSystem;
using ThatCore.Lifecycle;

namespace DropThat;

internal static class Startup
{
    public static void SetupServices()
    {
        // Config loading
        LifecycleManager.OnLateInit += LoadConfigs;

        // Sync
        LifecycleManager.OnNewConnection += SyncManager.SetupConfigSyncForPeer;

        // Setup modules
        GeneralConfigStartup.Setup();
        CharacterDropSystemStartup.Setup();
        DropTableSystemStartup.Setup();
    }

    private static void LoadConfigs()
    {
        // Only load if either single-player or server.
        if (LifecycleManager.GameState is (
            GameState.Singleplayer or 
            GameState.DedicatedServer))
        {
            DropSystemConfigManager.LoadConfigs();
        }
    }
}

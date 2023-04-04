using DropThat.Configuration;
using DropThat.Drop;
using ThatCore.Lifecycle;

namespace DropThat;

internal static class Startup
{
    public static void SetupServices()
    {
        // Config loading
        LifecycleManager.OnLateInit += LoadConfigs;

        // Sync
        LifecycleManager.OnNewConnection += ConfigSyncManager.SetupConfigSyncForPeer;

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

using DropThat.Configuration.Sync;
using ThatCore.Lifecycle;

namespace DropThat.Configuration;

internal static class GeneralConfigStartup
{
    public static void Setup()
    {
        // Setup sync
        GeneralConfigSyncManager.Configure();

        // Load configs on startup
        LifecycleManager.OnWorldInit += () =>
        {
            // Only load if either single-player or server.
            if (LifecycleManager.GameState is (
                GameState.Singleplayer or
                GameState.DedicatedServer))
            {
                GeneralConfigManager.Load();
            }
        };
    }
}

using DropThat.Configuration;
using ThatCore.Lifecycle;

namespace DropThat;

internal static class Startup
{
    public static void SetupServices()
    {
        ThatCore.Lifecycle.LifecycleManager.OnNewConnection += ConfigSyncManager.SetupConfigSyncForPeer;
    }
}

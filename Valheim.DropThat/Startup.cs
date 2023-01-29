using DropThat.Configuration;
using ThatCore.Lifecycle;

namespace DropThat;

internal static class Startup
{
    public static void SetupServices()
    {
        LifecycleManager.OnNewConnection += ConfigSyncManager.SetupConfigSyncForPeer;
    }
}

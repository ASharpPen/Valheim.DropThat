using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThatCore.Lifecycle;

namespace DropThat.Drop.CharacterDropSystem;

internal static class CharacterDropSystemSetup
{
    public static void PrepareModule()
    {
        LifecycleManager.OnSinglePlayerInit += LoadConfigs;
        LifecycleManager.OnDedicatedServerInit += LoadConfigs;

        LifecycleManager.OnNewConnection += HandleNetworking;
    }

    private static void LoadConfigs()
    {

    }

    private static void HandleNetworking(ZNetPeer peer)
    {

    }
}

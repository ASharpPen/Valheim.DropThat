using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropThat.Core.Network;

namespace DropThat.Core.Network;

internal static class NetworkSetup
{
    public static void SetupNetworking()
    {
        ZNet_Lifecycle_Patch.OnZnetUpdate += SplitPackageReceiverService.Update;
        ZNet_Lifecycle_Patch.OnZnetUpdate += Dispatcher.Dispatch;
    }
}

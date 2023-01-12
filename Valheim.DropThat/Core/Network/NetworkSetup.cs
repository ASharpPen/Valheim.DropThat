namespace Valheim.DropThat.Core.Network;

internal static class NetworkSetup
{
    public static void SetupNetworking()
    {
        ZNet_Lifecycle_Patch.OnZnetUpdate += SplitPackageReceiverService.Update;
        ZNet_Lifecycle_Patch.OnZnetUpdate += Dispatcher.Dispatch;
    }
}

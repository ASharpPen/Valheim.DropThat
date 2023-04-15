using System;
using DropThat.Core;
using ThatCore.Logging;
using ThatCore.Network;

namespace DropThat.Locations.Sync;

internal static class LocationSyncManager
{
    public static void Configure()
    {
        SyncManager.RegisterSyncHandlers(
            nameof(RPC_DropThat_ReceiveSimpleLocations),
            GenerateMessage,
            RPC_DropThat_ReceiveSimpleLocations);
    }

    private static IMessage GenerateMessage() => new SimpleLocationMessage();

    private static void RPC_DropThat_ReceiveSimpleLocations(ZRpc rpc, ZPackage package)
    {
        try
        {
            IncomingMessageService.ReceiveMessageAsync<SimpleLocationMessage>(package);
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error while attempting to receive locations package.", e);
        }
    }
}

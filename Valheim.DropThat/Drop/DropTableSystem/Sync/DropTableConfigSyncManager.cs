using System;
using DropThat.Configuration;
using ThatCore.Logging;
using ThatCore.Network;

namespace DropThat.Drop.DropTableSystem.Sync;

internal static class DropTableConfigSyncManager
{
    public static void Configure()
    {
        ConfigSyncManager.RegisterSyncHandlers(
            nameof(RPC_DropThat_ReceiveDropTableDropConfigs),
            GenerateMessage,
            RPC_DropThat_ReceiveDropTableDropConfigs);
    }

    private static IMessage GenerateMessage() => new DropTableConfigMessage();

    private static void RPC_DropThat_ReceiveDropTableDropConfigs(ZRpc rpc, ZPackage package)
    {
        try
        {
            IncomingMessageService.ReceiveMessageAsync<DropTableConfigMessage>(package);
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error while attempting to receive DropTable config package.", e);
        }
    }
}

using System;
using DropThat.Core;
using ThatCore.Logging;
using ThatCore.Network;

namespace DropThat.Configuration.Sync;

internal static class GeneralConfigSyncManager
{
    public static void Configure()
    {
        SyncManager.RegisterSyncHandlers(
            nameof(RPC_DropThat_ReceiveGeneralConfig),
            GenerateMessage,
            RPC_DropThat_ReceiveGeneralConfig);
    }

    private static IMessage GenerateMessage() => new GeneralConfigMessage();

    private static void RPC_DropThat_ReceiveGeneralConfig(ZRpc rpc, ZPackage package)
    {
        try
        {
            IncomingMessageService.ReceiveMessageAsync<GeneralConfigMessage>(package);
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error while attempting to receive general config package.", e);
        }
    }
}

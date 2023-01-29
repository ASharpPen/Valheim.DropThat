using System;
using DropThat.Configuration;
using ThatCore.Logging;
using ThatCore.Network;

namespace DropThat.Drop.CharacterDropSystem.Sync;

internal static class CharacterDropConfigSyncManager
{
    public static void Configure()
    {
        ConfigSyncManager.RegisterSyncHandlers(
            nameof(RPC_DropThat_ReceiveCharacterDropConfigs),
            GenerateMessage,
            RPC_DropThat_ReceiveCharacterDropConfigs);
    }

    private static IMessage GenerateMessage() => new CharacterDropConfigMessage();

    private static void RPC_DropThat_ReceiveCharacterDropConfigs(ZRpc rpc, ZPackage package)
    {
        try
        {
            IncomingMessageService.ReceiveMessageAsync<CharacterDropConfigMessage>(package);
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error while attempting to receive CharacterDrop config package.", e);
        }
    }
}

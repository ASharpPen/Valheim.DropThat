using System;
using DropThat.Core;
using DropThat.Drop;
using ThatCore.Lifecycle;
using ThatCore.Logging;

namespace DropThat.Commands;

internal static class ReloadConfigsCommand
{
    public const string CommandName = "dropthat:reload";

    internal static void Register()
    {
        LifecycleManager.OnNewConnection += ConfigureRPCs;

        new Terminal.ConsoleCommand(
            CommandName,
            "Reload configurations and re-syncronize. This will also reload all nearby entities with drop tables.",
            (args) =>
            {
                try
                {
                    if (ZNet.instance.IsServer())
                    {
                        ReloadConfigs();
                    }
                    else
                    {
                        RequestSync();
                    }
                }
                catch (Exception e)
                {
                    Log.Error?.Log($"Error while attempting to execute {CommandName}.", e);
                }
            },
            onlyAdmin: true);
    }

    private static void ConfigureRPCs(ZNetPeer peer)
    {
        if (ZNet.instance.IsServer())
        {
            peer.m_rpc.Register(
                nameof(RPC_DropThatCommand_ReloadConfigs),
                RPC_DropThatCommand_ReloadConfigs);
        }
    }

    private static void RPC_DropThatCommand_ReloadConfigs(ZRpc zrpc) => ReloadConfigs();

    private static void ReloadConfigs()
    {
        DropSystemConfigManager.LoadConfigs();

        if (LifecycleManager.GameState != GameState.Singleplayer)
        {
            SyncManager.SyncConfigs();
        }
    }

    private static void RequestSync()
    {
        ZNet.instance.GetServerRPC().Invoke(nameof(RPC_DropThatCommand_ReloadConfigs));
    }
}

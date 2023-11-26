using System;
using DropThat.Drop.DropTableSystem.Debug;
using ThatCore.Logging;

namespace DropThat.Commands;

internal static class WriteLoadedDropTablesCommand
{
    public const string CommandName = "dropthat:print_droptables_loaded";

    internal static void Register()
    {
        new Terminal.ConsoleCommand(
            CommandName,
            "Reload configurations and re-syncronize.",
            (args) =>
            {
                try
                {
                    if (ZNet.instance.IsServer())
                    {
                        TemplateWriter.PrintLoadedDropTables();
                    }
                    else
                    {
                        RequestPrint();
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
                nameof(RPC_DropThatCommand_PrintDropTablesLoaded),
                RPC_DropThatCommand_PrintDropTablesLoaded);
        }
    }

    private static void RPC_DropThatCommand_PrintDropTablesLoaded(ZRpc zrpc) => TemplateWriter.PrintLoadedDropTables();

    private static void RequestPrint()
    {
        ZNet.instance.GetServerRPC().Invoke(nameof(RPC_DropThatCommand_PrintDropTablesLoaded));
    }
}

using System;
using System.Collections.Generic;
using ThatCore.Logging;
using ThatCore.Network;

namespace DropThat.Core;

internal static class SyncManager
{
    private class HandlerPair
    {
        public string UnpackerRpcName { get; }
        public Func<IMessage> Packer { get; }
        public Action<ZRpc, ZPackage> Unpacker { get; }

        public HandlerPair(string unpackerRpcName, Func<IMessage> packer, Action<ZRpc, ZPackage> unpacker)
        {
            UnpackerRpcName = unpackerRpcName;
            Packer = packer;
            Unpacker = unpacker;
        }
    }

    private static Dictionary<string, HandlerPair> PackageHandlers { get; } = new();

    public static void RegisterSyncHandlers(string unpackerRpc, Func<IMessage> packer, Action<ZRpc, ZPackage> unpacker)
    {
        PackageHandlers[unpackerRpc] = new(unpackerRpc, packer, unpacker);
    }

    internal static void SetupConfigSyncForPeer(ZNetPeer peer)
    {
        try
        {
            if (ZNet.instance.IsServer())
            {
                Log.Debug?.Log("Registering server RPC for sending configs on request from client.");
                peer.m_rpc.Register(nameof(RPC_RequestConfigsDropThat), RPC_RequestConfigsDropThat);
            }
            else
            {
                Log.Trace?.Log("Registering client RPCs for receiving config packages from server.");
                foreach (var handler in PackageHandlers.Values)
                {
                    peer.m_rpc.Register(handler.UnpackerRpcName, handler.Unpacker);
                }

                Log.Debug?.Log("Requesting configs from server.");
                peer.m_rpc.Invoke(nameof(RPC_RequestConfigsDropThat));
            }
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error during setup of config sync", e);
        }
    }

    private static void RPC_RequestConfigsDropThat(ZRpc rpc)
    {
        try
        {
            if (!ZNet.instance.IsServer())
            {
                Log.Warning?.Log("Non-server instance received request for configs. Ignoring request.");
            }

            Log.Debug?.Log("Received request for configs.");

            foreach (var handler in PackageHandlers.Values)
            {
                OutgoingMessageService.AddToQueue(handler.Packer(), handler.UnpackerRpcName, rpc);
            }

            Log.Trace?.Log("Sending config packages.");
        }
        catch (Exception e)
        {
            Log.Error?.Log("Unexpected error while attempting to create and send config packages from server to client.", e);
        }
    }

    public static void SyncConfigs()
    {
        try
        {
            if (!ZNet.instance.IsServer())
            {
                Log.Warning?.Log("Non-server instance cannot sync configs. Ignoring request.");
            }

            foreach (var peer in ZNet.instance.GetPeers())
            {
                // Skip server.
                if (peer.m_server)
                {
                    continue;
                }
                
                foreach (var handler in PackageHandlers.Values)
                {
                    OutgoingMessageService.AddToQueue(handler.Packer(), handler.UnpackerRpcName, peer.m_rpc);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error?.Log("Unexpected error while attempting to create and send config packages from server to client.", e);
        }
    }
}

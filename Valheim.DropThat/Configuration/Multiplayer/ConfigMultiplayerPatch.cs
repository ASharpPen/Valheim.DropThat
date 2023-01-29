using HarmonyLib;
using System;
using DropThat.Core.Network;
using ThatCore.Logging;

namespace DropThat.Configuration.Multiplayer;

[HarmonyPatch(typeof(ZNet))]
	public class ConfigMultiplayerPatch
	{
		[HarmonyPatch("OnNewConnection")]
		[HarmonyPostfix]
		private static void SyncConfigs(ZNet __instance, ZNetPeer peer)
		{
			if (ZNet.instance.IsServer())
			{
				Log.Debug?.Log("Registering server RPC for sending configs on request from client.");
				peer.m_rpc.Register(nameof(RPC_RequestConfigsDropThat), new ZRpc.RpcMethod.Method(RPC_RequestConfigsDropThat));
			}
			else
			{
				Log.Debug?.Log("Registering client RPC for receiving configs from server.");
				peer.m_rpc.Register<ZPackage>(nameof(RPC_ReceiveConfigsDropThat), new Action<ZRpc, ZPackage>(RPC_ReceiveConfigsDropThat));

				Log.Debug?.Log("Requesting configs from server.");
				peer.m_rpc.Invoke(nameof(RPC_RequestConfigsDropThat));
			}
		}

		private static void RPC_RequestConfigsDropThat(ZRpc rpc)
		{
			try
			{
				if (!ZNet.instance.IsServer())
				{
					Log.Warning?.Log("Non-server instance received request for configs. Ignoring request.");
                return;
				}

				Log.Info?.Log("Received request for configs.");

				DataTransferService.Service.AddToQueue(SplitPackage.Pack(new GeneralConfigPackage()), nameof(RPC_ReceiveConfigsDropThat), rpc);
				DataTransferService.Service.AddToQueue(SplitPackage.Pack(new DropTablePackage()), nameof(RPC_ReceiveConfigsDropThat), rpc);

				Log.Trace?.Log("Sending config packages.");
			}
			catch (Exception e)
        {
				Log.Error?.Log("Unexpected error while attempting to create and send config package from server to client.", e);
        }
		}

		private static void RPC_ReceiveConfigsDropThat(ZRpc rpc, ZPackage pkg)
		{
			Log.Trace?.Log("Received config package.");
			try
			{
            var splitPackage = SplitPackage.Unpack(pkg);

            if (splitPackage is not null)
            {
                Log.Trace?.Log($"Package split received '{splitPackage.TransferId}:{splitPackage.SplitIndex}:{splitPackage.SplitCount}'");
                SplitPackageReceiverService.ReceivePackage(splitPackage);
            }
            else
            {
                Log.Warning?.Log($"Unable to read received config package with size '{pkg.Size()}'");
            }
        }
			catch(Exception e)
        {
				Log.Error?.Log("Error while attempting to read received config package.", e);
        }
		}
	}

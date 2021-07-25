using HarmonyLib;
using System;
using Valheim.DropThat.Core;
using Valheim.DropThat.Core.Network;

namespace Valheim.DropThat.Configuration.Multiplayer
{
    [HarmonyPatch(typeof(ZNet))]
	public class ConfigMultiplayerPatch
	{
		[HarmonyPatch("OnNewConnection")]
		[HarmonyPostfix]
		private static void SyncConfigs(ZNet __instance, ZNetPeer peer)
		{
			if (ZNet.instance.IsServer())
			{
				Log.LogDebug("Registering server RPC for sending configs on request from client.");
				peer.m_rpc.Register(nameof(RPC_RequestConfigsDropThat), new ZRpc.RpcMethod.Method(RPC_RequestConfigsDropThat));
			}
			else
			{
				Log.LogDebug("Registering client RPC for receiving configs from server.");
				peer.m_rpc.Register<ZPackage>(nameof(RPC_ReceiveConfigsDropThat), new Action<ZRpc, ZPackage>(RPC_ReceiveConfigsDropThat));

				Log.LogDebug("Requesting configs from server.");
				peer.m_rpc.Invoke(nameof(RPC_RequestConfigsDropThat));
			}
		}

		private static void RPC_RequestConfigsDropThat(ZRpc rpc)
		{
			try
			{
				if (!ZNet.instance.IsServer())
				{
					Log.LogWarning("Non-server instance received request for configs. Ignoring request.");
				}

				Log.LogInfo("Received request for configs.");

				DataTransferService.Service.AddToQueue(new GeneralConfigPackage().Pack(), nameof(RPC_ReceiveConfigsDropThat), rpc);
				DataTransferService.Service.AddToQueue(new CharacterDropConfigPackage().Pack(), nameof(RPC_ReceiveConfigsDropThat), rpc);
				DataTransferService.Service.AddToQueue(new DropTablePackage().Pack(), nameof(RPC_ReceiveConfigsDropThat), rpc);

				Log.LogTrace("Sending config packages.");
			}
			catch (Exception e)
            {
				Log.LogError("Unexpected error while attempting to create and send config package from server to client.", e);
            }
		}

		private static void RPC_ReceiveConfigsDropThat(ZRpc rpc, ZPackage pkg)
		{
			Log.LogInfo("Received package.");
			try
			{
				CompressedPackage.Unpack(pkg);
			}
			catch(Exception e)
            {
				Log.LogError("Error while attempting to read received config package.", e);
            }
		}
	}
}

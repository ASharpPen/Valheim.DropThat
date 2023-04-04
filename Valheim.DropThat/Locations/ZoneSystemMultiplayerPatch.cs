using HarmonyLib;
using System;
using ThatCore.Logging;
using ThatCore.Lifecycle;

namespace DropThat.Locations;

[HarmonyPatch(typeof(ZNet))]
public static class ZoneSystemMultiplayerPatch
{
	private static bool HaveReceivedLocations = false;

	static ZoneSystemMultiplayerPatch()
	{
        LifecycleManager.OnWorldInit += () =>
        {
            HaveReceivedLocations = false;
        };
	}

	[HarmonyPatch("OnNewConnection")]
	[HarmonyPostfix]
	private static void TransferLocationData(ZNet __instance, ZNetPeer peer)
	{
		if (ZNet.instance.IsServer())
		{
			Log.Debug?.Log("Registering server RPC for sending location data on request from client.");
			peer.m_rpc.Register(nameof(RPC_RequestLocationsDropThat), new ZRpc.RpcMethod.Method(RPC_RequestLocationsDropThat));
		}
		else
		{
			Log.Debug?.Log("Registering client RPC for receiving location data from server.");
			peer.m_rpc.Register<ZPackage>(nameof(RPC_ReceiveLocationsDropThat), new Action<ZRpc, ZPackage>(RPC_ReceiveLocationsDropThat));

			Log.Debug?.Log("Requesting location data from server.");
			peer.m_rpc.Invoke(nameof(RPC_RequestLocationsDropThat));
		}
	}

	private static void RPC_RequestLocationsDropThat(ZRpc rpc)
	{
		try
		{
			if (!ZNet.instance.IsServer())
			{
				Log.Warning?.Log("Non-server instance received request for location data. Ignoring request.");
				return;
			}

			Log.Debug?.Log($"Sending location data.");

			if (ZoneSystem.instance.m_locationInstances is null)
			{
				Log.Warning?.Log("Unable to get locations from zonesystem to send to client.");
				return;
			}

			var pck = new SimpleLocationPackage();

			ZPackage package = pck.Pack();

			DataTransferService.Service.AddToQueue(package, nameof(RPC_ReceiveLocationsDropThat), rpc);

			Log.Debug?.Log("Finished sending locations package.");
		}
		catch (Exception e)
		{
			Log.Error?.Log("Unexpected error while attempting to create and send locations package from server to client.", e);
		}
	}

	private static void RPC_ReceiveLocationsDropThat(ZRpc rpc, ZPackage pkg)
	{
		Log.Debug?.Log("Received locations package.");
		try
		{
			if (HaveReceivedLocations)
			{
				Log.Debug?.Log("Already received locations previously. Skipping.");
				return;
			}

			CompressedPackage.Unpack(pkg);
			HaveReceivedLocations = true;

			Log.Debug?.Log("Successfully received and unpacked locations package.");
		}
		catch (Exception e)
		{
			Log.Error?.Log("Error while attempting to read received locations package.", e);
		}
	}
}

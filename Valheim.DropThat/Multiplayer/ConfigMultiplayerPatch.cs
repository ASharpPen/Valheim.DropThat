using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Valheim.DropThat.ConfigurationCore;
using Valheim.DropThat.ConfigurationTypes;

namespace Valheim.DropThat.Multiplayer
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
				ConfigurationManager.LoadAllConfigurations();

				Log.LogDebug("Registering server RPC for sending configs on request from client.");
				peer.m_rpc.Register(nameof(RPC_RquestConfigsDropThat), new ZRpc.RpcMethod.Method(RPC_RquestConfigsDropThat));
			}
			else
			{
				Log.LogDebug("Registering client RPC for receiving configs from server.");
				peer.m_rpc.Register<ZPackage>(nameof(RPC_ReceiveConfigsDropThat), new Action<ZRpc, ZPackage>(RPC_ReceiveConfigsDropThat));

				Log.LogDebug("Requesting configs from server.");
				peer.m_rpc.Invoke(nameof(RPC_RquestConfigsDropThat));
			}
		}

		private static void RPC_RquestConfigsDropThat(ZRpc rpc)
		{
			try
			{
				if (!ZNet.instance.IsServer())
				{
					Log.LogWarning("Non-server instance received request for configs. Ignoring request.");
				}

				Log.LogInfo("Received request for configs.");

				ZPackage configPackage = new ZPackage();

				var package = new ConfigurationPackage(ConfigurationManager.GeneralConfig, ConfigurationManager.DropConfigs);

				Log.LogTrace("Serializing configs.");

				using (MemoryStream memStream = new MemoryStream())
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(memStream, package);

					byte[] serialized = memStream.ToArray();

					configPackage.Write(serialized);
				}

				Log.LogTrace("Sending config package.");

				rpc.Invoke(nameof(RPC_ReceiveConfigsDropThat), new object[] { configPackage });

				Log.LogTrace("Finished sending config package.");
			}
			catch (Exception e)
            {
				Log.LogError("Unexpected error while attempting to create and send config package from server to client.", e);
            }
		}

		private static void RPC_ReceiveConfigsDropThat(ZRpc rpc, ZPackage pkg)
		{
			Log.LogTrace("Received package.");
			try
			{
				var serialized = pkg.ReadByteArray();

				Log.LogTrace("Deserializing package.");

				using (MemoryStream memStream = new MemoryStream(serialized))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					var responseObject = binaryFormatter.Deserialize(memStream);

					if (responseObject is ConfigurationPackage configPackage)
					{
						Log.LogDebug("Received and deserialized config package");

						Log.LogTrace("Unpackaging general config.");

						ConfigurationManager.GeneralConfig = (GeneralConfiguration)configPackage.GeneralConfig.Configuration;

						Log.LogTrace("Successfully set general config.");
						Log.LogTrace("Unpackaging drop table configs.");

						ConfigurationManager.DropConfigs = (List<DropTableConfiguration>)configPackage.DropTableConfigs.Configuration;

						Log.LogTrace("Successfully set drop table configs.");
					}
					else
					{
						Log.LogWarning("Received bad config package. Unable to load.");
					}
				}
			}
			catch(Exception e)
            {
				Log.LogError("Error while attempting to read received config package.", e);
            }
		}
	}
}

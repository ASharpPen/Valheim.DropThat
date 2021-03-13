using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Valheim.DropThat.ConfigurationCore.Multiplayer
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
				//TODO: Maybe initiate config loading here?
				peer.m_rpc.Register(nameof(RPC_RquestConfigsDropThat), new ZRpc.RpcMethod.Method(RPC_RquestConfigsDropThat));
			}
			else
			{
				Log.LogDebug("Registering client RPC for receiving configs from server.");
				peer.m_rpc.Register<ZPackage>(nameof(RPC_ReceiveConfigsDropThat), new Action<ZRpc, ZPackage>(RPC_ReceiveConfigsDropThat));
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

				var test = new TestPackage("Send this please");

				using (MemoryStream memStream = new MemoryStream())
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					binaryFormatter.Serialize(memStream, test);

					byte[] serialized = memStream.ToArray();

					configPackage.Write(serialized);
				}

				rpc.Invoke(nameof(RPC_ReceiveConfigsDropThat), new object[] { configPackage });
			}
			catch(Exception e)
            {
				Log.LogError("Unexpected error while attempting to create and send config package from server to client.", e);
            }
		}

		private static void RPC_ReceiveConfigsDropThat(ZRpc rpc, ZPackage pkg)
		{
			try
			{
				var serialized = pkg.ReadByteArray();

				using (MemoryStream memStream = new MemoryStream(serialized))
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					var responseObject = binaryFormatter.Deserialize(memStream);

					if (responseObject is TestPackage configPackage)
					{
						Log.LogInfo("Received config package: " + configPackage.TestValue);
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

		[Serializable]
		public class TestPackage
		{
			public string TestValue = "Hello World";

			public TestPackage()
            {
            }

			public TestPackage(string anotherValue)
            {
				TestValue = anotherValue;
            }
		}
	}
}

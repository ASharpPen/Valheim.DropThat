#if FALSE && DEBUG

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Valheim.DropThat.Core.Transfer
{
    [HarmonyPatch(typeof(ZNet))]
    public static class DataTransferServiceTest
    {
        [HarmonyPatch("OnNewConnection")]
        [HarmonyPostfix]
        public static void SendLotsOfStuff(ZNet __instance, ZNetPeer peer)
        {
			if(ZNet.instance.IsServer())
            {
				Log.LogDebug("Sending data from server.");
				DataTransferServiceTest.SendDataTest(peer.m_rpc);
			}
			else
			{
				Log.LogDebug("Registering RPC for test data from server.");
				peer.m_rpc.Register<ZPackage>(nameof(RPC_DropThatReceiveDataTest), new Action<ZRpc, ZPackage>(RPC_DropThatReceiveDataTest));
			}
		}

		public static void SendDataTest(ZRpc rpc)
        {
			for (int batch = 0; batch < 100; ++batch)
			{
				ZPackage package = new ZPackage();

				for (int i = 0; i < 500000; ++i)
				{
					package.Write(new byte());
				}

				Log.LogDebug($"Queueing test-package {batch}");

				DataTransferService.Service.AddToQueue(package, nameof(RPC_DropThatReceiveDataTest), rpc);
			}
        }

		public static int ReceivedCount = 0;
		public static long ReceivedBytes = 0;

		private static void RPC_DropThatReceiveDataTest(ZRpc rpc, ZPackage pkg)
        {
			int packageSize = pkg.Size();

			ReceivedCount++;
			ReceivedBytes += packageSize;

			Log.LogInfo($"Received package {ReceivedCount} with size {packageSize}. Total received: {ReceivedBytes}");
        }
	}
}

#endif
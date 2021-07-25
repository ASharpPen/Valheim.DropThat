using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Valheim.DropThat.Core.Network
{
    /// <summary>
    /// Dispatcher for DataTransferService.
    /// Will wait for free space for each individual player's connection.
    /// If a players connection has enough space after valheim has done its business,
    /// the dispatcher will send a single item from the DataTransferService queue.
    /// 
    /// A maximum of one package will be sent to socket pr player pr frame.
    /// </summary>
    internal class Dispatcher
    {
        /// <summary>
        /// Must be lower than ca. 8000 for things like ZDO's to have a chance at being synced.
        /// </summary>
        private static int MaxQueueSizeForDispatch = 5000;

        private static Dictionary<string, Queue<QueueItem>> SocketQueues => DataTransferService.Service.SocketQueues;

        private static Dispatcher _instance;

        public static Dispatcher Instance => _instance ??= new();

        [HarmonyPatch(typeof(ZNet))]
        internal static class Patch_ZSteamSocket_Dispatcher
        {

            [HarmonyPatch(nameof(ZNet.Update))]
            [HarmonyPostfix]
            private static void Dispatch()
            {
                int sockets = ZSteamSocket.m_sockets.Count;
                Task[] socketTasks = new Task[sockets];

                for (int i = 0; i < sockets; ++i)
                {
                    socketTasks[i] = Instance.DispatchSocket(ZSteamSocket.m_sockets[i]);
                }

                Task.WaitAll(socketTasks);
            }
        }

        private static Task Completed = Task.CompletedTask;

        private Task DispatchSocket(ZSteamSocket __instance)
        {
            var socketQueueIdentifier = __instance.GetEndPointString();

            Queue<QueueItem> queue;
            lock (SocketQueues)
            {
                if (!SocketQueues.TryGetValue(socketQueueIdentifier, out queue))
                {
                    // No queue created yet. Nothing to dispatch.
                    return Completed;
                }
            }

            if (queue.Count == 0)
            {
                return Completed;
            }

            // Check if socket is ready for new packages.
            int queueSize = __instance.GetSendQueueSize();
            if (__instance.GetSendQueueSize() > MaxQueueSizeForDispatch)
            {
#if DEBUG
                Log.LogTrace("Package queue size: " + queueSize);
#endif
                return Completed;
            }

            // Send package
            QueueItem item;
            lock (queue)
            {
                item = queue.Dequeue();
            }

            try
            {
                item.ZRpc.Invoke(item.Target, new object[] { item.Package });
            }
            catch (Exception e)
            {
                if (item.Retries > 3)
                {
                    Log.LogWarning($"Error while trying to send package. Too many retries, will stop trying.", e);
                }
                else
                {
                    lock (queue)
                    {
                        // Requeue package
                        item.Retries++;
                        queue.Enqueue(item);
                    }
                }
            }

            return Completed;
        }

        [HarmonyPatch(typeof(ZSteamSocket))]
        internal static class Cleanup
        {
            private static Dictionary<string, Queue<QueueItem>> SocketQueues => DataTransferService.Service.SocketQueues;

            [HarmonyPatch(nameof(ZSteamSocket.Dispose))]
            [HarmonyPrefix]
            private static void Dispose(ZSteamSocket __instance)
            {
                var socketQueueIdentifier = __instance.GetEndPointString();

                lock (SocketQueues)
                {
                    SocketQueues.Remove(socketQueueIdentifier);
                }
            }
        }
    }
}
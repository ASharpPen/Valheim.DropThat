using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Valheim.DropThat.Reset;

namespace Valheim.DropThat.Core.Transfer
{
    internal class DataTransferService : IDisposable
    {
        private static DataTransferService _service;

        public static DataTransferService Service => _service ??= new();

        static DataTransferService()
        {
            StateResetter.Subscribe(() =>
            {
                _service = null;
            });
        }

        private Queue<QueueItem> Queue { get; } = new Queue<QueueItem>();

        private CancellationTokenSource TokenSource { get; } = new();
        private CancellationToken CancellationToken => TokenSource.Token;

        private Task DispatcherTask { get; set; }

        public void AddToQueue(ZPackage package, string rpcRoute, long playerId)
        {
            var peers = ZNet.instance.GetConnectedPeers();
            var zrpc = peers.FirstOrDefault(x => x.m_uid == playerId)?.m_rpc;

            AddToQueue(package, rpcRoute, zrpc);
        }

        public void AddToQueue(ZPackage package, string rpcRoute, ZRpc zrpc)
        {
            try
            {
                var item = new QueueItem
                {
                    Package = package,
                    Target = rpcRoute,
                    ZRpc = zrpc,
                };

                lock (Queue)
                {
                    Queue.Enqueue(item);
                }
            }
            catch (Exception e)
            {
                Log.LogError("Failed to queue package.", e);
            }

            try
            {
                if (DispatcherTask is null || DispatcherTask.IsCompleted)
                {
                    DispatcherTask = Dispatch();
                }
            }
            catch (Exception e)
            {
                Log.LogError("Failed to start package dispatcher.", e);
            }
        }

        private async Task Dispatch()
        {
#if DEBUG
            Log.LogInfo("Started dispatcher.");
#endif

            try
            {
#if DEBUG
                Log.LogInfo("Dispatcher Queue: " + Queue.Count);
#endif

                while (!CancellationToken.IsCancellationRequested && Queue.Count > 0)
                {
                    var item = Queue.Peek();

                    if (!item.ZRpc.IsConnected())
                    {
                        lock (Queue)
                        {
                            Queue.Dequeue();
                        }

                        continue;
                    }

                    // Delay sending if queue is already filled.
                    var queue = item.ZRpc.GetSocket().GetSendQueueSize();
                    if (queue > 10000)
                    {
#if DEBUG
                        Log.LogInfo("Socket Queue size: " + queue);
#endif
                        await Task.Delay(10);
                        continue;
                    }

                    try
                    {
                        item.ZRpc.Invoke(item.Target, new object[] { item.Package });

                        Log.LogTrace($"Sent {item.Package.Size()} byte package to {item.Target}");
                    }
                    catch (Exception e)
                    {
                        if (item.Retries < 3)
                        {
                            Log.LogWarning($"Error while trying to send package. Retry {item.Retries}.");
                            item.Retries++;
                            continue;
                        }
                        else
                        {
                            Log.LogWarning($"Error while trying to send package. Too many retries, will stop trying.", e);
                        }
                    }

                    lock (Queue)
                    {
                        Queue.Dequeue();
                    }
                }
            }
            catch (Exception e)
            {
                Log.LogWarning($"Error while trying to send package.", e);
            }
        }

        public void Dispose()
        {
            TokenSource.Cancel();

            if (DispatcherTask is not null && !DispatcherTask.IsCompleted)
            {
                try
                {
                    DispatcherTask.Wait();
                }
                catch(Exception e)
                {
                    Log.LogWarning($"Error while trying to shut down data dispatcher gracefully.", e);
                }
            }
        }

        private struct QueueItem
        {
            public ZPackage Package { get; set; }

            public ZRpc ZRpc { get; set; }

            public string Target { get; set; }

            public int Retries { get; set; }
        }
    }
}

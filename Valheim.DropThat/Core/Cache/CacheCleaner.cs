using System;
using System.Collections.Generic;
using System.Threading;

namespace Valheim.DropThat.Core
{
    internal static class CacheCleaner
    {
        private static Thread WorkerThread = null;

        private static List<IManagedCache> Watched { get; } = new();

        public static void Subscribe(IManagedCache dic)
        {
            lock (Watched)
            {
                Watched.Add(dic);

                if (WorkerThread == null)
                {
                    WorkerThread = new Thread(CleanupTask);
                    WorkerThread.IsBackground = true;

                    Log.LogTrace("Starting cache cleaner thread");

                    WorkerThread.Start();
                }
            }
        }

        public static void Unsubscribe(IManagedCache dic)
        {
            Watched.Remove(dic);
        }

        private static void CleanupTask()
        {
            while (true)
            {
                try
                {
                    int cleanPrCycle = (int)Math.Max(1, (Watched?.Count ?? 0) / 10.0);

                    for (int i = 0; i < (Watched?.Count ?? 0); ++i)
                    {
                        IManagedCache watched;

                        lock (Watched)
                        {
                            if (Watched.Count <= i)
                            {
                                continue;
                            }

                            watched = Watched[i];
                        }

                        watched.Clean();

                        if (i % cleanPrCycle == 0)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Log.LogError($"Error during {typeof(ManagedCache<>).Name} cleanup.", e);
#endif
                }

                Thread.Sleep(1000);
            }
        }
    }
}

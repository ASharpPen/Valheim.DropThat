using System;
using System.Collections.Generic;
using System.Linq;
using DropThat.Reset;
using ThatCore.Logging;

namespace DropThat.Core.Network;

internal static class SplitPackageReceiverService
{
    private static Dictionary<Guid, PackageSession> PackageCache { get; } = new();
    private static Dictionary<Guid, DateTimeOffset> RecentlyDroppedTransfers { get; } = new();

    private static TimeSpan RetentionTime { get; } = TimeSpan.FromMinutes(1);
    private static TimeSpan DroppedRetentionTime { get; } = TimeSpan.FromMinutes(2);

    static SplitPackageReceiverService()
    {
        StateResetter.Subscribe(() =>
        {
            PackageCache.Clear();
            RecentlyDroppedTransfers.Clear();
        });
    }

    public static void Update()
    {
        DateTimeOffset droppedRetentionTimestamp = DateTimeOffset.UtcNow - DroppedRetentionTime;

        foreach (var droppedTransfer in RecentlyDroppedTransfers.ToArray())
        {
            if (droppedTransfer.Value < droppedRetentionTimestamp)
            {
                RecentlyDroppedTransfers.Remove(droppedTransfer.Key);
            }
        }

        DateTimeOffset retentionTimestamp = DateTimeOffset.UtcNow - RetentionTime;

        foreach (var session in PackageCache.Values.ToArray())
        {
            if (session.LastPackage < retentionTimestamp)
            {
                Log.Trace?.Log($"Removing dead package session with transfer id '{session.TransferId}' and {session.Packages.Count}/{session.ExpectedPackages} received packages");

                PackageCache.Remove(session.TransferId);
                RecentlyDroppedTransfers[session.TransferId] = DateTimeOffset.UtcNow;
            }
        }
    }

    public static void ReceivePackage(SplitPackage splitPackage)
    {
        if (RecentlyDroppedTransfers.ContainsKey(splitPackage.TransferId))
        {
            RecentlyDroppedTransfers[splitPackage.TransferId] = DateTimeOffset.UtcNow;
            return;
        }

        PackageSession session;

        if (PackageCache.TryGetValue(splitPackage.TransferId, out session))
        {
            session.AddPackage(splitPackage);
        }
        else
        {
            PackageCache[splitPackage.TransferId] = session = new(splitPackage);
        }

        if (session.MissingSplits.Count == 0)
        {
            SplitPackage.CombineAndUnpackSplits(session.Packages);
            PackageCache.Remove(session.TransferId);
        }
    }

    private class PackageSession
    {
        public PackageSession(SplitPackage initialPackage)
        {
            TransferId = initialPackage.TransferId;
            ExpectedPackages = initialPackage.SplitCount;
            MissingSplits = Enumerable
                .Range(0, ExpectedPackages)
                .ToHashSet();

            Packages = new()
            {
                { initialPackage.SplitIndex, initialPackage }
            };
            MissingSplits.Remove(initialPackage.SplitIndex);
        }

        public Guid TransferId { get; }

        public int ExpectedPackages { get; }

        public DateTimeOffset SessionStarted { get; } = DateTimeOffset.UtcNow;

        public DateTimeOffset LastPackage { get; set; } = DateTimeOffset.UtcNow;

        public Dictionary<int, SplitPackage> Packages { get; }

        public HashSet<int> MissingSplits { get; }

        public void AddPackage(SplitPackage package)
        {
            if (!ValidPackage(package))
            {
                Log.Debug?.Log($"Received unexpected misformed package '{package.TransferId}:{package.SplitIndex}' for session '{package.TransferId}'. Dropping package.");
                return;
            }

            Packages[package.SplitIndex] = package;
            MissingSplits.Remove(package.SplitIndex);

            LastPackage = DateTimeOffset.UtcNow;
        }

        private bool ValidPackage(SplitPackage package) =>
            TransferId == package.TransferId &&
            ExpectedPackages == package.SplitCount
            ;
    }
}

using System.Collections.Generic;
using Valheim.DropThat.Core.Cache;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Creature.DamageRecords;

/// <summary>
/// Local cache of recent hit data pr creature.
/// </summary>
internal static class RecordRecentHits
{
    private static ManagedCache<List<DamageRecord>> RecentHits = new();

    public static List<DamageRecord> GetRecentHits(Character character)
    {
        if (!RecentHits.TryGet(character, out List<DamageRecord> recentHits))
        {
            return new(0);
        }

        // Clean up old hits
        CleanUpOldHits(recentHits);

        return recentHits;
    }

    public static void SetRecentHit(Character character, HitData hitData)
    {
        if (hitData is null)
        {
            return;
        }

        var recentHits = RecentHits.GetOrCreate(character);

        // Clean up old hits
        CleanUpOldHits(recentHits);

        recentHits.Add(new DamageRecord
        {
            Hit = hitData,
            Timestamp = GetTimestamp()
        });
    }

    private static void CleanUpOldHits(List<DamageRecord> hits, double retentionTime = 60)
    {
        if (hits.Count > 0)
        {
            var currentTimestamp = GetTimestamp();

            for (int i = 0; i < hits.Count; ++i)
            {
                // Check for hits older than one minute.
                if (currentTimestamp - hits[i].Timestamp > retentionTime)
                {
                    hits.RemoveAt(i);
                    --i;
                }
            }
        }
    }

    private static double GetTimestamp()
    {
        return (ZNet.instance.IsNotNull() ? ZNet.instance.GetTimeSeconds() : 0);
    }
}

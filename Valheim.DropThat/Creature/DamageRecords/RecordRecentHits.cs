using System.Collections.Generic;
using DropThat.Caches;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Utilities;
using ThatCore.Cache;
using UnityEngine;

namespace DropThat.Creature.DamageRecords;

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
        if (hitData is null ||
            character.IsNull())
        {
            return;
        }

        var recentHits = RecentHits.GetOrCreate(character);

        // Clean up old hits
        CleanUpOldHits(recentHits);



        recentHits.Add(new DamageRecord
        {
            Hit = hitData,
            Timestamp = GetTimestamp(),
            AttackerType = GetAttackerEntityType(hitData)
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

    private static EntityType GetAttackerEntityType(HitData hit)
    {
        if (!hit.HaveAttacker())
        {
            return EntityType.Other;
        }

        GameObject attacker = ZNetScene.instance.FindInstance(hit.m_attacker);

        if (attacker.IsNull())
        {
            return EntityType.Other;
        }

        var attackerCharacter = ComponentCache.Get<Character>(attacker);

        if (attackerCharacter.IsNull())
        {
            return EntityType.Other;
        }

        return attackerCharacter.IsPlayer()
            ? EntityType.Player
            : EntityType.Creature;
    }
}

using DropThat.Caches;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Utilities;
using ThatCore.Cache;
using UnityEngine;

namespace DropThat.Creature.DamageRecords;

/// <summary>
/// Local cache of last hit data.
/// </summary>
public static class RecordLastHit
{
    private static ManagedCache<DamageRecord> LastHits = new();

    public static DamageRecord GetLastHit(Character character)
    {
        if (character.IsNull())
        {
            return null;
        }

        if(LastHits.TryGet(character, out DamageRecord lastHit))
        {
            return lastHit;
        }

        return null;
    }

    public static void SetLastHit(Character character, HitData hitData)
    {
        if (hitData is null ||
            character.IsNull())
        {
            return;
        }

        var lastHit = LastHits.GetOrCreate(character);
        lastHit.Hit = hitData;
        
        if (ZNet.instance.IsNotNull())
        {
            lastHit.Timestamp = ZNet.instance.GetTimeSeconds();
        }

        lastHit.AttackerType = GetAttackerEntityType(hitData);
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

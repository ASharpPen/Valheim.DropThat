// #define VERBOSE

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using DropThat.Caches;
using DropThat.Core;
using DropThat.Creature.DamageRecords;
using DropThat.Drop.CharacterDropSystem.Caches;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Utilities;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionHitByEntityTypeRecently : ICondition
{
    private static ConditionHitByEntityTypeRecently _instance;

    public static ConditionHitByEntityTypeRecently Instance => _instance ??= new();

    public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop)
    {
        if (drop is null || 
            characterDrop.IsNull() ||
            extended?.Config is null)
        {
            return false;
        }

        var character = CharacterCache.GetCharacter(characterDrop);

        // Ignore if no character is associated with drop.
        if (character.IsNull())
        {
            return false;
        }

#if DEBUG && VERBOSE
        Log.LogTrace($"[{character.name}] Checking ConditionHitByEntityTypeRecently=" + extended.Config.ConditionHitByEntityTypeRecently.Value);
#endif

        var configTypes = extended.Config.ConditionHitByEntityTypeRecently.Value.SplitByComma();

        List<EntityType> entities;

        if (configTypes.Count == 0)
        {
            // Ignore if there are no entity types for condition to check.
            return false;
        }
        else if (!configTypes.TryConvertToEnum(out entities))
        {
#if DEBUG
            Log.LogWarning($"[{character.name}] Failed to convert EntityType to enum: " + configTypes.Join());
#endif
            return false;
        }

        var recentHits = RecordRecentHits.GetRecentHits(character);

#if DEBUG && VERBOSE
        Log.LogTrace($"[{character.name}] Checking recent hits: " + recentHits.Select(x => GetHitterType(x)).Join());
        Log.LogTrace($"[{character.name}] Searching for hits by: " + entities.Join());
#endif

        var match = recentHits.Any(x => entities.Contains(GetHitterType(x)));

        if (!match)
        {
            Log.LogTrace($"Filtered drop '{drop.m_prefab.name}' due not being hit recently by required entity type.");
        }

        return !match;
    }

    private EntityType GetHitterType(DamageRecord record)
    {
        EntityType hitBy = EntityType.Other;

        if (record.Hit.HaveAttacker())
        {
            GameObject attacker = ZNetScene.instance.FindInstance(record.Hit.m_attacker);

            var attackerCharacter = ComponentCache.Get<Character>(attacker);

            if (attackerCharacter is not null)
            {
                if (attackerCharacter.IsPlayer())
                {
                    hitBy = EntityType.Player;
                }
                else
                {
                    hitBy = EntityType.Creature;
                }
            }
        }

        return hitBy;
    }
}

// #define VERBOSE

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Core;
using Valheim.DropThat.Creature.DamageRecords;
using Valheim.DropThat.Drop.CharacterDropSystem.Caches;
using Valheim.DropThat.Drop.CharacterDropSystem.Models;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Drop.CharacterDropSystem.Conditions;

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

        var match = recentHits.Any(x => entities.Contains(x.AttackerType));

        if (!match)
        {
            Log.LogTrace($"Filtered drop '{drop.m_prefab.name}' due not being hit recently by required entity type.");
        }

        return !match;
    }
}

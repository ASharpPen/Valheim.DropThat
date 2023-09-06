﻿using System.Collections.Generic;
using System.Linq;
using DropThat.Creature.DamageRecords;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionHitByEntityTypeRecently : IDropCondition
{
    public HashSet<EntityType> EntityTypes { get; set; }

    public bool IsPointless() => (EntityTypes?.Count ?? 0) == 0;

    public bool IsValid(DropContext context)
    {
        if (EntityTypes is null ||
            EntityTypes.Count == 0)
        {
            return true;
        }

        var character = context.Character;

        if (character.IsNull())
        {
            return true;
        }

        var recentHits = RecordRecentHits.GetRecentHits(character);

        return recentHits.Any(x => EntityTypes.Contains(x.GetHitterType()));
    }
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionHitByEntityTypeRecently(
        this IHaveDropConditions template,
        IEnumerable<EntityType> entityTypes)
    {
        if (entityTypes?.Any() == true)
        {
            template.Conditions.GetOrCreate<ConditionHitByEntityTypeRecently>().EntityTypes = entityTypes.ToHashSet();
        }
        else
        {
            template.Conditions.Remove<ConditionHitByEntityTypeRecently>();
        }

        return template;
    }
}
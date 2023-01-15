using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DropThat.Caches;
using DropThat.Creature.DamageRecords;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionHitByEntityTypeRecently : IDropCondition
{
    public HashSet<EntityType> EntityTypes { get; set; }

    public ConditionHitByEntityTypeRecently() { }

    public ConditionHitByEntityTypeRecently(IEnumerable<EntityType> entityTypes)
    {
        EntityTypes = entityTypes
            .Distinct()
            .ToHashSet();
    }

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

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionHitByEntityTypeRecently(
        this CharacterDropDropTemplate template,
        IEnumerable<EntityType> entityTypes)
    {
        if (entityTypes?.Any() == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionHitByEntityTypeRecently(entityTypes));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionHitByEntityTypeRecently);
        }

        return template;
    }
}
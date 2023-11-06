using System.Collections.Generic;
using System.Linq;
using DropThat.Creature.DamageRecords;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;
using ThatCore.Logging;

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

#if DEBUG && VERBOSE
        Log.LogTrace($"[{character.name}] Checking recent hits: " + recentHits.Select(x => GetHitterType(x)).Join());
        Log.LogTrace($"[{character.name}] Searching for hits by: " + entities.Join());
#endif

        var match = recentHits.Any(x => EntityTypes.Contains(x.AttackerType));

        if (!match)
        {
            Log.Trace?.Log($"Filtered drop '{context.DropInfo.DisplayName}' due not being hit recently by required entity type.");
        }

        return !match;
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
using System.Collections.Generic;
using System.Linq;
using DropThat.Creature.DamageRecords;
using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionKilledByEntityType : IDropCondition
{
    public EntityType[] EntityTypes { get; set; }

    public bool IsPointless() => (EntityTypes?.Length ?? 0) == 0;

    public bool IsValid(DropContext context)
    {
        if (EntityTypes is null ||
            EntityTypes.Length == 0)
        {
            return true;
        }

        DamageRecord lastHit = RecordLastHit.GetLastHit(context.Character);

        if (lastHit is null)
        {
            return false;
        }

        EntityType lastHitter = lastHit.GetHitterType();

        return EntityTypes.Any(x => x == lastHitter);
    }
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionKilledByEntityType(
        this IHaveDropConditions template,
        IEnumerable<EntityType> entityTypes)
    {
        if (entityTypes?.Any() == true)
        {
            template.Conditions
                .GetOrCreate<ConditionKilledByEntityType>()
                .EntityTypes = entityTypes.Distinct().ToArray();
        }
        else
        {
            template.Conditions.Remove<ConditionKilledByEntityType>();
        }

        return template;
    }
}
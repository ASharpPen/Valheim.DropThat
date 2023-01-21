using System.Collections.Generic;
using System.Linq;
using DropThat.Creature.DamageRecords;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionKilledByEntityType : IDropCondition
{
    public EntityType[] EntityTypes { get; set; }

    public ConditionKilledByEntityType() { }

    public ConditionKilledByEntityType(IEnumerable<EntityType> entityTypes)
    {
        EntityTypes = entityTypes
            .Distinct()
            .ToArray();
    }

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
            template.Conditions.AddOrReplaceByType(new ConditionKilledByEntityType(entityTypes));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionKilledByEntityType);
        }

        return template;
    }
}
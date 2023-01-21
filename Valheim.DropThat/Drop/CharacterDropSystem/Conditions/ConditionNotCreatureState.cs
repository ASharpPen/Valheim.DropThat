using System.Collections.Generic;
using System.Linq;
using DropThat.Caches;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionNotCreatureState : IDropCondition
{
    public CreatureState[] CreatureStates { get; set; }

    public ConditionNotCreatureState()
    {
    }

    public ConditionNotCreatureState(IEnumerable<CreatureState> creatureStates)
    {
        CreatureStates = creatureStates.ToArray();
    }

    public bool IsValid(DropContext context)
    {
        if (CreatureStates is null ||
            CreatureStates.Length == 0)
        {
            return true;
        }

        return !CreatureStates.Any(x =>
        {
            switch (x)
            {
                case CreatureState.Tamed:
                    return context.Character.IsTamed();
                case CreatureState.Event:
                    MonsterAI ai = ComponentCache.Get<MonsterAI>(context.Character);

                    if (ai.IsNull())
                    {
                        return false;
                    }

                    return ai.IsEventCreature();
                default:
                    return true;
            }
        });
    }
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionNotCreatureState(
        this IHaveDropConditions template,
        List<CreatureState> states)
    {
        if (states?.Any() == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionNotCreatureState(states));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionNotCreatureState);
        }

        return template;
    }
}
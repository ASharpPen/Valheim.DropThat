using System.Collections.Generic;
using System.Linq;
using DropThat.Caches;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionCreatureState : IDropCondition
{
    public CreatureState[] CreatureStates { get; set; }

    public ConditionCreatureState()
    {
    }

    public ConditionCreatureState(IEnumerable<CreatureState> creatureStates)
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

        return CreatureStates.Any(x =>
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

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionCreatureState(
        this CharacterDropDropTemplate template, 
        List<CreatureState> states)
    {
        if (states?.Any() == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionCreatureState(states));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionCreatureState);
        }

        return template;
    }
}
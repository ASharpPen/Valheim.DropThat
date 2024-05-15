using System.Linq;
using DropThat.Caches;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public sealed class ConditionNotCreatureState : IDropCondition
{
    public CreatureState[] CreatureStates { get; set; }

    public bool IsPointless() => (CreatureStates?.Length ?? 0) == 0;

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
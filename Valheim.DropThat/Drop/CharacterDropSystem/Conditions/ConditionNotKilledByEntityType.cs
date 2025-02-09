using System.Linq;
using DropThat.Creature.DamageRecords;
using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public sealed class ConditionNotKilledByEntityType : IDropCondition
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
            return true;
        }

        EntityType lastHitter = lastHit.GetHitterType();

        return !EntityTypes.Contains(lastHitter);
    }
}

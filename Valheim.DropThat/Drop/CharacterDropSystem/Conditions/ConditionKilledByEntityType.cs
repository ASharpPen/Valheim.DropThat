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
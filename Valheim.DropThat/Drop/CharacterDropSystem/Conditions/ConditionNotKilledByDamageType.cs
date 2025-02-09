using DropThat.Creature.DamageRecords;
using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public sealed class ConditionNotKilledByDamageType : IDropCondition
{
    public HitData.DamageType DamageTypeMask { get; set; } = 0;

    public bool IsPointless() => DamageTypeMask == 0;

    public bool IsValid(DropContext context)
    {
        if (DamageTypeMask == 0)
        {
            return true;
        }

        DamageRecord lastHit = RecordLastHit.GetLastHit(context.Character);

        if (lastHit is null)
        {
            return true;
        }

        return (lastHit.DamageType & DamageTypeMask) == 0;
    }
}
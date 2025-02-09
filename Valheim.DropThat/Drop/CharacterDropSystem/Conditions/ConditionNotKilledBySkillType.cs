using DropThat.Creature.DamageRecords;
using DropThat.Drop.CharacterDropSystem.Models;
using System.Collections.Generic;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public sealed class ConditionNotKilledBySkillType : IDropCondition
{
    public HashSet<Skills.SkillType> SkillTypes { get; set; }

    public bool IsPointless() => (SkillTypes?.Count ?? 0) == 0;

    public bool IsValid(DropContext context)
    {
        if (SkillTypes is null ||
            SkillTypes.Count == 0)
        {
            return true;
        }

        DamageRecord lastHit = RecordLastHit.GetLastHit(context.Character);

        if (lastHit is null)
        {
            return true;
        }

        return !SkillTypes.Contains(lastHit.SkillType);
    }
}

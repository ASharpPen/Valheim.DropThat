using System.Collections.Generic;
using System.Linq;
using DropThat.Creature.DamageRecords;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionKilledByDamageType : IDropCondition
{
    public HitData.DamageType DamageTypeMask { get; set; } = 0;

    public ConditionKilledByDamageType() { }

    public ConditionKilledByDamageType(IEnumerable<HitData.DamageType> damageTypes)
    {
        foreach (var damageType in damageTypes)
        {
            DamageTypeMask |= damageType;
        }
    }

    public bool IsValid(DropContext context)
    {
        if (DamageTypeMask == 0)
        {
            return true;
        }

        DamageRecord lastHit = RecordLastHit.GetLastHit(context.Character);

        if (lastHit is null)
        {
            return false;
        }

        return (lastHit.DamageType & DamageTypeMask) > 0;
    }
}

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionKilledByDamageType(
        this CharacterDropDropTemplate template,
        IEnumerable<HitData.DamageType> damageTypes)
    {
        if (damageTypes?.Any() == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionKilledByDamageType(damageTypes));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionKilledByDamageType);
        }

        return template;
    }
}
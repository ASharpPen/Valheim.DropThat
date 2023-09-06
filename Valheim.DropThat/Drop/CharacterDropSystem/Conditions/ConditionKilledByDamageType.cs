﻿using System.Collections.Generic;
using System.Linq;
using DropThat.Creature.DamageRecords;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Utilities.Valheim;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionKilledByDamageType : IDropCondition
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
            return false;
        }

        return (lastHit.DamageType & DamageTypeMask) > 0;
    }
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionKilledByDamageType(
        this IHaveDropConditions template,
        IEnumerable<HitData.DamageType> damageTypes)
    {
        if (damageTypes?.Any() == true)
        {
            template.Conditions
                .GetOrCreate<ConditionKilledByDamageType>()
                .DamageTypeMask = damageTypes.ToBitmask();
        }
        else
        {
            template.Conditions.Remove<ConditionKilledByDamageType>();
        }

        return template;
    }
}
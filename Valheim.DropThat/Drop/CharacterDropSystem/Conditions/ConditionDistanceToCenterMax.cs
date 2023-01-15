﻿using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionDistanceToCenterMax : IDropCondition
{
    public float DistanceToCenterMax { get; set; }

    public ConditionDistanceToCenterMax() { }

    public ConditionDistanceToCenterMax(float maxDistance)
    {
        DistanceToCenterMax = maxDistance;
    }

    public bool IsValid(DropContext context)
    {
        var distance = context.ZDO.m_position.magnitude;

        return distance <= DistanceToCenterMax;
    }
}

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionDistanceToCenterMax(
        this CharacterDropDropTemplate template,
        float? distance)
    {
        if (distance > 0)
        {
            template.Conditions.AddOrReplaceByType(new ConditionDistanceToCenterMax(distance.Value));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionDistanceToCenterMax);
        }

        return template;
    }
}
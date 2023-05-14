﻿using System;
using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionWithinCircle : IDropCondition
{
    public float? CenterX { get; set; }
    public float? CenterZ { get; set; }
    public float? Radius { get; set; }

    public bool IsValid(DropContext context)
    {
        if (!Radius.HasValue ||
            !CenterX.HasValue ||
            !CenterZ.HasValue)
        {
            return true;
        }

        double distX = context.Pos.x - CenterX.Value;
        double distZ = context.Pos.z - CenterZ.Value;

        var dist = Math.Sqrt(distX * distX + distZ * distZ);

        return dist <= Radius;
    }
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionWithinCircle(
        this IHaveDropConditions template,
        float? centerX,
        float? centerZ,
        float? radius
        )
    {
        if (centerX is null &&
            centerZ is null &&
            radius is null)
        {
            template.Conditions.Remove<ConditionWithinCircle>();
        }
        else if (
            radius is null || 
            radius < 0)
        {
            template.Conditions.Remove<ConditionWithinCircle>();
        }
        else
        {
            var cond = template.Conditions.GetOrCreate<ConditionWithinCircle>();

            cond.CenterX = centerX;
            cond.CenterZ = centerZ;
            cond.Radius = radius;
        }

        return template;
    }
}
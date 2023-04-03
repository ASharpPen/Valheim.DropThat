using System;
using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionWithinCircle : IDropCondition
{
    public float CenterX { get; set; }
    public float CenterZ { get; set; }
    public float Radius { get; set; }

    public ConditionWithinCircle()
    {
    }

    public ConditionWithinCircle(
        float? centerX,
        float? centerZ,
        float? radius)
    {
        CenterX = centerX ?? default;
        CenterZ = centerZ ?? default;
        Radius = radius ?? default;
    }

    public bool IsValid(DropContext context)
    {
        if (Radius < 0)
        {
            return true;
        }

        double distX = context.Pos.x - CenterX;
        double distZ = context.Pos.z - CenterZ;

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
            template.Conditions.RemoveAll(x => x is ConditionWithinCircle);
        }
        else if (
            radius is null || 
            radius < 0)
        {
            template.Conditions.RemoveAll(x => x is ConditionWithinCircle);
        }
        else
        {
            template.Conditions.AddOrReplaceByType(new ConditionWithinCircle(centerX, centerZ, radius));
        }

        return template;
    }
}
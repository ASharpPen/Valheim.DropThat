using System;
using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public sealed class ConditionNotWithinCircle : IDropCondition
{
    public float? CenterX { get; set; }
    public float? CenterZ { get; set; }
    public float? Radius { get; set; }

    public bool IsPointless() =>
        !Radius.HasValue ||
        !CenterX.HasValue ||
        !CenterZ.HasValue ||
        Radius < 0;

    public bool IsValid(DropContext context)
    {
        if (IsPointless())
        {
            return true;
        }

        double distX = context.Pos.x - CenterX.Value;
        double distZ = context.Pos.z - CenterZ.Value;

        var dist = Math.Sqrt(distX * distX + distZ * distZ);

        return dist > Radius;
    }
}
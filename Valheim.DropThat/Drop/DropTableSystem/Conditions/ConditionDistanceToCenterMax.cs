using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public sealed class ConditionDistanceToCenterMax : IDropCondition
{
    public float? MaxDistance { get; set; }

    public bool IsPointless() =>
        MaxDistance is null ||
        MaxDistance <= 0;

    public bool IsValid(DropContext context)
    {
        if (MaxDistance is null ||
            MaxDistance <= 0)
        {
            return true;
        }

        return context.Pos.magnitude <= MaxDistance;
    }
}
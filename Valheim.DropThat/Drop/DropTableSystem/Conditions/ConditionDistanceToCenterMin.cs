using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionDistanceToCenterMin : IDropCondition
{
    public float? MinDistance { get; set; }

    public bool IsPointless() =>
        MinDistance is null ||
        MinDistance <= 0;

    public bool IsValid(DropContext context)
    {
        if (MinDistance is null ||
            MinDistance <= 0)
        {
            return true;
        }

        return MinDistance <= context.Pos.magnitude;
    }
}
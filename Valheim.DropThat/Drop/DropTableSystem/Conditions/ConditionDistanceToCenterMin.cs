using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionDistanceToCenterMin : IDropCondition
{
    public float? MinDistance { get; set; }

    public ConditionDistanceToCenterMin() { }

    public ConditionDistanceToCenterMin(float? minDistance)
    {
        MinDistance = minDistance;
    }

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

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionDistanceToCenterMin(
        this IHaveDropConditions template,
        float? distance)
    {
        if (distance > 0)
        {
            template.Conditions.GetOrCreate<ConditionDistanceToCenterMin>().MinDistance = distance;
        }
        else
        {
            template.Conditions.Remove<ConditionDistanceToCenterMin>();
        }

        return template;
    }
}
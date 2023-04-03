using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Extensions;

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
            template.Conditions.AddOrReplaceByType(new ConditionDistanceToCenterMin(distance));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionDistanceToCenterMin);
        }

        return template;
    }
}
using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionDistanceToCenterMax : IDropCondition
{
    public float? MaxDistance { get; set; }

    public ConditionDistanceToCenterMax() { }

    public ConditionDistanceToCenterMax(float? maxDistance)
    {
        MaxDistance = maxDistance;
    }

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

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionDistanceToCenterMax(
        this IHaveDropConditions template,
        float? distance)
    {
        if (distance > 0)
        {
            template.Conditions.GetOrCreate<ConditionDistanceToCenterMax>().MaxDistance = distance;
        }
        else
        {
            template.Conditions.Remove<ConditionDistanceToCenterMax>();
        }

        return template;
    }
}
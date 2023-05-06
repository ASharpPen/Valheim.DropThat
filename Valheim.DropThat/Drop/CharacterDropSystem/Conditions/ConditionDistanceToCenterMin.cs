using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionDistanceToCenterMin : IDropCondition
{
    public float DistanceToCenterMin { get; set; }

    public ConditionDistanceToCenterMin() { }

    public ConditionDistanceToCenterMin(float minDistance)
    {
        DistanceToCenterMin = minDistance;
    }

    public bool IsValid(DropContext context)
    {
        var distance = context.ZDO.m_position.magnitude;

        return distance >= DistanceToCenterMin;
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
            template.Conditions.GetOrCreate<ConditionDistanceToCenterMin>().DistanceToCenterMin = distance.Value;
        }
        else
        {
            template.Conditions.Remove<ConditionDistanceToCenterMin>();
        }

        return template;
    }
}
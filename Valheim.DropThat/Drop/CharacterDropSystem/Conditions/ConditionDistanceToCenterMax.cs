using DropThat.Drop.CharacterDropSystem.Models;

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

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionDistanceToCenterMax(
        this IHaveDropConditions template,
        float? distance)
    {
        if (distance > 0)
        {
            template.Conditions.GetOrCreate<ConditionDistanceToCenterMax>().DistanceToCenterMax = distance.Value;
        }
        else
        {
            template.Conditions.Remove<ConditionDistanceToCenterMax>();
        }

        return template;
    }
}
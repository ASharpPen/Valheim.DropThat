using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

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

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionDistanceToCenterMin(
        this CharacterDropDropTemplate template,
        float? distance)
    {
        if (distance > 0)
        {
            template.Conditions.AddOrReplaceByType(new ConditionDistanceToCenterMin(distance.Value));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionDistanceToCenterMin);
        }

        return template;
    }
}
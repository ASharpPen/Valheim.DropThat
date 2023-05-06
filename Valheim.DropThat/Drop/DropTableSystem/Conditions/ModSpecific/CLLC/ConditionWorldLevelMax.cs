using CreatureLevelControl;
using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions.ModSpecific.CLLC;

public class ConditionWorldLevelMax : IDropCondition
{
    public int WorldLevel { get; set; }

    public ConditionWorldLevelMax()
    {
    }

    public ConditionWorldLevelMax(int? worldLevel)
    {
        WorldLevel = worldLevel ?? default;
    }

    public bool IsValid(DropContext context) => WorldLevel <= API.GetWorldLevel();
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionWorldLevelMax(
        this IHaveDropConditions template,
        int? worldLevel)
    {
        if (worldLevel > 0)
        {
            template.Conditions
                .GetOrCreate<ConditionWorldLevelMax>()
                .WorldLevel = worldLevel.Value;
        }
        else
        {
            template.Conditions.Remove<ConditionWorldLevelMax>();
        }

        return template;
    }
}
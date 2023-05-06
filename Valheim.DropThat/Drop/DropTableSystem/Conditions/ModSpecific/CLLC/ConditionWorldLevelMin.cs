using CreatureLevelControl;
using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions.ModSpecific.CLLC;

public class ConditionWorldLevelMin : IDropCondition
{
    public int WorldLevel { get; set; }

    public ConditionWorldLevelMin()
    {
    }

    public ConditionWorldLevelMin(int? worldLevel)
    {
        WorldLevel = worldLevel ?? default;
    }

    public bool IsValid(DropContext context) => WorldLevel >= API.GetWorldLevel();
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionWorldLevelMin(
        this IHaveDropConditions template,
        int? worldLevel)
    {
        if (worldLevel >= 0)
        {
            template.Conditions
                .GetOrCreate<ConditionWorldLevelMin>()
                .WorldLevel = worldLevel.Value;
        }
        else
        {
            template.Conditions.Remove<ConditionWorldLevelMin>();
        }

        return template;
    }
}
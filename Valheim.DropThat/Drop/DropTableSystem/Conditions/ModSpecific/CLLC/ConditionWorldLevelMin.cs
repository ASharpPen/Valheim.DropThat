using CreatureLevelControl;
using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Extensions;

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
            template.Conditions.AddOrReplaceByType(new ConditionWorldLevelMin(worldLevel));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionWorldLevelMin);
        }

        return template;
    }
}
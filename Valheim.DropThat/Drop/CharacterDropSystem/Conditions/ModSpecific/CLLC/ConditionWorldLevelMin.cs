using CreatureLevelControl;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Integrations;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;

public class ConditionWorldLevelMin : IDropCondition
{
    public int? MinLevel { get; set; }

    public bool IsPointless() => 
        MinLevel is null || 
        MinLevel == 0 ||
        !InstallationManager.CLLCInstalled;

    public bool IsValid(DropContext context)
    {
        if (MinLevel is null ||
            !InstallationManager.CLLCInstalled)
        {
            return true;
        }

        return API.GetWorldLevel() >= MinLevel;
    }
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionWorldLevelMin(
        this IHaveDropConditions template,
        int? minWorldLevel)
    {
        if (minWorldLevel > 0)
        {
            template.Conditions
                .GetOrCreate<ConditionWorldLevelMin>()
                .MinLevel = minWorldLevel.Value;
        }
        else
        {
            template.Conditions.Remove<ConditionWorldLevelMin>();
        }

        return template;
    }
}
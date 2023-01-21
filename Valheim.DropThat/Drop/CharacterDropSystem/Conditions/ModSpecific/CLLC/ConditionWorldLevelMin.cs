using CreatureLevelControl;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Integrations;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;

public class ConditionWorldLevelMin : IDropCondition
{
    public int? MinLevel { get; }

    public ConditionWorldLevelMin() { }

    public ConditionWorldLevelMin(int? minWorldLevel)
    {
        MinLevel = minWorldLevel;
    }

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

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionWorldLevelMin(
        this CharacterDropDropTemplate template,
        int? minWorldLevel)
    {
        if (minWorldLevel > 0)
        {
            template.Conditions.AddOrReplaceByType(new ConditionWorldLevelMin(minWorldLevel.Value));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionWorldLevelMin);
        }

        return template;
    }
}
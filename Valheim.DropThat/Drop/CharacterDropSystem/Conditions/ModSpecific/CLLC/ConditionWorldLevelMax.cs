using CreatureLevelControl;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;

public class ConditionWorldLevelMax : IDropCondition
{
    public int? MaxLevel { get; }

    public ConditionWorldLevelMax() { }

    public ConditionWorldLevelMax(int? maxWorldLevel)
    {
        MaxLevel = maxWorldLevel;
    }

    public bool IsValid(DropContext context)
    {
        if (MaxLevel is null ||
            !ConditionLoaderCLLC.InstalledCLLC)
        {
            return true;
        }

        return API.GetWorldLevel() <= MaxLevel;
    }
}

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionWorldLevelMax(
        this CharacterDropDropTemplate template,
        int? maxWorldLevel)
    {
        if (maxWorldLevel > 0)
        {
            template.Conditions.AddOrReplaceByType(new ConditionWorldLevelMax(maxWorldLevel.Value));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionWorldLevelMax);
        }

        return template;
    }
}
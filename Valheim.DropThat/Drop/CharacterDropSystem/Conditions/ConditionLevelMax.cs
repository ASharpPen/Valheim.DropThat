using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionLevelMax : IDropCondition
{
    public int? MaxLevel { get; set; }

    public ConditionLevelMax() { }

    public ConditionLevelMax(int? maxLevel)
    {
        MaxLevel = maxLevel;
    }

    public bool IsValid(DropContext context)
    {
        if (MaxLevel is null ||
            context.Character.IsNull())
        {
            return true;
        }

        return context.Character.GetLevel() <= MaxLevel;
    }
}

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionLevelMax(
        this CharacterDropDropTemplate template,
        int? maxLevel)
    {
        if (maxLevel is not null)
        {
            template.Conditions.AddOrReplaceByType(new ConditionLevelMax(maxLevel.Value));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionLevelMax);
        }

        return template;
    }
}
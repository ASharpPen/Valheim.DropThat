using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionLevelMin : IDropCondition
{
    public int? MinLevel { get; set; }

    public ConditionLevelMin() { }

    public ConditionLevelMin(int? minLevel)
    {
        MinLevel = minLevel;
    }

    public bool IsValid(DropContext context)
    {
        if (MinLevel is null ||
            context.Character.IsNull())
        {
            return true;
        }

        return context.Character.GetLevel() >= MinLevel;
    }
}

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionLevelMin(
        this CharacterDropDropTemplate template,
        int? minLevel)
    {
        if (minLevel is not null)
        {
            template.Conditions.AddOrReplaceByType(new ConditionLevelMin(minLevel.Value));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionLevelMin);
        }

        return template;
    }
}
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

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionLevelMin(
        this IHaveDropConditions template,
        int? minLevel)
    {
        if (minLevel is not null)
        {
            template.Conditions
                .GetOrCreate<ConditionLevelMin>()
                .MinLevel = minLevel.Value;
        }
        else
        {
            template.Conditions.Remove<ConditionLevelMin>();
        }

        return template;
    }
}
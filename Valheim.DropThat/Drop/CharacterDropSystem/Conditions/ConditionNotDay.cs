using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionNotDay : IDropCondition
{
    public bool IsValid(DropContext context) => !EnvMan.instance.IsDay();
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionNotDay(
        this IHaveDropConditions template,
        bool? notDay)
    {
        if (notDay == true)
        {
            template.Conditions.GetOrCreate<ConditionNotDay>();
        }
        else
        {
            template.Conditions.Remove<ConditionNotDay>();
        }

        return template;
    }
}
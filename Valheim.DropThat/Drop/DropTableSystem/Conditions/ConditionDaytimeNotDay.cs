using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionDaytimeNotDay : IDropCondition
{
    public bool IsValid(DropContext context) =>
        !EnvMan.instance.IsDay();
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionDaytimeNotDay(
        this IHaveDropConditions template,
        bool? notDay)
    {
        if (notDay == true)
        {
            template.Conditions.GetOrCreate<ConditionDaytimeNotDay>();
        }
        else
        {
            template.Conditions.Remove<ConditionDaytimeNotDay>();
        }

        return template;
    }
}
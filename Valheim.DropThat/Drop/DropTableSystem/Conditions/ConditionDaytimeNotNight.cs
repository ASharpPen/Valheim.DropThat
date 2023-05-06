using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionDaytimeNotNight : IDropCondition
{
    public bool IsValid(DropContext context) =>
        !EnvMan.instance.IsNight();
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionDaytimeNotNight(
        this IHaveDropConditions template,
        bool? notNight)
    {
        if (notNight == true)
        {
            template.Conditions.GetOrCreate<ConditionDaytimeNotNight>();
        }
        else
        {
            template.Conditions.Remove<ConditionDaytimeNotNight>();
        }

        return template;
    }
}
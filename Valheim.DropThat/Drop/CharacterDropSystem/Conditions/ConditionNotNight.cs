using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionNotNight : IDropCondition
{
    public bool IsValid(DropContext context) => !EnvMan.instance.IsNight();
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionNotNight(
        this IHaveDropConditions template,
        bool? notNight)
    {
        if (notNight == true)
        {
            template.Conditions.GetOrCreate<ConditionNotNight>();
        }
        else
        {
            template.Conditions.Remove<ConditionNotNight>();
        }

        return template;
    }
}
using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionNotAfternoon : IDropCondition
{
    public bool IsValid(DropContext context) => !EnvMan.instance.IsAfternoon();
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionNotAfternoon(
        this IHaveDropConditions template,
        bool? notAfternoon)
    {
        if (notAfternoon == true)
        {
            template.Conditions.GetOrCreate<ConditionNotAfternoon>();
        }
        else
        {
            template.Conditions.Remove<ConditionNotAfternoon>();
        }

        return template;
    }
}
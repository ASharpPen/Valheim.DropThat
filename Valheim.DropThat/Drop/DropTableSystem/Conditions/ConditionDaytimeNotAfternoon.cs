using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionDaytimeNotAfternoon : IDropCondition
{
    public bool IsValid(DropContext context) =>
        !EnvMan.instance.IsAfternoon();
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionDaytimeNotAfternoon(
        this IHaveDropConditions template,
        bool? notAfternoon)
    {
        if (notAfternoon == true)
        {
            template.Conditions.GetOrCreate<ConditionDaytimeNotAfternoon>();
        }
        else
        {
            template.Conditions.Remove<ConditionDaytimeNotAfternoon>();
        }

        return template;
    }
}
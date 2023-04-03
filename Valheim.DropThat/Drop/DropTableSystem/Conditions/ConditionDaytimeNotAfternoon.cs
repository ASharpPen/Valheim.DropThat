using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Extensions;

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
            template.Conditions.AddOrReplaceByType(new ConditionDaytimeNotAfternoon());
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionDaytimeNotAfternoon);
        }

        return template;
    }
}
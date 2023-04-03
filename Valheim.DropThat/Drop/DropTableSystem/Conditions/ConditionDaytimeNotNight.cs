using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Extensions;

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
            template.Conditions.AddOrReplaceByType(new ConditionDaytimeNotNight());
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionDaytimeNotNight);
        }

        return template;
    }
}
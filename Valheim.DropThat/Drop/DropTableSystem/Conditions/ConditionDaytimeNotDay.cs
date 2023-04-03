using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Extensions;

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
            template.Conditions.AddOrReplaceByType(new ConditionDaytimeNotDay());
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionDaytimeNotDay);
        }

        return template;
    }
}
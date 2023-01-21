using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

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
            template.Conditions.AddOrReplaceByType(new ConditionNotDay());
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionNotDay);
        }

        return template;
    }
}
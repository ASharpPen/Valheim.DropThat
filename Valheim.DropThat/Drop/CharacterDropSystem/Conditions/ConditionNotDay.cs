using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionNotDay : IDropCondition
{
    public bool IsValid(DropContext context) => !EnvMan.instance.IsDay();
}

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionNotDay(
        this CharacterDropDropTemplate template,
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
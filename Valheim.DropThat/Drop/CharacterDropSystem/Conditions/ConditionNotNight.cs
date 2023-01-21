using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionNotNight : IDropCondition
{
    public bool IsValid(DropContext context) => !EnvMan.instance.IsNight();
}

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionNotNight(
        this CharacterDropDropTemplate template,
        bool? notNight)
    {
        if (notNight == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionNotNight());
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionNotNight);
        }

        return template;
    }
}
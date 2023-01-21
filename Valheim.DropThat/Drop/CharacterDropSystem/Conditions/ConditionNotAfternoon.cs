using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionNotAfternoon : IDropCondition
{
    public bool IsValid(DropContext context) => !EnvMan.instance.IsAfternoon();
}

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionNotAfternoon(
        this CharacterDropDropTemplate template,
        bool? notAfternoon)
    {
        if (notAfternoon == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionNotAfternoon());
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionNotAfternoon);
        }

        return template;
    }
}
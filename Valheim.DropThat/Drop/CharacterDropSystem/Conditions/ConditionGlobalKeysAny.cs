using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionGlobalKeysAny : IDropCondition
{
    public string[] GlobalKeys { get; set; }

    public ConditionGlobalKeysAny() { }

    public ConditionGlobalKeysAny(IEnumerable<string> keys)
    {
        GlobalKeys = keys.ToArray();
    }

    public bool IsValid(DropContext context)
    {
        if (GlobalKeys is null ||
            GlobalKeys.Length == 0)
        {
            return true;
        }

        return GlobalKeys.Any(ZoneSystem.instance.GetGlobalKey);
    }
}

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionGlobalKeysAny(
        this CharacterDropDropTemplate template,
        IEnumerable<string> globalKeys)
    {
        if (globalKeys?.Any() == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionGlobalKeysAny(globalKeys));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionGlobalKeysAny);
        }

        return template;
    }
}
using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionGlobalKeysAll : IDropCondition
{
    public string[] GlobalKeys { get; set; }

    public ConditionGlobalKeysAll()
    {
    }

    public ConditionGlobalKeysAll(IEnumerable<string> keys)
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

        return GlobalKeys.All(ZoneSystem.instance.GetGlobalKey);
    }
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionGlobalKeysAll(
        this IHaveDropConditions template,
        IEnumerable<string> globalKeys)
    {
        if (globalKeys?.Any() == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionGlobalKeysAll(globalKeys));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionGlobalKeysAll);
        }

        return template;
    }
}
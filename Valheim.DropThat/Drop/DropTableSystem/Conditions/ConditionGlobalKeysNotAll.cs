using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionGlobalKeysNotAll : IDropCondition
{
    public string[] GlobalKeys { get; set; }

    public ConditionGlobalKeysNotAll()
    {
    }

    public ConditionGlobalKeysNotAll(IEnumerable<string> keys)
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

        return !GlobalKeys.All(ZoneSystem.instance.GetGlobalKey);
    }
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionGlobalKeysNotAll(
        this IHaveDropConditions template,
        IEnumerable<string> globalKeys)
    {
        if (globalKeys?.Any() == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionGlobalKeysNotAll(globalKeys));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionGlobalKeysNotAll);
        }

        return template;
    }
}

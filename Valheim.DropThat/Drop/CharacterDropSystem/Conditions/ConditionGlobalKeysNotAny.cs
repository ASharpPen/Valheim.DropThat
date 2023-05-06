﻿using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

internal class ConditionGlobalKeysNotAny : IDropCondition
{
    public string[] GlobalKeys { get; set; }

    public ConditionGlobalKeysNotAny() { }

    public ConditionGlobalKeysNotAny(IEnumerable<string> keys)
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

        return !GlobalKeys.Any(ZoneSystem.instance.GetGlobalKey);
    }
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionGlobalKeysNotAny(
        this IHaveDropConditions template,
        IEnumerable<string> globalKeys)
    {
        if (globalKeys?.Any() == true)
        {
            template.Conditions.GetOrCreate<ConditionGlobalKeysNotAny>().GlobalKeys = globalKeys.ToArray();
        }
        else
        {
            template.Conditions.Remove<ConditionGlobalKeysNotAny>();
        }

        return template;
    }
}
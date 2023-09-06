using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionEnvironments : IDropCondition
{
    public HashSet<string> Environments { get; set; }

    public void SetEnvironments(IEnumerable<string> environments)
    {
        Environments = environments
            .Select(x => x
                .Trim()
                .ToUpperInvariant())
            .ToHashSet();
    }

    public bool IsPointless() => (Environments?.Count ?? 0) == 0;

    public bool IsValid(DropContext context)
    {
        if (Environments is null ||
            Environments.Count == 0)
        {
            return true;
        }

        var currentEnv = EnvMan.instance.GetCurrentEnvironment();

        var envName = currentEnv.m_name
            .Trim()
            .ToUpperInvariant();

        return Environments.Contains(envName);
    }
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionEnvironments(
        this IHaveDropConditions template,
        IEnumerable<string> environments)
    {
        if (environments?.Any() == true)
        {
            template.Conditions.GetOrCreate<ConditionEnvironments>().SetEnvironments(environments);
        }
        else
        {
            template.Conditions.Remove<ConditionEnvironments>();
        }

        return template;
    }
}
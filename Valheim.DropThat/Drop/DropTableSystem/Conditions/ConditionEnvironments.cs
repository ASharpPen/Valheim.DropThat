using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionEnvironments : IDropCondition
{
    public HashSet<string> Environments { get; set; }

    public ConditionEnvironments() { }

    public ConditionEnvironments(IEnumerable<string> environments) 
    {
        SetEnvironments(environments);
    }

    public void SetEnvironments(IEnumerable<string> environments)
    {
        Environments = environments
            .Select(x => x
                .Trim()
                .ToUpperInvariant())
            .ToHashSet();
    }

    public bool IsValid(DropContext context)
    {
        if (Environments is null ||
            Environments.Count == 0)
        {
            return true;
        }

        var currentEnv = EnvMan.instance
            .GetCurrentEnvironment()
            .m_name
            .Trim()
            .ToUpperInvariant();

        return Environments.Contains(currentEnv);
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
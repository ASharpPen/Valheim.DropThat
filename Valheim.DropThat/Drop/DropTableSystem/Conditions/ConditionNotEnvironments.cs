using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public sealed class ConditionNotEnvironments : IDropCondition
{
    public HashSet<string> Environments { get; set; }

    public bool IsPointless() => (Environments?.Count ?? 0) == 0;

    public void SetEnvironments(IEnumerable<string> environments)
    {
        Environments = environments?
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

        return !Environments.Contains(currentEnv);
    }
}
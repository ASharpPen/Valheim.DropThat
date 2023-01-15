using System.Collections.Generic;
using System.Linq;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionEnvironments : IDropCondition
{
    public HashSet<string> Environments { get; set; }

    public ConditionEnvironments() { }

    public ConditionEnvironments(IEnumerable<string> environments)
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

        var currentEnv = EnvMan.instance.GetCurrentEnvironment();

        var envName = currentEnv.m_name
            .Trim()
            .ToUpperInvariant();

        return Environments.Contains(envName);
    }
}

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionEnvironments(
        this CharacterDropDropTemplate template,
        IEnumerable<string> environments)
    {
        if (environments?.Any() == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionEnvironments(environments));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionEnvironments);
        }

        return template;
    }
}
using System.Collections.Generic;
using System.Linq;
using DropThat.Creature.StatusRecords;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionKilledWithStatusAll : IDropCondition
{
    public string[] Statuses { get; set; }

    public ConditionKilledWithStatusAll() { }

    public ConditionKilledWithStatusAll(IEnumerable<string> statuses)
    {
        SetStatuses(statuses);
    }

    public void SetStatuses(IEnumerable<string> statuses)
    {
        Statuses = statuses
            .Select(x => x
                .Trim()
                .ToUpperInvariant())
            .Distinct()
            .ToArray();
    }

    public bool IsValid(DropContext context)
    {
        if (Statuses is null ||
            Statuses.Length == 0)
        {
            return false;
        }

        var lastStatusRecord = RecordLastStatus.GetLastStatus(context.Character);

        if (lastStatusRecord is null)
        {
            return false;
        }

        var lastStatuses = Statuses
            .Select(x => x
                .Trim()
                .ToUpperInvariant())
            .ToHashSet();

        return Statuses.All(lastStatuses.Contains);
    }
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionKilledWithStatusAll(
        this IHaveDropConditions template,
        IEnumerable<string> statuses)
    {
        if (statuses?.Any() == true)
        {
            template.Conditions
                .GetOrCreate<ConditionKilledWithStatusAll>()
                .SetStatuses(statuses);
        }
        else
        {
            template.Conditions.Remove<ConditionKilledWithStatusAll>();
        }

        return template;
    }
}

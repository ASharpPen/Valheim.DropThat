using System.Linq;
using DropThat.Creature.StatusRecords;
using System.Collections.Generic;
using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public sealed class ConditionKilledWithStatusAny : IDropCondition
{
    public string[] Statuses { get; set; }

    public void SetStatuses(IEnumerable<string> statuses)
    {
        Statuses = statuses?
            .Select(x => x
                .Trim()
                .ToUpperInvariant())
            .Distinct()
            .ToArray();
    }

    public bool IsPointless() => (Statuses?.Length ?? 0) == 0;

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

        return Statuses.Any(lastStatuses.Contains);
    }
}
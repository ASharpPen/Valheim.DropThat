using System.Linq;
using DropThat.Creature.StatusRecords;
using System.Collections.Generic;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionKilledWithStatusAny : IDropCondition
{
    public string[] Statuses { get; set; }

    public ConditionKilledWithStatusAny() { }

    public ConditionKilledWithStatusAny(IEnumerable<string> statuses)
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

        return Statuses.Any(lastStatuses.Contains);
    }
}

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionKilledWithStatusAny(
        this CharacterDropDropTemplate template,
        IEnumerable<string> statuses)
    {
        if (statuses?.Any() == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionKilledWithStatusAny(statuses));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionKilledWithStatusAny);
        }

        return template;
    }
}

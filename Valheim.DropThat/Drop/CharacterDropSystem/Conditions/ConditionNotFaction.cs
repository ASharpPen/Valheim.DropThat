using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionNotFaction : IDropCondition
{
    public Character.Faction[] Factions { get; set; }

    public ConditionNotFaction() { }

    public ConditionNotFaction(IEnumerable<Character.Faction> factions)
    {
        Factions = factions.ToArray();
    }

    public bool IsValid(DropContext context)
    {
        if (context.Character.IsNull())
        {
            return true;
        }

        var characterFaction = context.Character.GetFaction();

        return !Factions.Contains(characterFaction);
    }
}

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionNotFaction(
        this IHaveDropConditions template,
        IEnumerable<Character.Faction> factions)
    {
        if (factions?.Any() == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionNotFaction(factions));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionNotFaction);
        }

        return template;
    }
}
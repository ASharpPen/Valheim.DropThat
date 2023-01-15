using System.Collections.Generic;
using System.Linq;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionFaction : IDropCondition
{
    public Character.Faction[] Factions { get; set; }

    public ConditionFaction() { }

    public ConditionFaction(IEnumerable<Character.Faction> factions)
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

        return Factions.Contains(characterFaction);
    }
}

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionFaction(
        this CharacterDropDropTemplate template,
        IEnumerable<Character.Faction> factions)
    {
        if (factions?.Any() == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionFaction(factions));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionFaction);
        }

        return template;
    }
}
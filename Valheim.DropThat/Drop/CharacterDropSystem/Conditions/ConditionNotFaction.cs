using System.Linq;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public sealed class ConditionNotFaction : IDropCondition
{
    public Character.Faction[] Factions { get; set; }

    public bool IsPointless() => (Factions?.Length ?? 0) == 0;

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
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionLevelMin : IDropCondition
{
    public int? MinLevel { get; set; }

    public bool IsPointless() => MinLevel is null;

    public bool IsValid(DropContext context)
    {
        if (MinLevel is null ||
            context.Character.IsNull())
        {
            return true;
        }

        return context.Character.GetLevel() >= MinLevel;
    }
}
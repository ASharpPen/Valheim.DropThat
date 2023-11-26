using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionLevelMax : IDropCondition
{
    public int? MaxLevel { get; set; }

    public bool IsPointless() => MaxLevel is null;

    public bool IsValid(DropContext context)
    {
        if (MaxLevel is null ||
            context.Character.IsNull())
        {
            return true;
        }

        return context.Character.GetLevel() <= MaxLevel;
    }
}
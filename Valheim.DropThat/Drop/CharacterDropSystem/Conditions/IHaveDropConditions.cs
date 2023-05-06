using ThatCore.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

internal interface IHaveDropConditions
{
    TypeSet<IDropCondition> Conditions { get; }
}

using ThatCore.Models;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public interface IHaveDropConditions
{
    TypeSet<IDropCondition> Conditions { get; }
}

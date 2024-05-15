using System.Collections.Generic;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public interface IHaveDropConditions
{
    ICollection<IDropCondition> Conditions { get; }
}
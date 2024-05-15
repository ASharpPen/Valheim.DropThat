using System.Collections.Generic;

namespace DropThat.Drop.DropTableSystem.Conditions;

public interface IHaveDropConditions
{
    ICollection<IDropCondition> Conditions { get; }
}

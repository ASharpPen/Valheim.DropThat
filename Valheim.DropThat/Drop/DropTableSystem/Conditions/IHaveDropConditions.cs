using System.Collections.Generic;

namespace DropThat.Drop.DropTableSystem.Conditions;

public interface IHaveDropConditions
{
    List<IDropCondition> Conditions { get; }
}

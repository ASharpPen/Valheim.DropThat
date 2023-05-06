using System;
using ThatCore.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public interface IHaveDropConditions
{
    TypeSet<IDropCondition> Conditions { get; }
}

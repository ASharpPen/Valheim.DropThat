using System;
using System.Collections.Generic;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

internal interface IHaveDropConditions
{
    List<IDropCondition> Conditions { get; }
}

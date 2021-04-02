using System.Collections.Generic;
using Valheim.DropThat.ConfigurationTypes;

namespace Valheim.DropThat.Conditions
{
    public interface ICondition
    {
        bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop);
    }
}

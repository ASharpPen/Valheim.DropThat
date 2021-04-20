
using Valheim.DropThat.Caches;

namespace Valheim.DropThat.Conditions
{
    public interface ICondition
    {
        bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop);
    }
}

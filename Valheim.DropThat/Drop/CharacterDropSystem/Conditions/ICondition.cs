using Valheim.DropThat.Drop.CharacterDropSystem.Caches;

namespace Valheim.DropThat.Drop.CharacterDropSystem.Conditions
{
    public interface ICondition
    {
        bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop);
    }
}

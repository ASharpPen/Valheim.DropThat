using DropThat.Drop.CharacterDropSystem.Caches;

namespace DropThat.Drop.CharacterDropSystem.Conditions
{
    public interface ICondition
    {
        bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop);
    }
}

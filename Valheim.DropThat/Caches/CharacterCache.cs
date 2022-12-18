using Valheim.DropThat.Core.Cache;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Caches
{
    public static class CharacterCache
    {
        private static ManagedCache<CharacterExtended> CharacterExtendedTable { get; } = new();
        private static ManagedCache<Character> CharacterDropTable { get; } = new();

        public static CharacterExtended GetData(Character character)
        {
            if (CharacterExtendedTable.TryGet(character.gameObject, out CharacterExtended extended))
            {
                return extended;
            }

            return null;
        }

        public static Character GetCharacter(CharacterDrop characterDrop)
        {
            if(CharacterDropTable.TryGet(characterDrop.gameObject, out Character cached))
            {
                return cached;
            }

            var component = characterDrop.GetComponent<Character>();
            CharacterDropTable.Set(characterDrop.gameObject, component);

            return component;
        }

        public static MonsterAI GetMonsterAI(Character character)
        {
            CharacterExtended cache = CharacterExtendedTable.GetOrCreate(character.gameObject);

            if (cache.HasMonsterAI.HasValue)
            {
                return cache.HasMonsterAI.Value
                    ? cache.MonsterAI
                    : null;
            }

            cache.MonsterAI = character.GetComponent<MonsterAI>();
            cache.HasMonsterAI = cache.MonsterAI || cache.MonsterAI is not null;

            return cache.MonsterAI;
        }

        public static Inventory GetInventory(Character character)
        {
            var extended = CharacterExtendedTable.GetOrCreate(character.gameObject);

            if(extended.HasInventory.HasValue)
            {
                return extended.HasInventory.Value
                    ? extended.Inventory
                    : null;
            }

            var humanoid = character.GetComponent<Humanoid>();

            var inventory = humanoid.IsNotNull()
                ? humanoid.GetInventory()
                : null;

            extended.Inventory = inventory;
            extended.HasInventory = inventory is not null;

            return inventory;
        }
    }
}

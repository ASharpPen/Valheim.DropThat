using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Valheim.DropThat.Caches
{
    public static class CharacterCache
    {
        private static ConditionalWeakTable<Character, CharacterExtended> CharacterTable = new ConditionalWeakTable<Character, CharacterExtended>();
        private static ConditionalWeakTable<CharacterDrop, Character> CharacterFromDropTable = new ConditionalWeakTable<CharacterDrop, Character>();

        public static CharacterExtended GetData(Character character)
        {
            if (CharacterTable.TryGetValue(character, out CharacterExtended extended))
            {
                return extended;
            }

            return null;
        }

        public static Character GetCharacter(CharacterDrop characterDrop)
        {
            if(CharacterFromDropTable.TryGetValue(characterDrop, out Character character))
            {
                return character;
            }

            var component = characterDrop.GetComponent<Character>();
            CharacterFromDropTable.Add(characterDrop, component);

            return component;
        }

        public static MonsterAI GetMonsterAI(Character character)
        {
            var extended = CharacterTable.GetOrCreateValue(character);

            if (extended.HasMonsterAI.HasValue)
            {
                return extended.HasMonsterAI.Value
                    ? extended.MonsterAI
                    : null;
            }

            extended.MonsterAI = character.GetComponent<MonsterAI>();
            extended.HasMonsterAI = extended.MonsterAI is not null;

            return extended.MonsterAI;
        }

        public static Inventory GetInventory(Character character)
        {
            var extended = CharacterTable.GetOrCreateValue(character);

            if(extended.HasInventory.HasValue)
            {
                return extended.HasInventory.Value
                    ? extended.Inventory
                    : null;
            }

            var inventory = character.GetComponent<Humanoid>()?.GetInventory();

            extended.Inventory = inventory;
            extended.HasInventory = inventory is not null;

            return inventory;
        }
    }
}

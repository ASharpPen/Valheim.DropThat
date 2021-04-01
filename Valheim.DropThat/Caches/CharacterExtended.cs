using System.Runtime.CompilerServices;

namespace Valheim.DropThat.Caches
{
    public class CharacterExtended
    {
        private static ConditionalWeakTable<Character, CharacterExtended> CharacterTable = new ConditionalWeakTable<Character, CharacterExtended>();
        
        public static CharacterExtended GetData(Character character)
        {
            if(CharacterTable.TryGetValue(character, out CharacterExtended extended))
            {
                return extended;
            }

            return null;
        }

        public static MonsterAI GetMonsterAI(Character character)
        {
            var extended = CharacterTable.GetOrCreateValue(character);

            if(extended.HasMonsterAI.HasValue)
            {
                return extended.HasMonsterAI.Value
                    ? extended.MonsterAI
                    : null;
            }

            extended.MonsterAI = character.GetComponent<MonsterAI>();
            extended.HasMonsterAI = extended.MonsterAI is not null;

            return extended.MonsterAI;
        }


        public bool? HasMonsterAI { get; set; }

        public MonsterAI MonsterAI { get; set; }
    }
}


using System.Runtime.CompilerServices;
using DropThat.Configuration.ConfigTypes;

namespace DropThat.Drop.CharacterDropSystem.Caches
{
    public class DropExtended
    {
        private static ConditionalWeakTable<CharacterDrop.Drop, DropExtended> DropTable = new ConditionalWeakTable<CharacterDrop.Drop, DropExtended>();

        public static void Set(CharacterDrop.Drop drop, CharacterDropItemConfiguration config)
        {
            DropTable.GetOrCreateValue(drop).Config = config;
        }

        public static DropExtended GetExtension(CharacterDrop.Drop drop)
        {
            if (DropTable.TryGetValue(drop, out DropExtended dropExtended))
            {
                return dropExtended;
            }

            return null;
        }

        public CharacterDropItemConfiguration Config { get; set; }
    }
}

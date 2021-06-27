
using System.Runtime.CompilerServices;
using Valheim.DropThat.Configuration.ConfigTypes;

namespace Valheim.DropThat.Caches
{
    public class DropExtended
    {
        private static ConditionalWeakTable<CharacterDrop.Drop, DropExtended> DropTable = new ConditionalWeakTable<CharacterDrop.Drop, DropExtended>();

        public static void Set(CharacterDrop.Drop drop, DropItemConfiguration config)
        {
            DropTable.GetOrCreateValue(drop).Config = config;
        }

        public static DropExtended GetExtension(CharacterDrop.Drop drop)
        {
            if(DropTable.TryGetValue(drop, out DropExtended dropExtended))
            {
                return dropExtended;
            }

            return null;
        }

        public DropItemConfiguration Config { get; set; }
    }
}

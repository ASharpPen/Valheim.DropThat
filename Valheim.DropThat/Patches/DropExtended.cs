
using System.Runtime.CompilerServices;

namespace Valheim.DropThat.ConfigurationTypes
{
    public class DropExtended
    {
        private static ConditionalWeakTable<CharacterDrop.Drop, DropExtended> DropTable = new ConditionalWeakTable<CharacterDrop.Drop, DropExtended>();

        public static void Set(CharacterDrop.Drop drop, DropConfiguration config)
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

        public DropConfiguration Config { get; set; }
    }
}

using System.Runtime.CompilerServices;
using Valheim.DropThat.Core;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Creature.DamageRecords
{
    /// <summary>
    /// Local cache of last hit data.
    /// </summary>
    public static class RecordLastHit
    {
        private static ManagedCache<DamageRecord> LastHits = new();

        public static DamageRecord GetLastHit(Character character)
        {
            if(LastHits.TryGet(character, out DamageRecord lastHit))
            {
                return lastHit;
            }

            return null;
        }

        public static void SetLastHit(Character character, HitData hitData)
        {
            var lastHit = LastHits.GetOrCreate(character);
            lastHit.Hit = hitData;
            
            if (ZNet.instance.IsNotNull())
            {
                lastHit.Timestamp = ZNet.instance.GetTimeSeconds();
            }
        }
    }
}

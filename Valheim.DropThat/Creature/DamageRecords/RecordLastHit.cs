using System.Runtime.CompilerServices;

namespace Valheim.DropThat.Creature.DamageRecords
{
    /// <summary>
    /// Local cache of last hit data.
    /// </summary>
    public static class RecordLastHit
    {
        private static ConditionalWeakTable<Character, DamageRecord> LastHits = new();

        public static DamageRecord? GetLastHit(Character character)
        {
            if(LastHits.TryGetValue(character, out DamageRecord lastHit))
            {
                return lastHit;
            }

            return null;
        }

        public static void SetLastHit(Character character, HitData hitData)
        {
            var lastHit = LastHits.GetOrCreateValue(character);
            lastHit.Hit = hitData;
        }
    }
}

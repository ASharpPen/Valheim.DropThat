using DropThat.Utilities;
using ThatCore.Cache;

namespace DropThat.Creature.DamageRecords;

/// <summary>
/// Local cache of last hit data.
/// </summary>
public static class RecordLastHit
{
    private static ManagedCache<DamageRecord> LastHits = new();

    public static DamageRecord GetLastHit(Character character)
    {
        if (character.IsNull())
        {
            return null;
        }

        if(LastHits.TryGet(character, out DamageRecord lastHit))
        {
            return lastHit;
        }

        return null;
    }

    public static void SetLastHit(Character character, HitData hitData)
    {
        if (character.IsNull())
        {
            return;
        }

        var lastHit = LastHits.GetOrCreate(character);
        lastHit.Hit = hitData;
        
        if (ZNet.instance.IsNotNull())
        {
            lastHit.Timestamp = ZNet.instance.GetTimeSeconds();
        }
    }
}

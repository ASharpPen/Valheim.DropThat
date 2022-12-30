using System.Linq;
using ThatCore.Cache;

namespace DropThat.Creature.StatusRecords;

public static class RecordLastStatus
{
    private static ManagedCache<StatusRecord> LastStatus = new();

    public static StatusRecord GetLastStatus(Character character)
    {
        if (LastStatus.TryGet(character, out StatusRecord cached))
        {
            return cached;
        }

        return null;
    }

    public static void SetLastStatus(Character character)
    {
        var statusRecord = LastStatus.GetOrCreate(character);
        statusRecord.Statuses = character
            .GetSEMan()?
            .GetStatusEffects()?
            .Select(x => x.name)?
            .Where(x => 
                x != null && 
                x.Length > 0)?
            .ToList() ?? new();
    }
}

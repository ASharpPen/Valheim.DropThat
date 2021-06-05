using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Valheim.DropThat.Creature.StatusRecords
{
    public static class RecordLastStatus
    {
        private static ConditionalWeakTable<Character, StatusRecord> LastStatus = new();

        public static StatusRecord? GetLastStatus(Character character)
        {
            if (LastStatus.TryGetValue(character, out StatusRecord cached))
            {
                return cached;
            }

            return null;
        }

        public static void SetLastStatus(Character character)
        {
            var statusRecord = LastStatus.GetOrCreateValue(character);
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
}

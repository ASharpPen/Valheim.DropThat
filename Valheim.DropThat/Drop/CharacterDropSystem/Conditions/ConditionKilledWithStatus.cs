using HarmonyLib;
using System.Linq;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;
using Valheim.DropThat.Creature.StatusRecords;
using Valheim.DropThat.Drop.CharacterDropSystem.Caches;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Drop.CharacterDropSystem.Conditions
{
    public class ConditionKilledWithStatus : ICondition
    {
        private static ConditionKilledWithStatus _instance;

        public static ConditionKilledWithStatus Instance => _instance ??= new();

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop)
        {
            if (!characterDrop || characterDrop is null || extended?.Config is null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(extended.Config.ConditionKilledWithStatus?.Value))
            {
                return false;
            }

            var character = CharacterCache.GetCharacter(characterDrop);

            if (IsValid(drop, extended.Config, character))
            {
                return false;
            }

            return true;
        }

        public bool IsValid(CharacterDrop.Drop drop, CharacterDropItemConfiguration config, Character character)
        {
            if (config.ConditionKilledWithStatus.Value.Length > 0)
            {
                var statuses = config.ConditionKilledWithStatus.Value.SplitByComma();

                if (statuses.Count == 0)
                {
                    //Skip if we have no states to check. This indicates all are allowed.
                    return true;
                }

                var lastStatusRecord = RecordLastStatus.GetLastStatus(character);

                if (lastStatusRecord is null)
                {
                    Log.LogTrace($"{nameof(ConditionKilledWithStatus)}: Disabling drop {drop.m_prefab.name} due to not finding any last status data.");
                    return false;
                }

#if DEBUG
                Log.LogTrace($"Looking for statuses '{config.ConditionKilledWithStatus}' among '{lastStatusRecord.Statuses.Join()}'");
#endif

                if (statuses.Any(x => lastStatusRecord.HasStatus(x)))
                {
                    return true;
                }

                Log.LogTrace($"{nameof(ConditionKilledWithStatus)}: Disabling drop {drop.m_prefab.name} due to not finding any of the required statuses in last hit.");
                return false;
            }

            return true;
        }
    }
}

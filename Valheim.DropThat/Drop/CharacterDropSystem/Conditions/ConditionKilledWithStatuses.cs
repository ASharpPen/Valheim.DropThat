using System.Linq;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;
using Valheim.DropThat.Creature.StatusRecords;
using Valheim.DropThat.Drop.CharacterDropSystem.Conditions;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Drop.Conditions
{
    public class ConditionKilledWithStatuses : ICondition
    {
        private static ConditionKilledWithStatuses _instance;

        public static ConditionKilledWithStatuses Instance
        {
            get
            {
                return _instance ??= new ConditionKilledWithStatuses();
            }
        }

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop)
        {
            if (!characterDrop || characterDrop is null || extended?.Config is null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(extended.Config.ConditionKilledWithStatuses?.Value))
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
            if (config.ConditionKilledWithStatuses.Value.Length > 0)
            {
                var statuses = config.ConditionKilledWithStatuses.Value.SplitByComma();

                if (statuses.Count == 0)
                {
                    //Skip if we have no states to check. This indicates all are allowed.
                    return true;
                }

                var lastStatusRecord = RecordLastStatus.GetLastStatus(character);

                if (lastStatusRecord is null)
                {
                    Log.LogTrace($"{nameof(ConditionKilledWithStatuses)}: Disabling drop {drop.m_prefab.name} due to not finding any last status data.");
                    return false;
                }

                if (statuses.All(x => lastStatusRecord.HasStatus(x)))
                {
                    return true;
                }

                Log.LogTrace($"{nameof(ConditionKilledWithStatuses)}: Disabling drop {drop.m_prefab.name} due to not finding the required statuses in last hit.");
                return false;
            }

            return true;
        }
    }
}

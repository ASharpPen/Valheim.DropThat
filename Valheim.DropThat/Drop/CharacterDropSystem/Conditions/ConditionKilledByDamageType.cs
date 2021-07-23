using System;
using System.Collections.Generic;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;
using Valheim.DropThat.Creature.DamageRecords;
using Valheim.DropThat.Drop.CharacterDropSystem.Conditions;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Drop.Conditions
{
    public class ConditionKilledByDamageType : ICondition
    {
        private static ConditionKilledByDamageType _instance;

        public static ConditionKilledByDamageType Instance
        {
            get
            {
                return _instance ??= new ConditionKilledByDamageType();
            }
        }

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended dropExtended, CharacterDrop characterDrop)
        {
            if (!characterDrop || characterDrop is null || dropExtended?.Config is null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(dropExtended.Config.ConditionKilledByDamageType?.Value))
            {
                return false;
            }

            var character = CharacterCache.GetCharacter(characterDrop);

            if (ValidConditionKilledByDamageType(drop, dropExtended.Config, character))
            {
                return false;
            }

            return true;
        }

        public bool ValidConditionKilledByDamageType(CharacterDrop.Drop drop, CharacterDropItemConfiguration config, Character character)
        {
            if (config.ConditionKilledByDamageType.Value.Length > 0)
            {
                var causes = config.ConditionKilledByDamageType.Value.SplitByComma();

                if (causes.Count == 0)
                {
                    //Skip if we have no states to check. This indicates all are allowed.
                    return true;
                }

                var lastHit = RecordLastHit.GetLastHit(character);

                if (lastHit is null)
                {
                    Log.LogTrace($"{nameof(config.ConditionKilledByDamageType)}: Disabling drop {drop.m_prefab.name} due to not finding any last hit data.");
                    return false;
                }

                var causesDamageType = ConvertToBitmask(causes);

#if DEBUG
                Log.LogTrace($"Searching for damage types '{causes}' as {causesDamageType} among '{lastHit.DamageType}' with result '{(causesDamageType & lastHit.DamageType)}'");
#endif

                if ((causesDamageType & lastHit.DamageType) == 0)
                {
                    Log.LogTrace($"{nameof(config.ConditionKilledByDamageType)}: Disabling drop {drop.m_prefab.name} due to not finding any of the required damage types in last hit.");
                    return false;
                }
            }

            return true;
        }

        private static HitData.DamageType ConvertToBitmask(List<string> damageTypes)
        {
            HitData.DamageType result = 0;

            foreach(var type in damageTypes)
            {
                if(Enum.TryParse(type, true, out HitData.DamageType damageType))
                {
                    result |= damageType;
                }
            }

            return result;
        }
    }
}

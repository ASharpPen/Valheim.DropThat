using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;
using Valheim.DropThat.Creature.DamageRecords;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Drop.CharacterDropSystem.Conditions
{
    public class ConditionKilledByEntityType : ICondition
    {
        private static ConditionKilledByEntityType _instance;

        public static ConditionKilledByEntityType Instance => _instance ??= new();

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop)
        {
            if (!characterDrop || characterDrop is null || extended?.Config is null)
            {
                return false;
            }

            var character = CharacterCache.GetCharacter(characterDrop);

            if (IsValid(character, extended.Config))
            {
                return false;
            }

            Log.LogTrace($"Filtered drop '{drop.m_prefab.name}' due not being killed by required entity type.");
            return true;
        }

        public bool IsValid(Character character, CharacterDropItemConfiguration config)
        {
            if (character is null)
            {
#if DEBUG
                Log.LogTrace("[ConditionKilledByEntityType] No character found.");
#endif
                return true;
            }

            if (string.IsNullOrWhiteSpace(config.ConditionKilledByEntityType.Value))
            {
#if DEBUG
                Log.LogTrace("[ConditionKilledByEntityType] No setting found.");
#endif
                return true;
            }

            var requiredEntityTypes = ConvertToEnum(config.ConditionKilledByEntityType.Value.SplitByComma());

            if (requiredEntityTypes.Count == 0)
            {
#if DEBUG
                Log.LogTrace("[ConditionKilledByEntityType] No valid requirements found.");
#endif

                return true;
            }

            var lastHit = RecordLastHit.GetLastHit(character);

            if (lastHit is null)
            {
#if DEBUG
                Log.LogTrace($"{nameof(config.ConditionKilledByDamageType)}: Disabling drop {config.SectionKey} due to not finding any last hit data.");
#endif
                return false;
            }

            EntityType killedBy = EntityType.Other;

            if (lastHit.Hit.HaveAttacker())
            {
                GameObject attacker = ZNetScene.instance.FindInstance(lastHit.Hit.m_attacker);

                var attackerCharacter = ComponentCache.GetComponent<Character>(attacker);

                if (attackerCharacter is not null)
                {
                    if (attackerCharacter.IsPlayer())
                    {
                        killedBy = EntityType.Player;
                    }
                    else
                    {
                        killedBy = EntityType.Creature;
                    }
                }
            }

#if DEBUG
            Log.LogTrace($"[ConditionKilledByEntityType] Killed by '{killedBy}'");
#endif


            return requiredEntityTypes.Any(x => x == killedBy);
        }

        private static List<EntityType> ConvertToEnum(List<string> entityTypes)
        {
            var result = new List<EntityType>(entityTypes.Count);

            foreach (var type in entityTypes)
            {
                if (Enum.TryParse(type, true, out EntityType entity))
                {
                    result.Add(entity);
                }
            }

            return result;
        }

        public enum EntityType
        {
            Player,
            Creature,
            Other
        }
    }
}

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;
using Valheim.DropThat.Creature.DamageRecords;
using Valheim.DropThat.Drop.CharacterDropSystem.Conditions;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Drop.Conditions
{
    public class ConditionKilledBySkillType : ICondition
    {
        private static ConditionKilledBySkillType _instance;

        public static ConditionKilledBySkillType Instance
        {
            get
            {
                return _instance ??= new ConditionKilledBySkillType();
            }
        }

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop)
        {
            if (!characterDrop || characterDrop is null || extended?.Config is null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(extended.Config.ConditionKilledBySkillType?.Value))
            {
                return false;
            }

            var character = CharacterCache.GetCharacter(characterDrop);

            if (ValidConditionKilledBySkillType(drop, extended.Config, character))
            {
                return false;
            }

            return true;
        }

        public bool ValidConditionKilledBySkillType(CharacterDrop.Drop drop, DropItemConfiguration config, Character character)
        {
            if (config.ConditionKilledBySkillType.Value.Length > 0)
            {
                var skillTypes = config.ConditionKilledBySkillType.Value.SplitByComma();

                if (skillTypes.Count == 0)
                {
                    //Skip if we have no states to check. This indicates all are allowed.
                    return true;
                }

                var lastHit = RecordLastHit.GetLastHit(character);

                if (lastHit is null)
                {
                    Log.LogTrace($"{nameof(ConditionKilledBySkillType)}: Disabling drop {drop.m_prefab.name} due to not finding any last hit data.");
                    return false;
                }

                var convertedSkills = ConvertToSkillType(skillTypes);

#if DEBUG
                Log.LogTrace($"Searching for skill type '{lastHit.SkillType}' among required types '{config.ConditionKilledBySkillType.Value}'");
#endif

                if (!convertedSkills.Any(x => x == lastHit.SkillType))
                {
                    Log.LogTrace($"{nameof(ConditionKilledBySkillType)}: Disabling drop {drop.m_prefab.name} due to not finding any of the required skill types in last hit.");
                    return false;
                }
            }

            return true;
        }

        private static List<Skills.SkillType> ConvertToSkillType(List<string> skillTypes)
        {
            List<Skills.SkillType> results = new();

            foreach (var type in skillTypes)
            {
                if (Enum.TryParse(type, true, out Skills.SkillType skillType))
                {
                    results.Add(skillType);
                }
                else
                {
                    Log.LogWarning($"[{nameof(ConditionKilledBySkillType)}]: Unable to parse skill '{type}'");
                }
            }

            return results;
        }
    }
}

using CreatureLevelControl;
using System;
using System.Linq;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;
using Valheim.DropThat.Core.Configuration;
using Valheim.DropThat.Drop.Conditions;

namespace Valheim.DropThat.Conditions.ModSpecific.CLLC
{
    internal class ConditionInfusion : ICondition
    {
        private static ConditionInfusion _instance;

        public static ConditionInfusion Instance
        {
            get
            {
                return _instance ??= new ConditionInfusion();
            }
        }

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop)
        {
            if (extended?.Config?.Subsections is null)
            {
                return false;
            }

            var character = CharacterCache.GetCharacter(characterDrop);

            if (extended.Config.Subsections.TryGetValue(DropModConfigCLLC.ModName, out Config config) && config is DropModConfigCLLC cllcConfig)
            {
                if (!ValidConditionInfusion(drop, cllcConfig, character))
                {
                    return true;
                }

                if(!ValidConditionNotInfusion(drop, cllcConfig, character))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ValidConditionInfusion(CharacterDrop.Drop drop, DropModConfigCLLC config, Character character)
        {
            if (config.ConditionInfusion.Value.Length > 0)
            {
                var states = config.ConditionInfusion.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if ((states?.Length ?? 0) == 0)
                {
#if DEBUG
                    Log.LogDebug("No conditions for CLLC infusions were found.");
#endif
                    //Skip if we have no states to check. This indicates all are allowed.
                    return true;
                }

                if (!states.Any(x => HasState(character, x)))
                {
                    Log.LogTrace($"{nameof(config.ConditionInfusion)}: Disabling drop {drop.m_prefab.name} due to not finding any of the requires infusions '{config.ConditionInfusion.Value}'.");
                    return false;
                }
            }

            return true;
        }

        public static bool ValidConditionNotInfusion(CharacterDrop.Drop drop, DropModConfigCLLC config, Character character)
        {
            if (config.ConditionNotInfusion.Value.Length > 0)
            {
                var states = config.ConditionNotInfusion.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if ((states?.Length ?? 0) == 0)
                {
#if DEBUG
                    Log.LogDebug("No conditions for CLLC infusions were found.");
#endif
                    //Skip if we have no states to check. This indicates all are allowed.
                    return true;
                }

                if (states.Any(x => HasState(character, x)))
                {
                    Log.LogTrace($"{nameof(config.ConditionNotInfusion)}: Disabling drop {drop.m_prefab.name} due finding one of the disabled infusions '{config.ConditionNotInfusion.Value}'.");
                    return false;
                }
            }

            return true;
        }

        private static bool HasState(Character character, string state)
        {
            if (Enum.TryParse(state.Trim(), true, out CreatureInfusion infusion))
            {
                return CreatureLevelControl.API.GetInfusionCreature(character) == infusion;
            }
            else
            {
                Log.LogWarning($"Unable to parse CLLC infusion '{state}'");
                return false;
            }
        }
    }
}

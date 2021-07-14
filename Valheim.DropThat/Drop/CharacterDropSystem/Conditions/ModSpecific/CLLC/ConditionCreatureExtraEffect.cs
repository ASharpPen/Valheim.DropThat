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
    internal class ConditionCreatureExtraEffect : ICondition
    {
        private static ConditionCreatureExtraEffect _instance;

        public static ConditionCreatureExtraEffect Instance
        {
            get
            {
                return _instance ??= new ConditionCreatureExtraEffect();
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
                if (!ValidConditionExtraEffect(drop, cllcConfig, character))
                {
                    return true;
                }

                if (!ValidConditionNotExtraEffect(drop, cllcConfig, character))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ValidConditionExtraEffect(CharacterDrop.Drop drop, DropModConfigCLLC config, Character character)
        {
            if (config.ConditionExtraEffect.Value.Length > 0)
            {
                var states = config.ConditionExtraEffect.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if ((states?.Length ?? 0) == 0)
                {
#if DEBUG
                    Log.LogDebug("No conditions for CLLC creature extra effects were found.");
#endif
                    //Skip if we have no states to check. This indicates all are allowed.
                    return true;
                }

                if (!states.Any(x => HasState(character, x)))
                {
                    Log.LogTrace($"{nameof(config.ConditionExtraEffect)}: Disabling drop {drop.m_prefab.name} due to not finding any of the requires extra creature effects '{config.ConditionExtraEffect.Value}'.");
                    return false;
                }
            }

            return true;
        }

        public static bool ValidConditionNotExtraEffect(CharacterDrop.Drop drop, DropModConfigCLLC config, Character character)
        {
            if (config.ConditionNotExtraEffect.Value.Length > 0)
            {
                var states = config.ConditionNotExtraEffect.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if ((states?.Length ?? 0) == 0)
                {
#if DEBUG
                    Log.LogDebug("No conditions for CLLC not creature extra effects were found.");
#endif
                    //Skip if we have no states to check. This indicates all are allowed.
                    return true;
                }

                if (states.Any(x => HasState(character, x)))
                {
                    Log.LogTrace($"{nameof(config.ConditionNotExtraEffect)}: Disabling drop {drop.m_prefab.name} due finding one of the disabled creature extra effects '{config.ConditionNotExtraEffect.Value}'.");
                    return false;
                }
            }

            return true;
        }

        private static bool HasState(Character character, string state)
        {
            if (Enum.TryParse(state.Trim(), true, out CreatureExtraEffect infusion))
            {
                return CreatureLevelControl.API.GetExtraEffectCreature(character) == infusion;
            }
            else
            {
                Log.LogWarning($"Unable to parse CLLC creature extra effect '{state}'");
                return false;
            }
        }
    }
}

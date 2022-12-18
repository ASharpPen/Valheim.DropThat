using System;
using System.Linq;
using DropThat.Caches;
using DropThat.Core;
using DropThat.Drop.CharacterDropSystem.Caches;
using DropThat.Utilities;

namespace DropThat.Drop.CharacterDropSystem.Conditions
{
    internal class ConditionCreatureState : ICondition
    {
        private static ConditionCreatureState _instance;

        public static ConditionCreatureState Instance
        {
            get
            {
                return _instance ??= new ConditionCreatureState();
            }
        }

        private enum CreatureState
        {
            DEFAULT,
            TAMED,
            EVENT,
        }

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop)
        {
            var character = CharacterCache.GetCharacter(characterDrop);

            if (!ValidConditionCreatureStates(drop, extended, character))
            {
                return true;
            }

            if (!ValidConditionNotCreatureStates(drop, extended, character))
            {
                return true;
            }

            return false;
        }

        public static bool ValidConditionCreatureStates(CharacterDrop.Drop drop, DropExtended extended, Character character)
        {
            if (extended.Config.ConditionCreatureStates.Value.Length > 0)
            {
                var states = extended.Config.ConditionCreatureStates.Value.SplitByComma();

                if (states.Count == 0)
                {
#if DEBUG
                    Log.LogDebug("No conditions for creature state were found.");
#endif
                    //Skip if we have no states to check. This indicates all are allowed.
                    return true;
                }

                if (!states.Any(x => HasState(character, x)))
                {
                    Log.LogTrace($"{nameof(extended.Config.ConditionCreatureStates)}: Disabling drop {drop.m_prefab.name} due to not finding any of the requires creature states '{extended.Config.ConditionCreatureStates.Value}'.");
                    return false;
                }
            }

            return true;
        }

        public static bool ValidConditionNotCreatureStates(CharacterDrop.Drop drop, DropExtended extended, Character character)
        {
            if (extended.Config.ConditionNotCreatureStates.Value.Length > 0)
            {
                var states = extended.Config.ConditionNotCreatureStates.Value.SplitByComma();

                if (states.Count == 0)
                {
#if DEBUG
                    Log.LogDebug("No conditions for not having a creature state were found.");
#endif

                    //Skip if we have no states to check. This indicates all are allowed.
                    return true;
                }

                if (states.Any(x => HasState(character, x)))
                {
                    Log.LogTrace($"{nameof(extended.Config.ConditionNotCreatureStates)}: Disabling drop {drop.m_prefab.name} due to forbidden creature state.");
                    return false;
                }
            }

            return true;
        }

        private static bool HasState(Character character, string state)
        {
            if (Enum.TryParse(state.Trim().ToUpperInvariant(), out CreatureState creatureState))
            {
                switch (creatureState)
                {
                    case CreatureState.TAMED:
                        return character.IsTamed();
                    case CreatureState.EVENT:
                        MonsterAI ai = CharacterCache.GetMonsterAI(character);

                        if (ai is null)
                        {
                            return false;
                        }

                        return ai.IsEventCreature();
                    default:
                        return true;
                }
            }
            else
            {
                Log.LogWarning($"Unable to parse creature state {state}");
                return false;
            }
        }
    }
}

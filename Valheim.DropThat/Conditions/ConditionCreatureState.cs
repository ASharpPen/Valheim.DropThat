using System;
using Valheim.DropThat.Caches;
using Valheim.DropThat.ConfigurationCore;
using Valheim.DropThat.ConfigurationTypes;

namespace Valheim.DropThat.Conditions
{
    internal static class ConditionCreatureState
    {
        private enum CreatureState
        {
            DEFAULT,
            TAMED,
            EVENT,
        }

        public static bool ConditionCreatureStates(CharacterDrop.Drop drop, DropExtended extended, Character character)
        {
            if(extended.Config.ConditionCreatureStates.Value.Length > 0)
            {
                var states = extended.Config.ConditionCreatureStates.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if((states?.Length?? 0) == 0)
                {
                    //Skip if we have no states to check. This indicates all are allowed.
                    return true;
                }

                foreach(var state in states)
                {
                    if(!HasState(character, state))
                    {
                        Log.LogTrace($"{nameof(extended.Config.ConditionCreatureStates)}: Disabling drop {drop.m_prefab.name} due to not finding any of the requires creature states '{extended.Config.ConditionCreatureStates.Value}'.");
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool ConditionNotCreatureStates(CharacterDrop.Drop drop, DropExtended extended, Character character)
        {
            if (extended.Config.ConditionNotCreatureStates.Value.Length > 0)
            {
                var states = extended.Config.ConditionNotCreatureStates.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if ((states?.Length ?? 0) == 0)
                {
                    //Skip if we have no states to check. This indicates all are allowed.
                    return true;
                }

                foreach (var state in states)
                {
                    if (HasState(character, state))
                    {
                        Log.LogTrace($"{nameof(extended.Config.ConditionNotCreatureStates)}: Disabling drop {drop.m_prefab.name} due to creature state '{state}'.");
                        return false;
                    }
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
                        MonsterAI ai = CharacterExtended.GetMonsterAI(character);

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

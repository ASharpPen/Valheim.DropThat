using System;
using DropThat.Caches;
using DropThat.Core;
using DropThat.Drop.CharacterDropSystem.Caches;
using DropThat.Utilities;

namespace DropThat.Drop.CharacterDropSystem.Conditions
{
    public class ConditionNotFaction : ICondition
    {
        private static ConditionNotFaction _instance;

        public static ConditionNotFaction Instance => _instance ??= new();

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop)
        {
            if (drop is null || extended is null || !characterDrop || characterDrop is null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(extended.Config.ConditionNotFaction.Value))
            {
                return false;
            }

            var character = CharacterCache.GetCharacter(characterDrop);

            if (!character || character is null)
            {
                return false;
            }

            var characterFaction = character.GetFaction();

            var requiredNotFactions = extended.Config.ConditionNotFaction.Value.SplitByComma();

            foreach (var requiredNotFaction in requiredNotFactions)
            {
                if (Enum.TryParse(requiredNotFaction, true, out Character.Faction faction))
                {
                    if (characterFaction == faction)
                    {
                        Log.LogTrace($"{nameof(extended.Config.ConditionNotFaction)}: Disabling drop {drop.m_prefab.name} due to having faction {characterFaction}.");
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

using System;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Conditions;
using Valheim.DropThat.Core;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Drop.Conditions
{
    public class ConditionFaction : ICondition
    {
        private static ConditionFaction _instance;

        public static ConditionFaction Instance
        {
            get
            {
                return _instance ??= new ConditionFaction();
            }
        }

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop)
        {
            if(drop is null || extended is null || !characterDrop || characterDrop is null)
            {
                return false;
            }

            if(string.IsNullOrEmpty(extended.Config.ConditionFaction.Value))
            {
                return false;
            }

            var character = CharacterCache.GetCharacter(characterDrop);

            if(!character || character is null)
            {
                return false;
            }

            var characterFaction = character.GetFaction();

            var requiredFactions = extended.Config.ConditionFaction.Value.SplitByComma();

            foreach (var requiredFaction in requiredFactions)
            {
                if(Enum.TryParse(requiredFaction, out Character.Faction faction))
                {
                    if(characterFaction == faction)
                    {
                        return false;
                    }
                }
            }

            Log.LogTrace($"{nameof(extended.Config.ConditionFaction)}: Disabling drop {drop.m_prefab.name} due to {characterFaction} not being in required list.");
            return true;
        }
    }
}

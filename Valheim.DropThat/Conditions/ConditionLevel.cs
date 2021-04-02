using Valheim.DropThat.Caches;
using Valheim.DropThat.ConfigurationCore;
using Valheim.DropThat.ConfigurationTypes;

namespace Valheim.DropThat.Conditions
{
    internal class ConditionLevel : ICondition
    {
        private static ConditionLevel _instance;

        public static ConditionLevel Instance
        {
            get
            {
                return _instance ??= new ConditionLevel();
            }
        }

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended dropExtended, CharacterDrop characterDrop)
        {
            var character = CharacterCache.GetCharacter(characterDrop);

            int minLevel = dropExtended.Config.ConditionMinLevel.Value;
            if (minLevel >= 0 && character is not null)
            {
                if (character.GetLevel() < minLevel)
                {
                    Log.LogTrace($"{nameof(dropExtended.Config.ConditionMinLevel)}: Disabling drop {drop.m_prefab.name} due to level {character.GetLevel()} being below limit {minLevel}.");

                    return true;
                }
            }

            int maxLevel = dropExtended.Config.ConditionMaxLevel.Value;
            if (maxLevel >= 0 && character is not null)
            {
                if (character.GetLevel() > maxLevel)
                {
                    Log.LogTrace($"{nameof(dropExtended.Config.ConditionMaxLevel)}: Disabling drop {drop.m_prefab.name} due to level {character.GetLevel()} being above limit {maxLevel}.");

                    return true;
                }
            }

            return false;
        }
    }
}

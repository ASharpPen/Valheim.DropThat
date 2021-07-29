using CreatureLevelControl;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;
using Valheim.DropThat.Drop.CharacterDropSystem.Caches;

namespace Valheim.DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC
{
    public class ConditionWorldLevel : ICondition
    {
        private static ConditionWorldLevel _instance;

        public static ConditionWorldLevel Instance => _instance ??= new();

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop)
        {
            if (IsValid(extended?.Config))
            {
                return false;
            }

            Log.LogTrace($"Filtered drop '{drop}' due to not being within required CLLC world level.");
            return true;
        }

        public bool IsValid(CharacterDropItemConfiguration itemConfig)
        {
            if (itemConfig is null)
            {
                return true;
            }

            if (itemConfig.TryGet(CharacterDropModConfigCLLC.ModName, out var modConfig) && modConfig is CharacterDropModConfigCLLC config)
            {
                int worldLevel = API.GetWorldLevel();
                
                if (worldLevel < config.ConditionWorldLevelMin)
                {
                    return false;
                }

                if (config.ConditionWorldLevelMax > 0 && worldLevel > config.ConditionWorldLevelMax)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

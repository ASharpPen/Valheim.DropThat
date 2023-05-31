using CreatureLevelControl;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Drop.DropTableSystem.Conditions.ModSpecific.ModCLLC
{
    public class ConditionWorldLevel : IDropTableCondition
    {
        private static ConditionWorldLevel _instance;

        public static ConditionWorldLevel Instance => _instance ??= new();

        public bool ShouldFilter(DropSourceTemplateLink context, DropTemplate template)
        {
            if (IsValid(template?.Config))
            {
                return false;
            }

            Log.LogTrace($"Filtered drop '{template.Drop.m_item.name}' due to not being within required CLLC world level.");
            return true;
        }

        public bool IsValid(DropTableItemConfiguration itemConfig)
        {
            if (itemConfig is null)
            {
                return true;
            }

            if (itemConfig.TryGet(DropTableModConfigCLLC.ModName, out var modConfig) && modConfig is DropTableModConfigCLLC config)
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

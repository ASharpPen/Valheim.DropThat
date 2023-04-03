using CreatureLevelControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropThat.Configuration.ConfigTypes;
using DropThat.Core;

namespace DropThat.Drop.DropTableSystem.Conditions.ModSpecific.ModCLLC;

public class OldConditionWorldLevel : IDropTableCondition
{
    private static OldConditionWorldLevel _instance;

    public static OldConditionWorldLevel Instance => _instance ??= new();

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

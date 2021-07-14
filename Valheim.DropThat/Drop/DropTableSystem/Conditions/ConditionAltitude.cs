using UnityEngine;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Drop.DropTableSystem.Conditions
{
    public class ConditionAltitude : IDropTableCondition
    { 
        private static ConditionAltitude _instance;

        public static ConditionAltitude Instance => _instance ??= new();

        public bool ShouldFilter(DropSourceTemplateLink context, DropTemplate template)
        {
            if (IsValid(context.Source.transform.position, template?.Config))
            {
                return false;
            }

            Log.LogTrace($"Filtered drop '{template.Drop.m_item.name}' due being outside required altitude.");
            return true;
        }

        public bool IsValid(Vector3 position, DropTableItemConfiguration config)
        {
            if (config is null)
            {
                return true;
            }

            var altitude = position.y - ZoneSystem.instance.m_waterLevel;

            if (altitude < config.ConditionAltitudeMin)
            {
                return false;
            }

            if (altitude > config.ConditionAltitudeMax)
            {
                return false;
            }

            return true;
        }
    }
}

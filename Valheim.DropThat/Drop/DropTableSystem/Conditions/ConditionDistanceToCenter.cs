using UnityEngine;
using DropThat.Configuration.ConfigTypes;
using DropThat.Core;

namespace DropThat.Drop.DropTableSystem.Conditions
{
    public class ConditionDistanceToCenter : IDropTableCondition
    {
        private static ConditionDistanceToCenter _instance;

        public static ConditionDistanceToCenter Instance => _instance ??= new();

        public bool ShouldFilter(DropSourceTemplateLink context, DropTemplate template)
        {
            if (IsValid(context.Source.transform.position, template?.Config))
            {
                return false;
            }

            Log.LogTrace($"Filtered drop '{template.Drop.m_item.name}' due not being within required distance to center of map.");
            return true;
        }

        public bool IsValid(Vector3 position, DropTableItemConfiguration config)
        {
            if (config is null)
            {
                return true;
            }

            var distance = position.magnitude;

            if (distance < config.ConditionDistanceToCenterMin.Value)
            {
                return false;
            }

            if (config.ConditionDistanceToCenterMax.Value > 0 && distance > config.ConditionDistanceToCenterMax.Value)
            {
                return false;
            }

            return true;
        }
    }
}

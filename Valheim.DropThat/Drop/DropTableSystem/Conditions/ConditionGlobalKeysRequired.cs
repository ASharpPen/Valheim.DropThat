using System.Linq;
using DropThat.Configuration.ConfigTypes;
using DropThat.Core;
using DropThat.Utilities;

namespace DropThat.Drop.DropTableSystem.Conditions
{
    public class ConditionGlobalKeysRequired : IDropTableCondition
    {
        private static ConditionGlobalKeysRequired _instance;

        public static ConditionGlobalKeysRequired Instance => _instance ??= new();

        public bool ShouldFilter(DropSourceTemplateLink context, DropTemplate template)
        {
            if (IsValid(template.Config))
            {
                return false;
            }

            Log.LogTrace($"Filtered drop '{template.Drop.m_item.name}' due to not finding required global key.");
            return true;
        }

        public bool IsValid(DropTableItemConfiguration config)
        {
            if (string.IsNullOrEmpty(config.ConditionGlobalKeys.Value))
            {
                return true;
            }

            var requiredKeys = config.ConditionGlobalKeys.Value.SplitByComma();

            if (requiredKeys.Count == 0)
            {
                return true;
            }

            if (requiredKeys.Any(x => ZoneSystem.instance.GetGlobalKey(x)))
            {
                return true;
            }

            return false;
        }
    }
}

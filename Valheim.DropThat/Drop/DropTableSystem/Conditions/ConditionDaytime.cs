using DropThat.Configuration.ConfigTypes;
using DropThat.Core;

namespace DropThat.Drop.DropTableSystem.Conditions
{
    public class ConditionDaytime : IDropTableCondition
    {
        private static ConditionDaytime _instance;

        public static ConditionDaytime Instance => _instance ??= new();

        public bool ShouldFilter(DropSourceTemplateLink context, DropTemplate template)
        {
            if (IsValid(template?.Config))
            {
                return false;
            }

            Log.LogTrace($"Filtered drop '{template.Drop.m_item.name}' due being outside allowed time of day.");
            return true;
        }

        public bool IsValid(DropTableItemConfiguration config)
        {
            if (config is null)
            {
                return true;
            }

            var envMan = EnvMan.instance;

            if (config.ConditionNotDay && envMan.IsDay())
            {
                return false;
            }

            if (config.ConditionNotAfternoon && envMan.IsAfternoon())
            {
                return false;
            }

            if (config.ConditionNotNight && envMan.IsNight())
            {
                return false;
            }

            return true;
        }
    }
}

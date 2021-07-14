using Valheim.DropThat.Caches;
using Valheim.DropThat.Core;
using Valheim.DropThat.Drop.Conditions;

namespace Valheim.DropThat.Conditions
{
    internal class ConditionDaytime : ICondition
    {
        private static ConditionDaytime _instance;

        public static ConditionDaytime Instance
        {
            get
            {
                return _instance ??= new ConditionDaytime();
            }
        }

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended dropExtended, CharacterDrop characterDrop)
        {
            var envMan = EnvMan.instance;

            if (dropExtended.Config.ConditionNotDay.Value && envMan.IsDay())
            {
                Log.LogTrace($"{nameof(dropExtended.Config.ConditionNotDay)}: Disabling drop {drop.m_prefab.name} due to time of day.");

                return true;
            }

            if (dropExtended.Config.ConditionNotAfternoon.Value && envMan.IsAfternoon())
            {
                Log.LogTrace($"{nameof(dropExtended.Config.ConditionNotAfternoon)}: Disabling drop {drop.m_prefab.name} due to time of day.");

                return true;
            }

            if (dropExtended.Config.ConditionNotNight.Value && envMan.IsNight())
            {
                Log.LogTrace($"{nameof(dropExtended.Config.ConditionNotNight)}: Disabling drop {drop.m_prefab.name} due to time of day.");

                return true;
            }

            return false;
        }
    }
}

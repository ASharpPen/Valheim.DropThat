using System.Collections.Generic;
using Valheim.DropThat.Core;
using Valheim.DropThat.Drop.CharacterDropSystem.Caches;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Drop.CharacterDropSystem.Conditions
{
    internal class ConditionEnvironments : ICondition
    {
        private static ConditionEnvironments _instance;

        public static ConditionEnvironments Instance => _instance ??= new();

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended dropExtended, CharacterDrop characterDrop)
        {
            if (!string.IsNullOrEmpty(dropExtended.Config.ConditionEnvironments.Value))
            {
                var envMan = EnvMan.instance;
                var currentEnv = envMan.GetCurrentEnvironment();

                var environments = dropExtended.Config.ConditionEnvironments.Value.SplitByComma(true);

                if (environments.Count > 0)
                {
                    var requiredSet = new HashSet<string>(environments);

                    if (!requiredSet.Contains(currentEnv.m_name.Trim().ToUpperInvariant()))
                    {
                        Log.LogTrace($"{nameof(dropExtended.Config.ConditionEnvironments)}: Disabling drop {drop.m_prefab.name} due to environment {currentEnv.m_name} not being in required list.");

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
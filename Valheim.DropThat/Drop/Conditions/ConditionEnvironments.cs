using System.Collections.Generic;
using System.Linq;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Core;
using Valheim.DropThat.Drop.Conditions;

namespace Valheim.DropThat.Conditions
{
    internal class ConditionEnvironments : ICondition
    {
        private static ConditionEnvironments _instance;

        public static ConditionEnvironments Instance
        {
            get
            {
                return _instance ??= new ConditionEnvironments();
            }
        }

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended dropExtended, CharacterDrop characterDrop)
        {
            if (!string.IsNullOrEmpty(dropExtended.Config.ConditionEnvironments.Value))
            {
                var envMan = EnvMan.instance;
                var currentEnv = envMan.GetCurrentEnvironment();

                var environments = dropExtended.Config.ConditionEnvironments.Value.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                if (environments.Length > 0)
                {
                    var requiredSet = new HashSet<string>(environments.Select(x => x.Trim().ToUpperInvariant()));

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
﻿using System.Linq;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Drop.DropTableSystem.Conditions
{
    public class ConditionEnvironments : IDropTableCondition
    {
        private static ConditionEnvironments _instance;

        public static ConditionEnvironments Instance => _instance ??= new();

        public bool ShouldFilter(DropSourceTemplateLink context, DropTemplate template)
        {
            if (IsValid(template.Config))
            {
                return false;
            }

            Log.LogTrace($"Filtered drop '{template.Drop.m_item.name}' due to current environment.");
            return true;
        }

        public bool IsValid(DropTableItemConfiguration config)
        {
            if (string.IsNullOrEmpty(config.ConditionEnvironments.Value))
            {
                return true;
            }

            var envMan = EnvMan.instance;
            var currentEnv = envMan.GetCurrentEnvironment().m_name.Trim().ToUpperInvariant();

            var environments = config.ConditionEnvironments.Value.SplitByComma(toUpper: true);

            if (environments.Count == 0)
            {
                return true;
            }

            if (environments.Any(x => x == currentEnv))
            {
                return true;
            }

            return false;
        }
    }
}

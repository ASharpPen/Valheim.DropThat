using System;
using System.Collections.Generic;
using Valheim.DropThat.ConfigurationCore;
using Valheim.DropThat.ConfigurationTypes;

namespace Valheim.DropThat.Conditions
{
    internal class ConditionGlobalKeys : ICondition
    {
        private static ConditionGlobalKeys _instance;

        public static ConditionGlobalKeys Instance
        {
            get
            {
                return _instance ??= new ConditionGlobalKeys();
            }
        }

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended dropExtended, CharacterDrop characterDrop)
        {
            if (!string.IsNullOrEmpty(dropExtended.Config.ConditionGlobalKeys.Value))
            {
                var requiredKeys = dropExtended.Config.ConditionGlobalKeys.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (requiredKeys.Length > 0)
                {
                    var keySet = new HashSet<string>(ZoneSystem.instance.GetGlobalKeys());

                    bool foundRequiredKey = false;

                    foreach (var key in requiredKeys)
                    {
                        foundRequiredKey = keySet.Contains(key);

                        if (foundRequiredKey)
                        {
                            break;
                        }
                    }

                    if (!foundRequiredKey)
                    {
                        Log.LogTrace($"{nameof(dropExtended.Config.ConditionGlobalKeys)}: Disabling drop {drop.m_prefab.name} due to not finding any of the requires global keys '{dropExtended.Config.ConditionGlobalKeys.Value}'.");

                        return true;
                    }
                }
            }

            return false;
        }
    }
}

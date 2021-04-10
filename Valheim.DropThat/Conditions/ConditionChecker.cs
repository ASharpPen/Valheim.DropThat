using System.Collections.Generic;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Configuration;
using Valheim.DropThat.Reset;

namespace Valheim.DropThat.Conditions
{
    public class ConditionChecker
    {
        private HashSet<ICondition> OnStartConditions;
        private HashSet<ICondition> OnDeathConditions;

        private static ConditionChecker _instance;

        public static ConditionChecker Instance
        {
            get
            {
                return _instance ??= new ConditionChecker();
            }
        }

        ConditionChecker()
        {
            StateResetter.Subscribe(() =>
            {
                _instance = null;
            });

            #region Add OnStart conditions

            OnStartConditions = new HashSet<ICondition>();
            OnStartConditions.Add(ConditionInventory.Instance);

            if (!ConfigurationManager.GeneralConfig.ApplyConditionsOnDeath.Value)
            {
                OnStartConditions.Add(ConditionLevel.Instance);
                OnStartConditions.Add(ConditionDaytime.Instance);
                OnStartConditions.Add(ConditionBiome.Instance);
                OnStartConditions.Add(ConditionEnvironments.Instance);
                OnStartConditions.Add(ConditionGlobalKeys.Instance);
            }
            #endregion

            #region Add OnDeath conditions

            OnDeathConditions = new HashSet<ICondition>();
            OnDeathConditions.Add(ConditionCreatureState.Instance);

            if (ConfigurationManager.GeneralConfig.ApplyConditionsOnDeath.Value)
            {
                OnDeathConditions.Add(ConditionLevel.Instance);
                OnDeathConditions.Add(ConditionDaytime.Instance);
                OnDeathConditions.Add(ConditionBiome.Instance);
                OnDeathConditions.Add(ConditionEnvironments.Instance);
                OnDeathConditions.Add(ConditionGlobalKeys.Instance);
            }

            #endregion
        }

        public static List<CharacterDrop.Drop> FilterOnStart(CharacterDrop characterDrop)
        {
            return Instance.Filter(characterDrop, Instance.OnStartConditions);
        }

        public static List<CharacterDrop.Drop> FilterOnDeath(CharacterDrop characterDrop)
        {
            return Instance.Filter(characterDrop, Instance.OnDeathConditions);
        }

        public List<CharacterDrop.Drop> Filter(CharacterDrop characterDrop, IEnumerable<ICondition> conditions)
        {
            List<CharacterDrop.Drop> validDrops = new List<CharacterDrop.Drop>();

            foreach (var drop in characterDrop.m_drops)
            {
                var dropExtended = DropExtended.GetExtension(drop);

                if (dropExtended is null)
                {
                    validDrops.Add(drop);
                    continue;
                }

                bool filterDrop = false;

                foreach (var condition in conditions)
                {
                    if (condition.ShouldFilter(drop, dropExtended, characterDrop))
                    {
                        filterDrop = true;
                        break;
                    }
                }

                if (!filterDrop)
                {
                    validDrops.Add(drop);
                }
            }

            return validDrops;
        }
    }
}

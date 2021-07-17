using System;
using System.Collections.Generic;
using System.Linq;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Conditions;
using Valheim.DropThat.Conditions.ModSpecific;
using Valheim.DropThat.Configuration;
using Valheim.DropThat.Core;
using Valheim.DropThat.Drop.CharacterDropSystem.Conditions;
using Valheim.DropThat.Drop.Conditions;
using Valheim.DropThat.Drop.Conditions.ModSpecific;
using Valheim.DropThat.Reset;

namespace Valheim.DropThat.Drop.CharacterDropSystem
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

            // Add OnStart conditions

            OnStartConditions = new HashSet<ICondition>();
            OnStartConditions.Add(ConditionInventory.Instance);
            OnStartConditions.Add(ConditionLocation.Instance);

            if (!ConfigurationManager.GeneralConfig.ApplyConditionsOnDeath.Value)
            {
                OnStartConditions.Add(ConditionLevel.Instance);
                OnStartConditions.Add(ConditionDaytime.Instance);
                OnStartConditions.Add(ConditionBiome.Instance);
                OnStartConditions.Add(ConditionEnvironments.Instance);
                OnStartConditions.Add(ConditionGlobalKeys.Instance);

                OnStartConditions.Add(ConditionLoaderCLLC.ConditionBossAffix);
                OnStartConditions.Add(ConditionLoaderCLLC.ConditionInfusion);
                OnStartConditions.Add(ConditionLoaderCLLC.ConditionCreatureExtraEffect);
            }

            // Add OnDeath conditions

            OnDeathConditions = new HashSet<ICondition>();
            OnDeathConditions.Add(ConditionCreatureState.Instance);
            OnDeathConditions.Add(ConditionLoaderSpawnThat.ConditionTemplateId);

            OnDeathConditions.Add(ConditionFaction.Instance);
            OnDeathConditions.Add(ConditionNotFaction.Instance);
            OnDeathConditions.Add(ConditionKilledByDamageType.Instance);
            OnDeathConditions.Add(ConditionKilledBySkillType.Instance);
            OnDeathConditions.Add(ConditionKilledWithStatus.Instance);
            OnDeathConditions.Add(ConditionKilledWithStatuses.Instance);
            OnDeathConditions.Add(ConditionKilledByEntityType.Instance);

            if (ConfigurationManager.GeneralConfig.ApplyConditionsOnDeath.Value)
            {
                OnDeathConditions.Add(ConditionLevel.Instance);
                OnDeathConditions.Add(ConditionDaytime.Instance);
                OnDeathConditions.Add(ConditionBiome.Instance);
                OnDeathConditions.Add(ConditionEnvironments.Instance);
                OnDeathConditions.Add(ConditionGlobalKeys.Instance);

                OnDeathConditions.Add(ConditionLoaderCLLC.ConditionBossAffix);
                OnDeathConditions.Add(ConditionLoaderCLLC.ConditionInfusion);
                OnDeathConditions.Add(ConditionLoaderCLLC.ConditionCreatureExtraEffect);
            }
        }

        public static List<CharacterDrop.Drop> FilterOnStart(CharacterDrop characterDrop)
        {
            try
            {
                return Instance.Filter(characterDrop, Instance.OnStartConditions);
            }
            catch (Exception e)
            {
                Log.LogError("Error while attempting to run OnStart conditions. Skipping filtering.", e);
                return characterDrop.m_drops;
            }
        }

        public static List<CharacterDrop.Drop> FilterOnDeath(CharacterDrop characterDrop)
        {
            try
            {
                return Instance.Filter(characterDrop, Instance.OnDeathConditions);
            }
            catch (Exception e)
            {
                Log.LogError("Error while attempting to run OnDeath conditions. Skipping filtering.", e);
                return characterDrop.m_drops;
            }
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

                if (!conditions.Any(x => x?.ShouldFilter(drop, dropExtended, characterDrop) ?? false))
                {
                    validDrops.Add(drop);
                }
            }

            return validDrops;
        }
    }
}

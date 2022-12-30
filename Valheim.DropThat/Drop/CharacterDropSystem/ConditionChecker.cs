using System;
using System.Collections.Generic;
using System.Linq;
using DropThat.Core;
using DropThat.Drop.CharacterDropSystem.Caches;
using DropThat.Drop.CharacterDropSystem.Conditions;
using DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific;
using DropThat.Reset;
using DropThat.Utilities;

namespace DropThat.Drop.CharacterDropSystem;

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
        OnStartConditions.AddNullSafe(ConditionInventory.Instance);
        OnStartConditions.AddNullSafe(ConditionLocation.Instance);
        OnStartConditions.AddNullSafe(ConditionDistanceToCenter.Instance);
        OnStartConditions.AddNullSafe(ConditionBiome.Instance);

        // Add OnDeath conditions

        OnDeathConditions = new HashSet<ICondition>();

        OnDeathConditions.AddNullSafe(ConditionCreatureState.Instance);
        OnDeathConditions.AddNullSafe(ConditionLoaderSpawnThat.ConditionTemplateId);
        OnDeathConditions.AddNullSafe(ConditionFaction.Instance);
        OnDeathConditions.AddNullSafe(ConditionNotFaction.Instance);
        OnDeathConditions.AddNullSafe(ConditionKilledByDamageType.Instance);
        OnDeathConditions.AddNullSafe(ConditionKilledBySkillType.Instance);
        OnDeathConditions.AddNullSafe(ConditionKilledWithStatus.Instance);
        OnDeathConditions.AddNullSafe(ConditionKilledWithStatuses.Instance);
        OnDeathConditions.AddNullSafe(ConditionKilledByEntityType.Instance);
        OnDeathConditions.AddNullSafe(ConditionEnvironments.Instance);
        OnDeathConditions.AddNullSafe(ConditionGlobalKeys.Instance);
        OnDeathConditions.AddNullSafe(ConditionLevel.Instance);
        OnDeathConditions.AddNullSafe(ConditionDaytime.Instance);
        OnDeathConditions.AddNullSafe(ConditionHitByEntityTypeRecently.Instance);

        OnDeathConditions.AddNullSafe(ConditionLoaderCLLC.ConditionBossAffix);
        OnDeathConditions.AddNullSafe(ConditionLoaderCLLC.ConditionInfusion);
        OnDeathConditions.AddNullSafe(ConditionLoaderCLLC.ConditionCreatureExtraEffect);
        OnDeathConditions.AddNullSafe(ConditionLoaderCLLC.ConditionWorldLevel);
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

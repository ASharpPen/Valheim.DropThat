using UnityEngine;
using DropThat.Configuration.ConfigTypes;
using DropThat.Core;
using DropThat.Drop.CharacterDropSystem.Caches;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionDistanceToCenter : ICondition
{
    private static ConditionDistanceToCenter _instance;

    public static ConditionDistanceToCenter Instance => _instance ??= new();

    public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop)
    {
        if (!characterDrop || characterDrop is null || extended?.Config is null)
        {
            return false;
        }

        if (IsValid(characterDrop.transform.position, extended.Config))
        {
            return false;
        }

        Log.LogTrace($"Filtered drop '{drop.m_prefab.name}' due not being within required distance to center of map.");
        return true;
    }

    public bool IsValid(Vector3 position, CharacterDropItemConfiguration config)
    {
        var distance = position.magnitude;

        if (distance < config.ConditionDistanceToCenterMin.Value)
        {
            return false;
        }

        if (config.ConditionDistanceToCenterMax.Value > 0 && distance > config.ConditionDistanceToCenterMax.Value)
        {
            return false;
        }

        return true;
    }
}

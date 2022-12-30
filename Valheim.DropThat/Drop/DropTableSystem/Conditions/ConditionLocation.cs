using System.Linq;
using UnityEngine;
using DropThat.Configuration.ConfigTypes;
using DropThat.Core;
using DropThat.Locations;
using DropThat.Utilities;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionLocation : IDropTableCondition
{
    private static ConditionLocation _instance;

    public static ConditionLocation Instance => _instance ??= new();

    public bool ShouldFilter(DropSourceTemplateLink context, DropTemplate template)
    {
        if (IsValid(context.Source.transform.position, template.Config))
        {
            return false;
        }

        Log.LogTrace($"Filtered drop '{template.Drop.m_item.name}' due to not being in required location.");
        return true;
    }

    public bool IsValid(Vector3 position, DropTableItemConfiguration config)
    {
        if (string.IsNullOrWhiteSpace(config?.ConditionLocations?.Value))
        {
            return true;
        }

        var locations = config.ConditionLocations.Value.SplitByComma(toUpper: true);

        if (locations.Count == 0)
        {
            return true;
        }

        var currentLocation = LocationHelper.FindLocation(position);

        var currentLocationName = currentLocation.LocationName.Trim().ToUpperInvariant();

        return locations.Any(x => x == currentLocationName);
    }
}

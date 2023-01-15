using System.Collections.Generic;
using UnityEngine;
using DropThat.Reset;
using DropThat.Utilities;
using ThatCore.Logging;

namespace DropThat.Locations;

public static class LocationHelper
{
    private static Dictionary<Vector2i, SimpleLocation> _simpleLocationsByZone { get; set; }

    static LocationHelper()
    {
        StateResetter.Subscribe(() =>
        {
            _simpleLocationsByZone = null;
        });
    }

    internal static void SetLocations(IEnumerable<SimpleLocation> locations)
    {
        if (_simpleLocationsByZone is null)
        {
            _simpleLocationsByZone = new Dictionary<Vector2i, SimpleLocation>();
        }

        foreach (var location in locations)
        {
            _simpleLocationsByZone[location.ZonePosition] = location;
        }
    }

    public static SimpleLocation FindLocation(Vector3 position)
    {
        if (ZoneSystem.instance.IsNull())
        {
            Log.Warning?.Log("Attempting to retrieve location before ZoneSystem is initialized.");
            return null;
        }

        var zoneId = ZoneSystem.instance.GetZone(position);

        if (ZoneSystem.instance.IsNotNull() &&
            (ZoneSystem.instance.m_locationInstances?.Count ?? 0) > 0)
        {
            if (ZoneSystem.instance.m_locationInstances.TryGetValue(zoneId, out ZoneSystem.LocationInstance defaultLocation))
            {
                return new SimpleLocation
                {
                    LocationName = defaultLocation.m_location?.m_prefabName ?? "",
                    Position = defaultLocation.m_position,
                    ZonePosition = zoneId,
                };
            }
        }

        if (_simpleLocationsByZone is not null)
        {
            if (_simpleLocationsByZone.TryGetValue(zoneId, out SimpleLocation location))
            {
                return location;
            }
        }

        return null;
    }
}

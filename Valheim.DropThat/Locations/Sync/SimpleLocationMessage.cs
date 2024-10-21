using System.Collections.Generic;
using ThatCore.Logging;
using ThatCore.Network;

namespace DropThat.Locations.Sync;

internal sealed class SimpleLocationMessage : IMessage
{
    public string[] LocationNames;

    public SimpleLocationDTO[] Locations;

    public void Initialize()
    {
        var locationInstances = ZoneSystem.instance.m_locationInstances;

        Dictionary<string, ushort> locationNameIndexes = new Dictionary<string, ushort>();

        List<string> locationNames = new List<string>();
        List<SimpleLocationDTO> locationDtos = new List<SimpleLocationDTO>();

        foreach (var location in locationInstances)
        {
            ushort nameIndex;

            string locationName = location.Value.m_location.m_prefabName;

            if (locationNameIndexes.TryGetValue(location.Value.m_location.m_prefabName, out ushort index))
            {
                nameIndex = index;
            }
            else
            {
                locationNames.Add(locationName);
                nameIndex = (ushort)(locationNames.Count - 1);
                locationNameIndexes.Add(locationName, nameIndex);
            }

            locationDtos.Add(new SimpleLocationDTO(location.Key, nameIndex));
        }

        LocationNames = locationNames.ToArray();
        Locations = locationDtos.ToArray();

        Log.Trace?.Log($"Packed {LocationNames.Length} location names");
        Log.Trace?.Log($"Packed {Locations.Length} locations");
    }

    public void AfterUnpack()
    {
        Log.Trace?.Log($"Unpacking {LocationNames.Length} location names");
        Log.Trace?.Log($"Unpacking {Locations.Length} locations");

        List<SimpleLocation> simpleLocations = new List<SimpleLocation>(Locations.Length);

        foreach (var location in Locations)
        {
            var position = new Vector2i(location.X, location.Y);

            simpleLocations.Add(new SimpleLocation
            {
                LocationName = LocationNames[location.Location],
                Position = ZoneSystem.GetZonePos(position),
                ZonePosition = position
            });
        }

        LocationHelper.SetLocations(simpleLocations);
    }
}

public struct SimpleLocationDTO
{
    public int X;
    public int Y;

    public ushort Location;

    public SimpleLocationDTO(Vector2i pos, ushort location)
    {
        X = pos.x;
        Y = pos.y;

        Location = location;
    }
}

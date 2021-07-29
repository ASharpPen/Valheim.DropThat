using System;
using System.Collections.Generic;
using Valheim.DropThat.Core;
using Valheim.DropThat.Core.Network;

namespace Valheim.DropThat.Locations
{
    [Serializable]
    internal class SimpleLocationPackage : CompressedPackage
    {
        public string[] LocationNames;

        public SimpleLocationDTO[] Locations;

        protected override void BeforePack()
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

            Log.LogTrace($"Packed {LocationNames.Length} location names");
            Log.LogTrace($"Packed {Locations.Length} locations");
        }

        protected override void AfterUnpack(object responseObject)
        {
            if (responseObject is SimpleLocationPackage)
            {
                Log.LogTrace($"Unpacking {LocationNames.Length} location names");
                Log.LogTrace($"Unpacking {Locations.Length} locations");

                List<SimpleLocation> simpleLocations = new List<SimpleLocation>(Locations.Length);

                foreach (var location in Locations)
                {
                    var position = new Vector2i(location.PositionX, location.PositionY);

                    simpleLocations.Add(new SimpleLocation
                    {
                        LocationName = LocationNames[location.Location],
                        Position = ZoneSystem.instance.GetZonePos(position),
                        ZonePosition = position
                    });
                }

                LocationHelper.SetLocations(simpleLocations);
            }
        }
    }

    [Serializable]
    public struct SimpleLocationDTO
    {
        public int PositionX;
        public int PositionY;

        public ushort Location;

        public SimpleLocationDTO(Vector2i pos, ushort location)
        {
            PositionX = pos.x;
            PositionY = pos.y;

            Location = location;
        }
    }
}

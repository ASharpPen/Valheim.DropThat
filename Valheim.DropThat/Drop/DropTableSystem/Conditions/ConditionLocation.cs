using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Models;
using DropThat.Locations;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionLocation : IDropCondition
{
    public HashSet<string> Locations { get; set; }

    public bool IsPointless() => (Locations?.Count ?? 0) == 0;

    public void SetLocations(IEnumerable<string> locations)
    {
        Locations = locations?
            .Select(x =>
                x.Trim()
                .ToUpperInvariant())
            .ToHashSet();
    }

    public bool IsValid(DropContext context)
    {
        if (Locations is null ||
            Locations.Count == 0)
        {
            return true;
        }

        var currentLocation = LocationHelper.FindLocation(context.Pos);

        if (currentLocation is null)
        {
            return false;
        }

        var currentLocationName = currentLocation.LocationName
            .Trim()
            .ToUpperInvariant();

        return Locations.Contains(currentLocationName);
    }
}
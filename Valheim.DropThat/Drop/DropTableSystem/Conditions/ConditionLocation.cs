using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Models;
using DropThat.Locations;
using ThatCore.Extensions;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionLocation : IDropCondition
{
    public HashSet<string> Locations { get; set; }

    public ConditionLocation()
    {
    }

    public ConditionLocation(IEnumerable<string> locations)
    {
        Locations = locations
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

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionLocation(
        this IHaveDropConditions template,
        IEnumerable<string> locations)
    {
        if (locations?.Any() == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionLocation(locations));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionLocation);
        }

        return template;
    }
}
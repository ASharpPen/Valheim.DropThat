using System.Collections.Generic;
using System.Linq;
using DropThat.Caches;
using DropThat.Drop.CharacterDropSystem.Managers;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Locations;
using DropThat.Utilities.Valheim;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionLocation : IDropCondition
{
    public HashSet<string> Locations { get; set; }

    public ConditionLocation() { }

    public ConditionLocation(IEnumerable<string> locations) 
    {
        Locations = locations
            .Select(x => x
                .Trim()
                .ToUpperInvariant())
            .ToHashSet();
    }

    public bool IsValid(DropContext context)
    {
        if (Locations is null ||
            Locations.Count == 0 ||
            context.Character.IsNull())
        {
            return true;
        }

        var currentLocation = LocationHelper
            .FindLocation(context.Character.GetCenterPoint());

        if (currentLocation is null)
        {
            return true;
        }

        return Locations.Contains(currentLocation.LocationName.Trim().ToUpperInvariant());
    }
}

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionLocation(
        this CharacterDropDropTemplate template,
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
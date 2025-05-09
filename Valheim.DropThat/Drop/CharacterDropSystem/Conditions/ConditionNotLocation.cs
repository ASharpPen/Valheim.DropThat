﻿using System.Collections.Generic;
using System.Linq;
using DropThat.Creature;
using DropThat.Drop.CharacterDropSystem.Managers;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Locations;
using DropThat.Utilities.Valheim;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public sealed class ConditionNotLocation : IDropCondition
{
    public HashSet<string> Locations { get; set; }

    static ConditionNotLocation()
    {
        CharacterDropEventManager.OnDropTableInitializeSet.Add(RecordSpawnInfoService.SetSpawnLocationIfMissing);
    }

    public bool IsPointless() => (Locations?.Count ?? 0) == 0;

    public void SetLocations(IEnumerable<string> locations)
    {
        Locations = locations?
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

        var pos = context.ZDO?.GetSpawnPosition()
            ?? context.Character.GetCenterPoint();

        var currentLocation = LocationHelper
            .FindLocation(pos);

        if (currentLocation is null)
        {
            return true;
        }

        return !Locations.Contains(currentLocation.LocationName.Trim().ToUpperInvariant());
    }
}
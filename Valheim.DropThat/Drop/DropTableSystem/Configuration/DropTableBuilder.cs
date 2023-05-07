using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Models;

namespace DropThat.Drop.DropTableSystem.Configuration;

internal class DropTableBuilder
    : IHaveDropBuilders
{
    public DropTableBuilder(
        string prefab,
        DropTableSystemConfiguration builderCollection)
    {
        Id = prefab;
        BuilderCollection = builderCollection;
    }

    public string Id { get; }

    public DropTableSystemConfiguration BuilderCollection { get; }

    public Dictionary<uint, DropTableDropBuilder> DropBuilders { get; } = new();

    public Optional<int?> DropMin { get; set; }

    public Optional<int?> DropMax { get; set; }

    public Optional<float?> DropChance { get; set; }

    public Optional<bool?> DropOnlyOnce { get; set; }

    public Optional<List<string>> ListNames { get; set; }

    public DropTableDropBuilder GetDrop(uint id)
    {
        if (DropBuilders.TryGetValue(id, out var existing))
        {
            return existing;
        }

        return DropBuilders[id] = new(id, this);
    }

    public DropTableTemplate Build()
    {
        var drops = DropBuilders
            .Values
            .Select(x => x.Build())
            .ToDictionary(x => x.Id);

        return new()
        {
            PrefabName = Id,
            Drops = drops,
            DropChance = DropChance,
            DropMax = DropMax,
            DropMin = DropMin,
            DropOnlyOnce = DropOnlyOnce,
        };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        if (ListNames.IsSet)
        {
            foreach (var list in ListNames.Value)
            {
                var listBuilder = BuilderCollection.GetListBuilder(list);

                foreach (var listDropBuilder in listBuilder.DropBuilders)
                {
                    GetDrop(listDropBuilder.Key).Configure(listDropBuilder.Value);
                }
            }
        }

        var drops = DropBuilders
            .Values
            .Select(x => x.Build())
            .ToDictionary(x => x.Id);

        DropTableTemplate template = new()
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

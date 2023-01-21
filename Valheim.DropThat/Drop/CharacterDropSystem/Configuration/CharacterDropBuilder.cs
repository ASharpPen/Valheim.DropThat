using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Models;

namespace DropThat.Drop.CharacterDropSystem.Configuration;

internal class CharacterDropBuilder : IHaveDropBuilders
{
    public CharacterDropBuilder(
        string mob, 
        CharacterDropSystemConfiguration builderCollection)
    {
        Id = mob;
        BuilderCollection = builderCollection;
    }

    public string Id { get; }

    public CharacterDropSystemConfiguration BuilderCollection { get; }

    public Dictionary<uint, CharacterDropDropBuilder> DropBuilders { get; } = new();

    public Optional<List<string>> ListNames { get; set; }

    public CharacterDropDropBuilder GetDrop(uint id)
    {
        if (DropBuilders.TryGetValue(id, out var existing))
        {
            return existing;
        }

        return DropBuilders[id] = new(id, this);
    }

    public CharacterDropMobTemplate Build()
    {
        // Initialize drops by applying list configs first.
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

        CharacterDropMobTemplate mobTemplate = new()
        {
            PrefabName = Id,
            Drops = drops
        };

        return mobTemplate;
    }
}

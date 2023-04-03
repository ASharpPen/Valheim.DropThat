using System.Collections.Generic;

namespace DropThat.Drop.DropTableSystem.Configuration;

internal class DropTableListBuilder
    : IHaveDropBuilders
{
    public DropTableListBuilder(
        string name,
        DropTableSystemConfiguration builderCollection)
    {
        Id = name;
        BuilderCollection = builderCollection;
    }

    public string Id { get; }

    public DropTableSystemConfiguration BuilderCollection { get; }

    public Dictionary<uint, DropTableDropBuilder> DropBuilders { get; } = new();

    public DropTableDropBuilder GetDrop(uint id)
    {
        if (DropBuilders.TryGetValue(id, out var existing))
        {
            return existing;
        }

        return DropBuilders[id] = new(id, this);
    }
}

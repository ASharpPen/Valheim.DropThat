using System.Collections.Generic;

namespace DropThat.Drop.CharacterDropSystem.Configuration;

internal class CharacterDropListBuilder : IHaveDropBuilders
{
    public CharacterDropListBuilder(
        string name, 
        CharacterDropSystemConfiguration builderCollection)
    {
        Id = name;
        BuilderCollection = builderCollection;
    }

    public CharacterDropSystemConfiguration BuilderCollection { get; }
    
    public string Id { get; }

    public Dictionary<uint, CharacterDropDropBuilder> DropBuilders { get; } = new();

    public CharacterDropDropBuilder GetDrop(uint id)
    {
        if (DropBuilders.TryGetValue(id, out var existing))
        {
            return existing;
        }

        return DropBuilders[id] = new(id, this);
    }
}

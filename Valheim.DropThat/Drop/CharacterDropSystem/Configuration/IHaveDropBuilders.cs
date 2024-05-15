using System.Collections.Generic;

namespace DropThat.Drop.CharacterDropSystem.Configuration;

internal interface IHaveDropBuilders
{
    string Id { get; }

    Dictionary<uint, CharacterDropDropBuilder> DropBuilders { get; }
}

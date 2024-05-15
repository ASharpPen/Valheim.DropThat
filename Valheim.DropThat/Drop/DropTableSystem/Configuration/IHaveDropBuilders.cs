using System.Collections.Generic;

namespace DropThat.Drop.DropTableSystem.Configuration;

internal class IHaveDropBuilders
{
    string Id { get; }

    Dictionary<uint, DropTableDropBuilder> DropBuilders { get; }
}

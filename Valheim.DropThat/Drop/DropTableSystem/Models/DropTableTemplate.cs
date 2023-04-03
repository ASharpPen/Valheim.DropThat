using System.Collections.Generic;
using ThatCore.Models;

namespace DropThat.Drop.DropTableSystem.Models;

public class DropTableTemplate
{
    public string PrefabName { get; set; }

    public Dictionary<int, DropTableDropTemplate> Drops { get; set; } = new();

    public Optional<int?> DropMin { get; set; }

    public Optional<int?> DropMax { get; set; }

    public Optional<float?> DropChance { get; set; }

    public Optional<bool?> DropOnlyOnce { get; set; }
}

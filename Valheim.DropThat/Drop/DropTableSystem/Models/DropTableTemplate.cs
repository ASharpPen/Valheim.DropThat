using System.Collections.Generic;

namespace DropThat.Drop.DropTableSystem.Models;

public sealed class DropTableTemplate
{
    public string PrefabName { get; set; }

    public Dictionary<int, DropTableDropTemplate> Drops { get; set; } = new();

    public int? DropMin { get; set; }

    public int? DropMax { get; set; }

    public float? DropChance { get; set; }

    public bool? DropOnlyOnce { get; set; }
}

using System.Collections.Generic;

namespace DropThat.Drop.DropTableSystem.Models;

public sealed class ExpectedDropTable
{
    public string PrefabName { get; set; }

    public int DropMin { get; set; }

    public int DropMax { get; set; }

    public float DropChance { get; set; }

    public bool OneOfEach { get; set; }

    /// <summary>
    /// A table or drop configuration was applied.
    /// </summary>
    public bool IsModified { get; set; }

    public List<DropTable.DropData> Drops { get; set; } = [];
}

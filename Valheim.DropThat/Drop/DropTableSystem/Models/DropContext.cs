using UnityEngine;

namespace DropThat.Drop.DropTableSystem.Models;

public sealed class DropContext
{
    private Vector3? _pos;

    public DropContext(
        GameObject dropTableSource,
        DropTable dropTable,
        DropTable.DropData drop)
    {
        DropTableSource = dropTableSource;
        DropTable = dropTable;
        Drop = drop;
    }

    public GameObject DropTableSource { get; set; }

    public DropTable DropTable { get; set; }

    public DropTable.DropData Drop { get; set; }

    public DropTableTemplate DropTableConfig { get; set; }

    public DropTableDropTemplate DropConfig { get; set; }

    public Vector3 Pos => _pos ??= DropTableSource.transform.position;
}

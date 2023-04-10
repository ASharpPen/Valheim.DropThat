namespace DropThat.Drop.DropTableSystem.Models;

internal class DropTableDrop
{
    public int DropTableIndex { get; set; }

    public int CurrentIndex { get; set; }

    public DropTable TableData { get; set; }

    public DropTable.DropData DropData { get; set; }

    public DropTableTemplate TableTemplate { get; set; }

    public DropTableDropTemplate DropTemplate { get; set; }
}
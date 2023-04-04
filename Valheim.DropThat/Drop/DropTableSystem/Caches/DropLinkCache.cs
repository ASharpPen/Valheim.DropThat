using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Caches;

internal static class DropLinkCache
{
    private static ConditionalWeakTable<DropTable, Dictionary<int, DropLink>> IdLinkTable { get; } = new();

    private static ConditionalWeakTable<DropTable, Dictionary<int, DropLink>> IndexLinkTable { get; } = new();

    public static bool TryGetById(DropTable key, int id, out DropLink link)
    {
        if (IdLinkTable.TryGetValue(key, out var dropTable) &&
            dropTable.TryGetValue(id, out link))
        {
            return true;
        }

        link = null;
        return false;
    }

    public static bool TryGetByIndex(DropTable key, int index, out DropLink link)
    {
        if (IndexLinkTable.TryGetValue(key, out var dropTable) &&
            dropTable.TryGetValue(index, out link))
        {
            return true;
        }

        link = null;
        return false;
    }

    public static void SetLink(DropTable key, int index, DropLink link)
    {
        var id = link.Drop.Id;

        var idKeyTable = IdLinkTable.GetOrCreateValue(key);
        idKeyTable[id] = link;

        var indexKeyTable = IndexLinkTable.GetOrCreateValue(key);
        indexKeyTable[index] = link;
    }
}

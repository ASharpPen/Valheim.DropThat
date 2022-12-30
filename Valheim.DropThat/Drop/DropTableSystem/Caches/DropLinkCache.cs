using System.Runtime.CompilerServices;

namespace DropThat.Drop.DropTableSystem.Caches;

internal static class DropLinkCache
{
    private static ConditionalWeakTable<DropTable, DropSourceTemplateLink> LinkTable = new();

    public static DropSourceTemplateLink GetLink(DropTable key)
    {
        if (LinkTable.TryGetValue(key, out DropSourceTemplateLink value))
        {
            return value;
        }

        return null;
    }

    public static void SetLink(DropTable key, DropSourceTemplateLink context)
    {
        if (context is null)
        {
            return;
        }

        LinkTable.Add(key, context);
    }
}

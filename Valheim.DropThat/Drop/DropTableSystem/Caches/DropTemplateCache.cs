using System.Runtime.CompilerServices;
using UnityEngine;

namespace Valheim.DropThat.Drop.DropTableSystem.Caches
{
    internal class DropTemplateCache
    {
        private static ConditionalWeakTable<object, DropTemplateCache> DropTemplateTable = new();

        public static void RegisterTemplate(GameObject drop, DropTemplate template)
        {
            var dropExtended = DropTemplateTable.GetOrCreateValue(drop);
            dropExtended.Template = template;
        }

        public static void RegisterTemplate(ItemDrop.ItemData item, DropTemplate template)
        {
            var dropExtended = DropTemplateTable.GetOrCreateValue(item);
            dropExtended.Template = template;
        }

        public static DropTemplate GetTemplate(ItemDrop.ItemData item)
        {
            if (DropTemplateTable.TryGetValue(item, out DropTemplateCache extended))
            {
                return extended.Template;
            }

            return null;
        }

        public static DropTemplate GetTemplate(GameObject drop)
        {
            if (DropTemplateTable.TryGetValue(drop, out DropTemplateCache extended))
            {
                return extended.Template;
            }

            return null;
        }

        public DropTemplate Template { get; set; }
    }
}

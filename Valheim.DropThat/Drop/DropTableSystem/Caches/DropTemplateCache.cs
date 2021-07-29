using System.Runtime.CompilerServices;
using UnityEngine;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Drop.DropTableSystem.Caches
{
    internal class DropTemplateCache
    {
        private static ManagedCache<DropTemplate> DropTemplates { get; } = new();
        private static ConditionalWeakTable<ItemDrop.ItemData, DropTemplateCache> ItemTemplates { get; } = new();

        public static void RegisterTemplate(GameObject drop, DropTemplate template)
        {
            DropTemplates.Set(drop, template);
        }

        public static void RegisterTemplate(ItemDrop.ItemData item, DropTemplate template)
        {
            var dropExtended = ItemTemplates.GetOrCreateValue(item);
            dropExtended.Template = template;
        }

        public static DropTemplate GetTemplate(ItemDrop.ItemData item)
        {
            if (ItemTemplates.TryGetValue(item, out DropTemplateCache extended))
            {
                return extended.Template;
            }

            return null;
        }

        public static DropTemplate GetTemplate(GameObject drop)
        {
            if (DropTemplates.TryGet(drop, out DropTemplate cached))
            {
                return cached;
            }

            return null;
        }

        public DropTemplate Template { get; set; }
    }
}

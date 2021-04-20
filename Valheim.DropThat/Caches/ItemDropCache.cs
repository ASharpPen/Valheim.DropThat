using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Valheim.DropThat.Caches
{
    public static class ItemDropCache
    {
        private static ConditionalWeakTable<GameObject, ItemDrop> ItemDropTable = new ConditionalWeakTable<GameObject, ItemDrop>();

        public static ItemDrop Get(GameObject item)
        {
            if(ItemDropTable.TryGetValue(item, out ItemDrop existing))
            {
                return existing;
            }

            var component = item.GetComponent<ItemDrop>();

            ItemDropTable.Add(item, component);

            return component;
        }
    }
}

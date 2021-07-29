using UnityEngine;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Caches
{
    public static class ZdoCache
    {
        private static ManagedCache<ZDO> ZdoTable { get; } = new();

        public static ZDO GetZDO(GameObject gameObject)
        {
            if (ZdoTable.TryGet(gameObject, out ZDO cached))
            {
                return cached;
            }

            var znetView = ComponentCache.GetComponent<ZNetView>(gameObject);

            if (!znetView || znetView is null)
            {
                return null;
            }

            var zdo = znetView.GetZDO();
            ZdoTable.Set(gameObject, zdo);
            return zdo;
        }
    }
}

using UnityEngine;
using DropThat.Core.Cache;

namespace DropThat.Caches
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

            var znetView = ComponentCache.Get<ZNetView>(gameObject);

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

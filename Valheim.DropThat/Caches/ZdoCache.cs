using ThatCore.Cache;
using ThatCore.Extensions;
using UnityEngine;

namespace DropThat.Caches;

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

        if (znetView.IsNull())
        {
            ZdoTable.Set(gameObject, null);
            return null;
        }

        var zdo = znetView.GetZDO();
        ZdoTable.Set(gameObject, zdo);
        return zdo;
    }

    public static ZDO GetZDO(Component component)
    {
        if (component.IsNull())
        {
            return null;
        }

        return GetZDO(component.gameObject);
    }
}

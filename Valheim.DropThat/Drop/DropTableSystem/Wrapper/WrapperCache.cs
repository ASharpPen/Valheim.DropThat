using UnityEngine;
using ThatCore.Cache;
using System.Collections.Generic;

namespace DropThat.Drop.DropTableSystem.Wrapper;

internal sealed class WrapperCache
{
    public WrapperComponent Wrapper { get; set; }

    public GameObject Wrapped { get; set; }

    public bool Unwrapped { get; set; }

    internal static ManagedCache<WrapperCache> Cache { get; } = new();

    internal static Dictionary<int, WrapperCache> CacheById { get; } = new();

    internal static bool TryGet(GameObject key, out WrapperCache cache)
    {
        if (Cache.TryGet(key, out cache))
        {
            return true;
        }

        return false;
    }

    internal static bool TryGet(int instanceId, out WrapperCache cache)
    {
        if (CacheById.TryGetValue(instanceId, out cache))
        {
            return true;
        }

        return false;
    }

    internal static void Set(WrapperComponent wrapper, GameObject wrapped, bool unwrapped = false)
    {
        var cached = new WrapperCache()
        {
            Wrapper = wrapper,
            Wrapped = wrapped,
            Unwrapped = unwrapped
        };

        Cache.Set(wrapper.gameObject, cached);

        CacheById[wrapper.SourceId] = cached;
    }

    internal static void CleanUp(int instanceId)
    {
        CacheById.Remove(instanceId);
    }
}

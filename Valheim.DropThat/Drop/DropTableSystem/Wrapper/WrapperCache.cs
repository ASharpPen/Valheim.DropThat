using UnityEngine;
using ThatCore.Cache;

namespace DropThat.Drop.DropTableSystem.Wrapper;

internal class WrapperCache
{
    public GameObject Wrapper { get; set; }

    public GameObject Wrapped { get; set; }

    public bool Unwrapped { get; set; }

    internal static ManagedCache<WrapperCache> Cache { get; } = new();

    internal static WrapperCache Get(GameObject go)
    {
        if (Cache.TryGet(go, out WrapperCache cached))
        {
            return cached;
        }

        return null;
    }

    internal static void SetStatus(GameObject wrapper, bool unwrapped = false)
    {
        if (Cache.TryGet(wrapper, out WrapperCache cached))
        {
            cached.Unwrapped = unwrapped;
        }
    }

    internal static void Set(GameObject wrapper, GameObject wrapped, bool unwrapped = false)
    {
        Cache.Set(wrapper, new()
        {
            Wrapper = wrapper,
            Wrapped = wrapped,
            Unwrapped = unwrapped
        });
    }
}

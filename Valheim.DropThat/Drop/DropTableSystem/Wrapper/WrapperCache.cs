using UnityEngine;
using ThatCore.Cache;

namespace DropThat.Drop.DropTableSystem.Wrapper;

internal class WrapperCache
{
    public WrapperComponent Wrapper { get; set; }

    public GameObject Wrapped { get; set; }

    public bool Unwrapped { get; set; }

    internal static ManagedCache<WrapperCache> Cache { get; } = new();

    internal static bool TryGet(GameObject key, out WrapperCache cache)
    {
        if (Cache.TryGet(key, out cache))
        {
            return true;
        }

        return false;
    }

    internal static void Set(WrapperComponent wrapper, GameObject wrapped, bool unwrapped = false)
    {
        Cache.Set(wrapper, new()
        {
            Wrapper = wrapper,
            Wrapped = wrapped,
            Unwrapped = unwrapped
        });
    }
}

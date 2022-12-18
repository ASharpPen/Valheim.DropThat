using UnityEngine;
using DropThat.Core;

namespace DropThat.Drop.DropTableSystem.Wrapper;

public static class WrapperGameObjectExtensions
{
    public static GameObject Wrap(this GameObject gameObject)
    {
        var cached = WrapperCache.Get(gameObject);

        if (cached is not null)
        {
            // Object is already a wrapper.
            return gameObject;
        }

        GameObject wrapper = new GameObject(gameObject.name);
        wrapper.AddComponent<WrapperComponent>();

        WrapperCache.Set(wrapper, gameObject);

        return wrapper;
    }

    public static GameObject Unwrap(this GameObject gameObject)
    {
        if (gameObject == null || !gameObject)
        {
            return gameObject;
        }

        var cached = WrapperCache.Get(gameObject);

        if (cached is not null)
        {
#if DEBUG
            Log.LogTrace($"Unwrapped '{cached.Wrapped.name}'");
#endif
            // Mark as successfully unwrapping to tell the WrapperComponent that everything is fine.
            cached.Unwrapped = true;

            return cached.Wrapped;
        }

        return gameObject;
    }
}

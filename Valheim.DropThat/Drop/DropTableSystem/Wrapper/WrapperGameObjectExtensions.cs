using UnityEngine;
using ThatCore.Extensions;
using ThatCore.Logging;

namespace DropThat.Drop.DropTableSystem.Wrapper;

/// <summary>
/// TODOFUTURE: Consider using an object pool for the wrapper gameobjects.
/// </summary>
public static class WrapperGameObjectExtensions
{
    public static WrapperComponent Wrap(this GameObject gameObject)
    {
        if (WrapperCache.TryGet(gameObject, out var existing))
        {
            // Object is already a wrapper.
            return existing.Wrapper;
        }

        GameObject wrapperObj = new GameObject(gameObject.name);
        var wrapper = wrapperObj.AddComponent<WrapperComponent>();

        wrapper.SourceId = wrapperObj.GetInstanceID();

        WrapperCache.Set(wrapper, gameObject);

        return wrapper;
    }

    public static GameObject Unwrap(this GameObject gameObject)
    {
        if (gameObject.IsNull())
        {
            return gameObject;
        }

        if (WrapperCache.TryGet(gameObject, out var cached))
        {
            Log.Development?.Log($"Unwrapped '{cached.Wrapped.name}'");
      
            // Mark as successfully unwrapping to tell the WrapperComponent that everything is fine.
            cached.Unwrapped = true;

            return cached.Wrapped;
        }

        return gameObject;
    }
}

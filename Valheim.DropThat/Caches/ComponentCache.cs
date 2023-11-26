using System;
using System.Collections.Generic;
using UnityEngine;
using ThatCore.Cache;
using DropThat.Utilities;

namespace DropThat.Caches;

public sealed class ComponentCache
{
    private static ManagedCache<ComponentCache> CacheTable { get; } = new();

    public static T Get<T>(GameObject obj) where T : Component
    {
        if (obj.IsNull())
        {
            return null;
        }

        ComponentCache cache = CacheTable.GetOrCreate(obj);

        Type componentType = typeof(T);

        if (cache.ComponentTable.TryGetValue(componentType, out Component cached))
        {
            return (T)cached;
        }

        if (obj.TryGetComponent(componentType, out Component component))
        {
            cache.ComponentTable.Add(componentType, component);
            return (T)component;
        }
        else
        {
            cache.ComponentTable.Add(componentType, null);
            return null;
        }
    }

    public static T Get<T>(Component obj) where T : Component
    {
        if (obj.IsNull())
        {
            return null;
        }

        return Get<T>(obj.gameObject);
    }

    private Dictionary<Type, Component> ComponentTable { get; } = new();
}

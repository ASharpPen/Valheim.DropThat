using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Cache;
using ThatCore.Extensions;
using ThatCore.Logging;

namespace DropThat.Drop.CharacterDropSystem.Caches;

/// <summary>
/// Temporary caching for carrying config information between CharacterDrop.GenerateDropList and CharacterDrop.DropItems.
/// </summary>
internal class TempDropListCache
{
    private static ConditionalWeakTable<object, TempDropListCache> DropListTable = new ConditionalWeakTable<object, TempDropListCache>();

    private static ManagedCache<TempDropListCache> ObjectTable = new();

    public Dictionary<int, DropConfigInfo> InfoByIndex { get; } = new();

    public static void SetDrop(UnityEngine.Component key, DropConfigInfo dropInfo, int? index = null)
    {
        Log.DevelopmentOnly($"Setting temp drop cache {dropInfo.Index}:{key.GetHashCode()}");

        var cache = ObjectTable.GetOrCreate(key.gameObject);
        cache.InfoByIndex[index ?? dropInfo.Index] = dropInfo;
    }

    public static void SetDrop(object key, DropConfigInfo dropInfo, int? index = null)
    {
        Log.DevelopmentOnly($"Setting temp drop cache {dropInfo.Index}:{key.GetHashCode()}");

        var cache = DropListTable.GetOrCreateValue(key);
        cache.InfoByIndex[index ?? dropInfo.Index] = dropInfo;
    }

    public static TempDropListCache GetDrops(UnityEngine.Component key)
    {
        if (key.IsNull())
        {
            return null;
        }

        if (ObjectTable.TryGet(key.gameObject, out var cache))
        {
            return cache;
        }

        return null;
    }

    public static TempDropListCache GetDrops(object key)
    {
        Log.DevelopmentOnly($"Getting temp drop cache {key.GetHashCode()}");

        if (key is null)
        {
            return null;
        }

        if (DropListTable.TryGetValue(key, out TempDropListCache cache))
        {
            return cache;
        }

        return null;
    }

    public static DropConfigInfo GetDrop(UnityEngine.Component key, int index)
    {
        if (ObjectTable.TryGet(key.gameObject, out TempDropListCache cache))
        {
            if (cache.InfoByIndex.TryGetValue(index, out DropConfigInfo info))
            {
                return info;
            }
        }

        return null;
    }

    public static DropConfigInfo GetDrop(object key, int index)
    {
        if (DropListTable.TryGetValue(key, out TempDropListCache cache))
        {
            Log.DevelopmentOnly($"Found temp drop cache for {key.GetHashCode()}");

            if (cache.InfoByIndex.TryGetValue(index, out DropConfigInfo dropInfo))
            {
                Log.DevelopmentOnly($"Successfully retrieved config from temp drop cache");

                return dropInfo;
            }
        }

        return null;
    }
}

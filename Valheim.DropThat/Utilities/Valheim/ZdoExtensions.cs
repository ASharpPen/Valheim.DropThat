using UnityEngine;
#if TEST
using ThatCore.Extensions;
#endif

namespace DropThat.Utilities.Valheim;

public static class ZdoExtensions
{
    private const string Prefix = "DropThat_";

#if TEST
    private static readonly int _spawnBiomeHash = (Prefix + "spawn-biome").HashInteger();
    private static readonly int _spawnPositionHash = (Prefix + "spawn-position").HashInteger();
#else
    private static readonly int _spawnBiomeHash = (Prefix + "spawn-biome").GetStableHashCode();
    private static readonly int _spawnPositionHash = (Prefix + "spawn-position").GetStableHashCode();
#endif

    public static Heightmap.Biome? GetSpawnBiome(this ZDO zdo)
    {
        var biome = zdo.GetInt(_spawnBiomeHash, -1);

        if (biome < 0)
        {
            return null;
        }

        return (Heightmap.Biome)biome;
    }

    public static void SetSpawnBiome(this ZDO zdo, Heightmap.Biome? biome)
    {
        if (biome is null ||
            biome < 0)
        {
            zdo.RemoveInt(_spawnBiomeHash);
        }
        else
        {
            zdo.Set(_spawnBiomeHash, (int)biome);
        }
    }

    public static Vector3? GetSpawnPosition(this ZDO zdo)
    {
        if (ZDOExtraData.s_vec3.TryGetValue(zdo.m_uid, out var zdoCache) &&
            zdoCache.TryGetValue(_spawnPositionHash, out Vector3 result))
        {
            return result;
        }

        return null;
    }

    public static void SetSpawnPosition(this ZDO zdo, Vector3? value)
    {
        if (value is null)
        {
            zdo.RemoveInt(_spawnPositionHash);
        }
        else
        {
            zdo.Set(_spawnPositionHash, value.Value);
        }
    }
}

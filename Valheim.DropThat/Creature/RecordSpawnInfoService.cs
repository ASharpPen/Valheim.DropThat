using DropThat.Caches;
using DropThat.Utilities.Valheim;

namespace DropThat.Creature;

internal static class RecordSpawnInfoService
{
    public static void SetSpawnBiomeIfMissing(CharacterDrop droptable)
    {
        var zdo = ZdoCache.GetZDO(droptable);

        if (zdo is not null &&
            zdo.GetSpawnBiome() is null)
        {
            var biome = Heightmap.FindBiome(zdo.m_position);

            zdo.SetSpawnBiome(biome);
        }
    }

    public static void SetSpawnLocationIfMissing(CharacterDrop droptable)
    {
        var zdo = ZdoCache.GetZDO(droptable);

        if (zdo is not null &&
            zdo.GetSpawnPosition() is null)
        {
            zdo.SetSpawnPosition(zdo.m_position);
        }
    }
}

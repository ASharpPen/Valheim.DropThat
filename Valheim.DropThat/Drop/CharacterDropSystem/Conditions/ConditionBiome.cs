using DropThat.Caches;
using DropThat.Drop.CharacterDropSystem.Managers;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Utilities.Valheim;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionBiome : IDropCondition
{
    public Heightmap.Biome BiomeBitmask { get; set; }

    static ConditionBiome()
    {
        CharacterDropEventManager.OnDropTableInitialize += SetSpawnBiomeIfMissing;
    }

    public bool IsPointless() => BiomeBitmask == Heightmap.Biome.None;

    public bool IsValid(DropContext context)
    {
        if (BiomeBitmask == Heightmap.Biome.None)
        {
            return true;
        }

        var spawnBiome = context.ZDO?.GetSpawnBiome();

        if (spawnBiome is null)
        {
            return false;
        }

        return (spawnBiome.Value & BiomeBitmask) > 0;
    }

    private static void SetSpawnBiomeIfMissing(CharacterDrop droptable)
    {
        var zdo = ZdoCache.GetZDO(droptable);

        if (zdo is not null &&
            zdo.GetSpawnBiome() is null)
        {
            var biome = Heightmap.FindBiome(zdo.m_position);

            zdo.SetSpawnBiome(biome);
        }
    }
}
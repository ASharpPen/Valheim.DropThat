using System.Collections.Generic;
using System.Linq;
using DropThat.Caches;
using DropThat.Drop.CharacterDropSystem.Managers;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Utilities.Valheim;
using ThatCore.Utilities.Valheim;

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

internal static partial class IHaveDropConditionsExtensions
{
    public static IHaveDropConditions ConditionBiome(
        this IHaveDropConditions template, 
        IEnumerable<Heightmap.Biome> biomes)
    {
        if (biomes?.Any() == true)
        {
            template.Conditions.GetOrCreate<ConditionBiome>().BiomeBitmask = biomes.ToBitmask();
        }
        else
        {
            template.Conditions.Remove<ConditionBiome>();
        }

        return template;
    }
}
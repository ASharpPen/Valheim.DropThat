using System.Collections.Generic;
using System.Linq;
using DropThat.Caches;
using DropThat.Drop.CharacterDropSystem.Managers;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Utilities.Valheim;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionBiome : IDropCondition
{
    public Heightmap.Biome BiomeMask { get; set; }

    static ConditionBiome()
    {
        DropTableManager.OnDropTableInitialize += SetSpawnBiomeIfMissing;
    }

    public ConditionBiome()
    {
    }

    public ConditionBiome(params Heightmap.Biome[] biomes)
    {
        BiomeMask = Heightmap.Biome.None;

        if (biomes is not null)
        {
            foreach (var biome in biomes)
            {
                BiomeMask |= biome;
            }
        }
    }

    public bool IsValid(DropContext context)
    {
        if (BiomeMask == Heightmap.Biome.None)
        {
            return true;
        }

        var spawnBiome = context.ZDO?.GetSpawnBiome();

        if (spawnBiome is null)
        {
            return false;
        }

        return (spawnBiome.Value & BiomeMask) > 0;
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
            template.Conditions.AddOrReplaceByType(new ConditionBiome(biomes.ToArray()));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionBiome);
        }

        return template;
    }
}
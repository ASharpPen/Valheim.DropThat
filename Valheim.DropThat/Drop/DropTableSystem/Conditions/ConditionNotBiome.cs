using System.Collections.Generic;
using DropThat.Drop.DropTableSystem.Models;

namespace DropThat.Drop.DropTableSystem.Conditions;

public sealed class ConditionNotBiome : IDropCondition
{
    public Heightmap.Biome BiomeMask { get; set; }

    public bool IsPointless() => BiomeMask == Heightmap.Biome.None;

    public void SetBiomes(IEnumerable<Heightmap.Biome> biomes)
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

        var biome = Heightmap.FindBiome(context.Pos);

        return (BiomeMask & biome) == 0;
    }
}

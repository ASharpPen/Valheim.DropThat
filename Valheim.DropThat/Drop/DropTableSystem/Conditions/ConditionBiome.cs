using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Extensions;

namespace DropThat.Drop.DropTableSystem.Conditions;

public class ConditionBiome : IDropCondition
{
    public Heightmap.Biome BiomeMask { get; set; }

    public ConditionBiome() { }

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

        var biome = Heightmap.FindBiome(context.Pos);

        return (BiomeMask & biome) > 0;
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
            template.Conditions.AddOrReplaceByType(new ConditionBiome(biomes));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionBiome);
        }

        return template;
    }
}
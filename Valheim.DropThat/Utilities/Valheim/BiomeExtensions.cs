using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DropThat.Utilities.Valheim;

internal static class BiomeExtensions
{
    public static string GetNames(this Heightmap.Biome biome)
    {
        List<string> biomes = new List<string>();

        foreach (Heightmap.Biome potentialBiome in Enum.GetValues(typeof(Heightmap.Biome)))
        {
            if (potentialBiome == Heightmap.Biome.None)
            {
                continue;
            }

            if ((biome & potentialBiome) > 0)
            {
                biomes.Add(potentialBiome.ToString());
            }
        }

        return biomes.Join(delimiter: ", ");
    }
}

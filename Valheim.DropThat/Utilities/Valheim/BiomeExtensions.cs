using System.Linq;
using ThatCore.Extensions;
using ThatCore.Valheim.Extensions;

namespace DropThat.Utilities.Valheim;

internal static class BiomeExtensions
{
    public static string GetNames(this Heightmap.Biome biome) =>
        biome.Split().Join();
}

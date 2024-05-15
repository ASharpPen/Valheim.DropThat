using System.Linq;
using ThatCore.Extensions;
using ThatCore.Utilities.Valheim;

namespace DropThat.Utilities.Valheim;

internal static class BiomeExtensions
{
    public static string GetNames(this Heightmap.Biome biome) =>
        biome.Split().Join();
}

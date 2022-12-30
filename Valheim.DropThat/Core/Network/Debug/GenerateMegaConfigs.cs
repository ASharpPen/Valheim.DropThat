#if FALSE && DEBUG
using System.IO;
using System.Text;
using BepInEx;
using HarmonyLib;

namespace Valheim.DropThat.Core.Network.Debug;

[HarmonyPatch]
internal static class GenerateMegaConfigs
{
    [HarmonyPatch(typeof(Game), nameof(Game.Awake))]
    [HarmonyPostfix]
    private static void Generate()
    {
        var dir = Path.Combine(Paths.ConfigPath, "DropThatTesting");

        Directory.CreateDirectory(dir);

        for (int i = 0; i < 100; ++i)
        {
            string file = Path.Combine(dir, $"drop_that.character_drop.test_{i}.cfg");

            var content = GenerateFile(i);

            File.WriteAllText(file, content);
        }
    }

    private static string GenerateFile(int batch, int batchSize = 500)
    {
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < batchSize; ++i)
        {
            builder.AppendLine($"[Boar.{(batch * batchSize) + i}]");
            builder.AppendLine("PrefabName=Coins");
        }

        return builder.ToString();
    }
}

#endif
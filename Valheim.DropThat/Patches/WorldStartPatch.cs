using HarmonyLib;
using Valheim.DropThat.ConfigurationCore;

namespace Valheim.DropThat.Patches
{
    [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.OnWorldStart))]
    public static class WorldStartPatch
    {
        private static void Postfix(ref FejdStartup __instance)
        {
            Log.LogDebug("World startet. Loading drop configurations.");
            ConfigurationManager.LoadAllDropTableConfigurations();
        }
    }
}

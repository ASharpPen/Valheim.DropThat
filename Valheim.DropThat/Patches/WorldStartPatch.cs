using HarmonyLib;
using Valheim.DropThat.ConfigurationCore;

namespace Valheim.DropThat.Patches
{
    [HarmonyPatch(typeof(FejdStartup), "OnWorldStart")]
    public static class WorldStartPatch
    {
        private static void Postfix()
        {
            //Turns out, ZNet is null when starting singleplayer worlds.
            if (ZNet.instance == null)
            {
                Log.LogDebug("World startet. Loading configurations.");
                ConfigurationManager.LoadAllConfigurations();
            }
        }
    }
}

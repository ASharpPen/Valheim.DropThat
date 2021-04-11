using HarmonyLib;
using Valheim.DropThat.Configuration;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Reset
{
    [HarmonyPatch(typeof(FejdStartup), "OnWorldStart")]
    public static class WorldStartPatch
    {
        private static void Postfix()
        {
            //Turns out, ZNet is null when starting singleplayer worlds.
            if (ZNet.instance == null)
            {
                StateResetter.Reset();

                Log.LogDebug("World startet. Loading configurations.");
                ConfigurationManager.LoadAll();
            }
        }
    }
}

using HarmonyLib;
using Valheim.DropThat.Configuration;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Reset
{
    [HarmonyPatch(typeof(FejdStartup), "OnWorldStart")]
    public static class WorldStartPatch
    {
        /// <summary>
        /// Singleplayer
        /// </summary>
        [HarmonyPatch("OnWorldStart")]
        [HarmonyPrefix]
        private static void ResetState()
        {
            Log.LogDebug("OnWorldStart - Resetting configurations");
            StateResetter.Reset();
            ConfigurationManager.LoadAll();
        }

        /// <summary>
        /// Multiplayer
        /// </summary>
        [HarmonyPatch("JoinServer")]
        [HarmonyPrefix]
        private static void ResetStateMultiplayer()
        {
            Log.LogDebug("JoinServer - Resetting configurations");
            StateResetter.Reset();
        }

        /// <summary>
        /// Server
        /// </summary>
        [HarmonyPatch("ParseServerArguments")]
        [HarmonyPrefix]
        private static void ResetStateServer()
        {
            Log.LogDebug("ParseServerArguments - Resetting configurations");
            StateResetter.Reset();
            ConfigurationManager.LoadAllCharacterDropConfigurations();
            ConfigurationManager.LoadAllCharacterDropLists();
            ConfigurationManager.LoadAllDropTableConfigurations();
            ConfigurationManager.LoadAllDropTableLists();
        }
    }
}

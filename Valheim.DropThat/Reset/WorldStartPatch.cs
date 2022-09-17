using HarmonyLib;
using DropThat.Configuration;
using DropThat.Core;
using DropThat.Drop.CharacterDropSystem.Configurations;

namespace DropThat.Reset;

[HarmonyPatch(typeof(FejdStartup))]
public static class WorldStartPatch
{
    /// <summary>
    /// Singleplayer
    /// </summary>
    [HarmonyPatch(nameof(FejdStartup.OnWorldStart))]
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
    [HarmonyPatch(nameof(FejdStartup.JoinServer))]
    [HarmonyPrefix]
    private static void ResetStateMultiplayer()
    {
        Log.LogDebug("JoinServer - Resetting configurations");
        StateResetter.Reset();
    }

    /// <summary>
    /// Server
    /// </summary>
    [HarmonyPatch(nameof(FejdStartup.ParseServerArguments))]
    [HarmonyPrefix]
    private static void ResetStateServer()
    {
        Log.LogDebug("ParseServerArguments - Resetting configurations");
        StateResetter.Reset();
        CharacterDropConfigurationFileManager.LoadAllConfigurations();
        ConfigurationManager.LoadAllDropTableConfigurations();
        ConfigurationManager.LoadAllDropTableLists();
    }
}

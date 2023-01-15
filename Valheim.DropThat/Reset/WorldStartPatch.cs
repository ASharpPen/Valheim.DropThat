using HarmonyLib;
using DropThat.Configuration;
using DropThat.Core;
using ThatCore.Logging;
using System;

namespace DropThat.Reset;

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
        try
        {
            Log.Debug?.Log("OnWorldStart - Resetting configurations");
            StateResetter.Reset();
            ConfigurationManager.LoadAll();
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error during loadup.", e);
        }
    }

    /// <summary>
    /// Multiplayer
    /// </summary>
    [HarmonyPatch("JoinServer")]
    [HarmonyPrefix]
    private static void ResetStateMultiplayer()
    {
        try
        {
            Log.Debug?.Log("JoinServer - Resetting configurations");
            StateResetter.Reset();
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error during loadup.", e);
        }
    }

    /// <summary>
    /// Server
    /// </summary>
    [HarmonyPatch("ParseServerArguments")]
    [HarmonyPrefix]
    private static void ResetStateServer()
    {
        try
        {
            Log.Debug?.Log("ParseServerArguments - Resetting configurations");
            StateResetter.Reset();
            ConfigurationManager.LoadAllCharacterDropConfigurations();
            ConfigurationManager.LoadAllCharacterDropLists();
            ConfigurationManager.LoadAllDropTableConfigurations();
            ConfigurationManager.LoadAllDropTableLists();
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error during loadup.", e);
        }
    }
}

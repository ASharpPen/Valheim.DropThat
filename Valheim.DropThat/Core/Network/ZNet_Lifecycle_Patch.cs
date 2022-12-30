using System;
using HarmonyLib;

namespace Valheim.DropThat.Core.Network;

[HarmonyPatch]
internal static class ZNet_Lifecycle_Patch
{
    public static event Action OnZnetUpdate;

    [HarmonyPatch(typeof(ZNet), nameof(ZNet.Update))]
    [HarmonyPostfix]
    private static void HookZnetUpdate()
    {
        try
        {
            OnZnetUpdate();
        }
        catch (Exception e)
        {
            Log.LogError($"Error during {nameof(OnZnetUpdate)} for {nameof(ZNet)}.{nameof(ZNet.Update)}", e);
        }
    }
}

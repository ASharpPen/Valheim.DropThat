using HarmonyLib;

namespace DropThat.Core.Patches;

[HarmonyPatch]
internal static class Lifecycle_Patches
{
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    [HarmonyPostfix]
    private static void ZnetSceneStarted() => 
        DropThatLifecycleManager.ZnetSceneStarted();
}

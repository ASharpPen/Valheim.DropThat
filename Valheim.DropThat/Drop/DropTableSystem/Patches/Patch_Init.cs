using DropThat.Drop.DropTableSystem.Managers;
using HarmonyLib;

namespace DropThat.Drop.DropTableSystem.Patches;

/// <summary>
/// Store reference from gameobject of DropTable to the DropTable itself,
/// and cleanup the reference when the gameobject is destroyed.
/// 
/// Since the DropTable is a standard object and not a unity engine Object, we need
/// to store what object it belongs with for later reference.
/// </summary>
[HarmonyPatch]
internal static class Patch_Init
{
    [HarmonyPatch(typeof(Container))]
    internal static class Patch_Container_Awake_InitDropContext
    {
        [HarmonyPatch(nameof(Container.Awake))]
        [HarmonyPrefix]
        private static void SetLink(Container __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_defaultItems);
    }

    [HarmonyPatch(typeof(DropOnDestroyed))]
    internal static class Patch_DropOnDestroyed_Awake_InitDropContext
    {
        [HarmonyPatch(nameof(DropOnDestroyed.Awake))]
        [HarmonyPostfix]
        private static void SetLink(DropOnDestroyed __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_dropWhenDestroyed);
    }

    [HarmonyPatch(typeof(LootSpawner))]
    internal static class Patch_LootSpawner_Awake_InitDropContext
    {
        [HarmonyPatch(nameof(LootSpawner.Awake))]
        [HarmonyPostfix]
        private static void SetLink(LootSpawner __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_items);
    }

    [HarmonyPatch(typeof(TreeBase))]
    internal static class Patch_TreeBase_Awake_InitDropContext
    {
        [HarmonyPatch(nameof(TreeBase.Awake))]
        [HarmonyPostfix]
        private static void SetLink(TreeBase __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_dropWhenDestroyed);
    }

    [HarmonyPatch(typeof(TreeLog))]
    internal static class Patch_TreeLog_Awake_InitDropContext
    {
        [HarmonyPatch(nameof(TreeLog.Awake))]
        [HarmonyPostfix]
        private static void SetLink(TreeLog __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_dropWhenDestroyed);
    }

    [HarmonyPatch(typeof(MineRock))]
    internal static class Patch_MineRock_Awake_InitDropContext
    {
        [HarmonyPatch(nameof(MineRock.Start))]
        [HarmonyPostfix]
        private static void SetLink(MineRock __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_dropItems);
    }

    [HarmonyPatch(typeof(MineRock5))]
    internal static class Patch_MineRock5_Awake_InitDropContext
    {
        [HarmonyPatch(nameof(MineRock5.Awake))]
        [HarmonyPostfix]
        private static void SetLink(MineRock5 __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_dropItems);
    }
}

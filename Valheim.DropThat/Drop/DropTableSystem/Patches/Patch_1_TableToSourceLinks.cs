using DropThat.Drop.DropTableSystem.Managers;
using HarmonyLib;

namespace DropThat.Drop.DropTableSystem.Patches;

/// <summary>
/// Step 1 - Store reference from gameobject of DropTable to the DropTable itself.
/// 
/// Since the DropTable is a standard object and not a unity engine Object, we need
/// to store what object it belongs with for later reference.
/// </summary>
[HarmonyPatch]
internal static class Patch_1_TableToSourceLinks
{
    [HarmonyPatch(typeof(Container))]
    internal static class Patch_Container_Awake_InitDropContext
    {
        [HarmonyPatch(nameof(Container.Awake))]
        [HarmonyPrefix]
        private static void SetLink(Container __instance) =>
            DropTableManager.Initialize(__instance, __instance.m_defaultItems);

        [HarmonyPatch(nameof(Container.Destroy), typeof(UnityEngine.Object))]
        [HarmonyPrefix]
        private static void CleanupLink(Container __instance) => 
            DropTableManager.Cleanup(__instance, __instance.m_defaultItems);
    }

    [HarmonyPatch(typeof(DropOnDestroyed))]
    internal static class Patch_DropOnDestroyed_Awake_InitDropContext
    {
        [HarmonyPatch(nameof(DropOnDestroyed.Awake))]
        [HarmonyPostfix]
        private static void SetLink(DropOnDestroyed __instance) =>
            DropTableManager.Initialize(__instance, __instance.m_dropWhenDestroyed);

        [HarmonyPatch(nameof(DropOnDestroyed.Destroy), typeof(UnityEngine.Object))]
        [HarmonyPrefix]
        private static void CleanupLink(DropOnDestroyed __instance) => 
            DropTableManager.Cleanup(__instance, __instance.m_dropWhenDestroyed);
    }

    [HarmonyPatch(typeof(LootSpawner))]
    internal static class Patch_LootSpawner_Awake_InitDropContext
    {
        [HarmonyPatch(nameof(LootSpawner.Awake))]
        [HarmonyPostfix]
        private static void SetLink(LootSpawner __instance) =>
            DropTableManager.Initialize(__instance, __instance.m_items);

        [HarmonyPatch(nameof(LootSpawner.Destroy), typeof(UnityEngine.Object))]
        [HarmonyPrefix]
        private static void CleanupLink(LootSpawner __instance) =>
            DropTableManager.Cleanup(__instance, __instance.m_items);
    }

    [HarmonyPatch(typeof(TreeBase))]
    internal static class Patch_TreeBase_Awake_InitDropContext
    {
        [HarmonyPatch(nameof(TreeBase.Awake))]
        [HarmonyPostfix]
        private static void SetLink(TreeBase __instance) =>
            DropTableManager.Initialize(__instance, __instance.m_dropWhenDestroyed);

        [HarmonyPatch(nameof(TreeBase.Destroy), typeof(UnityEngine.Object))]
        [HarmonyPrefix]
        private static void CleanupLink(TreeBase __instance) =>
            DropTableManager.Cleanup(__instance, __instance.m_dropWhenDestroyed);
    }

    [HarmonyPatch(typeof(TreeLog))]
    internal static class Patch_TreeLog_Awake_InitDropContext
    {
        [HarmonyPatch(nameof(TreeLog.Awake))]
        [HarmonyPostfix]
        private static void SetLink(TreeLog __instance) =>
            DropTableManager.Initialize(__instance, __instance.m_dropWhenDestroyed);

        [HarmonyPatch(nameof(TreeLog.Destroy), typeof(UnityEngine.Object))]
        [HarmonyPrefix]
        private static void CleanupLink(TreeLog __instance) =>
            DropTableManager.Cleanup(__instance, __instance.m_dropWhenDestroyed);
    }

    [HarmonyPatch(typeof(MineRock))]
    internal static class Patch_MineRock_Awake_InitDropContext
    {
        [HarmonyPatch(nameof(MineRock.Start))]
        [HarmonyPostfix]
        private static void SetLink(MineRock __instance) =>
            DropTableManager.Initialize(__instance, __instance.m_dropItems);

        [HarmonyPatch(nameof(MineRock.Destroy), typeof(UnityEngine.Object))]
        [HarmonyPrefix]
        private static void CleanupLink(MineRock __instance) =>
            DropTableManager.Cleanup(__instance, __instance.m_dropItems);
    }

    [HarmonyPatch(typeof(MineRock5))]
    internal static class Patch_MineRock5_Awake_InitDropContext
    {
        [HarmonyPatch(nameof(MineRock5.Start))]
        [HarmonyPostfix]
        private static void SetLink(MineRock5 __instance) =>
            DropTableManager.Initialize(__instance, __instance.m_dropItems);

        [HarmonyPatch(nameof(MineRock5.Destroy), typeof(UnityEngine.Object))]
        [HarmonyPrefix]
        private static void CleanupLink(MineRock5 __instance) =>
            DropTableManager.Cleanup(__instance, __instance.m_dropItems);
    }
}

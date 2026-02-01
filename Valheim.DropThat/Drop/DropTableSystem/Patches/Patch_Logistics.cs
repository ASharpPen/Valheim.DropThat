using DropThat.Drop.DropTableSystem.Managers;
using HarmonyLib;

namespace DropThat.Drop.DropTableSystem.Patches;

/// <summary>
/// Store reference from gameobject of DropTable to the DropTable itself,
/// and cleanup the reference when the gameobject is destroyed.
/// 
/// Since the DropTable is a standard object and not a unity engine Object, we need
/// to store what object it belongs with for later reference.
/// 
/// On finishing whatever function can roll and instantiate drops,
/// call cleanup to remove drop configs referenced for the session.
/// </summary>
internal static class Patch_Logistics
{
    [HarmonyPatch(typeof(Container))]
    internal static class Patch_Container
    {
        [HarmonyPatch(nameof(Container.Awake))]
        [HarmonyPrefix]
        private static void SetLink(Container __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_defaultItems);
    }

    [HarmonyPatch(typeof(DropOnDestroyed))]
    internal static class Patch_DropOnDestroyed
    {
        [HarmonyPatch(nameof(DropOnDestroyed.Awake))]
        [HarmonyPostfix]
        private static void SetLink(DropOnDestroyed __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_dropWhenDestroyed);


        [HarmonyPatch(nameof(DropOnDestroyed.OnDestroyed))]
        [HarmonyPostfix]
        private static void Clean(DropOnDestroyed __instance) =>
            DropTableSessionManager.Cleanup(__instance.m_dropWhenDestroyed);
    }

    [HarmonyPatch(typeof(LootSpawner))]
    internal static class Patch_LootSpawner
    {
        [HarmonyPatch(nameof(LootSpawner.Awake))]
        [HarmonyPostfix]
        private static void SetLink(LootSpawner __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_items);

        [HarmonyPatch(nameof(LootSpawner.UpdateSpawner))]
        [HarmonyPostfix]
        private static void Clean(LootSpawner __instance) =>
            DropTableSessionManager.Cleanup(__instance.m_items);
    }

    [HarmonyPatch(typeof(TreeBase))]
    internal static class Patch_TreeBase
    {
        [HarmonyPatch(nameof(TreeBase.Awake))]
        [HarmonyPostfix]
        private static void SetLink(TreeBase __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_dropWhenDestroyed);

        [HarmonyPatch(nameof(TreeBase.RPC_Damage))]
        [HarmonyPostfix]
        private static void Clean(TreeBase __instance) =>
            DropTableSessionManager.Cleanup(__instance.m_dropWhenDestroyed);
    }

    [HarmonyPatch(typeof(TreeLog))]
    internal static class Patch_TreeLog
    {
        [HarmonyPatch(nameof(TreeLog.Awake))]
        [HarmonyPostfix]
        private static void SetLink(TreeLog __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_dropWhenDestroyed);

        [HarmonyPatch(nameof(TreeLog.Destroy))]
        [HarmonyPostfix]
        private static void Clean(TreeLog __instance) =>
            DropTableSessionManager.Cleanup(__instance.m_dropWhenDestroyed);
    }

    [HarmonyPatch(typeof(MineRock))]
    internal static class Patch_MineRock
    {
        [HarmonyPatch(nameof(MineRock.Start))]
        [HarmonyPostfix]
        private static void SetLink(MineRock __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_dropItems);

        [HarmonyPatch(nameof(MineRock.RPC_Hit))]
        [HarmonyPostfix]
        private static void Clean(MineRock __instance) =>
            DropTableSessionManager.Cleanup(__instance.m_dropItems);
    }

    [HarmonyPatch(typeof(MineRock5))]
    internal static class Patch_MineRock5
    {
        [HarmonyPatch(nameof(MineRock5.Awake))]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.LowerThanNormal)] // Give time for the name fix patch to apply.
        private static void SetLink(MineRock5 __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_dropItems);

        [HarmonyPatch(nameof(MineRock5.DamageArea))]
        [HarmonyPostfix]
        private static void Clean(MineRock5 __instance) =>
            DropTableSessionManager.Cleanup(__instance.m_dropItems);
    }
}

using DropThat.Drop.DropTableSystem.Managers;
using HarmonyLib;

namespace DropThat.Drop.DropTableSystem.Patches;

/// <summary>
/// 1) Init: Store references to the DropTable's gameobject and prepare
/// the DropTable with initial drop configurations if a template exist.
/// 2) Begin: Track gameobject when starting a potential drop session.
/// 3) Modify: Drop modifiers applied to rolled items.
/// 4) End: Clean up references when the potential drop session has ended.
/// 
/// Drop modifications in step 3) are handled in a separate patch 
/// to allow better patch control when compatibility is needed.
/// </summary>
internal static class Patch_Logistics
{
    [HarmonyPatch(typeof(Container))]
    internal static class Patch_Container
    {
        [HarmonyPatch(nameof(Container.Awake))]
        [HarmonyPrefix]
        private static void Init(Container __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_defaultItems);

        [HarmonyPatch(nameof(Container.AddDefaultItems))]
        [HarmonyPrefix]
        private static void Begin(Container __instance) =>
            DropTableSessionManager.StartSession(__instance);

        [HarmonyPatch(nameof(Container.AddDefaultItems))]
        [HarmonyPostfix]
        private static void End() =>
            DropTableSessionManager.EndSession();
    }

    [HarmonyPatch(typeof(DropOnDestroyed))]
    internal static class Patch_DropOnDestroyed
    {
        [HarmonyPatch(nameof(DropOnDestroyed.Awake))]
        [HarmonyPostfix]
        private static void Init(DropOnDestroyed __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_dropWhenDestroyed);

        [HarmonyPatch(nameof(DropOnDestroyed.OnDestroyed))]
        [HarmonyPrefix]
        private static void Begin(DropOnDestroyed __instance) =>
            DropTableSessionManager.StartSession(__instance);

        [HarmonyPatch(nameof(DropOnDestroyed.OnDestroyed))]
        [HarmonyPostfix]
        private static void End() =>
            DropTableSessionManager.EndSession();
    }

    [HarmonyPatch(typeof(LootSpawner))]
    internal static class Patch_LootSpawner
    {
        [HarmonyPatch(nameof(LootSpawner.Awake))]
        [HarmonyPostfix]
        private static void Init(LootSpawner __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_items);

        [HarmonyPatch(nameof(LootSpawner.UpdateSpawner))]
        [HarmonyPrefix]
        private static void Begin(LootSpawner __instance) =>
            DropTableSessionManager.StartSession(__instance);

        [HarmonyPatch(nameof(LootSpawner.UpdateSpawner))]
        [HarmonyPostfix]
        private static void End() =>
            DropTableSessionManager.EndSession();
    }

    [HarmonyPatch(typeof(TreeBase))]
    internal static class Patch_TreeBase
    {
        [HarmonyPatch(nameof(TreeBase.Awake))]
        [HarmonyPostfix]
        private static void Init(TreeBase __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_dropWhenDestroyed);

        [HarmonyPatch(nameof(TreeBase.RPC_Damage))]
        [HarmonyPrefix]
        private static void Begin(TreeBase __instance) =>
            DropTableSessionManager.StartSession(__instance);

        [HarmonyPatch(nameof(TreeBase.RPC_Damage))]
        [HarmonyPostfix]
        private static void End() =>
            DropTableSessionManager.EndSession();
    }

    [HarmonyPatch(typeof(TreeLog))]
    internal static class Patch_TreeLog
    {
        [HarmonyPatch(nameof(TreeLog.Awake))]
        [HarmonyPostfix]
        private static void Init(TreeLog __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_dropWhenDestroyed);

        [HarmonyPatch(nameof(TreeLog.Destroy))]
        [HarmonyPrefix]
        private static void Begin(TreeLog __instance) =>
            DropTableSessionManager.StartSession(__instance);

        [HarmonyPatch(nameof(TreeLog.Destroy))]
        [HarmonyPostfix]
        private static void End() =>
            DropTableSessionManager.EndSession();
    }

    [HarmonyPatch(typeof(MineRock))]
    internal static class Patch_MineRock
    {
        [HarmonyPatch(nameof(MineRock.Start))]
        [HarmonyPostfix]
        private static void Init(MineRock __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_dropItems);

        [HarmonyPatch(nameof(MineRock.RPC_Hit))]
        [HarmonyPrefix]
        private static void Begin(MineRock __instance) =>
            DropTableSessionManager.StartSession(__instance);

        [HarmonyPatch(nameof(MineRock.RPC_Hit))]
        [HarmonyPostfix]
        private static void End() =>
            DropTableSessionManager.EndSession();
    }

    [HarmonyPatch(typeof(MineRock5))]
    internal static class Patch_MineRock5
    {
        [HarmonyPatch(nameof(MineRock5.Awake))]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.LowerThanNormal)] // Give time for the name fix patch to apply.
        private static void Init(MineRock5 __instance) =>
            DropTableSessionManager.Initialize(__instance, __instance.m_dropItems);

        [HarmonyPatch(nameof(MineRock5.DamageArea))]
        [HarmonyPrefix]
        private static void Begin(MineRock5 __instance) =>
            DropTableSessionManager.StartSession(__instance);

        [HarmonyPatch(nameof(MineRock5.DamageArea))]
        [HarmonyPostfix]
        private static void End() =>
            DropTableSessionManager.EndSession();
    }
}

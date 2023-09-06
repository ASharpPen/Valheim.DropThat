using System.Collections.Generic;
using System.Reflection.Emit;
using DropThat.Drop.DropTableSystem.Managers;
using DropThat.Utilities;
using HarmonyLib;

namespace DropThat.Drop.DropTableSystem.Patches;

/// <summary>
/// When drops are rolled as GameObject prefabs, we need to wait until
/// they are instantiated, and then apply the Modifiers to their ItemDrop component.
/// 
/// We have also previously wrapped most prefabs in a custom Wrapper GameObject, that
/// is lets us track the configurations from roll-time to instantiation.
/// 
/// While the Wrapper object is built for disaster scenarios of complete overhauls done
/// by other mods, it is still preferable to properly unwrap and modify the instantiated prefab.
/// </summary>
internal static class Patch_ModifyInstantiatedDrops
{
    [HarmonyPatch(typeof(DropOnDestroyed))]
    internal static class Patch_DropOnDestroyed_OnDestroyed
    {
        [HarmonyPatch(nameof(DropOnDestroyed.OnDestroyed))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) =>
            instructions.InsertDropManagementInstructions();
    }

    [HarmonyPatch(typeof(LootSpawner))]
    internal static class Patch_LootSpawner_UpdateSpawner
    {
        [HarmonyPatch(nameof(LootSpawner.UpdateSpawner))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) =>
            instructions.InsertDropManagementInstructions();
    }

    [HarmonyPatch(typeof(TreeLog))]
    internal static class Patch_TreeLog_Destroy
    {
        [HarmonyPatch(nameof(TreeLog.Destroy))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) =>
            instructions.InsertDropManagementInstructions();
    }

    [HarmonyPatch(typeof(TreeBase))]
    internal static class Patch_TreeBase_RPC_Damage
    {
        [HarmonyPatch(nameof(TreeBase.RPC_Damage))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) =>
            instructions.InsertDropManagementInstructions();
    }

    [HarmonyPatch(typeof(MineRock))]
    internal static class Patch_MineRock_RPC_Hit
    {
        [HarmonyPatch(nameof(MineRock.RPC_Hit))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) =>
            instructions.InsertDropManagementInstructions();
    }

    [HarmonyPatch(typeof(MineRock5))]
    internal static class Patch_MineRock5_DamageArea
    {
        [HarmonyPatch(nameof(MineRock5.DamageArea))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) =>
            instructions.InsertDropManagementInstructions();
    }

    private static IEnumerable<CodeInstruction> InsertDropManagementInstructions(this IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            // Move to right after drop prefab gets retrieved from list
            .MatchForward(false,
                new CodeMatch(OpCodes.Call, ReflectionUtils.InstantiateGameObjectMethod))
            .Advance(-2)
            // Insert unwrapping call in preparation for instantiation.
            .InsertAndAdvance(Transpilers.EmitDelegate(DropTableManager.UnwrapDrop))
            // Move to right after drop is instantiated, and duplicate reference
            .MatchForward(true,
                new CodeMatch(OpCodes.Call, ReflectionUtils.InstantiateGameObjectMethod))
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Dup))
            // Insert own call
            .InsertAndAdvance(Transpilers.EmitDelegate(DropTableManager.ModifyInstantiatedDrop))
            .InstructionEnumeration();
    }
}

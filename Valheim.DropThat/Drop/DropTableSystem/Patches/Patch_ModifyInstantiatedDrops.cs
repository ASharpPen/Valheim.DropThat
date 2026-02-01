using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DropThat.Drop.DropTableSystem.Managers;
using DropThat.Utilities;
using HarmonyLib;
using ThatCore.Extensions;
using UnityEngine;

namespace DropThat.Drop.DropTableSystem.Patches;

/// <summary>
/// When drops are rolled as GameObject prefabs, we need to wait until
/// they are instantiated, and then apply the Modifiers to their ItemDrop component.
/// </summary>
[HarmonyPatch]
internal static class Patch_ModifyInstantiatedDrops
{
    private static MethodInfo GetDropListMethod = AccessTools.Method(typeof(DropTable), nameof(DropTable.GetDropList));

    [HarmonyPatch(typeof(DropOnDestroyed))]
    internal static class Patch_DropOnDestroyed_OnDestroyed
    {
        [HarmonyPatch(nameof(DropOnDestroyed.OnDestroyed))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) =>
            instructions.InsertDropManagementInstructions<DropOnDestroyed>(ModifyInstantiatedObjectDrop);

        private static GameObject ModifyInstantiatedObjectDrop(GameObject drop, DropOnDestroyed instance, int index)
        {
            DropTableSessionManager.ModifyInstantiatedObjectDrop(drop, instance.m_dropWhenDestroyed, index);
            return drop;
        }
    }

    [HarmonyPatch(typeof(LootSpawner))]
    internal static class Patch_LootSpawner_UpdateSpawner
    {
        [HarmonyPatch(nameof(LootSpawner.UpdateSpawner))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) =>
            instructions.InsertDropManagementInstructions<LootSpawner>(ModifyInstantiatedObjectDrop);

        private static GameObject ModifyInstantiatedObjectDrop(GameObject drop, LootSpawner instance, int index)
        {
            DropTableSessionManager.ModifyInstantiatedObjectDrop(drop, instance.m_items, index);
            return drop;
        }
    }

    [HarmonyPatch(typeof(TreeLog))]
    internal static class Patch_TreeLog_Destroy
    {
        [HarmonyPatch(nameof(TreeLog.Destroy))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) =>
            new CodeMatcher(instructions)
            // Move to after drop list is retrieved
            .MatchForward(true,
                new CodeMatch(OpCodes.Callvirt, GetDropListMethod))
            // Move forward to loop index being initialized
            .MatchEndForward(OpCodes.Ldc_I4_0)
            .Advance(1)
            // Expecting the index to be stored in stloc immediately after, get instruction for loading later
            .GetInstruction(out CodeInstruction index)
            // Move to right after drop is instantiated, and duplicate reference
            .MatchForward(true,
                new CodeMatch(OpCodes.Call, ReflectionUtils.InstantiateGameObjectMethod))
            .Advance(1)
            // Insert own call, expecting gameobject to be on top of stack
            .InsertAndAdvance(OpCodes.Ldarg_0)
            .InsertAndAdvance(index.GetLdlocFromStLoc())
            .InsertAndAdvance(Transpilers.EmitDelegate(ModifyInstantiatedObjectDrop))
            .InstructionEnumeration();

        private static GameObject ModifyInstantiatedObjectDrop(GameObject drop, TreeLog instance, int index)
        {
            DropTableSessionManager.ModifyInstantiatedObjectDrop(drop, instance.m_dropWhenDestroyed, index);
            return drop;
        }
    }

    [HarmonyPatch(typeof(TreeBase))]
    internal static class Patch_TreeBase_RPC_Damage
    {
        [HarmonyPatch(nameof(TreeBase.RPC_Damage))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) =>
            instructions.InsertDropManagementInstructions<TreeBase>(ModifyInstantiatedObjectDrop);

        private static GameObject ModifyInstantiatedObjectDrop(GameObject drop, TreeBase instance, int index)
        {
            DropTableSessionManager.ModifyInstantiatedObjectDrop(drop, instance.m_dropWhenDestroyed, index);
            return drop;
        }
    }

    [HarmonyPatch(typeof(MineRock))]
    internal static class Patch_MineRock_RPC_Hit
    {
        /// <summary>
        /// Index of foreach loop iterating over DropTable.GetDropList results.
        /// </summary>
        private static int LoopIndex;

        private static void InitLoopIndex() =>
            LoopIndex = -1;

        private static void IncrementLoopIndex() =>
            LoopIndex++;

        private static GameObject ModifyInstantiatedObjectDrop(GameObject obj, MineRock instance)
        {
            DropTableSessionManager.ModifyInstantiatedObjectDrop(obj, instance.m_dropItems, LoopIndex);
            return obj;
        }

        [HarmonyPatch(nameof(MineRock.RPC_Hit))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) =>
            new CodeMatcher(instructions)
            .Start()
            // Insert loop index initalizer. Does not matter where, as long as its before the increment.
            .InsertAndAdvance(Transpilers.EmitDelegate(InitLoopIndex))
            // Move to right after drop is instantiated
            .MatchForward(true,
                new CodeMatch(OpCodes.Call, ReflectionUtils.InstantiateGameObjectMethod))
            .Advance(1)
            // Insert own call, expecting gameobject to be on top of stack
            .InsertAndAdvance(OpCodes.Ldarg_0)
            .InsertAndAdvance(Transpilers.EmitDelegate(ModifyInstantiatedObjectDrop))
            // Move forward to loop being iterated, and increment our custom index
            .MatchEndForward(new CodeMatch(OpCodes.Call, ReflectionUtils.ListGameObjectMoveNextMethod))
            .Advance(1)
            .InsertAndAdvance(Transpilers.EmitDelegate(IncrementLoopIndex))
            .InstructionEnumeration();
    }

    [HarmonyPatch(typeof(MineRock5))]
    internal static class Patch_MineRock5_DamageArea
    {
        /// <summary>
        /// Index of foreach loop iterating over DropTable.GetDropList results.
        /// </summary>
        private static int LoopIndex;

        private static void InitLoopIndex() =>
            LoopIndex = -1;

        private static void IncrementLoopIndex() =>
            LoopIndex++;

        private static GameObject ModifyInstantiatedObjectDrop(GameObject obj, MineRock5 instance)
        {
            DropTableSessionManager.ModifyInstantiatedObjectDrop(obj, instance.m_dropItems, LoopIndex);
            return obj;
        }

        [HarmonyPatch(nameof(MineRock5.DamageArea))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) =>
            new CodeMatcher(instructions)
            .Start()
            // Insert loop index initalizer. Does not matter where, as long as its before the increment.
            .InsertAndAdvance(Transpilers.EmitDelegate(InitLoopIndex))
            // Move to right after drop is instantiated
            .MatchForward(true,
                new CodeMatch(OpCodes.Call, ReflectionUtils.InstantiateGameObjectMethod))
            .Advance(1)
            // Insert own call, expecting gameobject to be on top of stack
            .InsertAndAdvance(OpCodes.Ldarg_0)
            .InsertAndAdvance(Transpilers.EmitDelegate(ModifyInstantiatedObjectDrop))
            // Move forward to loop being iterated, and increment our custom index
            .MatchEndForward(new CodeMatch(OpCodes.Call, ReflectionUtils.ListGameObjectMoveNextMethod))
            .Advance(1)
            .InsertAndAdvance(Transpilers.EmitDelegate(IncrementLoopIndex))
            .InstructionEnumeration();
    }

    private static IEnumerable<CodeInstruction> InsertDropManagementInstructions<T>(
        this IEnumerable<CodeInstruction> instructions,
        Func<GameObject, T, int, GameObject> modifyAction) =>
            new CodeMatcher(instructions)
            // Move to right before drop is instantiated
            .MatchForward(false,
                new CodeMatch(OpCodes.Call, ReflectionUtils.InstantiateGameObjectMethod))
            // Move back to where the index used for retrieving drop item from list is loaded
            .Advance(-4)
            // Get the index instruction, to load again later.
            .GetInstruction(out CodeInstruction loadIndex)
            // Move to right after drop is instantiated
            .MatchForward(true,
                new CodeMatch(OpCodes.Call, ReflectionUtils.InstantiateGameObjectMethod))
            .Advance(1)
            // Insert own call, expecting gameobject to be on top of stack
            .InsertAndAdvance(OpCodes.Ldarg_0)
            .InsertAndAdvance(loadIndex)
            .InsertAndAdvance(Transpilers.EmitDelegate(modifyAction))
            .InstructionEnumeration();
}

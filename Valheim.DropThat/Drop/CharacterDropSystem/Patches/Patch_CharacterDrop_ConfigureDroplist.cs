using System.Collections.Generic;
using System.Reflection.Emit;
using DropThat.Drop.CharacterDropSystem.Managers;
using HarmonyLib;
using UnityEngine;

namespace DropThat.Drop.CharacterDropSystem.Patches;

[HarmonyPatch]
internal static class Patch_CharacterDrop_ConfigureDroplist
{
    /// <summary>
    /// 1. Drop table is instantiated.
    /// </summary>
    [HarmonyPatch(typeof(CharacterDrop), nameof(CharacterDrop.Start))]
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    private static void Init(CharacterDrop __instance)
    {
        DropTableManager.Initialize(__instance);
    }

    /// <summary>
    /// 2. Grab the CharacterDrop.m_drops list as it is being retrieved 
    /// and return a filtered list instead.
    /// </summary>
    [HarmonyPatch(typeof(CharacterDrop), nameof(CharacterDrop.GenerateDropList))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ConfigureDrops(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false, new CodeMatch(OpCodes.Ldfld, AccessTools.DeclaredField(typeof(CharacterDrop), nameof(CharacterDrop.m_drops))))
            .SetInstructionAndAdvance(Transpilers.EmitDelegate(DropTableManager.FilterDrops))
            .InstructionEnumeration();
    }

    /// <summary>
    /// 3. Ensure each drop amount does not go above the configured limits.
    /// </summary>
    [HarmonyPatch(typeof(CharacterDrop), nameof(CharacterDrop.GenerateDropList))]
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    private static void LimitAmounts(CharacterDrop __instance, List<KeyValuePair<GameObject, int>> __result)
    {
        DropTableManager.LimitDropAmounts(__instance, __result);
    }
}

using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Valheim.DropThat.Drop.CharacterDropSystem.Patches
{
    [HarmonyPatch(typeof(CharacterDrop))]
    public static class Patch_CharacterDrop_GenerateDropList_FilterDrops
    {
        [HarmonyPatch(nameof(CharacterDrop.GenerateDropList))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> FilterDrops(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch(OpCodes.Ldfld, AccessTools.DeclaredField(typeof(CharacterDrop), nameof(CharacterDrop.m_drops))))
                .SetInstructionAndAdvance(Transpilers.EmitDelegate(ConditionChecker.FilterOnDeath))
                .InstructionEnumeration();

        }
    }
}

#if TRUE && DEBUG

using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using Valheim.DropThat.Core;
using Valheim.DropThat.Drop.CharacterDropSystem.Patches;

namespace Valheim.DropThat.Debugging
{
    [HarmonyPatch(typeof(CharacterDrop))]
    internal static class Patch_CharacterDrop
    {
        [HarmonyPatch("OnDeath")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false, 
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(CharacterDrop), "m_character")))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_CharacterDrop), nameof(InvestigateList))))
                .InstructionEnumeration();
        }

        private static List<KeyValuePair<GameObject, int>> InvestigateList(List<KeyValuePair<GameObject, int>> drops)
        {
            Log.LogDebug("OnDeath");
            TempDropListCache.GetDrops(drops);

            return drops;
        }
    }
}

#endif
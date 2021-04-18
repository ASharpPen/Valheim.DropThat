using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Valheim.DropThat.Conditions;

namespace Valheim.DropThat.Patches
{   
    [HarmonyPatch(typeof(CharacterDrop))]
    public static class CharacterDropGenerateDropListPatch
    {
        private static MethodInfo DropFilter = AccessTools.Method(typeof(ConditionChecker), nameof(ConditionChecker.FilterOnDeath), new[] { typeof(CharacterDrop) });

        [HarmonyPatch(nameof(CharacterDrop.GenerateDropList))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> FilterDrops(IEnumerable<CodeInstruction> instructions)
        {
            var operandToReplace = AccessTools.DeclaredField(typeof(CharacterDrop), nameof(CharacterDrop.m_drops));

            var resultInstructions = new List<CodeInstruction>();

            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; ++i)
            {
                CodeInstruction instruction = codes[i];

                if (instruction.opcode == OpCodes.Ldfld)
                {
                    if (instruction.OperandIs(operandToReplace))
                    {
                        resultInstructions.Add(new CodeInstruction(OpCodes.Callvirt, DropFilter));
                    }
                    else
                    {
                        resultInstructions.Add(codes[i]);
                    }
                }
                else
                {
                    resultInstructions.Add(codes[i]);
                }
            }

            return resultInstructions;
        }
    }
}

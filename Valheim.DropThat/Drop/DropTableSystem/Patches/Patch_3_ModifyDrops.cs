using System.Collections.Generic;
using System.Reflection.Emit;
using DropThat.Drop.DropTableSystem.Managers;
using HarmonyLib;
using ThatCore.Extensions;

namespace DropThat.Drop.DropTableSystem.Patches;

internal static class Patch_3_ModifyDrops
{
    [HarmonyPatch(typeof(Container))]
    internal static class Patch_Container_AddDefaultItems
    {
        [HarmonyPatch(nameof(Container.AddDefaultItems))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> InsertItemManagement(IEnumerable<CodeInstruction> instructions)
        {
            var addItem = AccessTools.Method(
                typeof(Inventory),
                nameof(Inventory.AddItem),
                new[] { typeof(ItemDrop.ItemData) });

            return new CodeMatcher(instructions)
                // Move to right before drop is added to inventory
                .MatchForward(false, new CodeMatch(OpCodes.Callvirt, addItem))
                // Run modifications, and replace previous ItemData on stack with modified one.
                .InsertAndAdvance(OpCodes.Ldarg_0)
                .InsertAndAdvance(Transpilers.EmitDelegate(DropTableManager.ModifyContainerItem))
                .InstructionEnumeration();
        }
    }
}

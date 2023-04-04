using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using ThatCore.Extensions;

namespace DropThat.Drop.DropTableSystem.Patches;

/// <summary>
/// Turns out. DropTable's really want to do things the hard way.
/// DropTable.DropData is a struct, meaning we can't keep track of
/// the reference to individual objects, since it will keep changing
/// as copies are made.
/// 
/// The code maps every DropData to a proper ItemDrop object, but does
/// it as a side-effect in a different method, meaning we will have
/// to trace it.
/// 
/// On top of that, lists are copied multiple times, and other mods
/// are expected to change the list reference too.
/// 
/// On the positive side, the most likely mod conflict will be CLLC
/// which has all its changes done as postfixes, and keep all references
/// to the resulting ItemDrops intact. This means if we can keep track
/// of the ItemDrop, we can keep configs associated.
/// 
/// Now, we don't want to fuck with pre- or postfixes. So for this setup
/// we are going to assume and hope that I am the only crazy person
/// transpiling the insides of the method. To anyone who is also trying
/// to get into here, sorry.
/// 
/// Instead of prefixing and derouting the method, this patch will
/// skip the original code and use a re-implemented version
/// that uses DropTemplates objects.
/// 
/// The re-implemented version is not entirely desireable by anyone,
/// but it will keep existing behaviour consistent, which follows
/// the general philosophy of Drop That to only change behaviour when
/// specifically told to do so.
/// 
/// </summary>
[HarmonyPatch]
internal static class Patch_2_OnDrop
{
    [HarmonyPatch(nameof(DropTable.GetDropListItems))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> DropListItemsOverhaul(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        return new CodeMatcher(instructions, generator)
            // Move to start, and insert overhaul
            .Start()
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Old_Patch_DropTable_GetDrops_Overhaul), nameof(GetDropListItems))))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ret))
            // Set label for where the original code would have started to run.
            .CreateLabel(out Label originalStart)
            // Move back again to start, and insert check and skip of overhaul.
            .Start()
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Old_Patch_DropTable_GetDrops_Overhaul), nameof(UseOriginal))))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Brtrue, originalStart))
            .InstructionEnumeration();
    }
}

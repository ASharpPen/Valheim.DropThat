using System;
using System.Collections.Generic;
using DropThat.Drop.DropTableSystem.Managers;
using HarmonyLib;
using ThatCore.Logging;
using UnityEngine;

namespace DropThat.Drop.DropTableSystem.Patches;

/// <summary>
/// DropTable is a real messy piece of work. To avoid transpiling in and
/// handling indexes, tracking rolled drops and their configs, overriding field lookups and more, 
/// its easier to simply overhaul the whole thing.
/// 
/// Sorry to whomever might be reading this one.
/// </summary>
[HarmonyPatch]
internal static class Patch_RollDrops
{
    [HarmonyPatch(typeof(DropTable), nameof(DropTable.GetDropListItems))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    private static bool DropListItemsOverhaul(DropTable __instance, ref List<ItemDrop.ItemData> __result)
    {
        try
        {
            if (DropTableManager.HasChanges(__instance))
            {
                __result = DropTableManager.GenerateItemDrops(__instance);

                // Skip original
                return false;
            }
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error while attempting to generate drops. Running vanilla code instead.", e);
        }

        // No changes detected. Run default vanilla code as usual.
        return true;
    }

    [HarmonyPatch(typeof(DropTable), nameof(DropTable.GetDropList), new[] { typeof(int) })]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    private static bool DropListOverhaul(DropTable __instance, ref List<GameObject> __result)
    {
        try
        {
            if (DropTableManager.HasChanges(__instance))
            {
                __result = DropTableManager.GenerateDrops(__instance);

                // Skip original
                return false;
            }
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error while attempting to generate drops. Running vanilla code instead.", e);
        }

        return true;
    }
}

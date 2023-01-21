using HarmonyLib;
using System;
using DropThat.Caches;
using ThatCore.Logging;
using ThatCore.Extensions;

namespace DropThat.Creature;

[HarmonyPatch(typeof(Character))]
internal static class Patch_Character_RecordHit
{
    [HarmonyPatch(nameof(Character.ApplyDamage))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)] // Let other prefixes apply their changes, so we can properly pick up the final result.
    private static void RecordLastHit(Character __instance, HitData hit)
    {
        try
        {
            Log.DevelopmentOnly("Applying damage.");

            if (__instance.IsNull())
            {
                return;
            }

            var zdo = ZdoCache.GetZDO(__instance);

            if (zdo is null)
            {
                Log.DevelopmentOnly($"[{__instance.name}] Skipping record of last hit.");
                return;
            }

            Log.DevelopmentOnly($"[{__instance.name}] Recording hit.");

            DamageRecords.RecordLastHit.SetLastHit(__instance, hit);
            DamageRecords.RecordRecentHits.SetRecentHit(__instance, hit);
            StatusRecords.RecordLastStatus.SetLastStatus(__instance);
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error during attempt at recording last hit", e);
        }
    }
}

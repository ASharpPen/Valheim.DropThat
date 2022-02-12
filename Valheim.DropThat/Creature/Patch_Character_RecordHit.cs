using HarmonyLib;
using System;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Creature
{
    [HarmonyPatch(typeof(Character))]
    internal static class Patch_Character_RecordHit
    {
        private const string ZdoHealth = "health";

        [HarmonyPatch(nameof(Character.ApplyDamage))]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)] // Let other prefixes apply their changes, so we can properly pick up the final result.
        private static void RecordLastHit(Character __instance, HitData hit)
        {
            try
            {
#if DEBUG
                Log.LogTrace("Applying damage.");
#endif

                var zdo = ZdoCache.GetZDO(__instance.gameObject);

                if (zdo is null)
                {
#if DEBUG
                    Log.LogTrace($"[{__instance.name}] Skipping record of last hit.");
#endif
                    return;
                }

#if DEBUG
                Log.LogTrace($"[{__instance.name}] Recording hit.");
#endif

                DamageRecords.RecordLastHit.SetLastHit(__instance, hit);
                StatusRecords.RecordLastStatus.SetLastStatus(__instance);
            }
            catch(Exception e)
            {
                Log.LogError("Error during attempt at recording last hit", e);
            }
        }
    }
}

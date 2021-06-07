using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valheim.DropThat.Configuration;
using Valheim.DropThat.Debugging;

namespace Valheim.DropThat.Locations
{
    [HarmonyPatch(typeof(ZoneSystem))]
    internal static class ZoneSystemStartPatch
    {
        [HarmonyPatch(nameof(ZoneSystem.Load))]
        [HarmonyPostfix]
        private static void LoadLocations(Dictionary<Vector2i, ZoneSystem.LocationInstance> ___m_locationInstances)
        {
            if (___m_locationInstances is not null && ___m_locationInstances.Count > 0)
            {
                LocationHelper.SetLocationInstances(___m_locationInstances);
            }
        }

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void PrintLocations(ZoneSystem __instance)
        {
            if (__instance.m_locations is null)
            {
                return;
            }

            if (ConfigurationManager.GeneralConfig.WriteLocationsToFile)
            {
                LocationsFileWriter.WriteToList(__instance.m_locations);
            }
        }
    }
}

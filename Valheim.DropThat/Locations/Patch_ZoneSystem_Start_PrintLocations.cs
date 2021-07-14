using HarmonyLib;
using Valheim.DropThat.Configuration;
using Valheim.DropThat.Debugging;

namespace Valheim.DropThat.Locations
{
    [HarmonyPatch(typeof(ZoneSystem))]
    internal static class Patch_ZoneSystem_Start_PrintLocations
    {
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

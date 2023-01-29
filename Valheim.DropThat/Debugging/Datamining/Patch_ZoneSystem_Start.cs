using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using DropThat.Configuration;

namespace DropThat.Debugging.Datamining;

/// <summary>
/// Could have loaded in at ZNetScene Awake, 
/// but we want to be later to let other mods fill in their prefabs.
/// </summary>
[HarmonyPatch(typeof(ZoneSystem))]
internal static class Patch_ZoneSystem_Start
{
    [HarmonyPatch(nameof(ZoneSystem.Start))]
    [HarmonyPostfix]
    private static void WriteDebugFiles(ZoneSystem __instance)
    {
        var prefabs = ZNetScene.instance.m_prefabs;

        if (prefabs is not null)
        {
            var prefabsWithDropTables = new List<Tuple<GameObject, CharacterDrop>>();

            foreach (var prefab in prefabs)
            {
                var characterDrop = prefab.GetComponent<CharacterDrop>();
                if (characterDrop)
                {
                    prefabsWithDropTables.Add(new Tuple<GameObject, CharacterDrop>(prefab, characterDrop));
                }
            }

            if (ConfigurationManager.GeneralConfig?.WriteCharacterDropsToFile?.Value == true)
            {
                CharacterDropFileWriter.WriteToFile(prefabsWithDropTables);
            }

            if (ConfigurationManager.GeneralConfig?.WriteCreatureItemsToFile?.Value == true)
            {
                CreatureItemFileWriter.WriteToFile(prefabsWithDropTables);
            }
        }

        if (ConfigurationManager.GeneralConfig?.WriteLocationsToFile.Value == true)
        {
            LocationFileWriter.WriteToFile(__instance.m_locations);
        }
    }
}

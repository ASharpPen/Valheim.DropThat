using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Valheim.DropThat.Configuration;

namespace Valheim.DropThat.Debugging
{
    /// <summary>
    /// Could have loaded in at ZNetScene Awake, 
    /// but we want to be later to let other mods fill in their prefabs.
    /// </summary>
    [HarmonyPatch(typeof(ZoneSystem))]
    public static class DropTableDumperPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void ScanPrefabs(ZoneSystem __instance)
        {
            var prefabs = ZNetScene.instance.m_prefabs;

            {
                var prefabsWithDropTables = new List<Tuple<GameObject, CharacterDrop>>();
                    
                foreach(var prefab in prefabs)
                {
                    var characterDrop = prefab.GetComponent<CharacterDrop>();
                    if (characterDrop)
                    {
                        prefabsWithDropTables.Add(new Tuple<GameObject, CharacterDrop>(prefab, characterDrop));
                    }
                }

                if (ConfigurationManager.GeneralConfig?.WriteDefaultDropTableToFile?.Value == true)
                {
                    DropTableFileWriter.WriteToFile(prefabsWithDropTables);
                }

                if (ConfigurationManager.GeneralConfig?.WriteCreatureItemsToFile?.Value == true)
                {
                    CreatureItemFileWriter.WriteToFile(prefabsWithDropTables);
                }
            }
        }
    }
}

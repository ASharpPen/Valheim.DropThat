using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Patches
{
    [HarmonyPatch(typeof(CharacterDrop))]
    public static class Patch_CharacterDrop_GenerateDropList_CarryDropExtended
    {
        private static MethodInfo Anchor = AccessTools.Method(typeof(List<CharacterDrop.Drop>.Enumerator), "MoveNext");
        private static MethodInfo CarryExtendedMethod = AccessTools.Method(typeof(Patch_CharacterDrop_GenerateDropList_CarryDropExtended), nameof(CarryExtended));

        [HarmonyPatch(nameof(CharacterDrop.GenerateDropList))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> CarryItemConfigs(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(true, //Move to right before moving enumeration head and after having added the most recent key-value pair to results.
                    new CodeMatch(OpCodes.Ldloca_S),
                    new CodeMatch(OpCodes.Call, Anchor))
                .Advance(1)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0)) //Loads the list with resulting key-value pairs.
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_3)) //Load current CharacterDrop.Drop
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0)) //Load instance
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, CarryExtendedMethod))
                .InstructionEnumeration();
        }

        private static void CarryExtended(List<KeyValuePair<GameObject, int>> dropItems, CharacterDrop.Drop drop, CharacterDrop characterDrop)
        {
            if(dropItems is null)
            {
#if DEBUG
                Log.LogWarning("Unable to carry drop due to dropitems being null.");
#endif
            }

            if (drop is null)
            {
#if DEBUG
                Log.LogWarning($"Unable to carry drop due to being null for {characterDrop}.");
#endif
                return;
            }

            var extended = DropExtended.GetExtension(drop);

            if (extended is not null && dropItems is not null)
            {
#if DEBUG
                //Log.LogDebug($"Carrying configs for drop {extended.Config.SectionKey}:{characterDrop.GetHashCode()}");
                //Log.LogDebug($"Carrying configs for drop {drop.m_prefab.name}");
#endif
                TempDropListCache.SetDrop(characterDrop, dropItems.Count - 1, extended);
            }
            /*
#if DEBUG
            else if (dropItems is null)
            {
                Log.LogDebug("Disregard. No items to carry");
                //Log.LogDebug($"Carrying configs for drop {drop.m_prefab.name}");
            }
            else if(extended is null)
            {
                Log.LogDebug($"Disregard. No config to carry for item {drop}:{drop.m_prefab?.name}");
            }
#endif
            */
        }
    }
}

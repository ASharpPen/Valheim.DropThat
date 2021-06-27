using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Patches
{
    /// <summary>
    /// This part of the code is extremely busy with a number of mods wanting in on the item drop action.
    /// 
    /// The code is always discarding most information that could be used to associate drops with configs, 
    /// and we need the configs for later modifications when the item is spawned in.
    /// 
    /// Therefore, the strategy here is relying on assigning configs to index of drop being generated.
    /// The index is associated with the CharacterDrop component itself, to have a stable reference.
    /// 
    /// It would be preferable to use the List generated itself, but that solution has conflicts with at least CLLC,
    /// which will assign a new list object before returning.
    /// 
    /// To combat this, MoveConfigReferenceFromComponentToDrop will be attempting to run as one of the final changes to the code,
    /// and apply the component-referenced indexes and configs to the at that point resulting list.
    /// 
    /// This whole setup is still extremely volatile, but there are almost no carriers with unique identifiers available 
    /// for keeping the config references associated with the item to be spawned :s
    /// </summary>
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
                .MatchForward(false, //Move to right before moving enumeration head and after having added the most recent key-value pair to results.
                    new CodeMatch(OpCodes.Ldloca_S),
                    new CodeMatch(OpCodes.Call, Anchor))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0)) //Loads the list with resulting key-value pairs.
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_3)) //Load current CharacterDrop.Drop
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0)) //Load instance
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, CarryExtendedMethod))
                .InstructionEnumeration();
        }

        [HarmonyPatch(nameof(CharacterDrop.GenerateDropList))]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        private static void MoveConfigReferenceFromComponentToDrop(CharacterDrop __instance, List<KeyValuePair<GameObject, int>> __result)
        {
            var instanceReferences = TempDropListCache.GetDrops(__instance);

            if (instanceReferences is not null)
            {
                //Re-associate result with configs.
                foreach (var reference in instanceReferences.ConfigByIndex)
                {
                    TempDropListCache.SetDrop(__result, reference.Key, reference.Value);
                }
            }
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
                Log.LogDebug($"Carrying configs for drop {extended.Config.SectionKey}:{characterDrop.GetHashCode()}");
                Log.LogDebug($"Carrying configs for drop {drop.m_prefab.name}");
#endif
                TempDropListCache.SetDrop(characterDrop, dropItems.Count - 1, extended);
            }
            
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
        }
    }
}

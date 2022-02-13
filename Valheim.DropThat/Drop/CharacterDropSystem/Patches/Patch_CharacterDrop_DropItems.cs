using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEngine;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Configuration;
using Valheim.DropThat.Core;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Drop.CharacterDropSystem.Patches
{
    [HarmonyPatch(typeof(CharacterDrop))]
    public static class Patch_CharacterDrop_DropItems
    {
        private static MethodInfo OnSpawnedItemMethod = AccessTools.Method(typeof(Patch_CharacterDrop_DropItems), nameof(OnSpawnedItem), new[] { typeof(GameObject), typeof(List<KeyValuePair<GameObject, int>>), typeof(Vector3) });
        private static MethodInfo AfterSpawnedItemMethod = AccessTools.Method(typeof(Patch_CharacterDrop_DropItems), nameof(AfterSpawnedItem));

        [HarmonyPatch(nameof(CharacterDrop.DropItems))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> HookSpawnedItem(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                // Move to right after object instantiation
                .MatchForward(false, new CodeMatch(OpCodes.Ldloc_3))
                .Advance(3)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, 5))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, OnSpawnedItemMethod))
                // Insert auto stacking, and set loop index to result.
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, 5))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_2))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AfterSpawnedItemMethod))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Stloc_2)) // Overwrite existing loop index
                .InstructionEnumeration();
        }

        private static int AfterSpawnedItem(GameObject item, List<KeyValuePair<GameObject, int>> drops, int index)
        {
            var resultIndex = index;

            try
            {
                var count = LoopCounter.GetCount(drops) - 1;
                int itemIndex = GetIndex(drops, count);

#if DEBUG
                //Log.LogDebug($"[AfterSpawnedItem] Count: {count}, Index: {itemIndex}");
#endif

                if (itemIndex >= drops.Count)
                {
#if DEBUG
                    // Log.LogWarning($"Ups. Attempting to access drop index {itemIndex} in drop list with count {drops.Count}");
#endif
                    return resultIndex;
                }

                var drop = drops[itemIndex];

                bool shouldStack = false;

                // Check if we should be stacking the item
                if (drop.Value > 1)
                {
                    if (ConfigurationManager.GeneralConfig.AlwaysAutoStack)
                    {
                        shouldStack = true;
                    }
                    else
                    {
                        var extendedData = TempDropListCache.GetDrop(drops, itemIndex);

                        if (extendedData is not null && extendedData.Config.SetAutoStack)
                        {
                            shouldStack = true;
                        }
                    }
                }

                if (shouldStack)
                {
                    var maxStack = GetMaxStackSize(drop.Key);
                    var stackSize = Math.Min(maxStack, drop.Value - index);

                    if (stackSize > 1)
                    {
                        // Apply stack to item, and skip loop equivalently.
                        var itemDrop = ComponentCache.GetComponent<ItemDrop>(item);

                        Log.LogTrace($"Stacking item '{drop.Key.name}' {stackSize} times out of maximum {maxStack}.");

                        itemDrop.SetStack(stackSize);

                        // Deduct 1 from result, since loop will increment on its own, and OnSpawnedItem will have incremented loop too.
                        LoopCounter.Increment(drops, stackSize - 1);
                        resultIndex += stackSize - 1;

#if DEBUG
                        //Log.LogTrace($"Setting new loop index: {resultIndex}");
#endif
                    }
                }
            }
            catch (Exception e)
            {
                Log.LogError($"Error while attempting to stack item '{item.name}'. Skipping stacking.", e);
            }

            return resultIndex;

            int GetMaxStackSize(GameObject item)
            {
                var itemDrop = ComponentCache.GetComponent<ItemDrop>(item);

                if (itemDrop.IsNull() ||
                    itemDrop.m_itemData?.m_shared is null)
                {
                    return -1;
                }

                return itemDrop.m_itemData.m_shared.m_maxStackSize;
            }
        }

        private static void OnSpawnedItem(GameObject item, List<KeyValuePair<GameObject, int>> drops, Vector3 centerPos)
        {
#if DEBUG
            //Log.LogDebug($"Attempting to apply modifiers to item {item.name}:{drops.GetHashCode()}");
            //Log.LogDebug($"Possible drops: " + drops.Join(x => $"{x.Key.name}:{x.Value}"));
#endif

            int count = LoopCounter.GetCount(drops);
            int index = GetIndex(drops, count);
            LoopCounter.Increment(drops);

#if DEBUG
            //Log.LogDebug($"[OnSpawnedItem] Count: {count}, Index: {index}");
#endif

            var extendedData = TempDropListCache.GetDrop(drops, index);

            if (extendedData is null)
            {
#if DEBUG
                //Log.LogDebug($"No config for item {item.name} at index {index}");
#endif
                return;
            }

#if DEBUG
            //Log.LogDebug($"Found config, applying modifiers to item {item.name}");
#endif

            DropModificationManager.Instance.ApplyModifications(item, extendedData, centerPos);
        }

        private static int GetIndex(List<KeyValuePair<GameObject, int>> drops, int itemCount)
        {
            int index = 0;
            for (int i = 0, count = 0; i < drops.Count; ++i)
            {
                for (int j = 0; j < drops[i].Value; ++j, ++count)
                {
                    if (itemCount == count)
                    {
                        return index;
                    }
                }

                ++index;
            }
            return index;
        }

        /// <summary>
        /// Because I am shit at transpiling apparently. Just gonna hack the living hell out of this.
        /// </summary>
        private class LoopCounter
        {
            private static ConditionalWeakTable<object, LoopCounter> LoopCounterTable = new ConditionalWeakTable<object, LoopCounter>();

            public static void Increment(object obj)
            {
                var counter = LoopCounterTable.GetOrCreateValue(obj);

                counter.Count++;
            }

            public static void Increment(object obj, int count)
            {
                var counter = LoopCounterTable.GetOrCreateValue(obj);
                counter.Count += count;
            }

            public static int GetCount(object obj)
            {
                if (LoopCounterTable.TryGetValue(obj, out LoopCounter counter))
                {
                    return counter.Count;
                }

                return 0;
            }

            public int Count;
        }
    }
}

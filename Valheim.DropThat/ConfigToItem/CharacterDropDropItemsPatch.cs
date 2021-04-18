using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using UnityEngine;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Core;
using Valheim.DropThat.Drop;

namespace Valheim.DropThat.Patches
{
    [HarmonyPatch(typeof(CharacterDrop))]
    public static class CharacterDropDropItemsPatch
    {
        private static MethodInfo OnSpawnedItemMethod = AccessTools.Method(typeof(CharacterDropDropItemsPatch), nameof(OnSpawnedItem), new[] { typeof(GameObject), typeof(List<KeyValuePair<GameObject, int>>) });

        [HarmonyPatch(nameof(CharacterDrop.DropItems))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> HookSpawnedItem(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch(OpCodes.Ldloc_3))
                .Advance(3)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, 5))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, OnSpawnedItemMethod))
                .InstructionEnumeration();
        }

        private static void OnSpawnedItem(GameObject item, List<KeyValuePair<GameObject, int>> drops)
        {
#if DEBUG
            Log.LogDebug($"Attempting to apply modifiers to item {item.name}:{drops.GetHashCode()}");
            Log.LogDebug($"Possible drops: " + drops.Join(x => $"{x.Key.name}:{x.Value}"));
#endif

            int count = LoopCounter.GetCount(drops);
            int index = GetIndex(count);
            LoopCounter.Increment(drops);

#if DEBUG
            Log.LogDebug($"Count: {count}, Index: {index}");
#endif

            var extendedData = TempDropListCache.GetDrop(drops, index);

            if(extendedData is null)
            {
#if DEBUG
                Log.LogDebug($"No config for item {item.name} at index {index}");
#endif
                return;
            }

#if DEBUG
            Log.LogDebug($"Found config, applying modifiers to item {item.name}");
#endif

            DropModificationManager.Instance.ApplyModifications(item, extendedData);

            int GetIndex(int itemCount)
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

            public static int GetCount(object obj)
            {
                if(LoopCounterTable.TryGetValue(obj, out LoopCounter counter))
                {
                    return counter.Count;
                }

                return 0;
            }

            public int Count;
        }
    }
}

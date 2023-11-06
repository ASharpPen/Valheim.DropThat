using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using DropThat.Caches;
using DropThat.Configuration;
using DropThat.Drop.CharacterDropSystem.Caches;
using DropThat.Drop.CharacterDropSystem.Managers;
using HarmonyLib;
using ThatCore.Extensions;
using ThatCore.Logging;
using UnityEngine;

namespace DropThat.Drop.CharacterDropSystem.Patches;

[HarmonyPatch]
internal static class Patch_CharacterDrop_ConfigureDroppedItems
{
    private static int ItemCount = 0;

    [HarmonyPatch(typeof(CharacterDrop), nameof(CharacterDrop.DropItems))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> HookSpawnedItem(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        return new CodeMatcher(instructions, generator)
            // Initialize drop session
            .Start()
            .InsertAndAdvance(Transpilers.EmitDelegate(Init))
            // Move to right after object instantiation
            .MatchForward(false, new CodeMatch(OpCodes.Ldloc_3))
            .Advance(3)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, 5))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1))
            .InsertAndAdvance(Transpilers.EmitDelegate(OnSpawnedItem))
            // Insert auto stacking, and set loop index to result.
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, 5))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_2))
            .InsertAndAdvance(Transpilers.EmitDelegate(StackDropsAndReturnNewIndex))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Stloc_2)) // Overwrite existing loop index
            .InstructionEnumeration();
    }

    private static void Init() => ItemCount = 0;

    private static void OnSpawnedItem(GameObject item, List<KeyValuePair<GameObject, int>> drops, Vector3 centerPos)
    {
        int index = GetIndex(drops, ItemCount);
        ItemCount++;

        CharacterDropSessionManager.ModifyDrop(item, drops, index); 
    }

    private static int StackDropsAndReturnNewIndex(GameObject item, List<KeyValuePair<GameObject, int>> drops, int index)
    {
        var resultIndex = index;

        try
        {
            int itemIndex = GetIndex(drops, ItemCount - 1);

            if (itemIndex >= drops.Count)
            {
                return resultIndex;
            }

            var drop = drops[itemIndex];

            bool shouldStack = false;

            // Check if we should be stacking the item
            if (drop.Value > 1)
            {
                if (GeneralConfigManager.Config.AlwaysAutoStack)
                {
                    shouldStack = true;
                }
                else
                {
                    var configInfo = TempDropListCache.GetDrop(drops, itemIndex);

                    if (configInfo?.DropTemplate?.AutoStack == true)
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
                    var itemDrop = ComponentCache.Get<ItemDrop>(item);

                    Log.Trace?.Log($"Stacking item '{drop.Key.name}' {stackSize} times out of maximum {maxStack}.");

                    itemDrop.SetStack(stackSize);

                    // Deduct 1 from result, since loop will increment on its own, and OnSpawnedItem will have incremented loop too.
                    ItemCount += stackSize - 1;
                    resultIndex += stackSize - 1;
                }
            }
        }
        catch (Exception e)
        {
            Log.Error?.Log($"Error while attempting to stack item '{item.name}'. Skipping stacking.", e);
        }

        return resultIndex;
    }

    private static int GetMaxStackSize(GameObject item)
    {
        var itemDrop = ComponentCache.Get<ItemDrop>(item);

        if (itemDrop.IsNull() ||
            itemDrop.m_itemData?.m_shared is null)
        {
            return -1;
        }

        return itemDrop.m_itemData.m_shared.m_maxStackSize;
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
}

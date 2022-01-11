using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using UnityEngine;
using Valheim.DropThat.Core;
using Valheim.DropThat.Drop.DropTableSystem.Caches;
using Valheim.DropThat.Drop.DropTableSystem.Wrapper;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Drop.DropTableSystem.Patches
{
    internal static class Patch_OnDrop
    {
        private static MethodInfo DropInstantiation = ReflectionUtils.InstantiateGameObjectMethod;
        private static MethodInfo AddItemToInventory = AccessTools.Method(typeof(Inventory), nameof(Inventory.AddItem), new[] { typeof(ItemDrop.ItemData) });
        
        private static MethodInfo UnwrapDropMethod = AccessTools.Method(typeof(Patch_OnDrop), nameof(UnwrapDrop));
        private static MethodInfo ModifyDropMethod = AccessTools.Method(typeof(Patch_OnDrop), nameof(ModifyDrop));

        [HarmonyPatch(typeof(Container))]
        internal static class Patch_Container_AddDefaultItems
        {
            [HarmonyPatch("AddDefaultItems")]
            [HarmonyTranspiler]
            private static IEnumerable<CodeInstruction> InsertItemManagement(IEnumerable<CodeInstruction> instructions)
            {
                return new CodeMatcher(instructions)
                    // Move to right before drop is added to inventory
                    .MatchForward(false, new CodeMatch(OpCodes.Callvirt, AddItemToInventory))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                    .InsertAndAdvance(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_OnDrop), nameof(ModifyContainerItem))))
                    .InstructionEnumeration();
            }
        }
        
        [HarmonyPatch(typeof(DropOnDestroyed))]
        internal static class Patch_DropOnDestroyed_OnDestroyed
        {
            [HarmonyPatch("OnDestroyed")]
            [HarmonyTranspiler]
            private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) =>
                instructions.InsertDropManagementInstructions();
        }

        [HarmonyPatch(typeof(LootSpawner))]
        internal static class Patch_LootSpawner_UpdateSpawner
        {
            [HarmonyPatch("UpdateSpawner")]
            [HarmonyTranspiler]
            private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) =>
                instructions.InsertDropManagementInstructions();
        }

        [HarmonyPatch(typeof(TreeLog))]
        internal static class Patch_TreeLog_Destroy
        {
            [HarmonyPatch("Destroy")]
            [HarmonyTranspiler]
            private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) =>
                instructions.InsertDropManagementInstructions();
        }

        [HarmonyPatch(typeof(TreeBase))]
        internal static class Patch_TreeBase_RPC_Damage
        {
            [HarmonyPatch("RPC_Damage")]
            [HarmonyTranspiler]
            private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) =>
                instructions.InsertDropManagementInstructions();
        }

        [HarmonyPatch(typeof(MineRock))]
        internal static class Patch_MineRock_RPC_Hit
        {
            [HarmonyPatch("RPC_Hit")]
            [HarmonyTranspiler]
            private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) =>
                instructions.InsertDropManagementInstructions();
        }

        [HarmonyPatch(typeof(MineRock5))]
        internal static class Patch_MineRock5_DamageArea
        {
            [HarmonyPatch("DamageArea")]
            [HarmonyTranspiler]
            private static IEnumerable<CodeInstruction> InsertDropManagement(IEnumerable<CodeInstruction> instructions) => 
                instructions.InsertDropManagementInstructions();
        }

        private static IEnumerable<CodeInstruction> InsertDropManagementInstructions(this IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                // Move to right after drop prefab gets retrieved from list
                .MatchForward(false,
                    new CodeMatch(OpCodes.Call, DropInstantiation))
                .Advance(-2)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, UnwrapDropMethod))
                // Move to right after drop is instantiated, and replace the pop
                .MatchForward(true,
                    new CodeMatch(OpCodes.Call, DropInstantiation),
                    new CodeMatch(OpCodes.Pop))
                .RemoveInstruction()
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, ModifyDropMethod))
                .InstructionEnumeration();
        }

        private static GameObject _currentWrapped;

        private static GameObject UnwrapDrop(GameObject wrappedDrop)
        {
            try 
            {
                _currentWrapped = wrappedDrop;

                return wrappedDrop.Unwrap();
            }
            catch(Exception e)
            {
                Log.LogError("Error while attempting to unwrap drop", e);
                return wrappedDrop;
            }
        }

        private static void ModifyDrop(GameObject drop)
        {
            try
            {
                DropTemplate template = DropTemplateCache.GetTemplate(_currentWrapped);

                if (template is null)
                {
                    return;
                }

                DropModificationContext context = new DropModificationContext(drop, template);

                foreach (var modifier in template.Modifiers)
                {
                    try
                    {
                        modifier.Modify(context);
                    }
                    catch (Exception e)
                    {
                        Log.LogError($"Error while attempting to apply modifier '{modifier.GetType().Name}' to drop '{drop}'. Skipping modifier.", e);
                    }
                }
            }
            catch(Exception e)
            {
                Log.LogError($"Error while preparing to modify drop '{drop}'. Skipping modifiers.", e);
            }

            _currentWrapped = null;
        }

        private static ItemDrop.ItemData ModifyContainerItem(ItemDrop.ItemData item, Container container)
        {
            try
            {
                // Make sure item has its prefab unwrapped.
                item.m_dropPrefab = item.m_dropPrefab.Unwrap();

                DropTemplate template = DropTemplateCache.GetTemplate(item);

                if (template is null)
                {
#if DEBUG
                    Log.LogDebug($"Failed to find template for {item?.m_dropPrefab}");
#endif
                    return item;
                }

                foreach (var modifier in template.Modifiers)
                {
                    try
                    {
                        modifier.Modify(ref item, template, container.transform.position);
                    }
                    catch (Exception e)
                    {
                        Log.LogError($"Error while attempting to apply modifier '{modifier.GetType().Name}' to drop '{item.m_dropPrefab.name}'. Skipping modifier.", e);
                    }
                }
            }
            catch (Exception e)
            {
                Log.LogError($"Error while preparing to modify drop '{item.m_dropPrefab.name}'. Skipping modifiers.", e);
            }

            return item;
        }

        private static ItemDrop.ItemData _currentItem;

        private static void RememberCurrentItem(ItemDrop.ItemData itemData)
        {
#if DEBUG
            Log.LogDebug("Remembering item which is not null? "  + (itemData is not null));
#endif
            _currentItem = itemData;
        }
    }
}

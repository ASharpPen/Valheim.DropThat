using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using DropThat.Drop.CharacterDropSystem.Caches;
using ThatCore.Logging;
using DropThat.Drop.CharacterDropSystem.Managers;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DropThat.Drop.CharacterDropSystem.Models;

namespace DropThat.Drop.CharacterDropSystem.Patches;

/// <summary>
/// The drop generation part of the code is extremely busy with a number of mods wanting in on the item drop action.
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
public static class Patch_TrackDrops
{
    private static MethodInfo Anchor = AccessTools.Method(typeof(List<CharacterDrop.Drop>.Enumerator), "MoveNext");

    [HarmonyPatch(typeof(CharacterDrop), nameof(CharacterDrop.GenerateDropList))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TrackDrop(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false, //Move to right before moving enumeration head and after having added the most recent key-value pair to results.
                new CodeMatch(OpCodes.Ldloca_S),
                new CodeMatch(OpCodes.Call, Anchor))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0)) //Loads the list with resulting key-value pairs.
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_3)) //Load current CharacterDrop.Drop
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0)) //Load instance
            .InsertAndAdvance(Transpilers.EmitDelegate(KeepTrack))
            .InstructionEnumeration();
    }

    private static void KeepTrack(List<KeyValuePair<GameObject, int>> dropItems, CharacterDrop.Drop drop, CharacterDrop characterDrop)
    {
        try
        {
            if (drop is null)
            {
                return;
            }

            if (DropTableManager.DropInstanceTable.TryGetValue(drop, out var configInfo))
            {
                Log.DevelopmentOnly($"Carrying configs for drop {configInfo.DisplayName}:{drop.m_prefab.name}:{characterDrop.GetHashCode()}");

                TempDropListCache.SetDrop(characterDrop, configInfo, dropItems.Count - 1);
            }
        }
        catch (Exception e)
        {
            Log.Warning?.Log("Error while attempting to keep track of drops.", e);
        }
    }

    [HarmonyPatch(typeof(CharacterDrop), nameof(CharacterDrop.GenerateDropList))]
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    private static void MoveConfigReferenceFromComponentToDrop(CharacterDrop __instance, List<KeyValuePair<GameObject, int>> __result)
    {
        try
        {
            var instanceReferences = TempDropListCache.GetDrops(__instance);

            if (instanceReferences is not null)
            {
                //Re-associate result with configs.
                foreach (var reference in instanceReferences.InfoByIndex)
                {
                    TempDropListCache.SetDrop(__result, reference.Value, reference.Key);
                }
            }
        }
        catch (Exception e)
        {
            Log.Warning?.Log("Error while attempting to keep track of drops.", e);
        }
    }

    /// <summary>
    /// Store config references in ragdoll zdo, to keep track of drops.
    /// </summary>
    [HarmonyPatch(typeof(Ragdoll), nameof(Ragdoll.SaveLootList))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TranspileStoreConfigReferences(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false, new CodeMatch(OpCodes.Stloc_1)) //Move to storage of ZDO
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_1))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
            .InsertAndAdvance(Transpilers.EmitDelegate(StoreConfigReferences))
            .InstructionEnumeration();
    }

    /// <summary>
    /// Load config references from ragdoll zdo, and set up references to drops again.
    /// </summary>
    [HarmonyPatch(typeof(Ragdoll), nameof(Ragdoll.SpawnLoot))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> TranspileLoadConfigReferences(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .MatchForward(false, new CodeMatch(OpCodes.Stloc_2)) //Move to instantiation of new drop list
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0)) //Load zdo
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_2)) //Load drop list
            .InsertAndAdvance(Transpilers.EmitDelegate(LoadConfigReferences))
            .InstructionEnumeration();
    }

    private const string ZdoConfigReferenceKey = "DropThatConfigs";

    private static void StoreConfigReferences(ZDO zdo, CharacterDrop drop, List<KeyValuePair<GameObject, int>> drops)
    {
        try
        {
            Log.DevelopmentOnly($"Packing config references for zdo {zdo.m_uid}");

            var cache = TempDropListCache.GetDrops(drops);
            cache ??= TempDropListCache.GetDrops(drop); // If we somehow failed to keep a consistent list reference (probably mod conflict). Attempt with the original CharacterDrop instead.

            if (cache is null)
            {
                Log.DevelopmentOnly($"Found no drops for zdo {zdo.m_uid}");
                return;
            }

            List<ConfigReferenceDto> configReferences = cache.InfoByIndex
                .Select(x => new ConfigReferenceDto
                {
                    Mob = x.Value.MobTemplate.PrefabName,
                    Id = x.Value.DropTemplate.Id,
                    Index = x.Key
                })
                .ToList();

            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memStream, configReferences);

                byte[] serialized = memStream.ToArray();

                Log.DevelopmentOnly($"Serialized and set drops for zdo {zdo.m_uid}");

                zdo.Set(ZdoConfigReferenceKey, serialized);
            }
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error while attempting to store configurations for items to be dropped on ragdoll 'puff'.", e);
        }
    }

    [Serializable]
    private class ConfigReferenceDto
    {
        public string Mob { get; set; }

        public int Id { get; set; }

        public int Index { get; set; }
    }

    private static void LoadConfigReferences(ZDO zdo, List<KeyValuePair<GameObject, int>> dropList)
    {
        try
        {
            if (dropList is null)
            {
                return;
            }

            // Unpack references
            var serialized = zdo.GetByteArray(ZdoConfigReferenceKey);

            if (serialized is null)
            {
                return;
            }

            using MemoryStream stream = new(serialized);

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            var references = binaryFormatter.Deserialize(stream) as List<ConfigReferenceDto>;

            Log.DevelopmentOnly($"Deserialized config package for zdo {zdo.m_uid}");
            Log.DevelopmentOnly($"\t" + references?.Join(x => $"{x.Index}:{x.Mob}.{x.Id}"));

            foreach (var reference in references ?? Enumerable.Empty<ConfigReferenceDto>())
            {
                if (TemplateManager.TryGetTemplate(reference.Mob, out var mobTemplate) &&
                    mobTemplate.Drops.TryGetValue(reference.Id, out var dropTemplate))
                {
                    var configInfo = new DropConfigInfo()
                    {
                        MobTemplate = mobTemplate,
                        DropTemplate = dropTemplate,
                        Index = reference.Index,
                    };

                    TempDropListCache.SetDrop(dropList, configInfo, reference.Index);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error while attempting to load config references for ragdoll drops.", e);
        }
    }
}

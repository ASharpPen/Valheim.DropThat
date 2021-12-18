using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Valheim.DropThat.Configuration;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;
using Valheim.DropThat.Drop.CharacterDropSystem.Caches;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Drop.CharacterDropSystem.Patches
{
    /// <summary>
    /// Turns out, ragdoll rules the drops when a creature dies.
    /// To store the data from death to actual "puff" of drops, it records references of the prefab into the ragdoll zdo.
    /// 
    /// To make the configs follow the drops, we need the following:
    /// - To identify the config associated with each index in list.
    ///     - This will be accomplished by CharacterDropCarryDropExtendedPatch
    /// - To store the config associated with which prefab entry
    ///     - StoreConfigReferences will handle serialization of these
    /// - To load the configs associated on "puff".
    /// </summary>
    [HarmonyPatch(typeof(Ragdoll))]
    public static class Patch_Ragdoll_TransferConfigs
    {
        private static MethodInfo StoreConfigsMethod = AccessTools.Method(typeof(Patch_Ragdoll_TransferConfigs), nameof(StoreConfigReferences));
        private static MethodInfo LoadConfigsMethod = AccessTools.Method(typeof(Patch_Ragdoll_TransferConfigs), nameof(LoadConfigReferences));

        [HarmonyPatch("SaveLootList")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> TranspileStoreConfigReferences(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch(OpCodes.Stloc_1)) //Move to storage of ZDO
                .Advance(1)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_1))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, StoreConfigsMethod))
                .InstructionEnumeration();
        }

        [HarmonyPatch("SpawnLoot")]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> TranspileLoadConfigReferences(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch(OpCodes.Stloc_2)) //Move to instantiation of new drop list
                .Advance(1)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0)) //Load zdo
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_2)) //Load drop list
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, LoadConfigsMethod))
                .InstructionEnumeration();
        }

        private const string ZDOKey = "DropThatConfigs";

        private static void StoreConfigReferences(ZDO zdo, CharacterDrop drop, List<KeyValuePair<GameObject, int>> drops)
        {
            try
            {
#if DEBUG
                Log.LogDebug($"Packing config references for zdo {zdo.m_uid}");
#endif

                var cache = TempDropListCache.GetDrops(drops);
                cache ??= TempDropListCache.GetDrops(drop); // If we somehow failed to keep a consistent list reference (probably mod conflict). Attempt with the original CharacterDrop instead.

                if (cache is null)
                {
#if DEBUG
                    Log.LogDebug($"Found no drops for zdo {zdo.m_uid}");
#endif
                    return;
                }

                List<DropConfig> package = cache.ConfigByIndex
                    .Select(x =>
                        new DropConfig
                        {
                            Index = x.Key,
                            ConfigKey = x.Value.Config.SectionKey,
                            IsList = x.Value.Config.IsFromNamedList,
                        })
                    .ToList();

                using (MemoryStream memStream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memStream, package);

                    byte[] serialized = memStream.ToArray();

#if DEBUG
                    Log.LogDebug($"Serialized and set drops for zdo {zdo.m_uid}");
#endif

                    zdo.Set(ZDOKey, serialized);
                }
            }
            catch (Exception e)
            {
                Log.LogError("Error while attempting to store configurations for items to be dropped on ragdoll 'puff'.", e);
            }
        }

        private static void LoadConfigReferences(ZDO zdo, List<KeyValuePair<GameObject, int>> dropList)
        {
            try
            {
#if DEBUG
                Log.LogDebug($"Unpacking config references for zdo {zdo.m_uid}");
#endif

                if (dropList is null)
                {
#if DEBUG
                    Log.LogDebug($"Drop list is empty. Skipping unpacking of zdo {zdo.m_uid}");
#endif
                    return;
                }

                var serialized = zdo.GetByteArray(ZDOKey);

                if (serialized is null)
                {
#if DEBUG
                    Log.LogDebug($"Found nothing to unpack for zdo {zdo.m_uid}");
#endif
                    return;
                }

                using (MemoryStream memStream = new MemoryStream(serialized))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    var responseObject = binaryFormatter.Deserialize(memStream);

                    if (responseObject is List<DropConfig> configPackage)
                    {
#if DEBUG
                        Log.LogDebug($"Deserialized config package for zdo {zdo.m_uid}");
                        Log.LogDebug($"\t" + configPackage.Join(x => $"{x.Index}:{x.ConfigKey}"));
#endif

                        foreach (var entry in configPackage)
                        {
                            var configSections = entry.ConfigKey.SplitBy('.');

                            if (configSections.Count != 2)
                            {
                                Log.LogWarning($"Incorrect Drop That config section header '{entry.ConfigKey}' for zdo {zdo.m_uid}");
                                return;
                            }

                            if (entry.IsList)
                            {
                                if (ConfigurationManager.CharacterDropLists.TryGet(configSections[0], out CharacterDropListConfiguration listConfig))
                                {
                                    if (listConfig.TryGet(configSections[1], out CharacterDropItemConfiguration itemConfig))
                                    {
                                        TempDropListCache.SetDrop(dropList, entry.Index, new DropExtended
                                        {
                                            Config = itemConfig,
                                        });
                                    }
                                }
                            }
                            else if (ConfigurationManager.CharacterDropConfigs.TryGet(configSections[0], out CharacterDropMobConfiguration mobConfig))
                            {
                                if (mobConfig.TryGet(configSections[1], out CharacterDropItemConfiguration itemConfig))
                                {
                                    TempDropListCache.SetDrop(dropList, entry.Index, new DropExtended
                                    {
                                        Config = itemConfig,
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.LogError("Error while attempting to attach and apply configurations to items.", e);
            }
        }

        [Serializable]
        private class DropConfig
        {
            public int Index;
            public string ConfigKey;
            public bool IsList;
        }
    }
}

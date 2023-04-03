using System;
using HarmonyLib;
using UnityEngine;
using DropThat.Configuration;
using DropThat.Core;
using DropThat.Drop.DropTableSystem.Caches;
using DropThat.Utilities;

namespace DropThat.Drop.DropTableSystem.Patches;

internal static class Old_Patch_DropSourceLink
{
    [HarmonyPatch(typeof(Container))]
    internal static class Patch_Container_Awake_InitDropContext
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        private static void SetLink(Container __instance)
        {
            CreateLink(__instance.m_defaultItems, __instance.gameObject);
        }
    }

    [HarmonyPatch(typeof(DropOnDestroyed))]
    internal static class Patch_DropOnDestroyed_Awake_InitDropContext
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void SetLink(DropOnDestroyed __instance)
        {
            CreateLink(__instance.m_dropWhenDestroyed, __instance.gameObject);
        }
    }

    [HarmonyPatch(typeof(LootSpawner))]
    internal static class Patch_LootSpawner_Awake_InitDropContext
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void SetLink(LootSpawner __instance)
        {
            CreateLink(__instance.m_items, __instance.gameObject);
        }
    }

    [HarmonyPatch(typeof(TreeBase))]
    internal static class Patch_TreeBase_Awake_InitDropContext
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void SetLink(TreeBase __instance)
        {
            CreateLink(__instance.m_dropWhenDestroyed, __instance.gameObject);
        }
    }

    [HarmonyPatch(typeof(TreeLog))]
    internal static class Patch_TreeLog_Awake_InitDropContext
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void SetLink(TreeLog __instance)
        {
            CreateLink(__instance.m_dropWhenDestroyed, __instance.gameObject);
        }
    }

    [HarmonyPatch(typeof(MineRock))]
    internal static class Patch_MineRock_Awake_InitDropContext
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void SetLink(MineRock __instance)
        {
            CreateLink(__instance.m_dropItems, __instance.gameObject);
        }
    }

    [HarmonyPatch(typeof(MineRock5))]
    internal static class Patch_MineRock5_Awake_InitDropContext
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void SetLink(MineRock5 __instance)
        {
            CreateLink(__instance.m_dropItems, __instance.gameObject);
        }
    }

    private static void CreateLink(DropTable dropTable, GameObject source)
    {
        try
        {
            if (dropTable is null || source is null)
            {
                return;
            }

            DropSourceTemplateLink link = new()
            {
                Source = source,
            };

            string sourceName = source.GetCleanedName();

            if (string.IsNullOrWhiteSpace(sourceName))
            {
                return;
            }

#if FALSE && DEBUG
            Log.LogTrace($"Linking entity to config: '{source}':'{sourceName}'");
#endif

            // Configs haven't arrived yet.
            if (ConfigurationManager.DropTableConfigs is null)
            {
                Log.LogDebug($"{source} woke before configs were loaded. Skipping config for '{source}'");
                return;
            }

            if (ConfigurationManager.DropTableConfigs.TryGet(sourceName, out var config))
            {
                link.EntityConfig = config;
            }

            DropLinkCache.SetLink(dropTable, link);
        }
        catch(Exception e)
        {
            Log.LogError($"Error while attempting to create associate drop table for object with config.", e);
        }
    }
}

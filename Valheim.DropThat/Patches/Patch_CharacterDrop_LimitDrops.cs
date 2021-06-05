using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Configuration;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Patches
{
    [HarmonyPatch(typeof(CharacterDrop))]
    internal static class Patch_CharacterDrop_LimitDrops
    {
        [HarmonyPatch(nameof(CharacterDrop.GenerateDropList))]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Last)]
        private static void LimitDrops(CharacterDrop __instance, List<KeyValuePair<GameObject, int>> __result)
        {
            for (int i = 0; i < __result.Count; ++i)
            {
                var item = __result[i];
                if (ConfigurationManager.GeneralConfig.DropLimit > 0 && item.Value > ConfigurationManager.GeneralConfig.DropLimit)
                {
                    Log.LogTrace($"Limiting {item.Key.name}:{item.Value} to {ConfigurationManager.GeneralConfig.DropLimit}");

                    __result[i] = Limit(item, ConfigurationManager.GeneralConfig.DropLimit);
                    continue;
                }

                var config = TempDropListCache.GetDrop(__instance, i);

                if (config is not null && config.Config.SetAmountLimit > 0 && item.Value > config.Config.SetAmountLimit)
                {
                    Log.LogTrace($"Limiting {item.Key.name}:{item.Value} to {config.Config.SetAmountLimit}");

                    __result[i] = Limit(item, config.Config.SetAmountLimit);
                }
            }
        }

        private static KeyValuePair<GameObject, int> Limit(KeyValuePair<GameObject, int> drop, int limit)
        {
            return new KeyValuePair<GameObject, int>(drop.Key, Math.Min(drop.Value, limit));
        }
    }
}

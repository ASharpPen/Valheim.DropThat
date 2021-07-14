using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Debugging
{
    internal static class CreatureItemFileWriter
    {
        private const string FileName = "drop_that_items.txt";

        public static void WriteToFile(List<Tuple<GameObject, CharacterDrop>> characters)
        {
            List<string> lines = new List<string>(characters.Count * 20);

            foreach (var characterDrop in characters)
            {
                if ((characterDrop.Item2?.m_drops?.Count ?? 0) == 0)
                {
                    continue;
                }

                var chr = characterDrop.Item1.GetComponent<Humanoid>();

                HashSet<string> inventoryItems = new HashSet<string>();

                if (chr is not null)
                {
                    if (chr.m_defaultItems is not null)
                    {
                        foreach (var item in chr.m_defaultItems.Where(x => x is not null))
                        {
                            inventoryItems.Add(item.name);
                        }
                    }
                    if (chr.m_randomArmor is not null)
                    {
                        foreach (var item in chr.m_randomArmor.Where(x => x is not null))
                        {
                            inventoryItems.Add(item.name);
                        }
                    }
                    if (chr.m_randomShield is not null)
                    {
                        foreach (var item in chr.m_randomShield.Where(x => x is not null))
                        {
                            inventoryItems.Add(item.name);
                        }
                    }
                    if (chr.m_randomWeapon is not null)
                    {
                        foreach (var item in chr.m_randomWeapon.Where(x => x is not null))
                        {
                            inventoryItems.Add(item.name);
                        }
                    }
                    if (chr.m_randomSets is not null)
                    {
                        foreach (var set in chr.m_randomSets.Where(x => x is not null))
                        {
                            foreach (var item in set.m_items.Where(x => x is not null))
                            {
                                inventoryItems.Add(item.name);
                            }
                        }
                    }
                }

#if DEBUG
                if (inventoryItems.Count == 0)
                {
                    Log.LogTrace("No inventory items found for " + characterDrop.Item1.name);
                }
#endif
                lines.Add($"[{characterDrop.Item1.name}.0]");
                lines.Add($"{nameof(DropItemConfiguration.ConditionHasItem)}={inventoryItems?.Join()}");
                lines.Add("");
            }

            PrintDebugFile.PrintFile(lines, FileName, "default creature items");
        }
    }
}

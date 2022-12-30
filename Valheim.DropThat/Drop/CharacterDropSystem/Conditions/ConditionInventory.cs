using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DropThat.Caches;
using DropThat.Configuration.ConfigTypes;
using DropThat.Core;
using DropThat.Drop.CharacterDropSystem.Caches;
using DropThat.Integrations;
using DropThat.Utilities;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

internal class ConditionInventory : ICondition
{
    private static ConditionInventory _instance;

    public static ConditionInventory Instance => _instance ??= new();

    public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop)
    {
        var character = CharacterCache.GetCharacter(characterDrop);
        var inventory = CharacterCache.GetInventory(character);

        if (inventory is null)
        {
#if DEBUG
            Log.LogDebug("No inventory for creature were found.");
#endif

            //No inventory to compare against. Assume that all is allowed.
            return false;
        }

        var items = extended.Config.ConditionHasItem.Value.SplitByComma(true);

        if (items.Count == 0)
        {
            return false;
        }

        HashSet<string> inventoryItems;

        if (InstallationManager.RRRInstalled && character.name.StartsWith("RRR"))
        {
            // This is an RRR creature, item names will have been set with a specific pattern.
            inventoryItems = new();

            foreach (var item in inventory.GetAllItems())
            {
                var firstSection = item.m_dropPrefab.name.IndexOf('@');

                if (firstSection < 0)
                {
                    // Unformatted item, add as is
                    inventoryItems.Add(PrepareName(item.m_dropPrefab));
                    continue;
                }

                var endSection = item.m_dropPrefab.name.IndexOf('@', firstSection + 1);

                if (endSection < 0)
                {
                    inventoryItems.Add(CleanName(item.m_dropPrefab.name.Substring(firstSection + 1)));
                }
                else
                {
                    inventoryItems.Add(CleanName(item.m_dropPrefab.name.Substring(firstSection + 1, endSection - firstSection - 1)));
                }
            }
        }
        else
        {
            inventoryItems = inventory
                .GetAllItems()
                .Select(x => x.m_dropPrefab.name.Trim().ToUpperInvariant())
                .ToHashSet();
        }

#if DEBUG
        Log.LogTrace("Inventory: " + inventoryItems.Join());
#endif
        if (!items.Any(x => inventoryItems.Contains(x)))
        {
            //No inventory items matched an item in condition list.
            Log.LogTrace($"{nameof(CharacterDropItemConfiguration.ConditionHasItem)}: Found none of the required items '{items.Join()}' in inventory.");

            return true;
        }

        return false;
    }

    private static string CleanName(string str)
    {
        return str.Trim().ToUpperInvariant();
    }

    private static string PrepareName(GameObject go)
    {
        return go.name.Trim().ToUpperInvariant();
    }
}

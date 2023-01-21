using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Integrations;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public class ConditionInventory : IDropCondition
{
    public HashSet<string> Items { get; set; }

    public ConditionInventory() { }

    public ConditionInventory(IEnumerable<string> items)
    {
        Items = items
            .Select(x => x
                .Trim()
                .ToUpperInvariant())
            .ToHashSet();
    }

    public bool IsValid(DropContext context)
    {
        if (Items is null ||
            Items.Count == 0)
        {
            return true;
        }

        var character = context.Character;

        if (character.IsNull() ||
            character is not Humanoid humanoid ||
            humanoid.m_inventory is null)
        {
            return true;
        }

        var inventoryItems = GetInventoryItems(humanoid);

        return Items.Any(x => inventoryItems.Contains(x));
    }

    private static HashSet<string> GetInventoryItems(Humanoid humanoid)
    {
        HashSet<string> inventoryItems;

        if (InstallationManager.RRRInstalled && humanoid.name.StartsWith("RRR"))
        {
            // This is an RRR creature, item names will have been set with a specific pattern.
            inventoryItems = new();

            foreach (var item in humanoid.m_inventory.GetAllItems())
            {
                if (item.m_dropPrefab.IsNull())
                {
                    continue;
                }

                var firstSection = item.m_dropPrefab.name.IndexOf('@');

                if (firstSection < 0)
                {
                    // Unformatted item, add as is
                    inventoryItems.Add(item.m_dropPrefab.name
                        .Trim()
                        .ToUpperInvariant());

                    continue;
                }

                var endSection = item.m_dropPrefab.name.IndexOf('@', firstSection + 1);

                if (endSection < 0)
                {
                    inventoryItems.Add(item.m_dropPrefab.name
                        .Substring(firstSection + 1)
                        .Trim()
                        .ToUpperInvariant());
                }
                else
                {
                    inventoryItems.Add(item.m_dropPrefab.name
                        .Substring(firstSection + 1, endSection - firstSection - 1)
                        .Trim()
                        .ToUpperInvariant());
                }
            }
        }
        else
        {
            inventoryItems = humanoid.m_inventory
                .GetAllItems()
                .Where(x => x.m_dropPrefab.IsNotNull())
                .Select(x => x.m_dropPrefab.name
                    .Trim()
                    .ToUpperInvariant())
                .ToHashSet();
        }

        return inventoryItems;
    }
}

internal static partial class CharacterDropDropTemplateConditionExtensions
{
    public static CharacterDropDropTemplate ConditionInventory(
        this CharacterDropDropTemplate template,
        IEnumerable<string> items)
    {
        if (items?.Any() == true)
        {
            template.Conditions.AddOrReplaceByType(new ConditionInventory(items));
        }
        else
        {
            template.Conditions.RemoveAll(x => x is ConditionInventory);
        }

        return template;
    }
}
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valheim.DropThat.Caches;
using Valheim.DropThat.ConfigurationCore;
using Valheim.DropThat.ConfigurationTypes;

namespace Valheim.DropThat.Conditions
{
    internal class ConditionInventory : ICondition
    {
        private static ConditionInventory _instance;

        public static ConditionInventory Instance
        {
            get
            {
                return _instance ??= new ConditionInventory();
            }
        }

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended extended, CharacterDrop characterDrop)
        {
            var character = CharacterCache.GetCharacter(characterDrop);
            var inventory = CharacterCache.GetInventory(character);

            if(inventory is null)
            {
#if DEBUG
                Log.LogDebug("No inventory for creature were found.");
#endif

                //No inventory to compare against. Assume that all is allowed.
                return false;
            }

            var items = extended.Config.ConditionHasItem.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if((items?.Length ?? 0) == 0)
            {
                return false;
            }

            var inventoryItems = inventory
                .GetAllItems()
                .Select(x => x.m_dropPrefab.name.Trim().ToUpperInvariant())
                .ToHashSet();

#if DEBUG
            Log.LogTrace("Inventory: " + inventoryItems.Join());
#endif
            if (!items.Any(x => inventoryItems.Contains(x.Trim().ToUpperInvariant())))
            {
                //No inventory items matched an item in condition list.
                Log.LogTrace($"{nameof(DropConfiguration.ConditionHasItem)}: Found none of the required items in inventory.");

                return true;
            }

            return false;
        }
    }
}

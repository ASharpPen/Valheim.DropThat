using UnityEngine;
using Valheim.DropThat.Caches;
using System.Reflection;
using HarmonyLib;
using ExtendedItemDataFramework;
using EpicLoot;
using Valheim.DropThat.Core;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core.Configuration;

namespace Valheim.DropThat.Drop.Modifiers.ModSpecific.ModEpicLoot
{
    internal class ModifierMagicItem : IDropModifier
    {
        private static ModifierMagicItem _instance;

        public static ModifierMagicItem Instance
        {
            get
            {
                return _instance ??= new ModifierMagicItem();
            }
        }

        private static MethodInfo InitializeMagicItem = AccessTools.Method(typeof(LootRoller), "InitializeMagicItem");


        public void Modify(GameObject item, DropExtended extended)
        {
            DropModConfigEpicLoot config;
            if(extended.Config.TryGet(DropModConfigEpicLoot.ModName, out Config cfg) && cfg is DropModConfigEpicLoot modConfig)
            {
                config = modConfig;
            }
            else
            {
                return;
            }

#if DEBUG
            Log.LogDebug("Adding magic modifiers.");
#endif

            if (InitializeMagicItem is null)
            {
#if DEBUG
                Log.LogDebug("Couldn't find LootRoller.InitializeMagicItem.");
#endif
                return;
            }

            var itemDrop = ItemDropCache.Get(item);

            if (EpicLoot.EpicLoot.CanBeMagicItem(itemDrop.m_itemData))
            {
                var extendedItemData = new ExtendedItemData(itemDrop.m_itemData);

                var rarity = RollRarity(config);

                if(rarity is null)
                {
                    return;
                }

                //Make magic.
                MagicItemComponent magicComponent = extendedItemData.AddComponent<MagicItemComponent>();

                MagicItem magicItem = LootRoller.RollMagicItem(rarity.Value, extendedItemData);

                magicComponent.SetMagicItem(magicItem);
                itemDrop.m_itemData = extendedItemData;

                InitializeMagicItem.Invoke(null, new[] { extendedItemData });
            }
        }

        private ItemRarity? RollRarity(DropModConfigEpicLoot config)
        {
            var sumWeight =
                config.RarityWeightMagic +
                config.RarityWeightRare +
                config.RarityWeightEpic +
                config.RarityWeightLegendary;

            var random = Random.Range(0, sumWeight);

            float ongoingSum = 0;
            
            ongoingSum += config.RarityWeightLegendary;
            if (random <= ongoingSum)
                return ItemRarity.Legendary;

            ongoingSum += config.RarityWeightEpic;
            if (random <= ongoingSum)
                return ItemRarity.Epic;

            ongoingSum += config.RarityWeightRare;
            if (random <= ongoingSum)
                return ItemRarity.Rare;

            ongoingSum += config.RarityWeightMagic;
            if (random <= ongoingSum)
                return ItemRarity.Magic;

            return null;
        }
    }
}

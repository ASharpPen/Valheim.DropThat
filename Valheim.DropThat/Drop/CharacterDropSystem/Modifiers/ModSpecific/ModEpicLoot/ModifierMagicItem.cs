using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core.Configuration;
using Valheim.DropThat.Utilities;
using Valheim.DropThat.Core;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Integrations.EpicLootIntegration;
using Valheim.DropThat.Integrations;

namespace Valheim.DropThat.Drop.CharacterDropSystem.Modifiers.ModSpecific.ModEpicLoot
{
    internal class ModifierMagicItem : IDropModifier
    {
        private static ModifierMagicItem _instance;

        public static ModifierMagicItem Instance => _instance ??= new();

        public void Modify(DropContext context)
        {
            if (!InstallationManager.EpicLootInstalled)
            {
                return;
            }

            CharacterDropModConfigEpicLoot config;
            if (context.Extended.Config.TryGet(CharacterDropModConfigEpicLoot.ModName, out Config cfg) && cfg is CharacterDropModConfigEpicLoot modConfig)
            {
                config = modConfig;
            }
            else
            {
                return;
            }

            var itemDrop = ComponentCache.GetComponent<ItemDrop>(context.Item);

            if (itemDrop.IsNull())
            {
                return;
            }

#if DEBUG
            Log.LogDebug("Adding magic modifiers.");
#endif

            if (ItemRoller.TryRollMagic(
                itemDrop.m_itemData,
                itemDrop.transform.position,
                new ItemRollParameters
                {
                    RarityWeightNone = config.RarityWeightNone,
                    RarityWeightMagic = config.RarityWeightMagic,
                    RarityWeightRare = config.RarityWeightRare,
                    RarityWeightEpic = config.RarityWeightEpic,
                    RarityWeightLegendary = config.RarityWeightLegendary,
                    RarityWeightUnique = config.RarityWeightUnique,
                    UniqueIds = config.UniqueIDs.Value.SplitByComma(),
                }))
            {
                itemDrop.Save();
            }
        }
    }
}

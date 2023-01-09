using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core.Configuration;
using Valheim.DropThat.Core;
using Valheim.DropThat.Integrations.EpicLootIntegration;
using UnityEngine;
using Valheim.DropThat.Utilities;
using Valheim.DropThat.Integrations;

namespace Valheim.DropThat.Drop.DropTableSystem.Modifiers.ModSpecific.ModEpicLoot
{
    internal class ModifierMagicItem : IDropTableModifier
    {
        private static ModifierMagicItem _instance;

        public static ModifierMagicItem Instance => _instance ??= new();

        public void Modify(DropModificationContext context)
        {
            if (!InstallationManager.EpicLootInstalled)
            {
                return;
            }

            EpicLootItemConfiguration config = GetConfig(context.Template);

            if (config is null)
            {
#if DEBUG
                Log.LogDebug("Found no config for drop template.");
#endif
                return;
            }

            ItemDrop itemDrop = context.ItemDrop;

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

        public void Modify(ref ItemDrop.ItemData drop, DropTemplate template, Vector3 position)
        {
            if (!InstallationManager.EpicLootInstalled)
            {
                return;
            }

            EpicLootItemConfiguration config = GetConfig(template);

            if (config is null)
            {
#if DEBUG
                Log.LogDebug("Found no config for drop template.");
#endif

                return;
            }

            if (drop is null)


            {
                return;
            }

#if DEBUG
            Log.LogDebug("Adding magic modifiers.");
#endif

            ItemRoller.TryRollMagic(
                drop,
                position,
                new ItemRollParameters
                {
                    RarityWeightNone = config.RarityWeightNone,
                    RarityWeightMagic = config.RarityWeightMagic,
                    RarityWeightRare = config.RarityWeightRare,
                    RarityWeightEpic = config.RarityWeightEpic,
                    RarityWeightLegendary = config.RarityWeightLegendary,
                    RarityWeightUnique = config.RarityWeightUnique,
                    UniqueIds = config.UniqueIDs.Value.SplitByComma(),
                })
                ;
        }

        private EpicLootItemConfiguration GetConfig(DropTemplate template)
        {
            if (template is null)
            {
                return null;
            }

            if (template.Config.TryGet(EpicLootItemConfiguration.ModName, out Config cfg) && cfg is EpicLootItemConfiguration modConfig)
            {
                return modConfig;
            }
            else
            {
                return null;
            }
        }
    }
}

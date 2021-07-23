using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core.Configuration;
using Valheim.DropThat.Core;
using Valheim.DropThat.Integrations.EpicLootIntegration;
using UnityEngine;

namespace Valheim.DropThat.Drop.DropTableSystem.Modifiers.ModSpecific.ModEpicLoot
{
    internal class ModifierMagicItem : IDropTableModifier
    {
        private static ModifierMagicItem _instance;

        public static ModifierMagicItem Instance => _instance ??= new();

        public void Modify(DropModificationContext context)
        {
            EpicLootItemConfiguration config = GetConfig(context.Template);

            if (config is null)
            {
                return;
            }

            ItemDrop itemDrop = context.ItemDrop;

            if (itemDrop is null)
            {
                return;
            }

#if DEBUG
            Log.LogDebug("Adding magic modifiers.");
#endif

            var magicItemData = ItemRoller.Roll(
                itemDrop.m_itemData, 
                context.Drop.transform.position, 
                config);

            if (magicItemData is not null)
            {
                itemDrop.m_itemData = magicItemData;
            }
        }

        public void Modify(ref ItemDrop.ItemData drop, DropTemplate template, Vector3 position)
        {
            EpicLootItemConfiguration config = GetConfig(template);

            if (config is null)
            {
                return;
            }

            if (drop is null)
            {
                return;
            }

#if DEBUG
            Log.LogDebug("Adding magic modifiers.");
#endif

            var magicItemData = ItemRoller.Roll(
                drop,
                position,
                config);

            if (magicItemData is not null)
            {
                drop = magicItemData;
            }
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

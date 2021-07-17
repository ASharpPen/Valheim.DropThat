using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valheim.DropThat.Configuration;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;
using Valheim.DropThat.Core.Configuration;
using Valheim.DropThat.Drop.DropTableSystem.Conditions;
using Valheim.DropThat.Drop.DropTableSystem.Modifiers;
using Valheim.DropThat.Drop.DropTableSystem.Modifiers.ModSpecific.ModEpicLoot;
using Valheim.DropThat.Integrations;
using Valheim.DropThat.Reset;

namespace Valheim.DropThat.Drop.DropTableSystem.Managers
{
    internal static class DropConfigManager
    {
        private static Dictionary<int, List<DropTemplate>> TemplateCache { get; set; } = new();

        static DropConfigManager()
        {
            StateResetter.Subscribe(() =>
            {
                TemplateCache = new();
            });
        }

        private static GeneralConfiguration GeneralConfig => ConfigurationManager.GeneralConfig;

        public static List<DropTemplate> GetPossibleDrops(DropSourceTemplateLink context, DropTable dropTable)
        {
            var hash = context.Source.name.GetStableHashCode();

            if (TemplateCache.TryGetValue(hash, out List<DropTemplate> cached))
            {
                return new(cached);
            }

            var drops = PrepareDrops(context, dropTable);

            TemplateCache[hash] = new(drops);

            return drops;
        }

        private static List<DropTemplate> PrepareDrops(DropSourceTemplateLink context, DropTable dropTable)
        {
            List<DropTemplate> drops = new List<DropTemplate>();

            // Find drop list
            string dropListName = context.EntityConfig?.UseDropList?.Value;

            DropTableListConfiguration listConfig = null;

            if (!string.IsNullOrWhiteSpace(dropListName) &&
                ConfigurationManager.DropTableLists is not null &&
                ConfigurationManager.DropTableLists.TryGet(dropListName, out DropTableListConfiguration dropList))
            {
                listConfig = dropList;
            }

            bool skipExisting = false;

            if (GeneralConfig.ClearAllExistingDropTables ||
                GeneralConfig.ClearAllExistingDropTablesWhenModified &&
                (context.EntityConfig?.Subsections?.Any(x => x.Value.EnableConfig) == true ||
                listConfig?.Subsections?.Any(x => x.Value.EnableConfig) == true))
            {
                skipExisting = true;
            }

            if (!skipExisting)
            {
                // Get all existing drops.
                foreach (var drop in dropTable.m_drops)
                {
                    if (drop.m_weight <= 0)
                    {
                        continue;
                    }

                    drops.Add(new DropTemplate
                    {
                        Drop = drop,
                    });
                }
            }

            if (context.EntityConfig is null)
            {
                return drops;
            }

            var configs = MergeAndOrder(listConfig, context.EntityConfig);
            foreach (var config in configs)
            {
                InsertDrop(drops, config, context.EntityConfig);
            }

            // Go through all the config entries
            foreach (var dropEntry in context.EntityConfig.Subsections.OrderBy(x => x.Value.Index))
            {
                InsertDrop(drops, dropEntry.Value, context.EntityConfig);
            }

            return drops;
        }

        private static void InsertDrop(List<DropTemplate> drops, DropTableItemConfiguration itemConfig, DropTableEntityConfiguration entityConfig)
        {
            if (!itemConfig.EnableConfig)
            {
                return;
            }

            if (itemConfig.SetTemplateWeight <= 0)
            {
                return;
            }

            // Find the drop prefab, whereever it may be.
            GameObject item = ObjectDB.instance.GetItemPrefab(itemConfig.PrefabName.Value);
            item ??= ZNetScene.instance.GetPrefab(itemConfig.PrefabName.Value);

            if (item is null)
            {
                Log.LogWarning($"Unable to find prefab '{itemConfig.PrefabName.Value}' for '{itemConfig.SectionKey}'.");
                return;
            }

            DropTemplate newTemplate = new DropTemplate
            {
                Drop = new DropTable.DropData
                {
                    m_item = item,
                    m_weight = itemConfig.SetTemplateWeight,
                    m_stackMin = itemConfig.SetAmountMin,
                    m_stackMax = itemConfig.SetAmountMax,
                },
                Conditions = ExtractConditions(itemConfig),
                Modifiers = ExtractModifiers(itemConfig),
                Config = itemConfig,
                EntityConfig = entityConfig
            };

            if (GeneralConfig.AlwaysAppendCharacterDrops)
            {
                drops.Add(newTemplate);
            }
            else
            {
                int index = itemConfig.Index;

                if (drops.Count > index && index >= 0)
                {
                    drops.RemoveAt(index);
                }

                if (index >= 0 &&
                    index <= drops.Count)
                {
                    drops.Insert(index, newTemplate);
                }
                else
                {
                    drops.Add(newTemplate);
                }
            }
        }

        private static List<IDropTableCondition> ExtractConditions(DropTableItemConfiguration config)
        {
            List<IDropTableCondition> conditions = new();

            conditions.Add(ConditionBiome.Instance);
            conditions.Add(ConditionAltitude.Instance);
            conditions.Add(ConditionDaytime.Instance);
            conditions.Add(ConditionEnvironments.Instance);
            conditions.Add(ConditionGlobalKeysRequired.Instance);
            conditions.Add(ConditionLocation.Instance);
            conditions.Add(ConditionDistanceToCenter.Instance);

            return conditions;
        }

        private static List<IDropTableModifier> ExtractModifiers(DropTableItemConfiguration config)
        {
            List<IDropTableModifier> modifiers = new();

            modifiers.Add(ModifierSetQualityLevel.Instance);

            if (InstallationManager.EpicLootInstalled)
            {
                if (config.TryGet(EpicLootItemConfiguration.ModName, out Config modConfig) && modConfig is EpicLootItemConfiguration epicLootConfig)
                {
                    modifiers.Add(ModifierMagicItem.Instance);
                }
            }

            return modifiers;
        }

        private static IEnumerable<DropTableItemConfiguration> MergeAndOrder(DropTableListConfiguration list, DropTableEntityConfiguration entity)
        {
            Dictionary<int, DropTableItemConfiguration> templatesByIndex = new();

            if (list is not null)
            {
                foreach(var template in list.Subsections.Values)
                {
                    if (template.EnableConfig)
                    {
                        templatesByIndex[template.Index] = template;
                    }
                }
            }

            if (entity is not null)
            {
                foreach (var template in entity.Subsections.Values)
                {
                    if (template.EnableConfig)
                    {
                        templatesByIndex[template.Index] = template;
                    }
                }
            }

            return templatesByIndex.Values.OrderBy(x => x.Index);
        }
    }
}

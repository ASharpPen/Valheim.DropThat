using System;
using Valheim.DropThat.Core.Configuration;

namespace Valheim.DropThat.Configuration.ConfigTypes
{
    /// <summary>
    /// DropTable configurations
    /// </summary>
    [Serializable]
    public class DropTableConfiguration : ConfigWithSubsections<DropTableEntityConfiguration>, IConfigFile
    {
        protected override DropTableEntityConfiguration InstantiateSubsection(string subsectionName)
        {
            return new DropTableEntityConfiguration();
        }
    }

    [Serializable]
    public class DropTableEntityConfiguration : ConfigWithSubsections<DropTableItemConfiguration>
    {
        protected override DropTableItemConfiguration InstantiateSubsection(string subsectionName)
        {
            return new DropTableItemConfiguration();
        }

        public ConfigurationEntry<int> SetDropMin = new(1, "Minimum of randomly selected entries from drop list. Entries can be picked more than once.");
        public ConfigurationEntry<int> SetDropMax = new(1, "Maximum of randomly selected entries from drop list. Entries can be picked more than once.");
        public ConfigurationEntry<float> SetDropChance = new(100, "Chance to drop anything at all.");
        public ConfigurationEntry<bool> SetDropOnlyOnce = new(false, "If true, will ensure that when selecting entries from drop list, same entry will only be picked once.");

        public ConfigurationEntry<string> UseDropList = new("", "Name of drop list to load for this entity. List items will be overridden by other drops listed here, if index is the same.");
    }

    [Serializable]
    public class DropTableItemConfiguration : ConfigWithSubsections<Config>
    {
        protected override Config InstantiateSubsection(string subsectionName)
        {
            Config newModConfig = null;

            if (subsectionName == EpicLootItemConfiguration.ModName)
            {
                newModConfig = new EpicLootItemConfiguration();
            }
            else if (subsectionName == DropTableModConfigSpawnThat.ModName)
            {
                newModConfig = new DropTableModConfigSpawnThat();
            }
            else if (subsectionName == DropTableModConfigCLLC.ModName)
            {
                newModConfig = new DropTableModConfigCLLC();
            }

            return newModConfig;
        }

        private int? index = null;

        public int Index
        {
            get
            {
                if (index.HasValue)
                {
                    return index.Value;
                }

                if (int.TryParse(SectionName, out int sectionIndex) && sectionIndex >= 0)
                {
                    index = sectionIndex;
                }
                else
                {
                    index = int.MaxValue;
                }

                return index.Value;
            }
        }

        public ConfigurationEntry<string> PrefabName = new("", "Name of prefab to drop.");
        public ConfigurationEntry<bool> EnableConfig = new(true, "Toggle this specific config entry on/off.");
        public ConfigurationEntry<bool> Enable = new(true, "Toggle this specific drop. This can be used to disable existing drops.");

        public ConfigurationEntry<int> SetAmountMin = new(1, "Sets minimum amount pr drop. Behaviour depends on entity and item.");
        public ConfigurationEntry<int> SetAmountMax = new(1, "Sets maximum amount pr drop. Behaviour depends on entity and item.");
        public ConfigurationEntry<float> SetTemplateWeight = new(1, "Set weight for this drop. Used to control how likely it is that this item will be selected when rolling for drops. Note, same drop can be selected multiple times during table rolling.");

        public ConfigurationEntry<float> ConditionAltitudeMin = new(-10000, "Minimum distance above or below sea-level for drop to be enabled.");
        public ConfigurationEntry<float> ConditionAltitudeMax = new(10000, "Maximum distance above or below sea-level for drop to be enabled.");
        public ConfigurationEntry<string> ConditionBiomes = new("", "Biomes in which drop is enabled. If empty, condition will be ignored.");
        public ConfigurationEntry<bool> ConditionNotDay = new(false, "If true, will not drop during daytime.");
        public ConfigurationEntry<bool> ConditionNotNight = new(false, "If true, will not drop during afternoon.");
        public ConfigurationEntry<bool> ConditionNotAfternoon = new(false, "If true, will not drop during afternoon.");
        public ConfigurationEntry<string> ConditionEnvironments = new("", "Array of environment names that allow the item to drop while they are active.\nEg. Misty, Thunderstorm. Leave empty to always allow.");
        public ConfigurationEntry<string> ConditionGlobalKeys = new("", "Array of global keys names that allow the item to drop while they are active.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.");
        public ConfigurationEntry<string> ConditionLocations = new("", "Array of location names. When spawned in one of the listed locations, this drop is enabled.\nEg. Runestone_Boars");
        public ConfigurationEntry<float> ConditionDistanceToCenterMin = new(0, "Minimum distance to center of map, for drop to be enabled.");
        public ConfigurationEntry<float> ConditionDistanceToCenterMax = new(0, "Maximum distance to center of map, within which drop is enabled. 0 means limitless.");

        // ItemDrop.ItemData options

        public ConfigurationEntry<float> SetDurability = new(-1, "Sets the durability of the item. Does not change max durability. If less than 0, uses default.");
        public ConfigurationEntry<int> SetQualityLevel = new(1, "Sets the quality level of the item. If 0 or less, uses default quality level of drop.");
    }

    [Serializable]
    public class DropTableModConfigSpawnThat : Config
    {
        public const string ModName = "SpawnThat";

        //Might not be relevant. Need to verify if Spawn That applies the template id to all prefabs with a zdo, or just characters.
        public ConfigurationEntry<string> ConditionTemplateId = new ConfigurationEntry<string>("", "Array of Spawn That TemplateId values to enable to drop for.");
    }

    [Serializable]
    public class DropTableModConfigCLLC : Config
    {
        public const string ModName = "CreatureLevelAndLootControl";

        public ConfigurationEntry<int> ConditionWorldLevelMin = new(0, "Minimum CLLC world level, for which item will drop.");
        public ConfigurationEntry<int> ConditionWorldLevelMax = new(0, "Maximum CLLC world level, for which item will drop. 0 or less means no max.");
    }
}

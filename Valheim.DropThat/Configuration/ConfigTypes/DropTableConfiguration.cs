using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public ConfigurationEntry<float> SetDropChance = new(1, "Chance to drop anything at all.");
        public ConfigurationEntry<bool> SetDropOnlyOnce = new(false, "If true, will ensure that when selecting entries from drop list, same entry will only be picked once.");

        public ConfigurationEntry<string> UseDropList = new("", "Name of drop list to load for this entity. List items will be overriden by other drops listed here, if index is the same.");
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

        public ConfigurationEntry<int> SetAmountMin = new(1);
        public ConfigurationEntry<int> SetAmountMax = new(1);
        public ConfigurationEntry<float> SetTemplateWeight = new(1, "Set weight for this template. Used to control how likely it is that this item will be selected when rolling for drops. Note, same template can be selected multiple times during drop rolling.");

        public ConfigurationEntry<bool> SetAutoStack = new(false);

        public ConfigurationEntry<float> ConditionAltitudeMin = new(-10000, "");
        public ConfigurationEntry<float> ConditionAltitudeMax = new(10000, "");
        public ConfigurationEntry<string> ConditionBiomes = new("");
        public ConfigurationEntry<bool> ConditionNotDay = new(false);
        public ConfigurationEntry<bool> ConditionNotNight = new(false);
        public ConfigurationEntry<bool> ConditionNotAfternoon = new(false);
        public ConfigurationEntry<string> ConditionEnvironments = new("", "Array of environment names that allow the item to drop while they are active.\nEg. Misty, Thunderstorm. Leave empty to always allow.");
        public ConfigurationEntry<string> ConditionGlobalKeys = new("", "Array of global keys names that allow the item to drop while they are active.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.");
        public ConfigurationEntry<string> ConditionLocations = new("", "Array of location names. When spawned in one of the listed locations, this drop is enabled.\nEg. Runestone_Boars");
        public ConfigurationEntry<float> ConditionDistanceToCenterMin = new(0, "Minimum distance to center of map, for drop to be enabled.");
        public ConfigurationEntry<float> ConditionDistanceToCenterMax = new(0, "Maximum distance to center of map, within which drop is enabled. 0 means limitless.");

        // ItemDrop.ItemData options

        public ConfigurationEntry<float> SetDurability = new(-1);
        public ConfigurationEntry<int> SetQualityLevel = new(1);
    }

    [Serializable]
    public class DropTableModConfigSpawnThat : Config
    {
        public const string ModName = "SpawnThat";

        //Might not be relevant. Need to verify if Spawn That applies the template id to all prefabs with a zdo, or just characters.
        public ConfigurationEntry<string> ConditionTemplateId = new ConfigurationEntry<string>("", "Array of Spawn That TemplateId values to enable to drop for.");
    }
}

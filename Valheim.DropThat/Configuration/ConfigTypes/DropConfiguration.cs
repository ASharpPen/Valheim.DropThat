using System;
using Valheim.DropThat.Core.Configuration;

namespace Valheim.DropThat.Configuration.ConfigTypes
{
    /// <summary>
    /// CharacterDrop configurations
    /// </summary>
    [Serializable]
    public class DropConfiguration : ConfigWithSubsections<DropMobConfiguration>, IConfigFile
    {
        protected override DropMobConfiguration InstantiateSubsection(string subsectionName)
        {
            return new DropMobConfiguration();
        }
    }

    [Serializable]
    public class DropMobConfiguration : ConfigWithSubsections<DropItemConfiguration>
    {
        protected override DropItemConfiguration InstantiateSubsection(string subsectionName)
        {
            return new DropItemConfiguration();
        }

        public ConfigurationEntry<string> UseDropList = new("", "Name of drop list to load for this entity. List items will be overriden by other drops listed here, if index is the same.");
    }

    [Serializable]
    public class DropItemConfiguration : ConfigWithSubsections<Config>
    {
        protected override Config InstantiateSubsection(string subsectionName)
        {
            Config newModConfig = null;

            if(subsectionName == DropModConfigCLLC.ModName)
            {
                newModConfig = new DropModConfigCLLC();
            }
            else if(subsectionName == DropModConfigEpicLoot.ModName)
            {
                newModConfig = new DropModConfigEpicLoot();
            }
            else if(subsectionName == DropModConfigSpawnThat.ModName)
            {
                newModConfig = new DropModConfigSpawnThat();
            }

            return newModConfig;
        }

        #region CharacterDrop.Drop

        public ConfigurationEntry<string> ItemName = new ConfigurationEntry<string>("", "Prefab name of item to drop.");

        public ConfigurationEntry<bool> Enabled = new ConfigurationEntry<bool>(true, "Enable/disable this specific drop configuration.");

        public ConfigurationEntry<int> AmountMin = new ConfigurationEntry<int>(1, "Minimum amount dropped.");

        public ConfigurationEntry<int> AmountMax = new ConfigurationEntry<int>(1, "Maximum amount dropped.");

        public ConfigurationEntry<float> Chance = new ConfigurationEntry<float>(1, "Chance to drop. 1 is 100%.\nExample values: 0, 0.5, 1, 1.0");

        public ConfigurationEntry<bool> OnePerPlayer = new ConfigurationEntry<bool>(false, "If set, will drop one of this item per player. Ignoring other factors.");

        public ConfigurationEntry<bool> LevelMultiplier = new ConfigurationEntry<bool>(true, "Toggles mob levels scaling up dropped amount. Be aware, this scales up very quickly and may cause issues when dropping many items.");

        #endregion

        #region DropExtended Modifiers

        public ConfigurationEntry<int> SetQualityLevel = new ConfigurationEntry<int>(-1, "Sets the quality level of the item. If 0 or less, uses default quality level of drop.");

        public ConfigurationEntry<int> SetAmountLimit = new ConfigurationEntry<int>(-1, "Sets an absolute limit to the number of drops. This will stop multipliers from generating more than the amount set in this condition. Ignored if 0 or less.");

        public ConfigurationEntry<bool> SetAutoStack = new ConfigurationEntry<bool>(false, "If true, will attempt to stack items before dropping them. This means the item generation will only be run once per stack.");

        #endregion

        #region DropExtended Conditions

        public ConfigurationEntry<int> ConditionMinLevel = new ConfigurationEntry<int>(-1, "Minimum level of mob for which item drops.");

        public ConfigurationEntry<int> ConditionMaxLevel = new ConfigurationEntry<int>(-1, "Maximum level of mob for which item drops.");

        public ConfigurationEntry<bool> ConditionNotDay = new ConfigurationEntry<bool>(false, "If true, will not drop during daytime.");

        public ConfigurationEntry<bool> ConditionNotAfternoon = new ConfigurationEntry<bool>(false, "If true, will not drop during afternoon.");

        public ConfigurationEntry<bool> ConditionNotNight = new ConfigurationEntry<bool>(false, "If true, will not drop during night.");

        public ConfigurationEntry<string> ConditionEnvironments = new ConfigurationEntry<string>("", "Array (separated by ,) of environment names that allow the item to drop while they are active.\nEg. Misty, Thunderstorm. Leave empty to always allow.");

        public ConfigurationEntry<string> ConditionGlobalKeys = new ConfigurationEntry<string>("", "Array(separated by,) of global keys names that allow the item to drop while they are active.\nEg. defeated_eikthyr,defeated_gdking.Leave empty to always allow.");

        public ConfigurationEntry<string> ConditionNotGlobalKeys = new ConfigurationEntry<string>("", "Array (separated by ,) of global key names that stop the item from dropping if any are detected.\nEg. defeated_eikthyr,defeated_gdking");

        public ConfigurationEntry<string> ConditionBiomes = new ConfigurationEntry<string>("", "Array(separated by,) of biome names that allow the item to drop while they are active.\nEg. Meadows, Swamp. Leave empty to always allow.");

        public ConfigurationEntry<string> ConditionCreatureStates = new ConfigurationEntry<string>("", "Array (separated by,) of creature states for which the item drop. If empty, allows all.\nEg. Default,Tamed,Event");

        public ConfigurationEntry<string> ConditionNotCreatureStates = new ConfigurationEntry<string>("", "Array (separated by,) of creature states for which the item will not drop.\nEg. Default,Tamed,Event");

        public ConfigurationEntry<string> ConditionHasItem = new ConfigurationEntry<string>("", "Array of items (prefab names) that will enable this drop. If empty, allows all.\nEg. skeleton_bow");

        public ConfigurationEntry<string> ConditionFaction = new ConfigurationEntry<string>("", "Array of factions that will enable this drop. If empty, allows all.\nEg. Undead, Boss");

        public ConfigurationEntry<string> ConditionNotFaction = new ConfigurationEntry<string>("", "Array of factions that will disable this drop. If empty, this condition is ignored.\nEg. Undead, boss");

        public ConfigurationEntry<string> ConditionLocation = new ConfigurationEntry<string>("", "Array of location names. When mob spawned in one of the listed locations, this drop is enabled.\nEg. Runestone_Boars");

        public ConfigurationEntry<string> ConditionKilledByDamageType = new ConfigurationEntry<string>("", "Array of damage types that will enable this drop, if they were part of the final killing blow. If empty, this condition is ignored.\nEg. Blunt, Fire");

        public ConfigurationEntry<string> ConditionKilledWithStatus = new ConfigurationEntry<string>("", "Array of statuses that mob had any of while dying, to enable this drop. If empty, this condition is ignored.\nEg. Burning, Smoked");

        public ConfigurationEntry<string> ConditionKilledWithStatuses = new ConfigurationEntry<string>("", "Array of statuses that mob must have had all of while dying, to enable this drop. If empty, this condition is ignored.\nEg. Burning, Smoked");

        public ConfigurationEntry<string> ConditionKilledBySkillType = new ConfigurationEntry<string>("", "Array of skill types that will enable this drop, if they were listed as the skill causing the damage of the final killing blow. If empty, this condition is ignored.\nEg. Swords, Unarmed");

        #endregion

        // Inefficient, but will do for now.
        public int Index
        {
            get
            {
                if (int.TryParse(SectionName, out int result))
                {
                    if (result < 0)
                    {
                        return int.MaxValue;
                    }

                    return result;
                }
                return int.MaxValue;
            }
        }

        public bool IsValid()
        {
            if (ItemName == null || string.IsNullOrEmpty(ItemName.Value))
            {
                return false;
            }

            if (AmountMin == null || AmountMin.Value < 0)
            {
                return false;
            }

            if (AmountMax == null || AmountMax.Value < 0 || AmountMax.Value < AmountMin.Value)
            {
                return false;
            }

            return true;
        }
    }

    [Serializable]
    public class DropModConfigCLLC : Config
    {
        public const string ModName = "CreatureLevelAndLootControl";

        public ConfigurationEntry<string> ConditionBossAffix = new ConfigurationEntry<string>("", "Array (separated by ,) of boss affixes, for which item will drop.");

        public ConfigurationEntry<string> ConditionNotBossAffix = new ConfigurationEntry<string>("", "Array (separated by ,) of boss affixes, for which item will not drop.");

        public ConfigurationEntry<string> ConditionInfusion = new ConfigurationEntry<string>("", "Array (separated by ,) of creature infusions, for which item will drop.");

        public ConfigurationEntry<string> ConditionNotInfusion = new ConfigurationEntry<string>("", "Array (separated by ,) of creature infusions, for which item will not drop.");

        public ConfigurationEntry<string> ConditionExtraEffect = new ConfigurationEntry<string>("", "Array (separated by ,) of creature extra effects, for which item will drop.");

        public ConfigurationEntry<string> ConditionNotExtraEffect = new ConfigurationEntry<string>("", "Array (separated by ,) of creature extra effects, for which item will not drop.");
    }

    [Serializable]
    public class DropModConfigEpicLoot : Config
    {
        public const string ModName = "EpicLoot";

        public ConfigurationEntry<float> RarityWeightNone = new ConfigurationEntry<float>(0, "Weight to use for rolling as a non-magic item.");
        public ConfigurationEntry<float> RarityWeightMagic = new ConfigurationEntry<float>(0, "Weight to use for rolling as rarity 'Magic'");
        public ConfigurationEntry<float> RarityWeightRare = new ConfigurationEntry<float>(0, "Weight to use for rolling as rarity 'Rare'");
        public ConfigurationEntry<float> RarityWeightEpic = new ConfigurationEntry<float>(0, "Weight to use for rolling as rarity 'Epic'");
        public ConfigurationEntry<float> RarityWeightLegendary = new ConfigurationEntry<float>(0, "Weight to use for rolling as rarity 'Legendary'");
        public ConfigurationEntry<float> RarityWeightUnique = new ConfigurationEntry<float>(0, "Weight to use for rolling unique items from the UniqueIDs array. If item rolls as unique, a single id will be selected randomly from the UniqueIDs.");
        public ConfigurationEntry<string> UniqueIDs = new ConfigurationEntry<string>("", "Id's for unique legendaries from Epic Loot. Will drop as a non-magic item if the legendary does not meet its requirements.\nEg. HeimdallLegs, RagnarLegs");
    }

    [Serializable]
    public class DropModConfigSpawnThat : Config
    {
        public const string ModName = "SpawnThat";

        public ConfigurationEntry<string> ConditionTemplateId = new ConfigurationEntry<string>("", "Array of Spawn That TemplateId values to enable to drop for.");
    }
}

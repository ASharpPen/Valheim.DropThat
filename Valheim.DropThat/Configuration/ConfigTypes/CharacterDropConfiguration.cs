using System;
using Valheim.DropThat.Core.Configuration;

namespace Valheim.DropThat.Configuration.ConfigTypes
{
    /// <summary>
    /// CharacterDrop configurations
    /// </summary>
    [Serializable]
    public class CharacterDropConfiguration : ConfigWithSubsections<CharacterDropMobConfiguration>, IConfigFile
    {
        protected override CharacterDropMobConfiguration InstantiateSubsection(string subsectionName)
        {
            return new CharacterDropMobConfiguration();
        }
    }

    [Serializable]
    public class CharacterDropMobConfiguration : ConfigWithSubsections<CharacterDropItemConfiguration>
    {
        protected override CharacterDropItemConfiguration InstantiateSubsection(string subsectionName)
        {
            return new CharacterDropItemConfiguration();
        }

        public ConfigurationEntry<string> UseDropList = new("", "Name of drop list to load for this entity. List items will be overriden by other drops listed here, if index is the same.");
    }

    [Serializable]
    public class CharacterDropItemConfiguration : ConfigWithSubsections<Config>
    {
        protected override Config InstantiateSubsection(string subsectionName)
        {
            Config newModConfig = null;

            if(subsectionName == CharacterDropModConfigCLLC.ModName)
            {
                newModConfig = new CharacterDropModConfigCLLC();
            }
            else if(subsectionName == CharacterDropModConfigEpicLoot.ModName)
            {
                newModConfig = new CharacterDropModConfigEpicLoot();
            }
            else if(subsectionName == CharacterDropModConfigSpawnThat.ModName)
            {
                newModConfig = new CharacterDropModConfigSpawnThat();
            }

            return newModConfig;
        }

        #region CharacterDrop.Drop

        public ConfigurationEntry<string> PrefabName = new ConfigurationEntry<string>("", "Prefab name of item to drop.");
        public ConfigurationEntry<bool> EnableConfig = new ConfigurationEntry<bool>(true, "Enable/disable this specific drop configuration.");
        public ConfigurationEntry<int> SetAmountMin = new ConfigurationEntry<int>(1, "Minimum amount dropped.");
        public ConfigurationEntry<int> SetAmountMax = new ConfigurationEntry<int>(1, "Maximum amount dropped.");
        public ConfigurationEntry<float> SetChanceToDrop = new ConfigurationEntry<float>(100, "Chance to drop. 100 is 100%.\nExample values: 0, 50, 0.15");
        public ConfigurationEntry<bool> SetDropOnePerPlayer = new ConfigurationEntry<bool>(false, "If set, will drop one of this item per player. Ignoring other factors such as SetAmountMin / Max.");
        public ConfigurationEntry<bool> SetScaleByLevel = new ConfigurationEntry<bool>(true, "Toggles mob levels scaling up dropped amount. Be aware, this scales up very quickly and may cause issues when dropping many items.");
        public ConfigurationEntry<bool> DisableResourceModifierScaling = new ConfigurationEntry<bool>(false, "Disables resource scaling from world-modifiers if true.");
 
        #endregion

        #region DropExtended Modifiers

        public ConfigurationEntry<int> SetQualityLevel = new (-1, "Sets the quality level of the item. If 0 or less, uses default quality level of drop.");
        public ConfigurationEntry<int> SetAmountLimit = new (-1, "Sets an absolute limit to the number of drops. This will stop multipliers from generating more than the amount set in this condition. Ignored if 0 or less.");
        public ConfigurationEntry<bool> SetAutoStack = new (false, "If true, will attempt to stack items before dropping them. This means the item generation will only be run once per stack.");
        public ConfigurationEntry<float> SetDurability = new(-1, "Sets the durability of the item. Does not change max durability. If less than 0, uses default.");

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
        public ConfigurationEntry<float> ConditionDistanceToCenterMin = new(0, "Minimum distance to center of map, for drop to be enabled.");
        public ConfigurationEntry<float> ConditionDistanceToCenterMax = new(0, "Maximum distance to center of map, within which drop is enabled. 0 means limitless.");
        public ConfigurationEntry<string> ConditionKilledByDamageType = new ConfigurationEntry<string>("", "Array of damage types that will enable this drop, if they were part of the final killing blow. If empty, this condition is ignored.\nEg. Blunt, Fire");
        public ConfigurationEntry<string> ConditionKilledWithStatus = new ConfigurationEntry<string>("", "Array of statuses that mob had any of while dying, to enable this drop. If empty, this condition is ignored.\nEg. Burning, Smoked");
        public ConfigurationEntry<string> ConditionKilledWithStatuses = new ConfigurationEntry<string>("", "Array of statuses that mob must have had all of while dying, to enable this drop. If empty, this condition is ignored.\nEg. Burning, Smoked");
        public ConfigurationEntry<string> ConditionKilledBySkillType = new ConfigurationEntry<string>("", "Array of skill types that will enable this drop, if they were listed as the skill causing the damage of the final killing blow. If empty, this condition is ignored.\nEg. Swords, Unarmed");
        public ConfigurationEntry<string> ConditionKilledByEntityType = new("", "Array of entity types that if causing the last hit, will enable this drop. If empty, this condition is ignored.\nEg. Player, Creature, Other");
        public ConfigurationEntry<string> ConditionHitByEntityTypeRecently = new("", "Array of entity types. If any of the listed types have hit the creature recently (default 60 seconds), drop is enabled. If empty, this condition is ignored.\nEg. Player, Creature, Other");

        #endregion

        public bool IsFromNamedList { get; set; }

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
            if (PrefabName == null || string.IsNullOrEmpty(PrefabName.Value))
            {
                return false;
            }

            if (SetAmountMin == null || SetAmountMin.Value < 0)
            {
                return false;
            }

            if (SetAmountMax == null || SetAmountMax.Value < 0 || SetAmountMax.Value < SetAmountMin.Value)
            {
                return false;
            }

            return true;
        }
    }

    [Serializable]
    public class CharacterDropModConfigCLLC : Config
    {
        public const string ModName = "CreatureLevelAndLootControl";

        public ConfigurationEntry<string> ConditionBossAffix = new ("", "Array of boss affixes, for which item will drop.");
        public ConfigurationEntry<string> ConditionNotBossAffix = new ("", "Array of boss affixes, for which item will not drop.");
        public ConfigurationEntry<string> ConditionInfusion = new ("", "Array of creature infusions, for which item will drop.");
        public ConfigurationEntry<string> ConditionNotInfusion = new ("", "Array of creature infusions, for which item will not drop.");
        public ConfigurationEntry<string> ConditionExtraEffect = new ("", "Array of creature extra effects, for which item will drop.");
        public ConfigurationEntry<string> ConditionNotExtraEffect = new ("", "Array of creature extra effects, for which item will not drop.");
        public ConfigurationEntry<int> ConditionWorldLevelMin = new(0, "Minimum CLLC world level, for which item will drop.");
        public ConfigurationEntry<int> ConditionWorldLevelMax = new(0, "Maximum CLLC world level, for which item will drop. 0 or less means no max.");
    }

    [Serializable]
    public class CharacterDropModConfigEpicLoot : Config
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
    public class CharacterDropModConfigSpawnThat : Config
    {
        public const string ModName = "SpawnThat";

        public ConfigurationEntry<string> ConditionTemplateId = new ConfigurationEntry<string>("", "Array of Spawn That TemplateId values to enable to drop for.");
    }
}

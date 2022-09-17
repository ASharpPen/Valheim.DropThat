using System;
using System.Collections.Generic;
using DropThat.Core.Toml;

namespace DropThat.Drop.CharacterDropSystem.Configurations;

public class CharacterDropConfigurationFile : TomlConfigWithSubsections<CharacterDropMobConfiguration>, ITomlConfigFile
{
    protected override CharacterDropMobConfiguration InstantiateSubsection(string subsectionName)
    {
        return new CharacterDropMobConfiguration();
    }
}

public class CharacterDropMobConfiguration : TomlConfigWithSubsections<CharacterDropItemConfiguration>
{
    protected override CharacterDropItemConfiguration InstantiateSubsection(string subsectionName)
    {
        return new CharacterDropItemConfiguration();
    }

    public TomlConfigEntry<string> UseDropList = new("UseDropList", "", "Name of drop list to load for this entity. List items will be overriden by other drops listed here, if index is the same.");
}

public class CharacterDropItemConfiguration : TomlConfigWithSubsections<TomlConfig>
{
    protected override TomlConfig InstantiateSubsection(string subsectionName)
    {
        TomlConfig newModConfig = null;

        if (subsectionName == CharacterDropModConfigCLLC.ModName)
        {
            newModConfig = new CharacterDropModConfigCLLC();
        }
        else if (subsectionName == CharacterDropModConfigEpicLoot.ModName)
        {
            newModConfig = new CharacterDropModConfigEpicLoot();
        }
        else if (subsectionName == CharacterDropModConfigSpawnThat.ModName)
        {
            newModConfig = new CharacterDropModConfigSpawnThat();
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

    public bool IsFromNamedList { get; set; }

    #region CharacterDrop.Drop

    public TomlConfigEntry<string> PrefabName = new("PrefabName", "", "Prefab name of item to drop.");
    public TomlConfigEntry<bool> EnableConfig = new("EnableConfig", true, "Enable/disable this specific drop configuration.");
    public TomlConfigEntry<int> SetAmountMin = new("SetAmountMin", 1, "Minimum amount dropped.");
    public TomlConfigEntry<int> SetAmountMax = new("SetAmountMax", 1, "Maximum amount dropped.");
    public TomlConfigEntry<float> SetChanceToDrop = new("SetChanceToDrop", 100, "Chance to drop. 100 is 100%.\nExample values: 0, 50, 0.15");
    public TomlConfigEntry<bool> SetDropOnePerPlayer = new("SetDropOnePerPlayer", false, "If set, will drop one of this item per player. Ignoring other factors such as SetAmountMin / Max.");
    public TomlConfigEntry<bool> SetScaleByLevel = new("SetScaleByLevel", true, "Toggles mob levels scaling up dropped amount. Be aware, this scales up very quickly and may cause issues when dropping many items.");

    #endregion

    #region DropExtended Modifiers

    public TomlConfigEntry<int> SetQualityLevel = new("SetQualityLevel", -1, "Sets the quality level of the item. If 0 or less, uses default quality level of drop.");
    public TomlConfigEntry<int> SetAmountLimit = new("SetAmountLimit", -1, "Sets an absolute limit to the number of drops. This will stop multipliers from generating more than the amount set in this condition. Ignored if 0 or less.");
    public TomlConfigEntry<bool> SetAutoStack = new("SetAutoStack", false, "If true, will attempt to stack items before dropping them. This means the item generation will only be run once per stack.");
    public TomlConfigEntry<float> SetDurability = new("SetDurability", -1, "Sets the durability of the item. Does not change max durability. If less than 0, uses default.");

    #endregion

    #region DropExtended Conditions

    public TomlConfigEntry<int> ConditionMinLevel = new("ConditionMinLevel", -1, "Minimum level of mob for which item drops.");
    public TomlConfigEntry<int> ConditionMaxLevel = new("ConditionMaxLevel", -1, "Maximum level of mob for which item drops.");
    public TomlConfigEntry<bool> ConditionNotDay = new("ConditionNotDay", false, "If true, will not drop during daytime.");
    public TomlConfigEntry<bool> ConditionNotAfternoon = new("ConditionNotAfternoon", false, "If true, will not drop during afternoon.");
    public TomlConfigEntry<bool> ConditionNotNight = new("ConditionNotNight", false, "If true, will not drop during night.");
    public TomlConfigEntry<string> ConditionEnvironments = new("ConditionEnvironments", "", "Array (separated by ,) of environment names that allow the item to drop while they are active.\nEg. Misty, Thunderstorm. Leave empty to always allow.");
    public TomlConfigEntry<string> ConditionGlobalKeys = new("ConditionGlobalKeys", "", "Array(separated by,) of global keys names that allow the item to drop while they are active.\nEg. defeated_eikthyr,defeated_gdking.Leave empty to always allow.");
    public TomlConfigEntry<string> ConditionNotGlobalKeys = new("ConditionNotGlobalKeys", "", "Array (separated by ,) of global key names that stop the item from dropping if any are detected.\nEg. defeated_eikthyr,defeated_gdking");
    public TomlConfigEntry<string> ConditionBiomes = new("ConditionBiomes", "", "Array(separated by,) of biome names that allow the item to drop while they are active.\nEg. Meadows, Swamp. Leave empty to always allow.");
    public TomlConfigEntry<string> ConditionCreatureStates = new("ConditionCreatureStates", "", "Array (separated by,) of creature states for which the item drop. If empty, allows all.\nEg. Default,Tamed,Event");
    public TomlConfigEntry<string> ConditionNotCreatureStates = new("ConditionNotCreatureStates", "", "Array (separated by,) of creature states for which the item will not drop.\nEg. Default,Tamed,Event");
    public TomlConfigEntry<string> ConditionHasItem = new("ConditionHasItem", "", "Array of items (prefab names) that will enable this drop. If empty, allows all.\nEg. skeleton_bow");
    public TomlConfigEntry<string> ConditionFaction = new("ConditionFaction", "", "Array of factions that will enable this drop. If empty, allows all.\nEg. Undead, Boss");
    public TomlConfigEntry<string> ConditionNotFaction = new("ConditionNotFaction", "", "Array of factions that will disable this drop. If empty, this condition is ignored.\nEg. Undead, boss");
    public TomlConfigEntry<string> ConditionLocation = new("ConditionLocation", "", "Array of location names. When mob spawned in one of the listed locations, this drop is enabled.\nEg. Runestone_Boars");
    public TomlConfigEntry<float> ConditionDistanceToCenterMin = new("ConditionDistanceToCenterMin", 0, "Minimum distance to center of map, for drop to be enabled.");
    public TomlConfigEntry<float> ConditionDistanceToCenterMax = new("ConditionDistanceToCenterMax", 0, "Maximum distance to center of map, within which drop is enabled. 0 means limitless.");
    public TomlConfigEntry<string> ConditionKilledByDamageType = new("ConditionKilledByDamageType", "", "Array of damage types that will enable this drop, if they were part of the final killing blow. If empty, this condition is ignored.\nEg. Blunt, Fire");
    public TomlConfigEntry<string> ConditionKilledWithStatus = new("ConditionKilledWithStatus", "", "Array of statuses that mob had any of while dying, to enable this drop. If empty, this condition is ignored.\nEg. Burning, Smoked");
    public TomlConfigEntry<string> ConditionKilledWithStatuses = new("ConditionKilledWithStatuses", "", "Array of statuses that mob must have had all of while dying, to enable this drop. If empty, this condition is ignored.\nEg. Burning, Smoked");
    public TomlConfigEntry<string> ConditionKilledBySkillType = new("ConditionKilledBySkillType", "", "Array of skill types that will enable this drop, if they were listed as the skill causing the damage of the final killing blow. If empty, this condition is ignored.\nEg. Swords, Unarmed");
    public TomlConfigEntry<string> ConditionKilledByEntityType = new("ConditionKilledByEntityType", "", "Array of entity types that if causing the last hit, will enable this drop. If empty, this condition is ignored.\nEg. Player, Creature, Other");
    public TomlConfigEntry<string> ConditionHitByEntityTypeRecently = new("ConditionHitByEntityTypeRecently", "", "Array of entity types. If any of the listed types have hit the creature recently (default 60 seconds), drop is enabled. If empty, this condition is ignored.\nEg. Player, Creature, Other");

    #endregion
}

public class CharacterDropModConfigCLLC : TomlConfig
{
    public const string ModName = "CreatureLevelAndLootControl";

    public TomlConfigEntry<List<string>> ConditionBossAffix = new("ConditionBossAffix", new(), "Array of boss affixes, for which item will drop.");
    public TomlConfigEntry<List<string>> ConditionNotBossAffix = new("ConditionNotBossAffix", new(), "Array of boss affixes, for which item will not drop.");
    public TomlConfigEntry<List<string>> ConditionInfusion = new("ConditionInfusion", new(), "Array of creature infusions, for which item will drop.");
    public TomlConfigEntry<List<string>> ConditionNotInfusion = new("ConditionNotInfusion", new(), "Array of creature infusions, for which item will not drop.");
    public TomlConfigEntry<List<string>> ConditionExtraEffect = new("ConditionExtraEffect", new(), "Array of creature extra effects, for which item will drop.");
    public TomlConfigEntry<List<string>> ConditionNotExtraEffect = new("ConditionNotExtraEffect", new(), "Array of creature extra effects, for which item will not drop.");
    public TomlConfigEntry<int?> ConditionWorldLevelMin = new("ConditionWorldLevelMin", 0, "Minimum CLLC world level, for which item will drop.");
    public TomlConfigEntry<int?> ConditionWorldLevelMax = new("ConditionWorldLevelMax", 0, "Maximum CLLC world level, for which item will drop. 0 or less means no max.");
}

public class CharacterDropModConfigEpicLoot : TomlConfig
{
    public const string ModName = "EpicLoot";

    public TomlConfigEntry<float> RarityWeightNone = new("RarityWeightNone", 0, "Weight to use for rolling as a non-magic item.");
    public TomlConfigEntry<float> RarityWeightMagic = new("RarityWeightMagic", 0, "Weight to use for rolling as rarity 'Magic'");
    public TomlConfigEntry<float> RarityWeightRare = new("RarityWeightRare", 0, "Weight to use for rolling as rarity 'Rare'");
    public TomlConfigEntry<float> RarityWeightEpic = new("RarityWeightEpic", 0, "Weight to use for rolling as rarity 'Epic'");
    public TomlConfigEntry<float> RarityWeightLegendary = new("RarityWeightLegendary", 0, "Weight to use for rolling as rarity 'Legendary'");
    public TomlConfigEntry<float> RarityWeightUnique = new("RarityWeightUnique", 0, "Weight to use for rolling unique items from the UniqueIDs array. If item rolls as unique, a single id will be selected randomly from the UniqueIDs.");
    public TomlConfigEntry<List<string>> UniqueIDs = new("UniqueIDs", new(), "Id's for unique legendaries from Epic Loot. Will drop as a non-magic item if the legendary does not meet its requirements.\nEg. HeimdallLegs, RagnarLegs");
}

public class CharacterDropModConfigSpawnThat : TomlConfig
{
    public const string ModName = "SpawnThat";

    public TomlConfigEntry<List<string>> ConditionTemplateId = new("ConditionTemplateId", new(), "Array of Spawn That TemplateId values to enable to drop for.");
}
using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;
using DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.SpawnThat;
using DropThat.Drop.CharacterDropSystem.Conditions;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Drop.Options.Modifiers.ModEpicLoot;
using DropThat.Drop.Options.Modifiers;
using DropThat.Drop.Options;
using DropThat.Integrations.CllcIntegration;
using DropThat.Integrations;
using ThatCore.Utilities.Valheim;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Configuration;

internal static partial class ConfigurationFileManager
{
    private static void RegisterMainMappings(CharacterDropConfigMapper mapper)
    {
        mapper.AddMobSetting()
            .FromFile()
            .Map<List<string>>(
                "UseDropList",
                null,
                "List of droplists to load before other drop settings",
                (value, builder) => builder.ListNames = value);

        // Default settings
        mapper
            .AddDropSetting()
            .FromFile()
            .Map<string>(
                "PrefabName",
                null,
                "Prefab name of item to drop.",
                (value, builder) => builder.PrefabName = value)
            .FromObject()
            .Map("PrefabName", x => x.PrefabName)

            .AddOption()
            .FromFile()
            .Map<bool?>(
                "EnableConfig",
                true,
                "Enable/disable this specific drop configuration.",
                (value, builder) => builder.TemplateEnabled = value)
            .FromObject()
            .Map("EnableConfig", x => x.TemplateEnabled)

            .AddOption()
            .FromFile()
            .Map<int?>(
                "SetAmountMin",
                1,
                "Minimum amount dropped.",
                (value, builder) => builder.AmountMin = value
                )
            .FromObject()
            .Map("SetAmountMin", x => x.AmountMin)

            .AddOption()
            .FromFile()
            .Map<int?>(
                "SetAmountMax",
                1,
                "Maximum amount dropped.",
                (value, builder) => builder.AmountMax = value
                )
            .FromObject()
            .Map("SetAmountMax", x => x.AmountMax)

            .AddOption()
            .FromFile()
            .Map<float?>(
                "SetChanceToDrop", 100f, "Chance to drop. 100 is 100%.\nExample values: 0, 50, 0.15",
                (value, builder) => builder.ChanceToDrop = value)
            .FromObject()
            .Map<float?>("SetChanceToDrop", x => x.ChanceToDrop)

            .AddOption()
            .FromFile()
            .Map<bool?>(
                "SetDropOnePerPlayer", false, "If set, will drop one of this item per player. Ignoring other factors such as SetAmountMin / Max.",
                (value, builder) => builder.DropOnePerPlayer = value)
            .FromObject()
            .Map("SetDropOnePerPlayer", x => x.DropOnePerPlayer)

            .AddOption()
            .FromFile()
            .Map<bool?>(
                "SetScaleByLevel", true, "Toggles mob levels scaling up dropped amount. Be aware, this scales up very quickly and may cause issues when dropping many items.",
                (value, builder) => builder.ScaleByLevel = value)
            .FromObject()
            .Map("SetScaleByLevel", x => x.ScaleByLevel)
            ;

        // Modifiers
        mapper
            .AddDropSetting()
            .FromFile()
            .Map<int?>(
                "SetQualityLevel", -1, "Sets the quality level of the item. If 0 or less, uses default quality level of drop.",
                (value, builder) => builder.ModifierQualityLevel(value))
            .FromObject()
            .Map("SetQualityLevel", x => x.FindModifier<ModifierQualityLevel>()?.QualityLevel)

            .AddOption()
            .FromFile()
            .Map<int?>(
                "SetAmountLimit", -1, "Sets an absolute limit to the number of drops. This will stop multipliers from generating more than the amount set in this condition. Ignored if 0 or less.",
                (value, builder) => builder.AmountLimit = value)
            .FromObject()
            .Map("SetAmountLimit", x => x.AmountLimit)

            .AddOption()
            .FromFile()
            .Map<bool?>(
                "SetAutoStack", false, "If true, will attempt to stack items before dropping them. This means the item generation will only be run once per stack.",
                (value, builder) => builder.AutoStack = value)
            .FromObject()
            .Map("SetAutoStack", x => x.AutoStack)

            .AddOption()
            .FromFile()
            .Map<float?>(
                "SetDurability", -1f, "Sets the durability of the item. Does not change max durability. If less than 0, uses default.",
                (value, builder) => builder.ModifierDurability(value))
            .FromObject()
            .Map("SetDurability", x => x.FindModifier<ModifierDurability>()?.Durability)
            ;

        // Conditions
        mapper
            .AddDropSetting()
            .FromFile()
            .Map<int?>(
                "ConditionMinLevel", -1, "Minimum level of mob for which item drops.",
                (value, builder) => builder.ConditionLevelMin(value))
            .FromObject()
            .Map("ConditionMinLevel", x => x.FindCondition<ConditionLevelMin>()?.MinLevel)

            .AddOption()
            .FromFile()
            .Map<int?>(
                "ConditionMaxLevel", -1, "Maximum level of mob for which item drops.",
                (value, builder) => builder.ConditionLevelMax(value))
            .FromObject()
            .Map("ConditionMaxLevel", x => x.FindCondition<ConditionLevelMax>()?.MaxLevel)

            .AddOption()
            .FromFile()
            .Map<bool?>(
                "ConditionNotDay", false, "If true, will not drop during daytime.",
                (value, builder) => builder.ConditionNotDay(value))
            .FromObject()
            .Map("ConditionNotDay", x => x.FindCondition<ConditionNotDay>() is not null)

            .AddOption()
            .FromFile()
            .Map<bool?>(
                "ConditionNotAfternoon", false, "If true, will not drop during afternoon.",
                (value, builder) => builder.ConditionNotAfternoon(value))
            .FromObject()
            .Map("ConditionNotAfternoon", x => x.FindCondition<ConditionNotAfternoon>() is not null)

            .AddOption()
            .FromFile()
            .Map<bool?>(
                "ConditionNotNight", false, "If true, will not drop during night.",
                (value, builder) => builder.ConditionNotNight(value))
            .FromObject()
            .Map("ConditionNotNight", x => x.FindCondition<ConditionNotNight>() is not null)

            .AddOption()
            .FromFile()
            .Map<List<string>>(
                "ConditionEnvironments", null, "List of environment names that allow the item to drop while they are active.\nEg. Misty, Thunderstorm. Leave empty to always allow.",
                (value, builder) => builder.ConditionEnvironments(value))
            .FromObject()
            .Map("ConditionEnvironments", x => x.FindCondition<ConditionEnvironments>()?.Environments?.ToList())

            .AddOption()
            .FromFile()
            .Map<List<string>>(
                "ConditionGlobalKeys", null, "List of global keys names that allow the item to drop while they are active.\nEg. defeated_eikthyr,defeated_gdking.Leave empty to always allow.",
                (value, builder) => builder.ConditionGlobalKeysAny(value))
            .FromObject()
            .Map("ConditionGlobalKeys", x => x.FindCondition<ConditionGlobalKeysAny>()?.GlobalKeys?.ToList())

            .AddOption()
            .FromFile()
            .Map<List<string>>(
                "ConditionNotGlobalKeys", null, "List of global key names that stop the item from dropping if any are detected.\nEg. defeated_eikthyr,defeated_gdking",
                (value, builder) => builder.ConditionGlobalKeysNotAny(value))
            .FromObject()
            .Map("ConditionNotGlobalKeys", x => x.FindCondition<ConditionGlobalKeysNotAny>()?.GlobalKeys?.ToList())

            .AddOption()
            .FromFile()
            .Map<List<Heightmap.Biome>>(
                "ConditionBiomes", null, "List of biome names that allow the item to drop while they are active.\nEg. Meadows, Swamp. Leave empty to always allow.",
                (value, builder) => builder.ConditionBiome(value))
            .FromObject()
            .Map("ConditionBiomes", x => x.FindCondition<ConditionBiome>()?.BiomeMask.Split())

            .AddOption()
            .FromFile()
            .Map<List<CreatureState>>(
                "ConditionCreatureStates", null, "List of creature states for which the item drop. If empty, allows all.\nEg. Default,Tamed,Event",
                (value, builder) => builder.ConditionCreatureState(value))
            .FromObject()
            .Map("ConditionCreatureStates", x => x.FindCondition<ConditionCreatureState>()?.CreatureStates?.ToList())

            .AddOption()
            .FromFile()
            .Map<List<CreatureState>>(
                "ConditionNotCreatureStates", null, "List of creature states for which the item will not drop.\nEg. Default,Tamed,Event",
                (value, builder) => builder.ConditionNotCreatureState(value))
            .FromObject()
            .Map("ConditionNotCreatureStates", x => x.FindCondition<ConditionNotCreatureState>()?.CreatureStates?.ToList())

            .AddOption()
            .FromFile()
            .Map<List<string>>(
                "ConditionHasItem", null, "List of item prefab names that will enable this drop. If empty, allows all.\nEg. skeleton_bow",
                (value, builder) => builder.ConditionInventory(value))
            .FromObject()
            .Map("ConditionHasItem", x => x.FindCondition<ConditionInventory>()?.Items?.ToList())

            .AddOption()
            .FromFile()
            .Map<List<Character.Faction>>(
                "ConditionFaction", null, "List of factions that will enable this drop. If empty, allows all.\nEg. Undead, Boss",
                (value, builder) => builder.ConditionFaction(value))
            .FromObject()
            .Map("ConditionFaction", x => x.FindCondition<ConditionFaction>()?.Factions?.ToList())

            .AddOption()
            .FromFile()
            .Map<List<Character.Faction>>(
                "ConditionNotFaction", null, "List of factions that will disable this drop. If empty, this condition is ignored.\nEg. Undead, boss",
                (value, builder) => builder.ConditionNotFaction(value))
            .FromObject()
            .Map("ConditionNotFaction", x => x.FindCondition<ConditionNotFaction>()?.Factions?.ToList())

            .AddOption()
            .FromFile()
            .Map<List<string>>(
                "ConditionLocation", null, "List of location names. When mob spawned in one of the listed locations, this drop is enabled.\nEg. Runestone_Boars",
                (value, builder) => builder.ConditionLocation(value))
            .FromObject()
            .Map("ConditionLocation", x => x.FindCondition<ConditionLocation>()?.Locations?.ToList())

            .AddOption()
            .FromFile()
            .Map<float?>(
                "ConditionDistanceToCenterMin", 0, "Minimum distance to center of map, for drop to be enabled.",
                (value, builder) => builder.ConditionDistanceToCenterMin(value))
            .FromObject()
            .Map("ConditionDistanceToCenterMin", x => x.FindCondition<ConditionDistanceToCenterMin>()?.DistanceToCenterMin)

            .AddOption()
            .FromFile()
            .Map<float?>(
                "ConditionDistanceToCenterMax", 0, "Maximum distance to center of map, within which drop is enabled. 0 means limitless.",
                (value, builder) => builder.ConditionDistanceToCenterMax(value))
            .FromObject()
            .Map("ConditionDistanceToCenterMax", x => x.FindCondition<ConditionDistanceToCenterMax>()?.DistanceToCenterMax)

            .AddOption()
            .FromFile()
            .Map<List<HitData.DamageType>>(
                "ConditionKilledByDamageType", null, "List of damage types that will enable this drop, if they were part of the final killing blow. If empty, this condition is ignored.\nEg. Blunt, Fire",
                (value, builder) => builder.ConditionKilledByDamageType(value))
            .FromObject()
            .Map("ConditionKilledByDamageType", x => x.FindCondition<ConditionKilledByDamageType>()?.DamageTypeMask.Split())

            .AddOption()
            .FromFile()
            .Map<List<string>>(
                "ConditionKilledWithStatus", null, "List of statuses that mob had any of while dying, to enable this drop. If empty, this condition is ignored.\nEg. Burning, Smoked",
                (value, builder) => builder.ConditionKilledWithStatusAny(value))
            .FromObject()
            .Map("ConditionKilledWithStatus", x => x.FindCondition<ConditionKilledWithStatusAny>()?.Statuses?.ToList())

            .AddOption()
            .FromFile()
            .Map<List<string>>(
                "ConditionKilledWithStatuses", null, "List of statuses that mob must have had all of while dying, to enable this drop. If empty, this condition is ignored.\nEg. Burning, Smoked",
                (value, builder) => builder.ConditionKilledWithStatusAll(value))
            .FromObject()
            .Map("ConditionKilledWithStatuses", x => x.FindCondition<ConditionKilledWithStatusAll>()?.Statuses?.ToList())

            .AddOption()
            .FromFile()
            .Map<List<Skills.SkillType>>(
                "ConditionKilledBySkillType", null, "List of skill types that will enable this drop, if they were listed as the skill causing the damage of the final killing blow. If empty, this condition is ignored.\nEg. Swords, Unarmed",
                (value, builder) => builder.ConditionKilledBySkillType(value))
            .FromObject()
            .Map("ConditionKilledBySkillType", x => x.FindCondition<ConditionKilledBySkillType>()?.SkillTypes?.ToList())

            .AddOption()
            .FromFile()
            .Map<List<EntityType>>(
                "ConditionKilledByEntityType", null, "List of entity types that if causing the last hit, will enable this drop. If empty, this condition is ignored.\nEg. Player, Creature, Other",
                (value, builder) => builder.ConditionKilledByEntityType(value))
            .FromObject()
            .Map("ConditionKilledByEntityType", x => x.FindCondition<ConditionKilledByEntityType>()?.EntityTypes?.ToList())

            .AddOption()
            .FromFile()
            .Map<List<EntityType>>(
                "ConditionHitByEntityTypeRecently", null, "List of entity types. If any of the listed types have hit the creature recently (default 60 seconds), drop is enabled. If empty, this condition is ignored.\nEg. Player, Creature, Other",
                (value, builder) => builder.ConditionHitByEntityTypeRecently(value))
            .FromObject()
            .Map("ConditionHitByEntityTypeRecently", x => x.FindCondition<ConditionHitByEntityTypeRecently>()?.EntityTypes?.ToList())
            ;

        // Mods - Creature Level And Loot Control
        mapper
            .AddModRequirement("CreatureLevelAndLootControl", () => InstallationManager.CLLCInstalled)
            .AddModSettings("CreatureLevelAndLootControl")
            .FromFile()
            .Map<List<CllcBossAffix>>(
                "ConditionBossAffix", null, "Boss affixes for which item will drop.",
                (value, builder) => builder.ConditionBossAffix(value))
            .FromObject()
            .Map("ConditionBossAffix", x => x.FindCondition<ConditionBossAffix>()?.BossAffixes?.ToList())

            .AddOption()
            .FromFile()
            .Map<List<CllcBossAffix>>(
                "ConditionNotBossAffix", null, "Boss affixes for which item will not drop.",
                (value, builder) => builder.ConditionNotBossAffix(value))
            .FromObject()
            .Map("ConditionNotBossAffix", x => x.FindCondition<ConditionNotBossAffix>()?.BossAffixes?.ToList())

            .AddOption()
            .FromFile()
            .Map<List<CllcCreatureInfusion>>(
                "ConditionInfusion", null, "Creature infusions for which item will drop.",
                (value, builder) => builder.ConditionInfusion(value))
            .FromObject()
            .Map("ConditionInfusion", x => x.FindCondition<ConditionInfusion>()?.Infusions?.ToList())

            .AddOption()
            .FromFile()
            .Map<List<CllcCreatureInfusion>>(
                "ConditionNotInfusion", null, "Creature infusions for which item will not drop.",
                (value, builder) => builder.ConditionNotInfusion(value))
            .FromObject()
            .Map("ConditionNotInfusion", x => x.FindCondition<ConditionNotInfusion>()?.Infusions?.ToList())

            .AddOption()
            .FromFile()
            .Map<List<CllcCreatureExtraEffect>>(
                "ConditionExtraEffect", null, "Creature extra effects for which item will drop.",
                (value, builder) => builder.ConditionCreatureExtraEffect(value))
            .FromObject()
            .Map("ConditionExtraEffect", x => x.FindCondition<ConditionCreatureExtraEffect>()?.ExtraEffects?.ToList())

            .AddOption()
            .FromFile()
            .Map<List<CllcCreatureExtraEffect>>(
                "ConditionNotExtraEffect", null, "Creature extra effects for which item will not drop.",
                (value, builder) => builder.ConditionNotCreatureExtraEffect(value))
            .FromObject()
            .Map("ConditionNotExtraEffect", x => x.FindCondition<ConditionNotCreatureExtraEffect>()?.ExtraEffects?.ToList())

            .AddOption()
            .FromFile()
            .Map<int?>(
                "ConditionWorldLevelMin", 0, "Minimum CLLC world level for which item will drop.",
                (value, builder) => builder.ConditionWorldLevelMin(value))
            .FromObject()
            .Map("ConditionWorldLevelMin", x => x.FindCondition<ConditionWorldLevelMin>()?.MinLevel)

            .AddOption()
            .FromFile()
            .Map<int?>(
                "ConditionWorldLevelMax", 0, "Maximum CLLC world level for which item will drop. 0 or less means no max.",
                (value, builder) => builder.ConditionWorldLevelMax(value))
            .FromObject()
            .Map("ConditionWorldLevelMax", x => x.FindCondition<ConditionWorldLevelMax>()?.MaxLevel)
            ;

        // Mods - Epic Loot
        mapper
            .AddModRequirement("EpicLoot", () => InstallationManager.EpicLootInstalled)
            .AddModSettings("EpicLoot")
            .FromFile((CharacterDropDropBuilder builder) =>
            {
                var modifier = builder.FindModifier<ModifierEpicLootItem>();

                if (modifier is null)
                {
                    modifier = new ModifierEpicLootItem();
                    builder.ItemModifiers.Add(modifier);
                }

                return modifier;
            })
            .Map<float?>(
                "RarityWeightNone", 0, "Weight to use for rolling as a non-magic item.",
                (value, modifier) => modifier.RarityWeightNone = value)
            .Map<float?>(
                "RarityWeightMagic", 0, "Weight to use for rolling as rarity 'Magic'",
                (value, modifier) => modifier.RarityWeightMagic = value)
            .Map<float?>(
                "RarityWeightRare", 0, "Weight to use for rolling as rarity 'Rare'",
                (value, modifier) => modifier.RarityWeightRare = value)
            .Map<float?>(
                "RarityWeightEpic", 0, "Weight to use for rolling as rarity 'Epic'",
                (value, modifier) => modifier.RarityWeightEpic = value)
            .Map<float?>(
                "RarityWeightLegendary", 0, "Weight to use for rolling as rarity 'Legendary'",
                (value, modifier) => modifier.RarityWeightLegendary = value)
            .Map<float?>(
                "RarityWeightUnique", 0, "Weight to use for rolling unique items from the UniqueIDs list. If item rolls as unique, a single id will be selected randomly from the UniqueIDs.",
                (value, modifier) => modifier.RarityWeightUnique = value)
            .Map<List<string>>(
                "UniqueIDs", null, "Id's for unique legendaries from Epic Loot. Will drop as a non-magic item if the legendary does not meet its requirements.\nEg. HeimdallLegs, RagnarLegs",
                (value, modifier) => modifier.UniqueIds = value)

            .FromObject((CharacterDropDropTemplate template) => template.FindModifier<ModifierEpicLootItem>())
            .Map("RarityWeightNone", x => x.RarityWeightNone)
            .Map("RarityWeightMagic", x => x.RarityWeightMagic)
            .Map("RarityWeightRare", x => x.RarityWeightRare)
            .Map("RarityWeightEpic", x => x.RarityWeightEpic)
            .Map("RarityWeightLegendary", x => x.RarityWeightLegendary)
            .Map("RarityWeightUnique", x => x.RarityWeightUnique)
            .Map("UniqueIDs", x => x.UniqueIds)
            ;

        // Mods - Spawn That
        mapper
            .AddModRequirement("SpawnThat", () => InstallationManager.SpawnThatInstalled)
            .AddModSettings("SpawnThat")
            .FromFile()
            .Map<List<string>>(
                "ConditionTemplateId", null, "List of Spawn That TemplateId values to enable to drop for.",
                (value, builder) => builder.ConditionTemplateId(value))
            .FromObject()
            .Map("ConditionTemplateId", x => x.FindCondition<ConditionTemplateId>()?.TemplateIds?.ToList())
            ;
    }

}

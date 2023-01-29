using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DropThat.Drop.CharacterDropSystem.Conditions;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Integrations.CllcIntegration;
using ThatCore.Config.Toml.Extensions;
using ThatCore.Config.Toml.Mapping;
using ThatCore.Config.Toml.Schema;
using ThatCore.Config.Toml;
using ThatCore.Extensions;
using DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;
using DropThat.Integrations;
using DropThat.Drop.Options.Modifiers.ModEpicLoot;
using DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.SpawnThat;
using DropThat.Drop.Options.Modifiers;

namespace DropThat.Drop.CharacterDropSystem.Configuration;
internal static class CharacterDropListSchemaGenerator
{
    internal static TomlSchemaBuilder GenerateCfgSchema()
    {
        var builder = new TomlSchemaBuilder();

        var dropConfigBuilder = builder
            .SetLayerAsCollection() // List layer
            .GetNode()
            .SetNextLayerAsCollection(); // Drop layer

        dropConfigBuilder
            .GetNode()
            .AddSetting("PrefabName", "", "Prefab name of item to drop.")
            .AddSetting("EnableConfig", true, "Enable/disable this specific drop configuration.")
            .AddSetting<int?>("SetAmountMin", 1, "Minimum amount dropped.")
            .AddSetting<int?>("SetAmountMax", 1, "Maximum amount dropped.")
            .AddSetting<float?>("SetChanceToDrop", 100f, "Chance to drop. 100 is 100%.\nExample values: 0, 50, 0.15")
            .AddSetting<bool?>("SetDropOnePerPlayer", false, "If set, will drop one of this item per player. Ignoring other factors such as SetAmountMin / Max.")
            .AddSetting<bool?>("SetScaleByLevel", true, "Toggles mob levels scaling up dropped amount. Be aware, this scales up very quickly and may cause issues when dropping many items.")

            .AddSetting<int?>("SetQualityLevel", -1, "Sets the quality level of the item. If 0 or less, uses default quality level of drop.")
            .AddSetting<int?>("SetAmountLimit", -1, "Sets an absolute limit to the number of drops. This will stop multipliers from generating more than the amount set in this condition. Ignored if 0 or less.")
            .AddSetting("SetAutoStack", false, "If true, will attempt to stack items before dropping them. This means the item generation will only be run once per stack.")
            .AddSetting<float?>("SetDurability", -1f, "Sets the durability of the item. Does not change max durability. If less than 0, uses default.")

            .AddSetting<int?>("ConditionMinLevel", -1, "Minimum level of mob for which item drops.")
            .AddSetting<int?>("ConditionMaxLevel", -1, "Maximum level of mob for which item drops.")
            .AddSetting<bool?>("ConditionNotDay", false, "If true, will not drop during daytime.")
            .AddSetting<bool?>("ConditionNotAfternoon", false, "If true, will not drop during afternoon.")
            .AddSetting<bool?>("ConditionNotNight", false, "If true, will not drop during night.")
            .AddSetting<List<string>>("ConditionEnvironments", null, "List of environment names that allow the item to drop while they are active.\nEg. Misty, Thunderstorm. Leave empty to always allow.")
            .AddSetting<List<string>>("ConditionGlobalKeys", null, "List of global keys names that allow the item to drop while they are active.\nEg. defeated_eikthyr,defeated_gdking.Leave empty to always allow.")
            .AddSetting<List<string>>("ConditionNotGlobalKeys", null, "List of global key names that stop the item from dropping if any are detected.\nEg. defeated_eikthyr,defeated_gdking")
            .AddSetting<List<Heightmap.Biome>>("ConditionBiomes", null, "List of biome names that allow the item to drop while they are active.\nEg. Meadows, Swamp. Leave empty to always allow.")
            .AddSetting<List<CreatureState>>("ConditionCreatureStates", null, "List of creature states for which the item drop. If empty, allows all.\nEg. Default,Tamed,Event")
            .AddSetting<List<CreatureState>>("ConditionNotCreatureStates", null, "List of creature states for which the item will not drop.\nEg. Default,Tamed,Event")
            .AddSetting<List<string>>("ConditionHasItem", null, "List of item prefab names that will enable this drop. If empty, allows all.\nEg. skeleton_bow")
            .AddSetting<List<Character.Faction>>("ConditionFaction", null, "List of factions that will enable this drop. If empty, allows all.\nEg. Undead, Boss")
            .AddSetting<List<Character.Faction>>("ConditionNotFaction", null, "List of factions that will disable this drop. If empty, this condition is ignored.\nEg. Undead, boss")
            .AddSetting<List<string>>("ConditionLocation", null, "List of location names. When mob spawned in one of the listed locations, this drop is enabled.\nEg. Runestone_Boars")
            .AddSetting<float?>("ConditionDistanceToCenterMin", 0, "Minimum distance to center of map, for drop to be enabled.")
            .AddSetting<float?>("ConditionDistanceToCenterMax", 0, "Maximum distance to center of map, within which drop is enabled. 0 means limitless.")
            .AddSetting<List<HitData.DamageType>>("ConditionKilledByDamageType", null, "List of damage types that will enable this drop, if they were part of the final killing blow. If empty, this condition is ignored.\nEg. Blunt, Fire")
            .AddSetting<List<string>>("ConditionKilledWithStatus", null, "List of statuses that mob had any of while dying, to enable this drop. If empty, this condition is ignored.\nEg. Burning, Smoked")
            .AddSetting<List<string>>("ConditionKilledWithStatuses", null, "List of statuses that mob must have had all of while dying, to enable this drop. If empty, this condition is ignored.\nEg. Burning, Smoked")
            .AddSetting<List<Skills.SkillType>>("ConditionKilledBySkillType", null, "List of skill types that will enable this drop, if they were listed as the skill causing the damage of the final killing blow. If empty, this condition is ignored.\nEg. Swords, Unarmed")
            .AddSetting<List<EntityType>>("ConditionKilledByEntityType", null, "List of entity types that if causing the last hit, will enable this drop. If empty, this condition is ignored.\nEg. Player, Creature, Other")
            .AddSetting<List<EntityType>>("ConditionHitByEntityTypeRecently", null, "List of entity types. If any of the listed types have hit the creature recently (default 60 seconds), drop is enabled. If empty, this condition is ignored.\nEg. Player, Creature, Other")
            ;

        var modLayer = dropConfigBuilder
            .GetNode()
            .SetNextLayerAsNamed();

        modLayer
            .AddNode("CreatureLevelAndLootControl")
            .AddSetting<List<CllcBossAffix>>("ConditionBossAffix", null, "Boss affixes for which item will drop.")
            .AddSetting<List<CllcBossAffix>>("ConditionNotBossAffix", null, "Boss affixes for which item will not drop.")
            .AddSetting<List<CllcCreatureInfusion>>("ConditionInfusion", null, "Creature infusions for which item will drop.")
            .AddSetting<List<CllcCreatureInfusion>>("ConditionNotInfusion", null, "Creature infusions for which item will not drop.")
            .AddSetting<List<CllcCreatureExtraEffect>>("ConditionExtraEffect", null, "Creature extra effects for which item will drop.")
            .AddSetting<List<CllcCreatureExtraEffect>>("ConditionNotExtraEffect", null, "Creature extra effects for which item will not drop.")
            .AddSetting<int?>("ConditionWorldLevelMin", 0, "Minimum CLLC world level for which item will drop.")
            .AddSetting<int?>("ConditionWorldLevelMax", 0, "Maximum CLLC world level for which item will drop. 0 or less means no max.")
            ;

        modLayer
            .AddNode("EpicLoot")
            .AddSetting<float?>("RarityWeightNone", 0, "Weight to use for rolling as a non-magic item.")
            .AddSetting<float?>("RarityWeightMagic", 0, "Weight to use for rolling as rarity 'Magic'")
            .AddSetting<float?>("RarityWeightRare", 0, "Weight to use for rolling as rarity 'Rare'")
            .AddSetting<float?>("RarityWeightEpic", 0, "Weight to use for rolling as rarity 'Epic'")
            .AddSetting<float?>("RarityWeightLegendary", 0, "Weight to use for rolling as rarity 'Legendary'")
            .AddSetting<float?>("RarityWeightUnique", 0, "Weight to use for rolling unique items from the UniqueIDs list. If item rolls as unique, a single id will be selected randomly from the UniqueIDs.")
            .AddSetting<List<string>>("UniqueIDs", null, "Id's for unique legendaries from Epic Loot. Will drop as a non-magic item if the legendary does not meet its requirements.\nEg. HeimdallLegs, RagnarLegs")
            ;

        modLayer
            .AddNode("SpawnThat")
            .AddSetting<List<string>>("ConditionTemplateId", null, "List of Spawn That TemplateId values to enable to drop for.")
            ;

        return builder;
    }

    internal static ConfigToObjectMapper<CharacterDropSystemConfiguration> GenerateConfigLoader(CharacterDropSystemConfiguration configuration)
    {
        MappingInstantiationForParent<CharacterDropSystemConfiguration, CharacterDropListBuilder> listMappings = new()
        {
            SubPath = new()
            {
                new TomlPathSegment(TomlPathSegmentType.Collection)
            },
            Instantiation = new()
            {
                Instantiation = (CharacterDropSystemConfiguration parent, TomlConfig config) => parent.GetListBuilder(config.PathSegment.Name),
            },
            SubInstantiations = new()
            {
                new MappingInstantiationForParent<CharacterDropListBuilder, CharacterDropDropBuilder>()
                {
                    SubPath = new() { new TomlPathSegment(TomlPathSegmentType.Collection) },
                    Instantiation = new()
                    {
                        Instantiation = (CharacterDropListBuilder builder, TomlConfig config) => builder.GetDrop(uint.Parse(config.PathSegment.Name)),
                        InstanceActions = new()
                        {
                            (TomlConfig config, CharacterDropDropBuilder builder) =>
                            {
                                builder.PrefabName.Configure(config, "PrefabName");
                                builder.Enabled.Configure(config, "EnableConfig");
                                builder.AmountMin.Configure(config, "SetAmountMin");
                                builder.AmountMax.Configure(config, "SetAmountMax");
                                builder.ChanceToDrop.Configure(config, "SetChanceToDrop");
                                builder.ScaleByLevel.Configure(config, "SetScaleByLevel");
                                builder.AmountLimit.Configure(config, "SetAmountLimit");
                                builder.AutoStack.Configure(config, "SetAutoStack");

                                config.Configure<int?>("ConditionMinLevel", x => builder.ConditionLevelMin(x));
                                config.Configure<int?>("ConditionMaxLevel", x => builder.ConditionLevelMax(x));
                                config.Configure<bool?>("ConditionNotDay", x => builder.ConditionNotDay(x));
                                config.Configure<bool?>("ConditionNotAfternoon", x => builder.ConditionNotAfternoon(x));
                                config.Configure<bool?>("ConditionNotNight", x => builder.ConditionNotNight(x));
                                config.Configure<List<string>>("ConditionEnvironments", x => builder.ConditionEnvironments(x));
                                config.Configure<List<string>>("ConditionGlobalKeys", x => builder.ConditionGlobalKeysAny(x));
                                config.Configure<List<string>>("ConditionNotGlobalKeys", x => builder.ConditionGlobalKeysNotAny(x));
                                config.Configure<List<Heightmap.Biome>>("ConditionBiomes", x => builder.ConditionBiome(x));
                                config.Configure<List<CreatureState>>("ConditionCreatureStates", x => builder.ConditionCreatureState(x));
                                config.Configure<List<CreatureState>>("ConditionNotCreatureStates", x => builder.ConditionNotCreatureState(x));
                                config.Configure<List<string>>("ConditionHasItem", x => builder.ConditionInventory(x));
                                config.Configure<List<Character.Faction>>("ConditionFaction", x => builder.ConditionFaction(x));
                                config.Configure<List<Character.Faction>>("ConditionNotFaction", x => builder.ConditionNotFaction(x));
                                config.Configure<List<string>>("ConditionLocation", x => builder.ConditionLocation(x));
                                config.Configure<float?>("ConditionDistanceToCenterMin", x => builder.ConditionDistanceToCenterMin(x));
                                config.Configure<float?>("ConditionDistanceToCenterMax", x => builder.ConditionDistanceToCenterMax(x));
                                config.Configure<List<HitData.DamageType>>("ConditionKilledByDamageType", x => builder.ConditionKilledByDamageType(x));
                                config.Configure<List<string>>("ConditionKilledWithStatus", x => builder.ConditionKilledWithStatusAny(x));
                                config.Configure<List<string>>("ConditionKilledWithStatuses", x => builder.ConditionKilledWithStatusAll(x));
                                config.Configure<List<Skills.SkillType>>("ConditionKilledBySkillType", x => builder.ConditionKilledBySkillType(x));
                                config.Configure<List<EntityType>>("ConditionKilledByEntityType", x => builder.ConditionKilledByEntityType(x));
                                config.Configure<List<EntityType>>("ConditionHitByEntityTypeRecently", x => builder.ConditionHitByEntityTypeRecently(x));

                                config.Configure<int?>("SetQualityLevel", x => builder.ModifierQualityLevel(x));
                                config.Configure<float?>("SetDurability", x => builder.ModifierDurability(x));
                            },
                            (TomlConfig config, CharacterDropDropBuilder builder) =>
                            {
                                if (!InstallationManager.CLLCInstalled)
                                {
                                    return;
                                }

                                if (!config.Sections.TryGetValue(
                                    new(TomlPathSegmentType.Named, "CreatureLevelAndLootControl"),
                                    out var cllcConfig))
                                {
                                    return;
                                }

                                config.Configure<List<CllcBossAffix>>("ConditionBossAffix", x => builder.ConditionBossAffix(x));
                                config.Configure<List<CllcBossAffix>>("ConditionNotBossAffix", x => builder.ConditionNotBossAffix(x));
                                config.Configure<List<CllcCreatureInfusion>>("ConditionInfusion", x => builder.ConditionInfusion(x));
                                config.Configure<List<CllcCreatureInfusion>>("ConditionNotInfusion", x => builder.ConditionNotInfusion(x));
                                config.Configure<List<CllcCreatureExtraEffect>>("ConditionExtraEffect", x => builder.ConditionCreatureExtraEffect(x));
                                config.Configure<List<CllcCreatureExtraEffect>>("ConditionNotExtraEffect", x => builder.ConditionNotCreatureExtraEffect(x));
                                config.Configure<int?>("ConditionWorldLevelMin", x => builder.ConditionWorldLevelMin(x));
                                config.Configure<int?>("ConditionWorldLevelMax", x => builder.ConditionWorldLevelMax(x));
                            },
                            (TomlConfig config, CharacterDropDropBuilder builder) =>
                            {
                                if (!InstallationManager.EpicLootInstalled)
                                {
                                    return;
                                }

                                if (!config.Sections.TryGetValue(
                                    new(TomlPathSegmentType.Named, "EpicLoot"),
                                    out var cllcConfig))
                                {
                                    return;
                                }

                                // Epic loot is a bit special, since its a whole bunch of parameters for the same modifier.
                                // Get a single modifier to configure, set individual fields, and clean it up afterwards if all are empty.
                                var epicLootModifier = builder.ModifierEpicLootItem();

                                (epicLootModifier, config)
                                    .Configure(x => x.RarityWeightNone, "RarityWeightNone")
                                    .Configure(x => x.RarityWeightMagic, "RarityWeightMagic")
                                    .Configure(x => x.RarityWeightRare, "RarityWeightRare")
                                    .Configure(x => x.RarityWeightEpic, "RarityWeightEpic")
                                    .Configure(x => x.RarityWeightLegendary, "RarityWeightLegendary")
                                    .Configure(x => x.RarityWeightUnique, "RarityWeightUnique")
                                    .Configure(x => x.UniqueIds, "UniqueIDs")
                                    ;

                                if (epicLootModifier.IsPointless)
                                {
                                    builder.ItemModifiers.Remove(epicLootModifier);
                                }
                            },
                            (TomlConfig config, CharacterDropDropBuilder builder) =>
                            {
                                if (!InstallationManager.SpawnThatInstalled)
                                {
                                    return;
                                }

                                config.Configure<List<string>>("ConditionTemplateId", x => builder.ConditionTemplateId(x));
                            }
                        }
                    },
                },
            },
        };

        return new ConfigToObjectMapper<CharacterDropSystemConfiguration>()
        {
            Path = new() { },
            Instantiation = new()
            {
                Instantiation = (TomlConfig) => configuration,
            },
            SubInstantiations = new()
            {
                listMappings,
            }
        };
    }

    private static void Configure<T>(this TomlConfig config, string settingName, Action<T> action)
    {
        if (config.Settings.TryGetValue(settingName, out var tomlSetting) &&
           tomlSetting.IsSet &&
           tomlSetting is TomlSetting<T> setting)
        {
            action(setting.Value);
        }
    }

    private static (T, TomlConfig) Configure<T, TProp>(
        this (T Template, TomlConfig Config) input,
        Expression<Func<T, TProp>> selector,
        string settingName = null)
    {
        var propInfo = selector.GetPropertyInfo();

        settingName ??= propInfo.Name;

        if (input.Config.Settings.TryGetValue(settingName, out var tomlSetting) &&
            tomlSetting.IsSet &&
            tomlSetting is TomlSetting<TProp> setting)
        {
            propInfo.SetValue(input.Template, setting.Value);
        }

        return input;
    }
}

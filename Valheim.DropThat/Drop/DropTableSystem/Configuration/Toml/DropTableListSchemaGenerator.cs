using System.Collections.Generic;
using DropThat.Drop.DropTableSystem.Conditions;
using DropThat.Drop.DropTableSystem.Conditions.ModSpecific.CLLC;
using DropThat.Drop.DropTableSystem.Conditions.ModSpecific.SpawnThat;
using DropThat.Drop.Options.Modifiers;
using DropThat.Drop.Options.Modifiers.ModEpicLoot;
using DropThat.Integrations;
using ThatCore.Config.Toml;
using ThatCore.Config.Toml.Extensions;
using ThatCore.Config.Toml.Mapping;
using ThatCore.Config.Toml.Schema;

namespace DropThat.Drop.DropTableSystem.Configuration.Toml;

internal static class DropTableListSchemaGenerator
{
    public static TomlSchemaBuilder GenerateCfgSchema()
    {
        var builder = new TomlSchemaBuilder();

        var listBuilder = builder
            .SetLayerAsCollection() // List layer
            .GetNode();

        var dropLayer = listBuilder
            .SetNextLayerAsCollection() // Drop layer
            .GetNode()
            .AddSetting<string>("PrefabName", default, "Name of prefab to drop.")
            .AddSetting<bool?>("EnableConfig", true, "Toggle this specific config entry on/off.")
            .AddSetting<bool?>("Enable", true, "Toggle this specific drop. This can be used to disable existing drops.")

            .AddSetting<int?>("SetAmountMin", 1, "Sets minimum amount pr drop. Behaviour depends on entity and item.")
            .AddSetting<int?>("SetAmountMax", 1, "Sets maximum amount pr drop. Behaviour depends on entity and item.")
            .AddSetting<float?>("SetTemplateWeight", 1, "Set weight for this drop. Used to control how likely it is that this item will be selected when rolling for drops. Note, same drop can be selected multiple times during table rolling.")

            .AddSetting<float?>("ConditionAltitudeMin", -10_000, "Minimum distance above or below sea-level for drop to be enabled.")
            .AddSetting<float?>("ConditionAltitudeMax", 10_000, "Maximum distance above or below sea-level for drop to be enabled.")
            .AddSetting<List<Heightmap.Biome>>("ConditionBiomes", default, "Biomes in which drop is enabled. If empty, condition will be ignored.")
            .AddSetting<bool?>("ConditionNotDay", default, "If true, will not drop during daytime.")
            .AddSetting<bool?>("ConditionNotNight", default, "If true, will not drop during night.")
            .AddSetting<bool?>("ConditionNotAfternoon", default, "If true, will not drop during afternoon.")
            .AddSetting<List<string>>("ConditionEnvironments", default, "List of environment names that allow the item to drop while they are active.\nEg. Misty, Thunderstorm. Leave empty to always allow.")
            .AddSetting<List<string>>("ConditionGlobalKeys", default, "List of global keys names of which at least one must be present for item to drop.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.")
            .AddSetting<List<string>>("ConditionGlobalKeysAll", default, "List of global keys names that must all be present for item to drop.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.")
            .AddSetting<List<string>>("ConditionGlobalKeysNotAny", default, "List of global keys names of which none must be present for item to drop.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.")
            .AddSetting<List<string>>("ConditionGlobalKeysNotAll", default, "List of global keys names of which all of the keys must be missing for item to drop.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.")
            .AddSetting<List<string>>("ConditionLocations", default, "List of location names. When spawned in one of the listed locations, this drop is enabled.\nEg. Runestone_Boars")
            .AddSetting<float?>("ConditionDistanceToCenterMin", 0, description: "Minimum distance to center of map, for drop to be enabled.")
            .AddSetting<float?>("ConditionDistanceToCenterMax", 0, "Maximum distance to center of map, within which drop is enabled. 0 means limitless.")
            .AddSetting<float?>("ConditionWithinCircle.CenterX", 0, "Center X coordinate of circle within which drop is enabled.")
            .AddSetting<float?>("ConditionWithinCircle.CenterZ", 0, "Center Z coordinate of circle within which drop is enabled.")
            .AddSetting<float?>("ConditionWithinCircle.Radius", -1, "Radius of circle within which drop is enabled.")

            .AddSetting<float?>("SetDurability", -1, "Sets the durability of the item. Does not change max durability. If less than 0, uses default.")
            .AddSetting<int?>("SetQualityLevel", 1, "Sets the quality level of the item. If 0 or less, uses default quality level of drop.")
            ;

        var modLayer = dropLayer
            .SetNextLayerAsNamed();

        modLayer
            .AddNode("SpawnThat")
            .AddSetting<List<string>>("ConditionTemplateId", default, "Array of Spawn That TemplateId values to enable to drop for.");

        modLayer
            .AddNode("CreatureLevelAndLootControl")
            .AddSetting<int?>("ConditionWorldLevelMin", 0, "Minimum CLLC world level, for which item will drop.")
            .AddSetting<int?>("ConditionWorldLevelMax", 0, "Maximum CLLC world level, for which item will drop. 0 or less means no max.")
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

        return builder;
    }

    internal static ConfigToObjectMapper<DropTableSystemConfiguration> GenerateConfigLoader(DropTableSystemConfiguration configuration)
    {
        return new ConfigToObjectMapper<DropTableSystemConfiguration>()
        {
            Path = new() { },
            Instantiation = new()
            {
                Instantiation = (TomlConfig) => configuration,
            },
            SubInstantiations = new()
            {
                new MappingInstantiationForParent<DropTableSystemConfiguration, DropTableListBuilder>()
                {
                    SubPath = new() { new TomlPathSegment(TomlPathSegmentType.Collection) },
                    Instantiation = new()
                    {
                        Instantiation = (DropTableSystemConfiguration parent, TomlConfig config) =>
                            parent.GetListBuilder(config.PathSegment.Name),
                    },
                    SubInstantiations = new()
                    {
                        new MappingInstantiationForParent<DropTableListBuilder, DropTableDropBuilder>()
                        {
                            SubPath = new(){ new TomlPathSegment(TomlPathSegmentType.Collection)},
                            Instantiation = new()
                            {
                                Instantiation = (DropTableListBuilder parent, TomlConfig config) =>
                                    parent.GetDrop(uint.Parse(config.PathSegment.Name)),
                                InstanceActions = new()
                                {
                                    (TomlConfig config, DropTableDropBuilder builder) =>
                                    {
                                        config.DoIfAnySet<string>("PrefabName", x => builder.PrefabName = x);
                                        config.DoIfAnySet<bool?>("Enable", x => builder.Enabled = x);
                                        config.DoIfAnySet<bool?>("EnableConfig", x => builder.TemplateEnabled = x);
                                        config.DoIfAnySet<int?>("SetAmountMin", x => builder.AmountMin = x);
                                        config.DoIfAnySet<int?>("SetAmountMax", x => builder.AmountMax = x);
                                        config.DoIfAnySet<float?>("SetTemplateWeight", x => builder.Weight = x);

                                        // Conditions
                                        config
                                            .DoIfAnySet<float?>("ConditionAltitudeMin", x => builder.ConditionAltitudeMin(x))
                                            .DoIfAnySet<float?>("ConditionAltitudeMax", x => builder.ConditionAltitudeMax(x))
                                            .DoIfAnySet<List<Heightmap.Biome>>("ConditionBiomes", x => builder.ConditionBiome(x))
                                            .DoIfAnySet<bool?>("ConditionNotDay", x => builder.ConditionDaytimeNotDay(x))
                                            .DoIfAnySet<bool?>("ConditionNotNight", x => builder.ConditionDaytimeNotNight(x))
                                            .DoIfAnySet<bool?>("ConditionNotAfternoon", x => builder.ConditionDaytimeNotAfternoon(x))
                                            .DoIfAnySet<List<string>>("ConditionEnvironments", x => builder.ConditionEnvironments(x))
                                            .DoIfAnySet<List<string>>("ConditionGlobalKeys", x => builder.ConditionGlobalKeysAny(x))
                                            .DoIfAnySet<List<string>>("ConditionGlobalKeysAll", x => builder.ConditionGlobalKeysAll(x))
                                            .DoIfAnySet<List<string>>("ConditionGlobalKeysNotAny", x => builder.ConditionGlobalKeysNotAny(x))
                                            .DoIfAnySet<List<string>>("ConditionGlobalKeysNotAll", x => builder.ConditionGlobalKeysNotAll(x))
                                            .DoIfAnySet<float?>("ConditionDistanceToCenterMin", x => builder.ConditionDistanceToCenterMin(x))
                                            .DoIfAnySet<float?>("ConditionDistanceToCenterMax", x => builder.ConditionDistanceToCenterMax(x))
                                            .DoIfAnySet<float?, float?, float?>(
                                                "ConditionWithinCircle.CenterX",
                                                "ConditionWithinCircle.CenterZ",
                                                "ConditionWithinCircle.Radius",
                                                (x1, x2, x3) => builder.ConditionWithinCircle(x1, x2, x3)
                                                )
                                            ;

                                        // Modifiers
                                        config
                                            .DoIfAnySet<float?>("SetDurability", x => builder.ModifierDurability(x))
                                            .DoIfAnySet<int?>("SetQualityLevel", x => builder.ModifierQualityLevel(x))
                                            ;
                                    },
                                    (TomlConfig config, DropTableDropBuilder builder) =>
                                    {
                                        if (!InstallationManager.SpawnThatInstalled)
                                        {
                                            return;
                                        }

                                        if (!config.Sections.TryGetValue(
                                            new(TomlPathSegmentType.Named, "SpawnThat"),
                                            out var modConfig))
                                        {
                                            return;
                                        }

                                        modConfig.DoIfAnySet<List<string>>("ConditionTemplateId", x => builder.ConditionTemplateId(x));
                                    },
                                    (TomlConfig config, DropTableDropBuilder builder) =>
                                    {
                                        if (!InstallationManager.CLLCInstalled)
                                        {
                                            return;
                                        }

                                        if (!config.Sections.TryGetValue(
                                            new(TomlPathSegmentType.Named, "CreatureLevelAndLootControl"),
                                            out var modConfig))
                                        {
                                            return;
                                        }

                                        modConfig
                                            .DoIfAnySet<int?>("ConditionWorldLevelMin", x => builder.ConditionWorldLevelMin(x))
                                            .DoIfAnySet<int?>("ConditionWorldLevelMax", x => builder.ConditionWorldLevelMax(x))
                                            ;
                                    },
                                    (TomlConfig config, DropTableDropBuilder builder) =>
                                    {
                                        if (!InstallationManager.EpicLootInstalled)
                                        {
                                            return;
                                        }

                                        if (!config.Sections.TryGetValue(
                                            new(TomlPathSegmentType.Named, "EpicLoot"),
                                            out var modConfig))
                                        {
                                            return;
                                        }

                                        builder.ModifierEpicLootItem(
                                            modConfig.GetSettingAsOptional<float?>("RarityWeightNone"),
                                            modConfig.GetSettingAsOptional<float?>("RarityWeightMagic"),
                                            modConfig.GetSettingAsOptional<float?>("RarityWeightRare"),
                                            modConfig.GetSettingAsOptional<float?>("RarityWeightEpic"),
                                            modConfig.GetSettingAsOptional<float?>("RarityWeightLegendary"),
                                            modConfig.GetSettingAsOptional<float?>("RarityWeightUnique"),
                                            modConfig.GetSettingAsOptional<List<string>>("UniqueIDs")
                                            );
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}

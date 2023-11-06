using System.Collections.Generic;
using DropThat.Drop.DropTableSystem.Conditions.ModSpecific.CLLC;
using DropThat.Drop.DropTableSystem.Conditions.ModSpecific.SpawnThat;
using DropThat.Drop.DropTableSystem.Conditions;
using DropThat.Drop.Options.Modifiers.ModEpicLoot;
using DropThat.Drop.Options.Modifiers;
using DropThat.Integrations;

namespace DropThat.Drop.DropTableSystem.Configuration.Toml;

internal static partial class ConfigurationFileManager
{
    public static DropTableListConfigMapper RegisterListMappings()
    {
        var mapper = new DropTableListConfigMapper();

        // Register drop options
        mapper.AddDropOption()
            .FromFile(config => config
                .Map<string>(
                    "PrefabName", default, "Name of prefab to drop.",
                    (value, builder) => builder.PrefabName = value));

        mapper.AddDropOption()
            .FromFile(config => config
                .Map<bool?>(
                    "EnableConfig", true, "Toggle this specific config entry on/off.",
                    (value, builder) => builder.TemplateEnabled = value));

        mapper.AddDropOption()
            .FromFile(config => config
                .Map<bool?>(
                    "Enable", true, "Toggle this specific drop. This can be used to disable existing drops.",
                    (value, builder) => builder.Enabled = value));

        mapper.AddDropOption()
            .FromFile(config => config
                .Map<int?>(
                    "SetAmountMin",
                    1,
                    "Deprecated (use AmountMin). Sets minimum amount pr drop. Behaviour depends on entity and item.",
                    (value, builder) => builder.AmountMin = value)
                .Map<int?>(
                    "AmountMin", 1, "Sets minimum amount pr drop. Behaviour depends on entity and item.",
                    (value, builder) => builder.AmountMin = value)
                );

        mapper.AddDropOption()
            .FromFile(config => config
                .Map<int?>(
                    "SetAmountMax",
                    1,
                    "Deprecated (use AmountMax). Sets maximum amount pr drop. Behaviour depends on entity and item.",
                    (value, builder) => builder.AmountMax = value)
                .Map<int?>(
                    "AmountMax", 1, "Sets maximum amount pr drop. Behaviour depends on entity and item.",
                    (value, builder) => builder.AmountMax = value)
                );

        mapper.AddDropOption()
            .FromFile(c => c
                .Map<float?>(
                    "SetTemplateWeight",
                    1,
                    "Deprecated (use TemplateWeight). Set weight for this drop. Used to control how likely it is that this item will be selected when rolling for drops. Note, same drop can be selected multiple times during table rolling.",
                    (value, builder) => builder.Weight = value)
                .Map<float?>(
                    "TemplateWeight", 1, "Set weight for this drop. Used to control how likely it is that this item will be selected when rolling for drops. Note, same drop can be selected multiple times during table rolling.",
                    (value, builder) => builder.Weight = value)
                );

        mapper.AddDropOption()
            .FromFile(config => config
                .Map<bool?>(
                    "DisableResourceModifierScaling", false, "Disables resource scaling from world-modifiers if true.",
                    (value, builder) => builder.DisableResourceModifierScaling = value));

        // Drop conditions
        mapper.AddDropOption()
            .FromFile(c => c
                .Map<float?>(
                    "ConditionAltitudeMin", -10_000, "Minimum distance above or below sea-level for drop to be enabled.",
                    (value, builder) => builder.ConditionAltitudeMin(value)));

        mapper.AddDropOption()
            .FromFile(c => c
                .Map<float?>(
                    "ConditionAltitudeMax", 10_000, "Maximum distance above or below sea-level for drop to be enabled.",
                    (value, builder) => builder.ConditionAltitudeMax(value)));

        mapper.AddDropOption()
            .FromFile(c => c
                .Map<List<Heightmap.Biome>>(
                    "ConditionBiomes", default, "Biomes in which drop is enabled. If empty, condition will be ignored.",
                    (value, builder) => builder.ConditionBiome(value)));

        mapper.AddDropOption()
            .FromFile(c => c
                .Map<bool?>(
                    "ConditionNotDay", default, "If true, will not drop during daytime.",
                    (value, builder) => builder.ConditionDaytimeNotDay(value)));

        mapper.AddDropOption()
            .FromFile(c => c.Map<bool?>(
                "ConditionNotNight", default, "If true, will not drop during night.",
                (value, builder) => builder.ConditionDaytimeNotNight(value)));

        mapper.AddDropOption()
            .FromFile(c => c.Map<bool?>(
                "ConditionNotAfternoon", default, "If true, will not drop during afternoon.",
                (value, builder) => builder.ConditionDaytimeNotAfternoon(value)));

        mapper.AddDropOption()
            .FromFile(c => c.Map<List<string>>(
                "ConditionEnvironments", default, "List of environment names that allow the item to drop while they are active.\nEg. Misty, Thunderstorm. Leave empty to always allow.",
                (value, builder) => builder.ConditionEnvironments(value)));

        mapper.AddDropOption()
            .FromFile(c => c.Map<List<string>>(
                "ConditionGlobalKeys", default, "List of global keys names of which at least one must be present for item to drop.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.",
                (value, builder) => builder.ConditionGlobalKeysAny(value)));

        mapper.AddDropOption()
            .FromFile(c => c.Map<List<string>>(
                "ConditionGlobalKeysAll", default, "List of global keys names that must all be present for item to drop.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.",
                (value, builder) => builder.ConditionGlobalKeysAll(value)));

        mapper.AddDropOption()
            .FromFile(c => c.Map<List<string>>(
                "ConditionGlobalKeysNotAny", default, "List of global keys names of which none must be present for item to drop.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.",
                (value, builder) => builder.ConditionGlobalKeysNotAny(value)));

        mapper.AddDropOption()
            .FromFile(c => c.Map<List<string>>(
                "ConditionGlobalKeysNotAll", default, "List of global keys names of which all of the keys must be missing for item to drop.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.",
                (value, builder) => builder.ConditionGlobalKeysNotAll(value)));

        mapper.AddDropOption()
            .FromFile(c => c.Map<float?>(
                "ConditionDistanceToCenterMin", 0, description: "Minimum distance to center of map, for drop to be enabled.",
                (value, builder) => builder.ConditionDistanceToCenterMin(value)));

        mapper.AddDropOption()
            .FromFile(c => c.Map<float?>(
                "ConditionDistanceToCenterMax", 0, "Maximum distance to center of map, within which drop is enabled. 0 means limitless.",
                (value, builder) => builder.ConditionDistanceToCenterMax(value)));

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionWithinCircle>())
                .Map<float?>(
                    "ConditionWithinCircle.CenterX", 0, "Center X coordinate of circle within which drop is enabled.",
                    (value, condition) => condition.CenterX = value ?? 0f)
                .Map<float?>(
                    "ConditionWithinCircle.CenterZ", 0, "Center Z coordinate of circle within which drop is enabled.",
                    (value, condition) => condition.CenterZ = value ?? 0f)
                .Map<float?>(
                    "ConditionWithinCircle.Radius", -1, "Radius of circle within which drop is enabled.",
                    (value, condition) => condition.Radius = value ?? 0f));

        // Drop modifiers

        mapper.AddDropOption()
            .FromFile(c => c
                .Map<float?>(
                    "SetDurability",
                    -1,
                    "Deprecated (use Durability). Sets the durability of the item. Does not change max durability. If less than 0, uses default.",
                    (value, builder) => builder.ModifierDurability(value))
                .Map<float?>(
                    "Durability", -1, "Sets the durability of the item. Does not change max durability. If less than 0, uses default.",
                    (value, builder) => builder.ModifierDurability(value))
                );

        mapper.AddDropOption()
            .FromFile(c => c
                .Map<int?>(
                    "SetQualityLevel",
                    1,
                    "Deprecated (use QualityLevel). Sets the quality level of the item. If 0 or less, uses default quality level of drop.",
                    (value, builder) => builder.ModifierQualityLevel(value))
                .Map<int?>(
                    "QualityLevel", 1, "Sets the quality level of the item. If 0 or less, uses default quality level of drop.",
                    (value, builder) => builder.ModifierQualityLevel(value))
                );

        // Mod - SpawnThat

        mapper
            .AddModRequirement("SpawnThat", () => InstallationManager.SpawnThatInstalled)
            .AddModOption("SpawnThat")
            .FromFile(c => c
                .Map<List<string>>(
                    "ConditionTemplateId", default, "Array of Spawn That TemplateId values to enable to drop for.",
                    (value, builder) => builder.ConditionTemplateId(value)));

        // Mod - CLLC
        mapper
            .AddModRequirement("CreatureLevelAndLootControl", () => InstallationManager.CLLCInstalled)
            .AddModOption("CreatureLevelAndLootControl")
            .FromFile(c => c
                .Map<int?>(
                    "ConditionWorldLevelMin", 0, "Minimum CLLC world level, for which item will drop.",
                    (value, builder) => builder.ConditionWorldLevelMin(value)))

            .AddOption()
            .FromFile(c => c
                .Map<int?>(
                    "ConditionWorldLevelMax", 0, "Maximum CLLC world level, for which item will drop. 0 or less means no max.",
                    (value, builder) => builder.ConditionWorldLevelMax(value)))
            ;

        // Mod - Epic Loot
        mapper
            .AddModRequirement("EpicLoot", () => InstallationManager.EpicLootInstalled)
            .AddModOption("EpicLoot")
            .FromFile(c => c
                .Using(x => x.ItemModifiers.GetOrCreate<ModifierEpicLootItem>())
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
                    (value, modifier) => modifier.UniqueIds = value))
            ;

        return mapper;
    }
}
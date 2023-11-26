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
                    (value, builder) => builder.PrefabName = value)
                );

        mapper.AddDropOption()
            .FromFile(config => config
                .Map<bool?>(
                    "EnableConfig", true, "Toggle this specific config entry on/off.",
                    (value, builder) => builder.TemplateEnabled = value)
                );

        mapper.AddDropOption()
            .FromFile(config => config
                .Map<bool?>(
                    "Enable", true, "Toggle this specific drop. This can be used to disable existing drops.",
                    (value, builder) => builder.Enabled = value)
                );

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
                    "Deprecated (use Templ ateWeight). Set weight f or this drop. Used to control how likely it is that this item will be selected when rolling for drops. Note, same drop can be selected multiple times during table rolling.",
                    (value, builder) => builder.Weight = value)
                .Map<float?>(
                    "TemplateWeight", 1, "Set weight for this drop. Used to control how likely it is that this item will be selected when rolling for drops. Note, same drop can be selected multiple times during table rolling.",
                    (value, builder) => builder.Weight = value)
                );

        mapper.AddDropOption()
            .FromFile(config => config
                .Map<bool?>(
                    "DisableResourceModifierScaling", false, "Disables resource scaling from world-modifiers if true.",
                    (value, builder) => builder.DisableResourceModifierScaling = value)
                );

        // Drop conditions
        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionAltitudeMin>())
                .Map<float?>(
                    "ConditionAltitudeMin",
                    -10_000,
                    "Minimum distanc e above or below sea-level for  drop to be enabled.",
                    (value, builder) => builder.AltitudeMin = value)
                );

        mapper.AddDropOption()
            .FromFile(c => c
                    .Using(x => x.Conditions.GetOrCreate<ConditionAltitudeMax>())
                .Map<float?>(
                    "ConditionAltitudeMax",
                    10_000,
                    "Maximum distance  above or below sea-level for  drop to be enabled.",
                    (value, builder) => builder.AltitudeMax = value)
                );

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionBiome>())
                .Map<List<Heightmap.Biome>>(
                    "ConditionBiomes",
                    default,
                    "Biomes in which  drop is enabled. If empty, co ndition will be ignored.",
                    (value, builder) => builder.SetBiomes(value))
                );

        mapper.AddDropOption()
            .FromFile(c => c
                .Map<bool?>(
                    "ConditionNotDay",
                    null,
                    "If true, will not drop during daytime.",
                    (value, builder) =>
                    {
                        if (value == true)
                        {
                            builder.Conditions.GetOrCreate<ConditionDaytimeNotDay>();
                        }
                        else
                        {
                            builder.Conditions.Remove<ConditionDaytimeNotDay>();
                        }
                    })
                );

        mapper.AddDropOption()
            .FromFile(c => c
                .Map<bool?>(
                    "ConditionNotNight",
                    null,
                    "If true, will not  drop during night.",
                     (value, builder) =>
                    {
                        if (value == true)
                        {
                            builder.Conditions.GetOrCreate<ConditionDaytimeNotDay>();
                        }
                        else
                        {
                            builder.Conditions.Remove<ConditionDaytimeNotDay>();
                        }
                    })
                );

        mapper.AddDropOption()
            .FromFile(c => c
                .Map<bool?>(
                    "ConditionNotAfternoon",
                    null,
                    "If true, will not  drop during afternoon.",
                     (value, builder) =>
                    {
                        if (value == true)

                        {
                            builder.Conditions.GetOrCreate<ConditionDaytimeNotAfternoon> ();
                        }
                        else
                        {
                            builder.Conditions.Remove<ConditionDaytimeNotAfternoon>();
                        }
                    })
                 );

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionEnvironments>())
                .Map<List<string>>(
                    "ConditionEnvironments",
                    default,
                    "List of environ ment names that allow the item  to drop while they are active.\nEg. Misty, Thunderstorm. Leave empty to always allow.",
                    (value, builder) => builder.SetEnvironments(value))
                );

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionGlobalKeysAny>())
                .Map<List<string>>(
                    "ConditionGlobalKeys",
                    default,
                    "List of global  keys names of which at least o ne must be present for item to drop.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.",
                    (value, builder) => builder.GlobalKeys = value?.ToArray())
                );

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionGlobalKeysAll>())
                .Map<List<string>>(
                    "ConditionGlobalKeysAll",
                    default,
                    "List of global  keys names that must all be pr esent for item to drop.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.",
                    (value, builder) => builder.GlobalKeys = value?.ToArray())
                );

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionGlobalKeysNotAny>())
                .Map<List<string>>(
                    "ConditionGlobalKeysNotAny",
                    default,
                    "List of global  keys names of which none must  be present for item to drop.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.",
                    (value, builder) => builder.GlobalKeys = value?.ToArray())
                );

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionGlobalKeysNotAll>())
                .Map<List<string>>(
                    "ConditionGlobalKeysNotAll",
                    default,
                    "List of global  keys names of which all of the  keys must be missing for item to drop.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.",
                    (value, builder) => builder.GlobalKeys = value?.ToArray())
                );

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionDistanceToCenterMin>())
                .Map<float?>(
                    "ConditionDistanceToCenterMin",
                    0, description:
                    "Minimum  distance to center of map, for drop t o be enabled.",
                    (value, builder) => builder.MinDistance = value)
                );

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionDistanceToCenterMax>())
                .Map<float?>(
                    "ConditionDistanceToCenterMax",
                    0,
                    "Maximum distance to c enter of map, within whi ch drop is enabled. 0 means limitless.",
                    (value, builder) => builder.MaxDistance = value)
                );

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionWithinCircle>())
                .Map<float?>(
                    "ConditionWithinCircle.CenterX", null, "Center X coordinate of circle within which drop is enabled.",
                    (value, condition) => condition.CenterX = value ?? 0f)
                .Map<float?>(
                    "ConditionWithinCircle.CenterZ", null, "Center Z coordinate of circle within which drop is enabled.",
                    (value, condition) => condition.CenterZ = value ?? 0f)
                .Map<float?>(
                    "ConditionWithinCircle.Radius", null, "Radius of circle within which drop is enabled.",
                    (value, condition) => condition.Radius = value ?? -1f)
                );

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionLocation>())
                .Map<List<string>>(
            "ConditionLocation",
            null,
            "List of location names. When spawned in one of the listed locations, this drop is enabled.\nEg.Runestone_Boars",
                    (value, condition) => condition.SetLocations(value))
                );

        // Drop modifiers

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.ItemModifiers.GetOrCreate<ModifierDurability>())
                .Map<float?>(
                    "SetDurability",
                    -1,
                    "Deprecated (use Dura bility). Sets the durabil ity of the item. Does not change max durability. If less than 0, uses default.",
                    (value, builder) => builder.Durability = value)
                .Map<float?>(
                    "Durability",
                    -1,
                    "Sets the durability  of the item. Does not cha nge max durability. If less than 0, uses default.",
                    (value, builder) => builder.Durability = value)
                );

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.ItemModifiers.GetOrCreate<ModifierQualityLevel>())
                .Map<int?>(
                    "SetQualityLevel",
                    1,
                    "Deprecated (use Quali tyLevel). Sets the quali ty level of the item. If 0 or less, uses default quality level of drop.",
                    (value, builder) => builder.QualityLevel = value)
                .Map<int?>(
                    "QualityLevel",
                    1,
                    "Sets the quality leve l of the item. If 0 or l ess, uses default quality level of drop.",
                    (value, builder) => builder.QualityLevel = value)
                );

        // Mod - SpawnThat

        mapper
            .AddModRequirement("SpawnThat", () => InstallationManager.SpawnThatInstalled)
            .AddModOption("SpawnThat")
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionTemplateId>())
                .Map<List<string>>(
                    "ConditionTemplateId",
                    default,
                    "List of Spawn T hat TemplateId values to enabl e to drop for.",
                    (value, builder) => builder.TemplateIds = value?.ToArray())
                );

        // Mod - CLLC
        mapper
            .AddModRequirement("CreatureLevelAndLootControl", () => InstallationManager.CLLCInstalled)
            .AddModOption("CreatureLevelAndLootControl")
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionWorldLevelMin>())
                .Map<int?>(
                    "ConditionWorldLevelMin",
                    0,
                    "Minimum CLLC world le vel, for which item will  drop.",
                    (value, builder) => builder.WorldLevel = value)
                )

            .AddOption()
            .FromFile(c => c
                            .Using(x => x.Conditions.GetOrCreate<ConditionWorldLevelMax>())
                .Map<int?>(
                    "ConditionWorldLevelMax",
                    0,
                    "Maximum CLLC world le vel, for which item will  drop. 0 or less means no max.",
                    (value, builder) => builder.WorldLevel = value)
                );

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
                    (value, modifier) => modifier.UniqueIds = value)
                );

        return mapper;
    }
}
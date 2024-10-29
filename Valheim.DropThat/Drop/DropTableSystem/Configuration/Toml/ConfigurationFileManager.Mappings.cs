using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.DropTableSystem.Conditions;
using DropThat.Drop.DropTableSystem.Conditions.ModSpecific.CLLC;
using DropThat.Drop.DropTableSystem.Conditions.ModSpecific.SpawnThat;
using DropThat.Drop.Options;
using DropThat.Drop.Options.Modifiers;
using DropThat.Drop.Options.Modifiers.ModEpicLoot;
using DropThat.Integrations;
using ThatCore.Utilities.Valheim;

namespace DropThat.Drop.DropTableSystem.Configuration.Toml;

internal static partial class ConfigurationFileManager
{
    public static DropTableConfigMapper RegisterMainMappings()
    {
        var mapper = new DropTableConfigMapper();

        // Register table options
        mapper
            .AddTableOption()
            .FromFile(config => config
                .Map<int?>(
                    "SetDropMin", 1, "Deprecated (use DropMin). Minimum of randomly selected entries from drop list. Entries can be picked more than once.",
                    (value, builder) => builder.DropMin = value)
                .Map<int?>(
                    "DropMin", 1, "Minimum of randomly selected entries from drop list. Entries can be picked more than once.",
                    (value, builder) => builder.DropMin = value)
                )
            .ToFile(config => config
                .Map("DropMin", x => x.DropMin))
            ;

        mapper
            .AddTableOption()
            .FromFile(config => config
                .Map<int?>(
                    "SetDropMax", 
                    1, 
                    "Deprecated (use DropMax). Maximum of randomly selected entries from drop list. Entries can be picked more than once.",
                    (value, builder) => builder.DropMax = value)
                .Map<int?>(
                    "DropMax", 1, "Maximum of randomly selected entries from drop list. Entries can be picked more than once.",
                    (value, builder) => builder.DropMax = value)
                )
            .ToFile(config => config
                .Map("DropMax", x => x.DropMax));
            ;

        mapper.AddTableOption()
            .FromFile(config => config
                .Map<float?>(
                    "SetDropChance", 
                    100, 
                    "Deprecated (use DropChance). Chance to drop anything at all.",
                    (value, builder) => builder.DropChance = (value ?? 100) / 100)
                .Map<float?>(
                    "DropChance", 100, "Chance to drop anything at all.",
                    (value, builder) => builder.DropChance = (value ?? 100) / 100)
                )
            .ToFile(config => config
                .Map("DropChance", x => x.DropChance * 100));

        mapper.AddTableOption()
            .FromFile(config => config
                .Map<bool?>(
                    "SetDropOnlyOnce", 
                    false, 
                    "Deprecated (use DropOnlyOnce). If true, will ensure that when selecting entries from drop list, same entry will only be picked once.",
                    (value, builder) => builder.DropOnlyOnce = value)
                .Map<bool?>(
                    "DropOnlyOnce", false, "If true, will ensure that when selecting entries from drop list, same entry will only be picked once.",
                    (value, builder) => builder.DropOnlyOnce = value)
                )
            .ToFile(config => config
                .Map("DropOnlyOnce", x => x.DropOnlyOnce));

        mapper.AddTableOption()
            .FromFile(config => config
                .Map<List<string>>(
                    "UseDropList", default, "List of droplists to load before other drop settings.",
                    (value, builder) => builder.ListNames = value));

        // Register drop options
        mapper.AddDropOption()
            .FromFile(config => config
                .Map<string>(
                    "PrefabName", default, "Name of prefab to drop.",
                    (value, builder) => builder.PrefabName = value))
            .ToFile(config => config
                .Map("PrefabName", x => x.PrefabName));

        mapper.AddDropOption()
            .FromFile(config => config
                .Map<bool?>(
                    "EnableConfig", true, "Toggle this specific config entry on/off.",
                    (value, builder) => builder.TemplateEnabled = value))
            .ToFile(config => config
                .Map("EnableConfig", x => x.TemplateEnabled));

        mapper.AddDropOption()
            .FromFile(config => config
                .Map<bool?>(
                    "Enable", true, "Toggle this specific drop. This can be used to disable existing drops.",
                    (value, builder) => builder.Enabled = value))
            .ToFile(config => config
                .Map("Enable", x => x.Enabled));

        mapper.AddDropOption()
            .FromFile(config => config
                .Map<int?>(
                    "SetAmountMin", 
                    1, 
                    "Deprecated (use AmountMin). Sets minimum amount pr drop.",
                    (value, builder) => builder.AmountMin = value)
                .Map<int?>(
                    "AmountMin", 1, "Sets minimum amount pr drop.",
                    (value, builder) => builder.AmountMin = value)
                )
            .ToFile(config => config
                .Map("AmountMin", x => x.AmountMin));

        mapper.AddDropOption()
            .FromFile(config => config
                .Map<int?>(
                    "SetAmountMax", 
                    1, 
                    "Deprecated (use AmountMax). Sets maximum amount pr drop.",
                    (value, builder) => builder.AmountMax = value)
                .Map<int?>(
                    "AmountMax", 1, "Sets maximum amount pr drop.",
                    (value, builder) => builder.AmountMax = value)
                )
            .ToFile(config => config
                .Map("AmountMax", x => x.AmountMax));

        mapper.AddDropOption()
            .FromFile(c => c
                .Map<float?>(
                    "SetTemplateWeight", 
                    1, 
                    "Deprecated (use TemplateWeight). Set weight for this drop. Used to control how likely it is that this item will be selected when rolling for drops. \nNote, same drop can be selected multiple times during table rolling.",
                    (value, builder) => builder.Weight = value)
                .Map<float?>(
                    "Weight", 1, "Set weight for this drop. Used to control how likely it is that this item will be selected when rolling for drops.\nNote, same drop can be selected multiple times during table rolling.",
                    (value, builder) => builder.Weight = value)
                )
            .ToFile(c => c
                .Map("Weight", x => x.Weight));

        mapper.AddDropOption()
            .FromFile(config => config
                .Map<bool?>(
                    "DisableResourceModifierScaling", false, "Disables resource scaling from world-modifiers if true.",
                    (value, builder) => builder.DisableResourceModifierScaling = value))
            .ToFile(x => x
                .Map("DisableResourceModifierScaling", x => x.DisableResourceModifierScaling));

        // Drop conditions
        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionAltitudeMin>())
                .Map<float?>(
                    "ConditionAltitudeMin", 
                    null, 
                    "Minimum distance above or below sea-level for drop to be enabled.",
                    (value, builder) => builder.AltitudeMin = value)
                )
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionAltitudeMin>())
                .Map("ConditionAltitudeMin", x => x.AltitudeMin));
            ;

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionAltitudeMax>())
                .Map<float?>(
                    "ConditionAltitudeMax", 
                    null, 
                    "Maximum distance above or below sea-level for drop to be enabled.",
                    (value, builder) => builder.AltitudeMax = value)
                )
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionAltitudeMax>())
                .Map("ConditionAltitudeMax", x => x.AltitudeMax));

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionBiome>())
                .Map<List<Heightmap.Biome>>(
                    "ConditionBiomes", 
                    default, 
                    "Biomes in which drop is enabled. If empty, condition will be ignored.",
                    (value, builder) => builder.SetBiomes(value))
                )
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionBiome>())
                .Map("ConditionBiomes", x => x.BiomeMask.Split()));

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
                )
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionDaytimeNotDay>())
                .Map("ConditionNotDay", x => true));

        mapper.AddDropOption()
            .FromFile(c => c
                .Map<bool?>(
                    "ConditionNotNight", 
                    null, 
                    "If true, will not drop during night.",
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
                )
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionDaytimeNotNight>())
                .Map("ConditionNotNight", x => true));

        mapper.AddDropOption()
            .FromFile(c => c
                .Map<bool?>(
                    "ConditionNotAfternoon", 
                    null, 
                    "If true, will not drop during afternoon.",
                    (value, builder) => 
                    { 
                        if (value == true) 
                        { 
                            builder.Conditions.GetOrCreate<ConditionDaytimeNotAfternoon>(); 
                        } 
                        else 
                        { 
                            builder.Conditions.Remove<ConditionDaytimeNotAfternoon>(); 
                        } 
                    })
                )
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionDaytimeNotAfternoon>())
                .Map("ConditionNotAfternoon", x => true));

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionEnvironments>())
                .Map<List<string>>(
                    "ConditionEnvironments", 
                    default, 
                    "List of environment names that allow the item to drop while they are active.\nEg. Misty, Thunderstorm. Leave empty to always allow.",
                    (value, builder) => builder.SetEnvironments(value))
                )
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionEnvironments>())
                .Map("ConditionEnvironments", x => x.Environments?.ToList()));

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionGlobalKeysAny>())
                .Map<List<string>>(
                    "ConditionGlobalKeys", 
                    default, 
                    "List of global keys names of which at least one must be present for item to drop.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.",
                    (value, builder) => builder.GlobalKeys = value?.ToArray())
                )
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionGlobalKeysAny>())
                .Map("ConditionGlobalKeys", x => x.GlobalKeys?.ToList()));

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionGlobalKeysAll>())
                .Map<List<string>>(
                    "ConditionGlobalKeysAll", 
                    default, 
                    "List of global keys names that must all be present for item to drop.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.",
                    (value, builder) => builder.GlobalKeys = value?.ToArray())
                )
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionGlobalKeysAll>())
                .Map("ConditionGlobalKeysAll", x => x.GlobalKeys?.ToList()));

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionGlobalKeysNotAny>())
                .Map<List<string>>(
                    "ConditionGlobalKeysNotAny", 
                    default, 
                    "List of global keys names of which none must be present for item to drop.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.",
                    (value, builder) => builder.GlobalKeys = value?.ToArray())
                )
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionGlobalKeysNotAny>())
                .Map("ConditionGlobalKeysNotAny", x => x.GlobalKeys?.ToList()));

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionGlobalKeysNotAll>())
                .Map<List<string>>(
                    "ConditionGlobalKeysNotAll", 
                    default, 
                    "List of global keys names of which all of the keys must be missing for item to drop.\nEg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.",
                    (value, builder) => builder.GlobalKeys = value?.ToArray())
                )
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionGlobalKeysNotAll>())
                .Map("ConditionGlobalKeysNotAll", x => x.GlobalKeys?.ToList()));

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionDistanceToCenterMin>())
                .Map<float?>(
                    "ConditionDistanceToCenterMin", 
                    0, 
                    "Minimum distance to center of map, for drop to be enabled.",
                    (value, builder) => builder.MinDistance = value)
                )
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionDistanceToCenterMin>())
                .Map("ConditionDistanceToCenterMin", x => x.MinDistance));

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionDistanceToCenterMax>())
                .Map<float?>(
                    "ConditionDistanceToCenterMax", 
                    0, 
                    "Maximum distance to center of map, within which drop is enabled. 0 means limitless.",
                    (value, builder) => builder.MaxDistance = value)
                )
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionDistanceToCenterMax>())
                .Map("ConditionDistanceToCenterMax", x => x.MaxDistance));

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
                    (value, condition) => condition.Radius = value ?? -1f))
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionWithinCircle>())
                .Map("ConditionWithinCircle.CenterX", x => x.CenterX)
                .Map("ConditionWithinCircle.CenterZ", x => x.CenterZ)
                .Map("ConditionWithinCircle.Radius", x => x.Radius));

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionLocation>())
                .Map<List<string>>(
            "ConditionLocation",
            null,
            "List of location names. When spawned in one of the listed locations, this drop is enabled.\nEg.Runestone_Boars",
                    (value, condition) => condition.SetLocations(value))
                )
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionLocation>())
                .Map("ConditionLocation", x => x.Locations?.ToList()));

        // Drop modifiers

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.ItemModifiers.GetOrCreate<ModifierDurability>())
                .Map<float?>(
                    "SetDurability", 
                    -1, 
                    "Deprecated (use Durability). Sets the durability of the item. Does not change max durability. If less than 0, uses default.",
                    (value, builder) => builder.Durability = value)
                .Map<float?>(
                    "Durability",
                    -1, 
                    "Sets the durability of the item. Does not change max durability. If less than 0, uses default.",
                    (value, builder) => builder.Durability = value)
                )
            .ToFile(c => c
                .Using(x => x.ItemModifiers.GetOrDefault<ModifierDurability>())
                .Map("Durability", x => x.Durability));

        mapper.AddDropOption()
            .FromFile(c => c
                .Using(x => x.ItemModifiers.GetOrCreate<ModifierQualityLevel>())
                .Map<int?>(
                    "SetQualityLevel", 
                    -1, 
                    "Deprecated (use QualityLevel). Sets the quality level of the item. If 0 or less, uses default quality level of drop.",
                    (value, builder) => builder.QualityLevel = value)
                .Map<int?>(
                    "QualityLevel", 
                    -1, 
                    "Sets the quality level of the item. If 0 or less, uses default quality level of drop.",
                    (value, builder) => builder.QualityLevel = value)
                )
            .ToFile(c => c
                .Using(x => x.ItemModifiers.GetOrDefault<ModifierQualityLevel>())
                .Map("QualityLevel", x => x.QualityLevel));

        // Mod - SpawnThat

        mapper
            .AddModRequirement("SpawnThat", () => InstallationManager.SpawnThatInstalled)
            .AddModOption("SpawnThat")
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionTemplateId>())
                .Map<List<string>>(
                    "ConditionTemplateId", 
                    default, 
                    "List of Spawn That TemplateId values to enable to drop for.",
                    (value, builder) => builder.TemplateIds = value?.ToArray())
                )
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionTemplateId>())
                .Map("ConditionTemplateId", x => x.TemplateIds?.ToList()));

        // Mod - CLLC
        mapper
            .AddModRequirement("CreatureLevelAndLootControl", () => InstallationManager.CLLCInstalled)
            .AddModOption("CreatureLevelAndLootControl")
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionWorldLevelMin>())
                .Map<int?>(
                    "ConditionWorldLevelMin", 
                    0, 
                    "Minimum CLLC world level, for which item will drop.",
                    (value, builder) => builder.WorldLevel = value)
                )
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionWorldLevelMin>())
                .Map("ConditionWorldLevelMin", x => x.WorldLevel))
            
            .AddOption()
            .FromFile(c => c
                .Using(x => x.Conditions.GetOrCreate<ConditionWorldLevelMax>())
                .Map<int?>(
                    "ConditionWorldLevelMax",
                    0, 
                    "Maximum CLLC world level, for which item will drop. 0 or less means no max.",
                    (value, builder) => builder.WorldLevel = value)
                )
            .ToFile(c => c
                .Using(x => x.Conditions.GetOrDefault<ConditionWorldLevelMax>())
                .Map("ConditionWorldLevelMax", x => x.WorldLevel));

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
            .ToFile(c => c
                .Using(x => x.ItemModifiers.GetOrDefault<ModifierEpicLootItem>())
                .Map("RarityWeightNone", x => x.RarityWeightNone)
                .Map("RarityWeightMagic", x => x.RarityWeightMagic)
                .Map("RarityWeightRare", x => x.RarityWeightRare)
                .Map("RarityWeightEpic", x => x.RarityWeightEpic)
                .Map("RarityWeightLegendary", x => x.RarityWeightLegendary)
                .Map("RarityWeightUnique", x => x.RarityWeightUnique)
                .Map("UniqueIDs", x => x.UniqueIds))
            ;

        return mapper;
    }
}

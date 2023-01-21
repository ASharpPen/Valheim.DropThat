using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DropThat.Drop.CharacterDropSystem.Conditions;
using DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;
using DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.SpawnThat;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Drop.Options.Modifiers;
using DropThat.Drop.Options.Modifiers.ModEpicLoot;
using DropThat.Integrations;
using DropThat.Integrations.CllcIntegration;
using ThatCore.Config.Toml;
using ThatCore.Config.Toml.Mapping;
using ThatCore.Config.Toml.Schema;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Configuration;

internal static class ConfigurationFileManager
{
    private static TomlSchemaBuilder _schemaBuilder;
    private static ITomlSchemaLayer _schema;

    private static ConfigToObjectMapper<CharacterDropTemplate> _configMapper;

    public static void Setup()
    {
        if (_schema is null)
        {
            _schemaBuilder = GenerateCfgSchema();
            _schema = _schemaBuilder.Build();
        }

        _configMapper ??= GenerateConfigMapper();
    }

    public static CharacterDropTemplate Load(string filePath)
    {
    }

    internal static TomlSchemaBuilder GenerateCfgSchema()
    {
        var builder = new TomlSchemaBuilder();

        var dropConfigBuilder = builder
            .SetLayerAsCollection() // Mob layer
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

    internal static ConfigToObjectMapper<CharacterDropTemplate> GenerateConfigMapper()
    {
        MappingInstantiationForParent<CharacterDropTemplate, CharacterDropMobTemplate> mobMappings = new()
        {
            SubPath = new()
            {
                new TomlPathSegment(TomlPathSegmentType.Collection)
            },
            Instantiation = new()
            {
                Instantiation = (TomlConfig config) => new CharacterDropMobTemplate() { PrefabName = config.PathSegment.Name },
            },
            Action = (CharacterDropTemplate template, CharacterDropMobTemplate instance, TomlConfig config) =>
            {
                template.MobTemplates.Add(instance.PrefabName, instance);
            },
            SubInstantiations = new()
            {
                new MappingInstantiationForParent<CharacterDropMobTemplate, CharacterDropDropTemplate>()
                {
                    SubPath = new() { new TomlPathSegment(TomlPathSegmentType.Collection) },
                    Action = (CharacterDropMobTemplate template, CharacterDropDropTemplate instance, TomlConfig config) =>
                    {
                        template.Drops[int.Parse(config.PathSegment.Name)] = instance;
                    },
                    Instantiation = new()
                    {
                        Instantiation = (TomlConfig config) => new CharacterDropDropTemplate(),
                        InstanceActions = new()
                        {
                            (TomlConfig config, CharacterDropDropTemplate template) =>
                            {
                                template.Id = int.Parse(config.PathSegment.Name);

                                (template, config)
                                    .Configure(x => x.PrefabName)
                                    .Configure(x => x.Enabled, "EnableConfig")
                                    .Configure(x => x.AmountMin, "SetAmountMin")
                                    .Configure(x => x.AmountMax, "SetAmountMax")
                                    .Configure(x => x.ChanceToDrop, "SetChanceToDrop")
                                    .Configure(x => x.ScaleByLevel, "SetScaleByLevel")

                                    .Configure<int?>(x => template.ModifierQualityLevel(x), "SetQualityLevel")
                                    .Configure(x => x.AmountLimit, "SetAmountLimit")
                                    .Configure(x => x.AutoStack, "SetAutoStack")
                                    .Configure<float?>(x => template.ModifierDurability(x), "SetDurability")

                                    .Configure<int?>(x => template.ConditionLevelMin(x), "ConditionMinLevel")
                                    .Configure<int?>(x => template.ConditionLevelMax(x), "ConditionMaxLevel")
                                    .Configure<bool?>(x => template.ConditionNotDay(x), "ConditionNotDay")
                                    .Configure<bool?>(x => template.ConditionNotAfternoon(x), "ConditionNotAfternoon")
                                    .Configure<bool?>(x => template.ConditionNotNight(x), "ConditionNotNight")
                                    .Configure<List<string>>(x => template.ConditionEnvironments(x), "ConditionEnvironments")
                                    .Configure<List<string>>(x => template.ConditionGlobalKeysAny(x), "ConditionGlobalKeys")
                                    .Configure<List<string>>(x => template.ConditionGlobalKeysNotAny(x), "ConditionNotGlobalKeys")
                                    .Configure<List<Heightmap.Biome>>(x => template.ConditionBiome(x), "ConditionBiomes")
                                    .Configure<List<CreatureState>>(x => template.ConditionCreatureState(x), "ConditionCreatureStates")
                                    .Configure<List<CreatureState>>(x => template.ConditionNotCreatureState(x), "ConditionNotCreatureStates")
                                    .Configure<List<string>>(x => template.ConditionInventory(x), "ConditionHasItem")
                                    .Configure<List<Character.Faction>>(x => template.ConditionFaction(x), "ConditionFaction")
                                    .Configure<List<Character.Faction>>(x => template.ConditionNotFaction(x), "ConditionNotFaction")
                                    .Configure<List<string>>(x => template.ConditionLocation(x), "ConditionLocation")
                                    .Configure<float?>(x => template.ConditionDistanceToCenterMin(x), "ConditionDistanceToCenterMin")
                                    .Configure<float?>(x => template.ConditionDistanceToCenterMax(x), "ConditionDistanceToCenterMax")
                                    .Configure<List<HitData.DamageType>>(x => template.ConditionKilledByDamageType(x), "ConditionKilledByDamageType")
                                    .Configure<List<string>>(x => template.ConditionKilledWithStatusAny(x), "ConditionKilledWithStatus")
                                    .Configure<List<string>>(x => template.ConditionKilledWithStatusAll(x), "ConditionKilledWithStatuses")
                                    .Configure<List<Skills.SkillType>>(x => template.ConditionKilledBySkillType(x), "ConditionKilledBySkillType")
                                    .Configure<List<EntityType>>(x => template.ConditionKilledByEntityType(x), "ConditionKilledByEntityType")
                                    .Configure<List<EntityType>>(x => template.ConditionHitByEntityTypeRecently(x), "ConditionHitByEntityTypeRecently")
                                    ;
                            },
                            (TomlConfig config, CharacterDropDropTemplate template) =>
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

                                (template, cllcConfig)
                                    .Configure<List<CllcBossAffix>>(x => template.ConditionBossAffix(x), "ConditionBossAffix")
                                    .Configure<List<CllcBossAffix>>(x => template.ConditionNotBossAffix(x), "ConditionNotBossAffix")
                                    .Configure<List<CllcCreatureInfusion>>(x => template.ConditionInfusion(x), "ConditionInfusion")
                                    .Configure<List<CllcCreatureInfusion>>(x => template.ConditionNotInfusion(x), "ConditionNotInfusion")
                                    .Configure<List<CllcCreatureExtraEffect>>(x => template.ConditionCreatureExtraEffect(x), "ConditionExtraEffect")
                                    .Configure<List<CllcCreatureExtraEffect>>(x => template.ConditionNotCreatureExtraEffect(x), "ConditionNotExtraEffect")
                                    .Configure<int?>(x => template.ConditionWorldLevelMin(x), "ConditionWorldLevelMin")
                                    .Configure<int?>(x => template.ConditionWorldLevelMax(x), "ConditionWorldLevelMax")
                                    ;
                            },
                            (TomlConfig config, CharacterDropDropTemplate template) =>
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
                                var epicLootModifier = template.ModifierEpicLootItem();

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
                                    template.ItemModifiers.Remove(epicLootModifier);
                                }
                            },
                            (TomlConfig config, CharacterDropDropTemplate template) =>
                            {
                                if (!InstallationManager.SpawnThatInstalled)
                                {
                                    return;
                                }

                                (template, config)
                                    .Configure<List<string>>(x => template.ConditionTemplateId(x), "ConditionTemplateId")
                                    ;
                            }
                        }
                    },
                },
            },
        };

        return new ConfigToObjectMapper<CharacterDropTemplate>()
        {
            Path = new() { },
            Instantiation = new()
            {
                Instantiation = (TomlConfig) => new()
            },
            SubInstantiations = new()
            {
                mobMappings,
            }
        };
    }

    private static (CharacterDropDropTemplate, TomlConfig) Configure<T>(
        this (CharacterDropDropTemplate Template, TomlConfig Config) input,
        Action<T> configure, 
        string settingName)
    {
        if (input.Config.Settings.TryGetValue(settingName, out var tomlSetting) &&
            tomlSetting.IsSet &&
            tomlSetting is TomlSetting<T> setting)
        {
            configure(setting.Value);
        }

        return input;
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

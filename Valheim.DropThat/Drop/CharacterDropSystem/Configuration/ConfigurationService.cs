using System;
using System.Linq.Expressions;
using System.Reflection;
using DropThat.Drop.CharacterDropSystem.Modifiers;
using DropThat.Drop.Options.Extensions;
using ThatCore.Config.Toml;
using ThatCore.Config.Toml.Mapping;
using ThatCore.Config.Toml.Schema;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Configuration;
internal static class ConfigurationService
{
    public static void Register()
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
            .AddSetting("SetAmountLimit", -1, "Sets an absolute limit to the number of drops. This will stop multipliers from generating more than the amount set in this condition. Ignored if 0 or less.")
            .AddSetting("SetAutoStack", false, "If true, will attempt to stack items before dropping them. This means the item generation will only be run once per stack.")
            .AddSetting("SetDurability", -1, "Sets the durability of the item. Does not change max durability. If less than 0, uses default.")

            .AddSetting("ConditionMinLevel", -1, "Minimum level of mob for which item drops.")
            .AddSetting("ConditionMaxLevel", -1, "Maximum level of mob for which item drops.")
            .AddSetting("ConditionNotDay", false, "If true, will not drop during daytime.")
            .AddSetting("ConditionNotAfternoon", false, "If true, will not drop during afternoon.")
            .AddSetting("ConditionNotNight", false, "If true, will not drop during night.")
            .AddSetting("ConditionEnvironments", "", "List of environment names that allow the item to drop while they are active.\nEg. Misty, Thunderstorm. Leave empty to always allow.")
            .AddSetting("ConditionGlobalKeys", "", "List of global keys names that allow the item to drop while they are active.\nEg. defeated_eikthyr,defeated_gdking.Leave empty to always allow.")
            .AddSetting("ConditionNotGlobalKeys", "", "List of global key names that stop the item from dropping if any are detected.\nEg. defeated_eikthyr,defeated_gdking")
            .AddSetting("ConditionBiomes", "", "List of biome names that allow the item to drop while they are active.\nEg. Meadows, Swamp. Leave empty to always allow.")
            .AddSetting("ConditionCreatureStates", "", "List of creature states for which the item drop. If empty, allows all.\nEg. Default,Tamed,Event")
            .AddSetting("ConditionNotCreatureStates", "", "List of creature states for which the item will not drop.\nEg. Default,Tamed,Event")
            .AddSetting("ConditionHasItem", "", "List of item prefab names that will enable this drop. If empty, allows all.\nEg. skeleton_bow")
            .AddSetting("ConditionFaction", "", "List of factions that will enable this drop. If empty, allows all.\nEg. Undead, Boss")
            .AddSetting("ConditionNotFaction", "", "List of factions that will disable this drop. If empty, this condition is ignored.\nEg. Undead, boss")
            .AddSetting("ConditionLocation", "", "List of location names. When mob spawned in one of the listed locations, this drop is enabled.\nEg. Runestone_Boars")
            .AddSetting("ConditionDistanceToCenterMin", 0, "Minimum distance to center of map, for drop to be enabled.")
            .AddSetting("ConditionDistanceToCenterMax", 0, "Maximum distance to center of map, within which drop is enabled. 0 means limitless.")
            .AddSetting("ConditionKilledByDamageType", "", "List of damage types that will enable this drop, if they were part of the final killing blow. If empty, this condition is ignored.\nEg. Blunt, Fire")
            .AddSetting("ConditionKilledWithStatus", "", "List of statuses that mob had any of while dying, to enable this drop. If empty, this condition is ignored.\nEg. Burning, Smoked")
            .AddSetting("ConditionKilledWithStatuses", "", "List of statuses that mob must have had all of while dying, to enable this drop. If empty, this condition is ignored.\nEg. Burning, Smoked")
            .AddSetting("ConditionKilledBySkillType", "", "List of skill types that will enable this drop, if they were listed as the skill causing the damage of the final killing blow. If empty, this condition is ignored.\nEg. Swords, Unarmed")
            .AddSetting("ConditionKilledByEntityType", "", "List of entity types that if causing the last hit, will enable this drop. If empty, this condition is ignored.\nEg. Player, Creature, Other")
            .AddSetting("ConditionHitByEntityTypeRecently", "", "List of entity types. If any of the listed types have hit the creature recently (default 60 seconds), drop is enabled. If empty, this condition is ignored.\nEg. Player, Creature, Other")
            ;

        var modLayer = dropConfigBuilder
            .GetNode()
            .SetNextLayerAsNamed();

        modLayer
            .AddNode("CreatureLevelAndLootControl")
            .AddSetting("ConditionBossAffix", "", "Boss affixes for which item will drop.")
            .AddSetting("ConditionNotBossAffix", "", "Boss affixes for which item will not drop.")
            .AddSetting("ConditionInfusion", "", "Creature infusions for which item will drop.")
            .AddSetting("ConditionNotInfusion", "", "Creature infusions for which item will not drop.")
            .AddSetting("ConditionExtraEffect", "", "Creature extra effects for which item will drop.")
            .AddSetting("ConditionNotExtraEffect", "", "Creature extra effects for which item will not drop.")
            .AddSetting("ConditionWorldLevelMin", 0, "Minimum CLLC world level for which item will drop.")
            .AddSetting("ConditionWorldLevelMax", 0, "Maximum CLLC world level for which item will drop. 0 or less means no max.")
            ;

        modLayer
            .AddNode("EpicLoot")
            .AddSetting("RarityWeightNone", 0, "Weight to use for rolling as a non-magic item.")
            .AddSetting("RarityWeightMagic", 0, "Weight to use for rolling as rarity 'Magic'")
            .AddSetting("RarityWeightRare", 0, "Weight to use for rolling as rarity 'Rare'")
            .AddSetting("RarityWeightEpic", 0, "Weight to use for rolling as rarity 'Epic'")
            .AddSetting("RarityWeightLegendary", 0, "Weight to use for rolling as rarity 'Legendary'")
            .AddSetting("RarityWeightUnique", 0, "Weight to use for rolling unique items from the UniqueIDs array. If item rolls as unique, a single id will be selected randomly from the UniqueIDs.")
            .AddSetting("UniqueIDs", "", "Id's for unique legendaries from Epic Loot. Will drop as a non-magic item if the legendary does not meet its requirements.\nEg. HeimdallLegs, RagnarLegs")
            ;

        modLayer
            .AddNode("SpawnThat")
            .AddSetting("ConditionTemplateId", "", "Array of Spawn That TemplateId values to enable to drop for.")
            ;
    }

    public static void RegisterMappings()
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
                    Instantiation = new()
                    {
                        Instantiation = (TomlConfig config) => new CharacterDropDropTemplate(),
                        InstanceActions = new()
                        {
                            (TomlConfig config, CharacterDropDropTemplate template) =>
                            {
                                (template, config)
                                    .Configure(x => x.PrefabName)
                                    .Configure(x => x.Enabled, "EnableConfig")
                                    .Configure(x => x.AmountMin, "SetAmountMin")
                                    .Configure(x => x.AmountMax, "SetAmountMax")
                                    .Configure(x => x.ChanceToDrop, "SetChanceToDrop")
                                    .Configure(x => x.ScaleByLevel, "SetScaleByLevel")

                                    .Modifier(template.QualityLevel, "SetQualityLevel")
                                    ;

                                if (config.Settings.TryGetValue("SetQualityLevel", out var tomlSetting) &&
                                    tomlSetting.IsSet &&
                                    tomlSetting is TomlSetting<int?> setting)
                                {
                                    template.QualityLevel(setting.Value);
                                }
                            }
                        }
                    }
                }
            }
        };

        var configMapper = new ConfigToObjectMapper<CharacterDropTemplate>()
        {
            Path = new() { },
            Instantiation = new()
            {
                Instantiation = (TomlConfig) => new()
            },
            SubInstantiations = new()
            {
                
            }
        };
    }

    private static (TObj, TomlConfig) Configure<TObj, TProp>(
        this (TObj Obj, TomlConfig Config) input, 
        Expression<Func<TObj, TProp>> selector, 
        string settingName = null)
    {
        var propInfo = selector.GetPropertyInfo();

        settingName ??= propInfo.Name;

        if (input.Config.Settings.TryGetValue(settingName, out var tomlSetting) &&
            tomlSetting.IsSet &&
            tomlSetting is TomlSetting<TProp> setting)
        {
            propInfo.SetValue(input.Obj, setting.Value);
        }

        return input;
    }

    private static TObj Modifier<TObj, T>(
        this (TObj obj, TomlConfig Config) input,
        Func<T, TObj> configure,
        string settingName = null)
    {
        settingName ??= configure.GetMethodInfo().Name;

        if (input.Config.Settings.TryGetValue(settingName, out var tomlSetting) &&
            tomlSetting.IsSet &&
            tomlSetting is TomlSetting<T> setting)
        {
            configure(setting.Value);
        }

        return input;
    }
}

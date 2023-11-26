using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;
using DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.SpawnThat;
using DropThat.Drop.CharacterDropSystem.Conditions;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Drop.Options.Modifiers.ModEpicLoot;
using DropThat.Drop.Options.Modifiers;
using DropThat.Integrations.CllcIntegration;
using DropThat.Integrations;
using ThatCore.Utilities.Valheim;
using ThatCore.Extensions;
using DropThat.Drop.Options;

namespace DropThat.Drop.CharacterDropSystem.Configuration.Toml;

internal static partial class ConfigurationFileManager
{
    private static void RegisterMainMappings(CharacterDropConfigMapper mapper)
    {
        mapper
            .AddMobSetting()
            .FromFile(config => config
                .Map<List<string>>(
                    "UseDropList",
                    null,
                    "List of droplists to load before other drop settings",
                    (value, builder) => builder.ListNames = value));

        // Default settings
        mapper
            .AddDropSetting()
            .FromFile(config => config
                .Map<string>(
                    "PrefabName",
                    null,
                    "Prefab name of item to drop.",
                    (value, builder) => builder.PrefabName = value))
            .ToFile(config => config
                .Map("PrefabName", x => x.PrefabName))

            .AddOption()
            .FromFile(config => config
                .Map<bool?>(
                    "Enable",
                    true,
                    "Enable/disable this drop.",
                    (value, builder) => builder.Enabled = value))
            .ToFile(config => config
                .Map("Enable", x => x.Enabled))

            .AddOption()
            .FromFile(config => config
                .Map<bool?>(
                    "EnableConfig",
                    true,
                    "Enable/disable this specific drop configuration.",
                    (value, builder) => builder.TemplateEnabled = value))
            .ToFile(config => config
                .Map("EnableConfig", x => x.TemplateEnabled))

            .AddOption()
            .FromFile(config => config
                .Map<int?>(
                    "SetAmountMin",
                    1,
                    "Deprecated (use AmountMin). Minimum amount dropped.",
                    (value, builder) => builder.AmountMin = value)
                .Map<int?>(
                    "AmountMin",
                    1,
                    "Minimum amount dropped.",
                    (value, builder) => builder.AmountMin = value)
                )
            .ToFile(config => config
                .Map("AmountMin", x => x.AmountMin))

            .AddOption()
            .FromFile(config => config
                .Map<int?>(
                    "SetAmountMax",
                    1,
                    "Deprecated (use AmountMax). Maximum amount dropped.",
                    (value, builder) => builder.AmountMax = value)
                .Map<int?>(
                    "AmountMax",
                    1,
                    "Maximum amount dropped.",
                    (value, builder) => builder.AmountMax = value)
                )
            .ToFile(config => config
                .Map("AmountMax", x => x.AmountMax))

            .AddOption()
            .FromFile(config => config
                .Map<float?>(
                    "SetChanceToDrop", 
                    100f, 
                    "Deprecated (use ChanceToDrop). Chance to drop. 100 is 100%.\nExample values: 0, 50, 0.15",
                    (value, builder) => builder.ChanceToDrop = value)
                .Map<float?>(
                    "ChanceToDrop", 100f, "Chance to drop. 100 is 100%.\nExample values: 0, 50, 0.15",
                    (value, builder) => builder.ChanceToDrop = value))
            .ToFile(config => config
                .Map("ChanceToDrop", x => x.ChanceToDrop))

            .AddOption()
            .FromFile(config => config
                .Map<bool?>(
                    "SetDropOnePerPlayer", 
                    false, 
                    "Deprecated (use DropOnePerPlayer). If set, will drop one of this item per player. Ignoring other factors such as SetAmountMin / Max.",
                    (value, builder) => builder.DropOnePerPlayer = value)
                .Map<bool?>(
                    "DropOnePerPlayer", false, "If set, will drop one of this item per player. Ignoring other factors such as SetAmountMin / Max.",
                    (value, builder) => builder.DropOnePerPlayer = value)
                )
            .ToFile(config => config
                .Map("DropOnePerPlayer", x => x.DropOnePerPlayer))

            .AddOption()
            .FromFile(config => config
                .Map<bool?>(
                    "SetScaleByLevel", 
                    true, 
                    "Deprecated (use ScaleByLevel). Toggles mob levels scaling up dropped amount. Be aware, this scales up very quickly and may cause issues when dropping many items.",
                    (value, builder) => builder.ScaleByLevel = value)
                .Map<bool?>(
                    "ScaleByLevel", true, "Toggles mob levels scaling up dropped amount. Be aware, this scales up very quickly and may cause issues when dropping many items.",
                    (value, builder) => builder.ScaleByLevel = value)
                )
            .ToFile(config => config
                .Map("ScaleByLevel", x => x.ScaleByLevel))

            .AddOption()
            .FromFile(config => config
                .Map<bool?>(
                    "DisableResourceModifierScaling", false, "Disables resource scaling from world-modifiers if true.",
                    (value, builder) => builder.DisableResourceModifierScaling = value))
            .ToFile(config => config
                .Map("DisableResourceModifierScaling", x => x.DisableResourceModifierScaling))
            ;

        // Modifiers
        mapper
            .AddDropSetting()
            .FromFile(config => config
                .Using(x => x.ItemModifiers.GetOrCreate<ModifierQualityLevel>())
                .Map<int?>(
                    "SetQualityLevel", 
                    -1, 
                    "Deprecated (use QualityLevel). Sets the quality level of the item. If 0 or less, uses default quality level of drop.",
                    (value, builder) => builder.QualityLevel = value)
                .Map<int?>(
                    "QualityLevel", -1, "Sets the quality level of the item. If 0 or less, uses default quality level of drop.",
                    (value, builder) => builder.QualityLevel = value)
                )
            .ToFile(config => config
                .Using(x => x.ItemModifiers.GetOrDefault<ModifierQualityLevel>())
                .Map("QualityLevel", x => x.QualityLevel))

            .AddOption()
            .FromFile(config => config
                .Map<int?>(
                    "SetAmountLimit", 
                    -1, 
                    "Deprecated (use AmountLimit). Sets an absolute limit to the number of drops. This will stop multipliers from generating more than the amount set in this condition. Ignored if 0 or less.",
                    (value, builder) => builder.AmountLimit = value)
                .Map<int?>(
                    "AmountLimit", -1, "Sets an absolute limit to the number of drops. This will stop multipliers from generating more than the amount set in this condition. Ignored if 0 or less.",
                    (value, builder) => builder.AmountLimit = value)
                )
            .ToFile(config => config
                .Map("AmountLimit", x => x.AmountLimit))

            .AddOption()
            .FromFile(config => config
                .Map<bool?>(
                    "SetAutoStack", 
                    false, 
                    "Deprecated (use AutoStack). If true, will attempt to stack items before dropping them. This means the item generation will only be run once per stack.",
                    (value, builder) => builder.AutoStack = value)
                .Map<bool?>(
                    "AutoStack", false, "If true, will attempt to stack items before dropping them. This means the item generation will only be run once per stack.",
                    (value, builder) => builder.AutoStack = value)
                )
            .ToFile(config => config
                .Map("AutoStack", x => x.AutoStack))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.ItemModifiers.GetOrCreate<ModifierDurability>())
                .Map<float?>(
                    "SetDurability", 
                    -1f, 
                    "Deprecated (use Durability). Sets the durability of the item. Does not change max durability. If less than 0, uses default.",
                    (value, builder) => builder.Durability = value)
                .Map<float?>(
                    "Durability", -1f, "Sets the durability of the item. Does not change max durability. If less than 0, uses default.",
                    (value, builder) => builder.Durability = value)
                )
            .ToFile(config => config
                .Using(x => x.ItemModifiers.GetOrDefault<ModifierDurability>())
                .Map("Durability", x => x.Durability))
            ;

        // Conditions
        mapper
            .AddDropSetting()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionLevelMin>())
                .Map<int?>(
                    "ConditionMinLevel", 
                    -1, 
                    "Minimum level of mob for which item drops.",
                    (value, builder) => builder.MinLevel = value)
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionLevelMin>())
                .Map("ConditionMinLevel", x => x.MinLevel))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionLevelMax>())
                .Map<int?>(
                    "ConditionMaxLevel", -1, 
                    "Maximum level of mob for which item drops.",
                    (value, builder) => builder.MaxLevel = value)
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionLevelMax>())
                .Map("ConditionMaxLevel", x => x.MaxLevel))

            .AddOption()
            .FromFile(config => config
                .Map<bool?>(
                    "ConditionNotDay", 
                    false, 
                    "If true, will not drop during daytime.",
                    (value, builder) =>
                    {
                        if (value == true) 
                        { 
                            builder.Conditions.GetOrCreate<ConditionNotDay>(); 
                        }
                        else 
                        { 
                            builder.Conditions.Remove<ConditionNotDay>(); 
                        }
                    })
                )
            .ToFile(config => config
                .Map("ConditionNotDay", x => x.Conditions.GetOrDefault<ConditionNotDay>() is not null))

            .AddOption()
            .FromFile(config => config
                .Map<bool?>(
                    "ConditionNotAfternoon", 
                    false, 
                    "If true, will not drop during afternoon.",
                    (value, builder) =>
                    {
                        if (value == true)
                        {
                            builder.Conditions.GetOrCreate<ConditionNotAfternoon>();
                        }
                        else 
                        {
                            builder.Conditions.Remove<ConditionNotAfternoon>();
                        }
                    })
                )
            .ToFile(config => config
                .Map("ConditionNotAfternoon", x => x.Conditions.GetOrDefault<ConditionNotAfternoon>() is not null))

            .AddOption()
            .FromFile(config => config
                .Map<bool?>(
                    "ConditionNotNight", 
                    false, 
                    "If true, will not drop during night.",
                    (value, builder) =>
                    {
                        if (value == true)
                        {
                            builder.Conditions.GetOrCreate<ConditionNotNight>();
                        }
                        else
                        {
                            builder.Conditions.Remove<ConditionNotNight>();
                        }
                    })
                )
            .ToFile(config => config
                .Map("ConditionNotNight", x => x.Conditions.GetOrDefault<ConditionNotNight>() is not null))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionEnvironments>())
                .Map<List<string>>(
                    "ConditionEnvironments", 
                    null, 
                    "List of environment names that allow the item to drop while they are active.\nEg. Misty, Thunderstorm. Leave empty to always allow.",
                    (value, builder) => builder.SetEnvironments(value))
                )
            .ToFile(config => config    
                .Using(x => x.Conditions.GetOrDefault<ConditionEnvironments>())
                .Map("ConditionEnvironments", x => x.Environments?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionGlobalKeysAny>())
                .Map<List<string>>(
                    "ConditionGlobalKeys", 
                    null, 
                    "List of global keys names that allow the item to drop while they are active.\nEg. defeated_eikthyr,defeated_gdking.Leave empty to always allow.",
                    (value, builder) => builder.GlobalKeys = value?.ToArray())
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionGlobalKeysAny>())
                .Map("ConditionGlobalKeys", x => x.GlobalKeys?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionGlobalKeysNotAny>())
                .Map<List<string>>(
                    "ConditionNotGlobalKeys", 
                    null, 
                    "List of global key names that stop the item from dropping if any are detected.\nEg. defeated_eikthyr,defeated_gdking",
                    (value, builder) => builder.GlobalKeys = value?.ToArray())
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionGlobalKeysNotAny>())
                .Map("ConditionNotGlobalKeys", x => x.GlobalKeys?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionBiome>())
                .Map<List<Heightmap.Biome>>(
                    "ConditionBiomes", 
                    null, 
                    "List of biome names that allow the item to drop while they are active.\nEg. Meadows, Swamp. Leave empty to always allow.",
                    (value, builder) => builder.BiomeBitmask = value.ToBitmask())
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionBiome>())
                .Map("ConditionBiomes", x => x.BiomeBitmask.Split().ConvertAll(x => x.ToString())))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionCreatureState>())
                .Map<List<CreatureState>>(
                    "ConditionCreatureStates", 
                    null, 
                    "List of creature states for which the item drop. If empty, allows all.\nEg. Default,Tamed,Event",
                    (value, builder) => builder.CreatureStates = value?.ToArray())
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionCreatureState>())
                .Map("ConditionCreatureStates", x => x.CreatureStates?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionNotCreatureState>())
                .Map<List<CreatureState>>(
                    "ConditionNotCreatureStates", 
                    null, 
                    "List of creature states for which the item will not drop.\nEg. Default,Tamed,Event",
                    (value, builder) => builder.CreatureStates = value?.ToArray())
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionNotCreatureState>())
                .Map("ConditionNotCreatureStates", x => x.CreatureStates?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionInventory>())
                .Map<List<string>>(
                    "ConditionHasItem", 
                    null, 
                    "List of item prefab names that will enable this drop. If empty, allows all.\nEg. skeleton_bow",
                    (value, builder) => builder.SetItems(value))
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionInventory>())
                .Map("ConditionHasItem", x => x.Items?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionFaction>())
                .Map<List<Character.Faction>>(
                    "ConditionFaction", 
                    null, 
                    "List of factions that will enable this drop. If empty, allows all.\nEg. Undead, Boss",
                    (value, builder) => builder.Factions = value.ToArray())
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionFaction>())
                .Map("ConditionFaction", x => x.Factions?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionNotFaction>())
                .Map<List<Character.Faction>>(
                    "ConditionNotFaction", 
                    null, 
                    "List of factions that will disable this drop. If empty, this condition is ignored.\nEg. Undead, boss",
                    (value, builder) => builder.Factions = value?.ToArray())
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionNotFaction>())
                .Map("ConditionNotFaction", x => x.Factions?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionLocation>())
                .Map<List<string>>(
                    "ConditionLocation", 
                    null, 
                    "List of location names. When mob spawned in one of the listed locations, this drop is enabled.\nEg. Runestone_Boars",
                    (value, builder) => builder.SetLocations(value))
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionLocation>())
                .Map("ConditionLocation", x => x.Locations?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionDistanceToCenterMin>())
                .Map<float?>(
                    "ConditionDistanceToCenterMin", 
                    0, 
                    "Minimum distance to center of map, for drop to be enabled.",
                    (value, builder) => builder.DistanceToCenterMin = value ?? 0)
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionDistanceToCenterMin>())
                .Map("ConditionDistanceToCenterMin", x => x.DistanceToCenterMin))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionDistanceToCenterMax>())
                .Map<float?>(
                    "ConditionDistanceToCenterMax", 
                    0, 
                    "Maximum distance to center of map, within which drop is enabled. 0 means limitless.",
                    (value, builder) => builder.DistanceToCenterMax = value ?? 0)
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionDistanceToCenterMax>())
                .Map("ConditionDistanceToCenterMax", x => x.DistanceToCenterMax))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionKilledByDamageType>())
                .Map<List<HitData.DamageType>>(
                    "ConditionKilledByDamageType", 
                    null, 
                    "List of damage types that will enable this drop, if they were part of the final killing blow. If empty, this condition is ignored.\nEg. Blunt, Fire",
                    (value, builder) => builder.DamageTypeMask = value?.ToBitmask() ?? 0)
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionKilledByDamageType>())
                .Map("ConditionKilledByDamageType", x => x.DamageTypeMask.Split()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionKilledWithStatusAny>())
                .Map<List<string>>(
                    "ConditionKilledWithStatus", 
                    null, 
                    "List of statuses that mob had any of while dying, to enable this drop. If empty, this condition is ignored.\nEg. Burning, Smoked",
                    (value, builder) => builder.SetStatuses(value))
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionKilledWithStatusAny>())
                .Map("ConditionKilledWithStatus", x => x.Statuses?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionKilledWithStatusAll>())
                .Map<List<string>>(
                    "ConditionKilledWithStatuses", 
                    null, 
                    "List of statuses that mob must have had all of while dying, to enable this drop. If empty, this condition is ignored.\nEg. Burning, Smoked",
                    (value, builder) => builder.SetStatuses(value))
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionKilledWithStatusAll>())
                .Map("ConditionKilledWithStatuses", x => x.Statuses?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionKilledBySkillType>())
                .Map<List<Skills.SkillType>>(
                    "ConditionKilledBySkillType", 
                    null, 
                    "List of skill types that will enable this drop, if they were listed as the skill causing the damage of the final killing blow. If empty, this condition is ignored.\nEg. Swords, Unarmed",
                    (value, builder) => builder.SkillTypes = value?.ToHashSet())
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionKilledBySkillType>())
                .Map("ConditionKilledBySkillType", x => x.SkillTypes?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionKilledByEntityType>())
                .Map<List<EntityType>>(
                    "ConditionKilledByEntityType", 
                    null, 
                    "List of entity types that if causing the last hit, will enable this drop. If empty, this condition is ignored.\nEg. Player, Creature, Other",
                    (value, builder) => builder.EntityTypes = value?.Distinct().ToArray())
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionKilledByEntityType>())
                .Map("ConditionKilledByEntityType", x => x.EntityTypes?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionHitByEntityTypeRecently>())
                .Map<List<EntityType>>(
                    "ConditionHitByEntityTypeRecently", 
                    null, 
                    "List of entity types. If any of the listed types have hit the creature recently (default 60 seconds), drop is enabled. If empty, this condition is ignored.\nEg. Player, Creature, Other",
                    (value, builder) => builder.EntityTypes = value?.ToHashSet())
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionHitByEntityTypeRecently>())
                .Map("ConditionHitByEntityTypeRecently", x => x.EntityTypes?.ToList()))
            ;

        // Mods - Creature Level And Loot Control
        mapper
            .AddModRequirement("CreatureLevelAndLootControl", () => InstallationManager.CLLCInstalled)
            .AddModSettings("CreatureLevelAndLootControl")
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionBossAffix>())
                .Map<List<CllcBossAffix>>(
                    "ConditionBossAffix", 
                    null, 
                    "Boss affixes for which item will drop.",
                    (value, builder) => builder.BossAffixes = value?.ToArray())
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionBossAffix>())
                .Map("ConditionBossAffix", x => x.BossAffixes?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionNotBossAffix>())
                .Map<List<CllcBossAffix>>(
                    "ConditionNotBossAffix", 
                    null, 
                    "Boss affixes for which item will not drop.",
                    (value, builder) => builder.BossAffixes = value?.ToArray())
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionNotBossAffix>())
                .Map("ConditionNotBossAffix", x => x.BossAffixes?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionInfusion>())
                .Map<List<CllcCreatureInfusion>>(
                    "ConditionInfusion", 
                    null, 
                    "Creature infusions for which item will drop.",
                    (value, builder) => builder.Infusions = value?.ToArray())
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionInfusion>())
                .Map("ConditionInfusion", x => x.Infusions?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionNotInfusion>())
                .Map<List<CllcCreatureInfusion>>(
                    "ConditionNotInfusion", 
                    null, 
                    "Creature infusions for which item will not drop.",
                    (value, builder) => builder.Infusions = value?.ToArray())
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionNotInfusion>())
                .Map("ConditionNotInfusion", x => x.Infusions?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionCreatureExtraEffect>())
                .Map<List<CllcCreatureExtraEffect>>(
                    "ConditionExtraEffect", 
                    null, 
                    "Creature extra effects for which item will drop.",
                    (value, builder) => builder.ExtraEffects = value?.ToArray())
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionCreatureExtraEffect>())
                .Map("ConditionExtraEffect", x => x.ExtraEffects?.ToList()))

            .AddOption()
            .FromFile(config => config  
                .Using(x => x.Conditions.GetOrCreate<ConditionNotCreatureExtraEffect>())
                .Map<List<CllcCreatureExtraEffect>>(
                    "ConditionNotExtraEffect", 
                    null, 
                    "Creature extra effects for which item will not drop.",
                    (value, builder) => builder.ExtraEffects = value?.ToArray())
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionNotCreatureExtraEffect>())
                .Map("ConditionNotExtraEffect", x => x.ExtraEffects?.ToList()))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionWorldLevelMin>())
                .Map<int?>(
                    "ConditionWorldLevelMin", 
                    0, 
                    "Minimum CLLC world level for which item will drop.",
                    (value, builder) => builder.MinLevel = value)
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionWorldLevelMin>())
                .Map("ConditionWorldLevelMin", x => x.MinLevel))

            .AddOption()
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionWorldLevelMax>())
                .Map<int?>(
                    "ConditionWorldLevelMax", 
                    0, 
                    "Maximum CLLC world level for which item will drop. 0 or less means no max.",
                    (value, builder) => builder.MaxLevel = value)
                )
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionWorldLevelMax>())
                .Map("ConditionWorldLevelMax", x => x.MaxLevel))
            ;

        // Mods - Epic Loot
        mapper
            .AddModRequirement("EpicLoot", () => InstallationManager.EpicLootInstalled)
            .AddModSettings("EpicLoot")
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

        // Mods - Spawn That
        mapper
            .AddModRequirement("SpawnThat", () => InstallationManager.SpawnThatInstalled)
            .AddModSettings("SpawnThat")
            .FromFile(config => config
                .Using(x => x.Conditions.GetOrCreate<ConditionTemplateId>())
                .Map<List<string>>(
                    "ConditionTemplateId", null, "List of Spawn That TemplateId values to enable to drop for.",
                    (value, builder) => builder.TemplateIds = value.ToArray()))
            .ToFile(config => config
                .Using(x => x.Conditions.GetOrDefault<ConditionTemplateId>())
                .Map("ConditionTemplateId", x => x.TemplateIds?.ToList()))
            ;
    }

}

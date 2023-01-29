﻿using System;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Conditions;
using ThatCore.Config.Toml;
using ThatCore.Config.Toml.Writers;
using DropThat.Drop.CharacterDropSystem.Managers;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Config.Toml.Schema;
using ThatCore.Extensions;
using DropThat.Drop.Options.Modifiers;
using DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;
using DropThat.Drop.Options.Modifiers.ModEpicLoot;
using DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.SpawnThat;
using DropThat.Debugging;

namespace DropThat.Drop.CharacterDropSystem.Debug;

internal static class TemplateWriter
{
    public static void WriteToDiskAsToml()
    {
        var tomlFile = PrepareTomlFile();

        var content = TomlConfigWriter.WriteToString(tomlFile, new TomlWriterSettings()
        {
            FileName = "drop_that.character_drop_loaded_configs.cfg",
            AddComments = false,
            Header =
                $"# This file was auto-generated by Drop That {DropThatPlugin.Version} at {DateTimeOffset.UtcNow.ToString("u")}, with Valheim '{Version.m_major}.{Version.m_minor}.{Version.m_patch}'.\n" +
                $"# The entries listed here were generated from the internally loaded CharacterDrop configurations.\n" +
                $"# This is intended to reveal the state of Drop That, after loading configs from all sources, and before applying them to their respective target drop tables.\n" +
                $"# This file is not scanned by Drop That, and any changes done will therefore have no effect. Copy sections into a CharacterDrop config in the configs folder if you want to change things."
        });

        DebugFileWriter.WriteFile(content, "drop_that.character_drop_loaded_configs.cfg", "loaded CharacterDrop configs");
    }

    internal static TomlConfig PrepareTomlFile()
    {
        TomlConfig config = new();

        var templates = TemplateManager.GetTemplates();

        foreach ((string mob, CharacterDropMobTemplate mobTemplate) in templates)
        {
            var section = config.CreateSubsection(new TomlPathSegment(TomlPathSegmentType.Named, mob));

            foreach (var dropTemplate in mobTemplate.Drops)
            {
                var drop = dropTemplate.Value;

                AddDefaults(config, drop);

                // Add mod settings
                AddCllc(config, drop);
                AddEpicLoot(config, drop);
                AddSpawnThat(config, drop);
            }
        }

        return config;
    }

    private static void AddDefaults(TomlConfig config, CharacterDropDropTemplate drop)
    {
        config.AddIfSet(drop.PrefabName, "PrefabName");
        config.AddIfSet(drop.Enabled, "EnableConfig");
        config.AddIfSet(drop.AmountMin, "SetAmountMin");
        config.AddIfSet(drop.AmountMax, "SetAmountMax");
        config.AddIfSet(drop.ChanceToDrop, "SetChanceToDrop");
        config.AddIfSet(drop.DropOnePerPlayer, "SetDropOnePerPlayer");
        config.AddIfSet(drop.ScaleByLevel, "SetScaleByLevel");

        config.AddIfSet(drop.AmountLimit, "SetAmountLimit");
        config.AddIfSet(drop.AutoStack, "SetAutoStack");

        foreach (var modifier in drop.ItemModifiers)
        {
            switch (modifier)
            {
                case ModifierDurability mod:
                    config.AddIfSet(mod.Durability, "SetDurability");
                    break;
                case ModifierQualityLevel mod:
                    config.AddIfSet(mod.QualityLevel, "SetQualityLevel");
                    break;
            }
        }

        foreach (var condition in drop.Conditions)
        {
            switch (condition)
            {
                case ConditionLevelMin cond:
                    config.AddIfSet(cond.MinLevel, "ConditionMinLevel");
                    break;
                case ConditionLevelMax cond:
                    config.AddIfSet(cond.MaxLevel, "ConditionMaxLevel");
                    break;
                case ConditionNotDay:
                    config.AddIfSet(true, "ConditionNotDay");
                    break;
                case ConditionNotAfternoon:
                    config.AddIfSet(true, "ConditionNotAfternoon");
                    break;
                case ConditionNotNight:
                    config.AddIfSet(true, "ConditionNotNight");
                    break;
                case ConditionEnvironments cond:
                    config.AddIfSet(cond.Environments.ToList(), "ConditionEnvironments");
                    break;
                case ConditionGlobalKeysAny cond:
                    config.AddIfSet(cond.GlobalKeys.ToList(), "ConditionGlobalKeys");
                    break;
                case ConditionGlobalKeysNotAny cond:
                    config.AddIfSet(cond.GlobalKeys.ToList(), "ConditionNotGlobalKeys");
                    break;
                case ConditionBiome cond:
                    config.AddIfSet(cond.BiomeMask.Split(), "ConditionBiomes");
                    break;
                case ConditionCreatureState cond:
                    config.AddIfSet(cond.CreatureStates.ToList(), "ConditionCreatureStates");
                    break;
                case ConditionNotCreatureState cond:
                    config.AddIfSet(cond.CreatureStates.ToList(), "ConditionNotCreatureStates");
                    break;
                case ConditionInventory cond:
                    config.AddIfSet(cond.Items.ToList(), "ConditionHasItem");
                    break;
                case ConditionFaction cond:
                    config.AddIfSet(cond.Factions.ToList(), "ConditionFaction");
                    break;
                case ConditionNotFaction cond:
                    config.AddIfSet(cond.Factions.ToList(), "ConditionNotFaction");
                    break;
                case ConditionLocation cond:
                    config.AddIfSet(cond.Locations.ToList(), "ConditionLocation");
                    break;
                case ConditionDistanceToCenterMin cond:
                    config.AddIfSet(cond.DistanceToCenterMin, "ConditionDistanceToCenterMin");
                    break;
                case ConditionDistanceToCenterMax cond:
                    config.AddIfSet(cond.DistanceToCenterMax, "ConditionDistanceToCenterMax");
                    break;
                case ConditionKilledByDamageType cond:
                    config.AddIfSet(cond.DamageTypeMask.Split(), "ConditionKilledByDamageType");
                    break;
                case ConditionKilledWithStatusAny cond:
                    config.AddIfSet(cond.Statuses.ToList(), "ConditionKilledWithStatus");
                    break;
                case ConditionKilledWithStatusAll cond:
                    config.AddIfSet(cond.Statuses.ToList(), "ConditionKilledWithStatuses");
                    break;
                case ConditionKilledBySkillType cond:
                    config.AddIfSet(cond.SkillTypes.ToList(), "ConditionKilledBySkillType");
                    break;
                case ConditionKilledByEntityType cond:
                    config.AddIfSet(cond.EntityTypes.ToList(), "ConditionKilledByEntityType");
                    break;
                case ConditionHitByEntityTypeRecently cond:
                    config.AddIfSet(cond.EntityTypes.ToList(), "ConditionHitByEntityTypeRecently");
                    break;
            }
        }
    }

    private static void AddCllc(TomlConfig config, CharacterDropDropTemplate drop)
    {
        config = config.CreateSubsection(new TomlPathSegment(TomlPathSegmentType.Named, "CreatureLevelAndLootControl"));

        foreach (var condition in drop.Conditions)
        {
            switch (condition)
            {
                case ConditionBossAffix cond:
                    config.AddIfSet(cond.BossAffixes.ToList(), "ConditionBossAffix");
                    break;
                case ConditionNotBossAffix cond:
                    config.AddIfSet(cond.BossAffixes.ToList(), "ConditionNotBossAffix");
                    break;
                case ConditionInfusion cond:
                    config.AddIfSet(cond.Infusions.ToList(), "ConditionInfusion");
                    break;
                case ConditionNotInfusion cond:
                    config.AddIfSet(cond.Infusions.ToList(), "ConditionNotInfusion");
                    break;
                case ConditionCreatureExtraEffect cond:
                    config.AddIfSet(cond.ExtraEffects.ToList(), "ConditionExtraEffect");
                    break;
                case ConditionNotCreatureExtraEffect cond:
                    config.AddIfSet(cond.ExtraEffects.ToList(), "ConditionNotExtraEffect");
                    break;
                case ConditionWorldLevelMin cond:
                    config.AddIfSet(cond.MinLevel, "ConditionWorldLevelMin");
                    break;
                case ConditionWorldLevelMax cond:
                    config.AddIfSet(cond.MaxLevel, "ConditionWorldLevelMax");
                    break;
            }
        }
    }

    private static void AddEpicLoot(TomlConfig config, CharacterDropDropTemplate drop)
    {
        var modifier = drop.ItemModifiers
            .OfType<ModifierEpicLootItem>()
            .FirstOrDefault();

        if (modifier is null)
        {
            return;
        }

        config = config.CreateSubsection(new TomlPathSegment(TomlPathSegmentType.Named, "EpicLoot"));

        config.AddIfSet(modifier.RarityWeightNone, "RarityWeightNone");
        config.AddIfSet(modifier.RarityWeightMagic, "RarityWeightMagic");
        config.AddIfSet(modifier.RarityWeightRare, "RarityWeightRare");
        config.AddIfSet(modifier.RarityWeightEpic, "RarityWeightEpic");
        config.AddIfSet(modifier.RarityWeightLegendary, "RarityLegendary");
        config.AddIfSet(modifier.RarityWeightUnique, "RarityWeightUnique");
        config.AddIfSet(modifier.UniqueIds, "UniqueIds");
    }

    private static void AddSpawnThat(TomlConfig config, CharacterDropDropTemplate drop)
    {
        var condition = drop.Conditions
            .OfType<ConditionTemplateId>()
            .FirstOrDefault();

        if (condition is null)
        {
            return;
        }

        config = config.CreateSubsection(new TomlPathSegment(TomlPathSegmentType.Named, "SpawnThat"));

        config.AddIfSet(condition.TemplateIds.ToList(), "ConditionTemplateId");
    }

    private static void AddIfSet<T>(this TomlConfig config, T value, string name)
    {
        if (value is not null)
        {
            var setting = new TomlSetting<T>(name, default);
            setting.Value = value;
            setting.IsSet = true;

            config.SetSetting(name, setting);
        }
    }
}

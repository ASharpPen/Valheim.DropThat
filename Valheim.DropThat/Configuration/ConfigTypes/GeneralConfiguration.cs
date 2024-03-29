﻿using BepInEx.Configuration;
using System;
using Valheim.DropThat.Core.Configuration;

namespace Valheim.DropThat.Configuration.ConfigTypes
{
    [Serializable]
    public class GeneralConfiguration
    {
        [NonSerialized]
        private ConfigFile Config;

        // General

        public ConfigurationEntry<bool> StopTouchingMyConfigs = new(true, "Disables automatic updating and saving of drop table configurations.\nThis means the default settings note used will not automatically be added, and no helpful comments either, but.. allows you to keep things compact.\nNote: This setting can have massive impact on load time if disabled.");
        public ConfigurationEntry<bool> LoadSupplementalDropTables = new(true, "Loads drop table configurations from supplemental files.\nEg. drop_that.character_drop.my_drops.cfg will be included on load.");

        // CharacterDrop

        public ConfigurationEntry<bool> ClearAllExistingCharacterDrops = new(false, "When enabled, all existing items in drop tables gets removed.");
        public ConfigurationEntry<bool> ClearAllExistingCharacterDropsWhenModified = new(false, "When enabled, all existing items in drop tables are removed when a configuration for that entity exist. Eg. if 'Deer' is present in configuration table, the configured drops will be the only drops for 'Deer'.");
        public ConfigurationEntry<bool> AlwaysAppendCharacterDrops = new(false, "When enabled, drop configurations will not override existing items if their indexes match.");
        public ConfigurationEntry<bool> ApplyConditionsOnDeathCharacterDrops = new(false, "When enabled, drop conditions are checked at time of death, instead of at time of spawn.");

        // DropTable

        public ConfigurationEntry<bool> ClearAllExistingDropTables = new (false, "When enabled, all existing items in drop tables get removed.");
        public ConfigurationEntry<bool> ClearAllExistingDropTablesWhenModified = new (false, "When enabled, all existing items in drop tables are removed when a configuration for that entity exist.\nEg. if 'FirTree' is present in configuration table, the configured drops will be the only drops for 'FirTree'.");
        public ConfigurationEntry<bool> AlwaysAppendDropTables = new (false, "When enabled, drop configurations will not override existing items if their indexes match.");

        // Performance

        public ConfigurationEntry<bool> AlwaysAutoStack = new(false, "When enabled, will always attempt to create stacks of items when dropping, instead of creating items one by one.\nEg. 35 coin stack, instead of 35 individual 1 coin drops.");
        public ConfigurationEntry<int> DropLimit = new(-1, "When greater than 0, will limit the maximum number of items dropped at a time. This is intended for guarding against multipliers.\nEg. if limit is 100, and attempting to drop 200 coins, only 100 will be dropped.");

        // Debug 

        public ConfigurationEntry<bool> EnableDebugLogging = new(false, "Enable debug logging.");
        public ConfigurationEntry<bool> EnableTraceLogging = new(false, "Enables in-depth logging. Note, this might generate a LOT of log entries.");
        public ConfigurationEntry<bool> WriteCreatureItemsToFile = new(false, "When enabled, creates a file on world start, in the debug folder containing items of mobs that have drop tables.");
        public ConfigurationEntry<bool> WriteLocationsToFile = new(false, "When enables, creates a file on world start in the debug folder, containing the name of each location in the game.");
        public ConfigurationEntry<bool> WriteCharacterDropsToFile = new(false, "When enabled, creates a file on world start, in the debug folder containing the default CharacterDrop configurations.");
        public ConfigurationEntry<bool> WriteDropTablesToFiles = new(false, "When enabled, creates files on world start, in the debug folder, containing the default DropTable configurations.");

        public ConfigurationEntry<string> DebugFileFolder = new("Debug", "Folder path to write to. Root folder is BepInEx.");

        public void Load(ConfigFile configFile)
        {
            Config = configFile;

            EnableTraceLogging.Bind(Config, "Debug", nameof(EnableTraceLogging));

            AlwaysAutoStack.Bind(Config, "Performance", nameof(AlwaysAutoStack));
            DropLimit.Bind(Config, "Performance", nameof(DropLimit));

            ClearAllExistingCharacterDrops.Bind(Config, "CharacterDrop", "ClearAllExisting");
            ClearAllExistingCharacterDropsWhenModified.Bind(Config, "CharacterDrop", "ClearAllExistingWhenModified");
            AlwaysAppendCharacterDrops.Bind(Config, "CharacterDrop", "AlwaysAppend");

            ClearAllExistingDropTables.Bind(Config, "DropTable", "ClearAllExisting");
            ClearAllExistingDropTablesWhenModified.Bind(Config, "DropTable", "ClearAllExistingWhenModified");
            AlwaysAppendDropTables.Bind(Config, "DropTable", "AlwaysAppend");

            EnableDebugLogging.Bind(Config, "Debug", "EnableDebugLogging");
            WriteCharacterDropsToFile.Bind(Config, "Debug", nameof(WriteCharacterDropsToFile));
            WriteCreatureItemsToFile.Bind(Config, "Debug", nameof(WriteCreatureItemsToFile));
            WriteLocationsToFile.Bind(Config, "Debug", nameof(WriteLocationsToFile));
            WriteDropTablesToFiles.Bind(Config, "Debug", nameof(WriteDropTablesToFiles));

            DebugFileFolder.Bind(Config, "Debug", nameof(DebugFileFolder));

            StopTouchingMyConfigs.Bind(Config, "General", nameof(StopTouchingMyConfigs));
            LoadSupplementalDropTables.Bind(Config, "General", nameof(LoadSupplementalDropTables));
        }
    }
}

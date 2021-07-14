using BepInEx.Configuration;
using System;
using Valheim.DropThat.Core.Configuration;

namespace Valheim.DropThat.Configuration.ConfigTypes
{
    [Serializable]
    public class GeneralConfiguration
    {
        [NonSerialized]
        private ConfigFile Config;

        #region General

        public ConfigurationEntry<bool> DebugMode = new ConfigurationEntry<bool>(false, "Enable debug logging.");
        public ConfigurationEntry<bool> StopTouchingMyConfigs = new ConfigurationEntry<bool>(true, "Disables automatic updating and saving of drop table configurations.\nThis means no helpers will be added, but.. allows you to keep things compact.");
        public ConfigurationEntry<bool> LoadSupplementalDropTables = new ConfigurationEntry<bool>(true, "Loads drop table configurations from supplemental files.\nEg. drop_that.supplemental.my_drops.cfg will be included on load.");

        #endregion

        #region DropTables

        public ConfigurationEntry<bool> ClearAllExisting = new ConfigurationEntry<bool>(false, "When enabled, all existing items in drop tables gets removed.");
        public ConfigurationEntry<bool> ClearAllExistingWhenModified = new ConfigurationEntry<bool>(false, "When enabled, all existing items in drop tables are removed when a configuration for that entity exist. Eg. if 'Deer' is present in configuration table, the configured drops will be the only drops for 'Deer'.");
        public ConfigurationEntry<bool> AlwaysAppend = new ConfigurationEntry<bool>(false, "When enabled, drop configurations will not override existing items if their indexes match.");
        public ConfigurationEntry<bool> ApplyConditionsOnDeath = new ConfigurationEntry<bool>(false, "When enabled, drop conditions are checked at time of death, instead of at time of spawn.");

        #endregion

        public ConfigurationEntry<bool> ClearAllExistingDropTables = new (false);
        public ConfigurationEntry<bool> ClearAllExistingDropTablesWhenModified = new (false);
        public ConfigurationEntry<bool> AlwaysAppendDropTables = new (false);

        #region Performance

        public ConfigurationEntry<bool> AlwaysAutoStack = new ConfigurationEntry<bool>(false, "When enabled, will always attempt to create stacks of items when dropping, instead of creating items one by one.\nEg. 35 coin stack, instead of 35 individual 1 coin drops.");

        public ConfigurationEntry<int> DropLimit = new ConfigurationEntry<int>(-1, "When greater than 0, will limit the maximum number of items dropped at a time. This is intended for guarding against multipliers.\nEg. if limit is 100, and attempting to drop 200 coins, only 100 will be dropped.");

        #endregion

        #region Debug

        public ConfigurationEntry<bool> EnableTraceLogging = new ConfigurationEntry<bool>(false, "Enables in-depth logging. Note, this might generate a LOT of log entries.");
        public ConfigurationEntry<bool> WriteDefaultDropTableToFile = new ConfigurationEntry<bool>(false, "When enabled, creates a file on world start, in the debug folder containing the default mob drop tables.");
        public ConfigurationEntry<bool> WriteCreatureItemsToFile = new ConfigurationEntry<bool>(false, "When enabled, creates a file on world start, in the debug folder containing items of mobs that have drop tables.");
        public ConfigurationEntry<bool> WriteLocationsToFile = new ConfigurationEntry<bool>(false, "When enables, creates a file on world start in the debug folder, containing the name of each location in the game.");
        public ConfigurationEntry<bool> WriteDropTablesToFiles = new(false, "When enabled, creates files on world start, in the debug folder, containing the default drop tables of non-creatures.");

        public ConfigurationEntry<string> WriteDebugFilesToFolder = new("Debug", "Folder path to write to. Root folder is BepInEx.");

        #endregion

        public void Load(ConfigFile configFile)
        {
            Config = configFile;

            EnableTraceLogging.Bind(Config, "Debug", nameof(EnableTraceLogging));

            AlwaysAutoStack.Bind(Config, "Performance", nameof(AlwaysAutoStack));
            DropLimit.Bind(Config, "Performance", nameof(DropLimit));

            // TODO: Move all of these to a different naming scheme, so we avoid overlap for CharacterDrop configs.
            ClearAllExisting.Bind(Config, "DropTables", "ClearAllExisting");
            ClearAllExistingWhenModified.Bind(Config, "DropTables", "ClearAllExistingWhenModified");
            AlwaysAppend.Bind(Config, "DropTables", "AlwaysAppend");
            ApplyConditionsOnDeath.Bind(Config, "DropTables", nameof(ApplyConditionsOnDeath));

            // TODO: Rename so that we don't repeat the "DropTable" part of the name
            ClearAllExistingDropTables.Bind(Config, "DropTables", nameof(ClearAllExistingDropTables));
            ClearAllExistingDropTablesWhenModified.Bind(Config, "DropTables", nameof(ClearAllExistingDropTablesWhenModified));
            AlwaysAppendDropTables.Bind(Config, "DropTables", nameof(AlwaysAppendDropTables));

            DebugMode.Bind(Config, "General", "EnableDebug");
            WriteDefaultDropTableToFile.Bind(Config, "Debug", nameof(WriteDefaultDropTableToFile));
            WriteCreatureItemsToFile.Bind(Config, "Debug", nameof(WriteCreatureItemsToFile));
            WriteLocationsToFile.Bind(Config, "Debug", nameof(WriteLocationsToFile));
            WriteDropTablesToFiles.Bind(Config, "Debug", nameof(WriteDropTablesToFiles));



            StopTouchingMyConfigs.Bind(Config, "General", nameof(StopTouchingMyConfigs));
            LoadSupplementalDropTables.Bind(Config, "General", nameof(LoadSupplementalDropTables));
        }
    }
}

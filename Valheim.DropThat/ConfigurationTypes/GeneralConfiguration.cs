using BepInEx.Configuration;
using System;
using Valheim.DropThat.ConfigurationCore;

namespace Valheim.DropThat.ConfigurationTypes
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

        #region Debug

        public ConfigurationEntry<bool> EnableTraceLogging = new ConfigurationEntry<bool>(false, "Enables in-depth logging. Note, this might generate a LOT of log entries.");
        public ConfigurationEntry<bool> WriteDefaultDropTableToFile = new ConfigurationEntry<bool>(false, "When enabled, creates a file on world start, in the plugin folder containing the default mob drop tables.");

        #endregion

        public void Load(ConfigFile configFile)
        {
            Config = configFile;

            EnableTraceLogging.Bind(Config, "Debug", nameof(EnableTraceLogging));

            ClearAllExisting.Bind(Config, "DropTables", "ClearAllExisting");
            ClearAllExistingWhenModified.Bind(Config, "DropTables", "ClearAllExistingWhenModified");
            AlwaysAppend.Bind(Config, "DropTables", "AlwaysAppend");
            ApplyConditionsOnDeath.Bind(Config, "DropTables", nameof(ApplyConditionsOnDeath));

            DebugMode.Bind(Config, "General", "EnableDebug");
            WriteDefaultDropTableToFile.Bind(Config, "Debug", nameof(WriteDefaultDropTableToFile));

            StopTouchingMyConfigs.Bind(Config, "General", nameof(StopTouchingMyConfigs));
            LoadSupplementalDropTables.Bind(Config, "General", nameof(LoadSupplementalDropTables));
        }
    }
}

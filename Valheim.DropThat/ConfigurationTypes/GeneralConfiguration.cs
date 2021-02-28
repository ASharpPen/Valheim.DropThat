using BepInEx.Configuration;

namespace Valheim.DropThat.ConfigurationTypes
{
    public class GeneralConfiguration
    {
        private ConfigFile Config;

        #region General

        public ConfigEntry<bool> DebugMode { get; set; }

        public ConfigEntry<bool> StopTouchingMyConfigs;

        public ConfigEntry<bool> LoadDropTableConfigsOnWorldStart;

        public ConfigEntry<bool> LoadSupplementalDropTables;

        #endregion

        #region DropTables

        public ConfigEntry<bool> ClearAllExisting { get; set; }

        public ConfigEntry<bool> ClearAllExistingWhenModified { get; set; }

        public ConfigEntry<bool> AlwaysAppend { get; set; }

        #endregion

        #region Debug

        public ConfigEntry<bool> EnableTraceLogging;

        #endregion

        public void Load(ConfigFile configFile)
        {
            Config = configFile;

            DebugMode = configFile.Bind<bool>("General", "EnableDebug", false, "Enable debug logging.");

            StopTouchingMyConfigs = configFile.Bind<bool>("General", nameof(StopTouchingMyConfigs), false, "Disables automatic updating and saving of drop table configurations.\nThis means no helpers will be added, but.. allows you to keep things compact.");
            LoadDropTableConfigsOnWorldStart = configFile.Bind<bool>("General", nameof(LoadDropTableConfigsOnWorldStart), true, "Reloads drop table configurations when a game world starts.\nThis means if you are playing solo, you can edit the drop table files while logged out, without exiting the game completely.");
            LoadSupplementalDropTables = configFile.Bind<bool>("General", nameof(LoadSupplementalDropTables), true, "Loads drop table configurations from supplemental files.\nEg. drop_that.supplemental.my_drops.cfg will be included on load.");

            ClearAllExisting = configFile.Bind<bool>("DropTables", "ClearAllExisting", false, "When enabled, all existing items in drop tables gets removed.");
            ClearAllExistingWhenModified = configFile.Bind<bool>("DropTables", "ClearAllExistingWhenModified", false, "When enabled, all existing items in drop tables are removed when a configuration for that entity exist. Eg. if 'Deer' is present in configuration table, the configured drops will be the only drops for 'Deer'.");
            AlwaysAppend = configFile.Bind<bool>("DropTables", "AlwaysAppend", false, "When enabled, drop configurations will not override existing items if their indexes match.");

            EnableTraceLogging = configFile.Bind<bool>("Debug", nameof(EnableTraceLogging), false, "Enables in-depth logging. Note, this might generate a LOT of log entries.");
        }
    }
}

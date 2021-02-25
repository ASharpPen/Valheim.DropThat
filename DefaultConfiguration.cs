using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Valheim.DropThat
{
    public class DefaultConfiguration
    {
        #region General

        public ConfigEntry<bool> DebugMode { get; set; }

        #endregion

        #region DropTables

        public ConfigEntry<bool> ClearAllExisting { get; set; }

        public ConfigEntry<bool> ClearAllExistingWhenModified { get; set; }

        public ConfigEntry<bool> AlwaysAppend { get; set; }

        #endregion

        public void Load(ConfigFile configFile)
        {
            DebugMode = configFile.Bind<bool>("General", "EnableDebug", false, "Enable debug logging.");

            ClearAllExisting = configFile.Bind<bool>("DropTables", "ClearAllExisting", false, "When enabled, all existing items in drop tables gets removed.");

            ClearAllExistingWhenModified = configFile.Bind<bool>("DropTables", "ClearAllExistingWhenModified", false, "When enabled, all existing items in drop tables are removed when a configuration for that entity exist. Eg. if 'Deer' is present in configuration table, the configured drops will be the only drops for 'Deer'.");

            AlwaysAppend = configFile.Bind<bool>("DropTables", "AlwaysAppend", false, "When enabled, drop configurations will not override existing items if their indexes match.");
        }
    }
}

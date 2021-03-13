using System;
using System.Collections.Generic;
using Valheim.DropThat.ConfigurationTypes;

namespace Valheim.DropThat.Multiplayer
{
    [Serializable]
    internal class ConfigurationPackage
    {
        public ConfigurationDto GeneralConfig;

        public ConfigurationDto DropTableConfigs;

        public ConfigurationPackage(){ }

        public ConfigurationPackage(GeneralConfiguration generalConfig, List<DropTableConfiguration> dropTableConfigurations)
        {
            GeneralConfig = new ConfigurationDto
            {
                ConfigName = nameof(ConfigurationManager.GeneralConfig),
                ConfigType = typeof(GeneralConfiguration),
                Configuration = generalConfig
            };

            DropTableConfigs = new ConfigurationDto
            {
                ConfigName = nameof(ConfigurationManager.DropConfigs),
                ConfigType = typeof(List<DropTableConfiguration>),
                Configuration = dropTableConfigurations
            };
        }
    }
}

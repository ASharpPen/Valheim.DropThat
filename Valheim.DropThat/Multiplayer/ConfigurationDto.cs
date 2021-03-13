using System;

namespace Valheim.DropThat.Multiplayer
{
    [Serializable]
    public class ConfigurationDto
    {
        public Type ConfigType;

        public string ConfigName;

        public object Configuration;
    }
}

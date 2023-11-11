using BepInEx.Configuration;
using System;
using System.Runtime.Serialization;

namespace Valheim.DropThat.Core.Configuration
{
    public interface IConfigurationEntry
    {
        void Bind(ConfigFile config, string section, string key);
    }

    [Serializable]
    public class ConfigurationEntry<TIn> : IConfigurationEntry
    {
        public TIn DefaultValue { get; set; }

        [NonSerialized]
        private string Description;

        [NonSerialized]
        private ConfigEntry<TIn> Config;

        [OnSerializing]
        internal void OnSerialize(StreamingContext _)
        {
            // We cheat, and don't actually use the bepinex bindings for syncronized configurations.
            // Due to Config not being set, this should result in DefaultValue always being used instead.
            if(Config != null)
            {
                DefaultValue = Config.Value;
            }
        }

        public void Bind(ConfigFile config, string section, string key)
        {
            if (Description is null)
            {
                Config = config.Bind<TIn>(section, key, DefaultValue);
            }
            else
            {
                Config = config.Bind<TIn>(section, key, DefaultValue, Description);
            }
        }

        public override string ToString()
        {
            if(Config == null)
            {
                return $"[Entry: {DefaultValue}]";
            }
            return $"[{Config.Definition.Key}:{Config.Definition.Section}]: {Config.Value}";
        }

        public TIn Value 
        {
            get
            {
                if(Config is null)
                {
                    return DefaultValue;
                }

                return Config.Value;
            }
        }

        public static implicit operator TIn(ConfigurationEntry<TIn> entry)
        {
            return entry.Value;
        }

        public ConfigurationEntry()
        {

        }

        public ConfigurationEntry(TIn defaultValue, string description = null)
        {
            Description = description;
            DefaultValue = defaultValue;
        }
    }
}

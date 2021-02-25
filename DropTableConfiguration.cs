using BepInEx.Configuration;
using System.Collections.Generic;

namespace Valheim.DropThat
{
    public class DropTableConfiguration
    {
        public string EntityName { get; set; }

        public List<DropConfiguration> Drops { get; set; }
    }

    public class DropConfiguration
    {
        public int Index;

        public ConfigEntry<string> ItemName;

        public ConfigEntry<int> AmountMin;

        public ConfigEntry<int> AmountMax;

        public ConfigEntry<float> Chance;

        public ConfigEntry<bool> OnePerPlayer;

        public ConfigEntry<bool> LevelMultiplier;

        public ConfigEntry<bool> Enabled;

        public bool IsValid()
        {
            if(Enabled == null || !Enabled.Value)
            {
                return false;
            }

            if (Index < 0)
            {
                return false;
            }

            if (ItemName == null || string.IsNullOrEmpty(ItemName.Value))
            {
                return false;
            }

            if (AmountMin == null || AmountMin.Value < 0)
            {
                return false;
            }

            if (AmountMax == null || AmountMax.Value < 0 || AmountMax.Value < AmountMin.Value)
            {
                return false;
            }

            if (Chance == null)
            {
                return false;
            }

            if (OnePerPlayer == null)
            {
                return false;
            }

            if (LevelMultiplier == null)
            {
                return false;
            }

            return true;
        }
    }
}

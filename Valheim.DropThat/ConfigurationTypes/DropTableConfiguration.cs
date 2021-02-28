using System.Collections.Generic;
using System.Linq;
using Valheim.DropThat.ConfigurationCore;

namespace Valheim.DropThat.ConfigurationTypes
{
    public class DropTableConfiguration : ConfigurationGroup<DropConfiguration>
    {
        public ConfigurationEntry<bool> Enabled = new ConfigurationEntry<bool>(true, "Enable/disable configuration for this entity.");

        public List<DropConfiguration> DropConfigurations => Sections.Values.ToList();

        public string EntityName => GroupName.Trim().ToUpperInvariant();
    }

    public class DropConfiguration : ConfigurationSection
    {
        public ConfigurationEntry<string> ItemName = new ConfigurationEntry<string>("", "Prefab name of item to drop.");

        public ConfigurationEntry<bool> Enabled = new ConfigurationEntry<bool>(true, "Enable/disable this specific drop configuration.");

        public ConfigurationEntry<int> AmountMin = new ConfigurationEntry<int>(1, "Minimum amount dropped.");

        public ConfigurationEntry<int> AmountMax = new ConfigurationEntry<int>(1, "Maximum amount dropped.");

        public ConfigurationEntry<float> Chance = new ConfigurationEntry<float>(1f, "Chance to drop. 1 is 100%.\nExample values: 0, 0.5, 1, 1.0");

        public ConfigurationEntry<bool> OnePerPlayer = new ConfigurationEntry<bool>(false);

        public ConfigurationEntry<bool> LevelMultiplier = new ConfigurationEntry<bool>(true, "Toggles level multiplier for dropped amount.");

        // Inefficient, but will do for now.
        public int Index
        {
            get 
            {
                if (int.TryParse(SectionName, out int result))
                {
                    return result;
                }
                return int.MaxValue;
            }
        }

        public bool IsValid()
        {
            if (Enabled == null || !Enabled.Value)
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

            return true;
        }
    }
}

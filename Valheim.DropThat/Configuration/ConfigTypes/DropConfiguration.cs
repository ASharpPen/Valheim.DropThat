using System;
using Valheim.DropThat.Core.Configuration;

namespace Valheim.DropThat.Configuration.ConfigTypes
{
    [Serializable]
    public class DropConfiguration : ConfigWithSubsections<DropMobConfiguration>, IConfigFile
    {
        protected override DropMobConfiguration InstantiateSubsection(string subsectionName)
        {
            return new DropMobConfiguration();
        }
    }

    [Serializable]
    public class DropMobConfiguration : ConfigWithSubsections<DropItemConfiguration>
    {
        protected override DropItemConfiguration InstantiateSubsection(string subsectionName)
        {
            return new DropItemConfiguration();
        }
    }

    [Serializable]
    public class DropItemConfiguration : ConfigWithSubsections<Config>
    {
        protected override Config InstantiateSubsection(string subsectionName)
        {
            Config newModConfig = null;

            if(subsectionName == DropModConfigCLLC.ModName)
            {
                newModConfig = new DropModConfigCLLC();
            }

            return newModConfig;
        }

        #region CharacterDrop.Drop

        public ConfigurationEntry<string> ItemName = new ConfigurationEntry<string>("", "Prefab name of item to drop.");

        public ConfigurationEntry<bool> Enabled = new ConfigurationEntry<bool>(true, "Enable/disable this specific drop configuration.");

        public ConfigurationEntry<int> AmountMin = new ConfigurationEntry<int>(1, "Minimum amount dropped.");

        public ConfigurationEntry<int> AmountMax = new ConfigurationEntry<int>(1, "Maximum amount dropped.");

        public ConfigurationEntry<float> Chance = new ConfigurationEntry<float>(1, "Chance to drop. 1 is 100%.\nExample values: 0, 0.5, 1, 1.0");

        public ConfigurationEntry<bool> OnePerPlayer = new ConfigurationEntry<bool>(false, "If set, will drop one of this item per player. Ignoring other factors.");

        public ConfigurationEntry<bool> LevelMultiplier = new ConfigurationEntry<bool>(true, "Toggles mob levels scaling up dropped amount. Be aware, this scales up very quickly and may cause issues when dropping many items.");

        #endregion

        #region DropExtended Conditions

        public ConfigurationEntry<int> ConditionMinLevel = new ConfigurationEntry<int>(-1, "Minimum level of mob for which item drops.");

        public ConfigurationEntry<int> ConditionMaxLevel = new ConfigurationEntry<int>(-1, "Maximum level of mob for which item drops.");

        public ConfigurationEntry<bool> ConditionNotDay = new ConfigurationEntry<bool>(false, "If true, will not drop during daytime.");

        public ConfigurationEntry<bool> ConditionNotAfternoon = new ConfigurationEntry<bool>(false, "If true, will not drop during afternoon.");

        public ConfigurationEntry<bool> ConditionNotNight = new ConfigurationEntry<bool>(false, "If true, will not drop during night.");

        public ConfigurationEntry<string> ConditionEnvironments = new ConfigurationEntry<string>("", "Array (separated by ,) of environment names that allow the item to drop while they are active.\nEg. Misty, Thunderstorm. Leave empty to always allow.");

        public ConfigurationEntry<string> ConditionGlobalKeys = new ConfigurationEntry<string>("", "Array(separated by,) of global keys names that allow the item to drop while they are active.\nEg. defeated_eikthyr,defeated_gdking.Leave empty to always allow.");

        public ConfigurationEntry<string> ConditionBiomes = new ConfigurationEntry<string>("", "Array(separated by,) of biome names that allow the item to drop while they are active.\nEg. Meadows, Swamp. Leave empty to always allow.");

        public ConfigurationEntry<string> ConditionCreatureStates = new ConfigurationEntry<string>("", "Array (separated by,) of creature states for which the item drop. If empty, allows all.\nEg. Default,Tamed,Event");

        public ConfigurationEntry<string> ConditionNotCreatureStates = new ConfigurationEntry<string>("", "Array (separated by,) of creature states for which the item will not drop.\nEg. Default,Tamed,Event");

        public ConfigurationEntry<string> ConditionHasItem = new ConfigurationEntry<string>("", "Array of items (prefab names) that will enable this drop. If empty, allows all.\nEg. skeleton_bow");

        #endregion

        // Inefficient, but will do for now.
        public int Index
        {
            get
            {
                if (int.TryParse(SectionName, out int result))
                {
                    if (result < 0)
                    {
                        return int.MaxValue;
                    }

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

    [Serializable]
    public class DropModConfigCLLC : Config
    {
        public const string ModName = "CLLC";
    }
}

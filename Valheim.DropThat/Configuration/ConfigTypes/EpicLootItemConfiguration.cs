using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DropThat.Core.Configuration;

namespace DropThat.Configuration.ConfigTypes;

[Serializable]
public class EpicLootItemConfiguration : Config
{
    public const string ModName = "EpicLoot";

    public ConfigurationEntry<float> RarityWeightNone = new(0, "Weight to use for rolling as a non-magic item.");
    public ConfigurationEntry<float> RarityWeightMagic = new(0, "Weight to use for rolling as rarity 'Magic'");
    public ConfigurationEntry<float> RarityWeightRare = new(0, "Weight to use for rolling as rarity 'Rare'");
    public ConfigurationEntry<float> RarityWeightEpic = new(0, "Weight to use for rolling as rarity 'Epic'");
    public ConfigurationEntry<float> RarityWeightLegendary = new(0, "Weight to use for rolling as rarity 'Legendary'");
    public ConfigurationEntry<float> RarityWeightUnique = new(0, "Weight to use for rolling unique items from the UniqueIDs array. If item rolls as unique, a single id will be selected randomly from the UniqueIDs.");
    public ConfigurationEntry<string> UniqueIDs = new("", "Id's for unique legendaries from Epic Loot. Will drop as a non-magic item if the legendary does not meet its requirements.\nEg. HeimdallLegs, RagnarLegs");
}

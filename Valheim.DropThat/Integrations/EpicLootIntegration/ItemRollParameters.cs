using System.Collections.Generic;

namespace DropThat.Integrations.EpicLootIntegration;

internal class ItemRollParameters
{
    public float RarityWeightNone { get; set; }
    public float RarityWeightMagic { get; set; }
    public float RarityWeightRare { get; set; }
    public float RarityWeightEpic { get; set; }
    public float RarityWeightLegendary { get; set; }
    public float RarityWeightUnique { get; set; }
    public List<string> UniqueIds { get; set; }
}

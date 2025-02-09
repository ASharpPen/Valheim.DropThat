using DropThat.Creature;
using DropThat.Drop.CharacterDropSystem.Managers;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Utilities.Valheim;

namespace DropThat.Drop.CharacterDropSystem.Conditions;

public sealed class ConditionNotBiome : IDropCondition
{
    public Heightmap.Biome BiomeBitmask { get; set; }

    static ConditionNotBiome()
    {
        CharacterDropEventManager.OnDropTableInitializeSet.Add(RecordSpawnInfoService.SetSpawnBiomeIfMissing);
    }

    public bool IsPointless() => BiomeBitmask == Heightmap.Biome.None;

    public bool IsValid(DropContext context)
    {
        if (BiomeBitmask == Heightmap.Biome.None)
        {
            return true;
        }

        var spawnBiome = context.ZDO?.GetSpawnBiome();

        if (spawnBiome is null)
        {
            return false;
        }

        return (spawnBiome.Value & BiomeBitmask) == 0;
    }
}

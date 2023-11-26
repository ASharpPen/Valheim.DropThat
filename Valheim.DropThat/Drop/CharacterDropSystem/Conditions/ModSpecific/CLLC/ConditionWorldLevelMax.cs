using CreatureLevelControl;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Integrations;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;

public sealed class ConditionWorldLevelMax : IDropCondition
{
    public int? MaxLevel { get; set; }

    public bool IsPointless() => 
        MaxLevel is null || 
        MaxLevel == 0 ||
        !InstallationManager.CLLCInstalled;

    public bool IsValid(DropContext context)
    {
        if (MaxLevel is null ||
            !InstallationManager.CLLCInstalled)
        {
            return true;
        }

        return API.GetWorldLevel() <= MaxLevel;
    }
}
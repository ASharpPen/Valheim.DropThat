using CreatureLevelControl;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Integrations;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;

public sealed class ConditionWorldLevelMin : IDropCondition
{
    public int? MinLevel { get; set; }

    public bool IsPointless() => 
        MinLevel is null || 
        MinLevel == 0 ||
        !InstallationManager.CLLCInstalled;

    public bool IsValid(DropContext context)
    {
        if (MinLevel is null ||
            !InstallationManager.CLLCInstalled)
        {
            return true;
        }

        return API.GetWorldLevel() >= MinLevel;
    }
}
using System;
using DropThat.Core;
using DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific;

internal static class ConditionLoaderCLLC
{
    public static bool InstalledCLLC { get; } = Type.GetType("CreatureLevelControl.API, CreatureLevelControl") is not null;

    public static ConditionBossAffix ConditionBossAffix
    {
        get
        {
            if (InstalledCLLC) return ConditionBossAffix.Instance;

#if DEBUG
            if (!InstalledCLLC) Log.LogDebug("CLLC not found.");
#endif

            return null;
        }
    }

    public static ConditionInfusion ConditionInfusion
    {
        get
        {
            if (InstalledCLLC) return ConditionInfusion.Instance;

#if DEBUG
            if (!InstalledCLLC) Log.LogDebug("CLLC not found.");
#endif

            return null;
        }
    }

    public static ConditionCreatureExtraEffect ConditionCreatureExtraEffect
    {
        get
        {
            if (InstalledCLLC) return ConditionCreatureExtraEffect.Instance;
            return null;
        }
    }

    public static ConditionWorldLevel ConditionWorldLevel => (InstalledCLLC) ? ConditionWorldLevel.Instance : null;
}

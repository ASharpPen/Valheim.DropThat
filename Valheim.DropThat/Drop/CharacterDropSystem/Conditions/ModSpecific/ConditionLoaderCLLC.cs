using System;
using DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;
using DropThat.Integrations;
using ThatCore.Logging;

namespace DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific;

internal static class ConditionLoaderCLLC
{
    public static ConditionBossAffix ConditionBossAffix
    {
        get
        {
            if (InstallationManager.CLLCInstalled) return ConditionBossAffix.Instance;

#if DEBUG
            if (!InstallationManager.CLLCInstalled) Log.LogDebug("CLLC not found.");
#endif

            return null;
        }
    }

    public static ConditionInfusion ConditionInfusion
    {
        get
        {
            if (InstallationManager.CLLCInstalled) return ConditionInfusion.Instance;

#if DEBUG
            if (!InstallationManager.CLLCInstalled) Log.LogDebug("CLLC not found.");
#endif

            return null;
        }
    }

    public static ConditionCreatureExtraEffect ConditionCreatureExtraEffect
    {
        get
        {
            if (InstallationManager.CLLCInstalled) return ConditionCreatureExtraEffect.Instance;
            return null;
        }
    }

    public static ConditionWorldLevel ConditionWorldLevel => (InstallationManager.CLLCInstalled) ? ConditionWorldLevel.Instance : null;
}

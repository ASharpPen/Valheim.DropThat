using HarmonyLib;
using System;
using Valheim.DropThat.Conditions.ModSpecific.CLLC;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Conditions.ModSpecific
{
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
    }
}

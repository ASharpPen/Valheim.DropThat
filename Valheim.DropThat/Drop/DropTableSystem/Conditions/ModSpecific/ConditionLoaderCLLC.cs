using System;
using Valheim.DropThat.Drop.DropTableSystem.Conditions.ModSpecific.ModCLLC;

namespace Valheim.DropThat.Drop.DropTableSystem.Conditions.ModSpecific
{
    internal static class ConditionLoaderCLLC
    {
        public static bool InstalledCLLC { get; } = Type.GetType("CreatureLevelControl.API, CreatureLevelControl") is not null;

        public static ConditionWorldLevel ConditionWorldLevel => InstalledCLLC ? ConditionWorldLevel.Instance : null;
    }
}

using System;
using DropThat.Drop.DropTableSystem.Conditions.ModSpecific.ModCLLC;

namespace DropThat.Drop.DropTableSystem.Conditions.ModSpecific;

internal static class ConditionLoaderCLLC
{
    public static bool InstalledCLLC { get; } = Type.GetType("CreatureLevelControl.API, CreatureLevelControl") is not null;

    public static OldConditionWorldLevel ConditionWorldLevel => InstalledCLLC ? OldConditionWorldLevel.Instance : null;
}

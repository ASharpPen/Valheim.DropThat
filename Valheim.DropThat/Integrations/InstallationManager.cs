using System;

namespace DropThat.Integrations;

public static class InstallationManager
{
    public static bool EpicLootInstalled { get; } = Type.GetType("EpicLoot.EpicLoot, EpicLoot") is not null;

    public static bool RRRInstalled { get; } = Type.GetType("RRRCore.Plugin, RRRCore") is not null;

#if TEST
    public static bool SpawnThatInstalled { get; } = true;
#else
    public static bool SpawnThatInstalled { get; } = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("asharppen.valheim.spawn_that");
#endif

    public static bool CLLCInstalled { get; } = Type.GetType("CreatureLevelControl.API, CreatureLevelControl") is not null;
}

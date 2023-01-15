using System;

namespace DropThat.Integrations;

public static class InstallationManager
{
    private static bool? _epicLootInstalled = null;
    private static bool? _rrrInstalled = null;

    public static bool EpicLootInstalled => _epicLootInstalled ??= Type.GetType("EpicLoot.EpicLoot, EpicLoot") is not null;

    public static bool RRRInstalled => _rrrInstalled ??= Type.GetType("RRRCore.Plugin, RRRCore") is not null;

    public static bool SpawnThatInstalled { get; } = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("asharppen.valheim.spawn_that");

    public static bool CLLCInstalled { get; } = Type.GetType("CreatureLevelControl.API, CreatureLevelControl") is not null;
}

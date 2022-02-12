using System;

namespace Valheim.DropThat.Integrations
{
    public static class InstallationManager
    {
        private static bool? _epicLootInstalled = null;
        private static bool? _rrrInstalled = null;

        public static bool EpicLootInstalled => _epicLootInstalled ??= Type.GetType("EpicLoot.EpicLoot, EpicLoot") is not null;

        public static bool RRRInstalled => _rrrInstalled ??= Type.GetType("RRRCore.Plugin, RRRCore") is not null;
    }
}

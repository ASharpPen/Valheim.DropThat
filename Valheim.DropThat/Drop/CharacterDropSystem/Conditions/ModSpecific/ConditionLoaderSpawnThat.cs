using System;
using Valheim.DropThat.Core;
using Valheim.DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.SpawnThat;

namespace Valheim.DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific
{
    internal static class ConditionLoaderSpawnThat
    {
        public static bool InstalledSpawnThat { get; } = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("asharppen.valheim.spawn_that");

        public static ConditionTemplateId ConditionTemplateId
        {
            get
            {
                if (InstalledSpawnThat) return ConditionTemplateId.Instance;

#if DEBUG
                Log.LogDebug("SpawnThat not installed.");
#endif

                return null;
            }
        }

    }
}

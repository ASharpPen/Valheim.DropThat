using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valheim.DropThat.Core;
using Valheim.DropThat.Drop.Conditions.ModSpecific.SpawnThat;

namespace Valheim.DropThat.Drop.Conditions.ModSpecific
{
    internal static class ConditionLoaderSpawnThat
    {
        public static bool InstalledSpawnThat { get; } = Type.GetType("Valheim.SpawnThat.SpawnThatPlugin, Valheim.SpawnThat") is not null;

        public static ConditionTemplateId ConditionTemplateId
        {
            get
            {
                if(InstalledSpawnThat) return ConditionTemplateId.Instance;

#if DEBUG
                Log.LogDebug("SpawnThat not installed.");
#endif

                return null;
            }
        }

    }
}

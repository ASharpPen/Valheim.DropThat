using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valheim.DropThat.Integrations
{
    public static class InstallationManager
    {
        public static bool EpicLootInstalled = Type.GetType("EpicLoot.EpicLoot, EpicLoot") is not null;
    }
}

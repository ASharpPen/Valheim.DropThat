﻿using System;
using Valheim.DropThat.Core;
using Valheim.DropThat.Drop.CharacterDropSystem.Modifiers.ModSpecific.ModEpicLoot;

namespace Valheim.DropThat.Drop.CharacterDropSystem.Modifiers.ModSpecific
{
    internal static class ModifierLoaderEpicLoot
    {
        public static bool InstalledEpicLoot { get; } = Type.GetType("EpicLoot.EpicLoot, EpicLoot") is not null;

        public static ModifierMagicItem MagicItem
        {
            get
            {
                if (InstalledEpicLoot) return ModifierMagicItem.Instance;

#if DEBUG
                if (!InstalledEpicLoot) Log.LogDebug("Epic Loot found.");
#endif
                return null;
            }
        }
    }
}

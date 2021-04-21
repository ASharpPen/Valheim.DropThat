using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Core;
using Valheim.DropThat.Drop.Modifiers;
using Valheim.DropThat.Drop.Modifiers.ModSpecific;
using Valheim.DropThat.Reset;

namespace Valheim.DropThat.Drop
{
    public class DropModificationManager
    {
        private HashSet<IDropModifier> DropModifiers = new HashSet<IDropModifier>();

        private static DropModificationManager _instance;

        public static DropModificationManager Instance
        {
            get
            {
                return _instance ??= new DropModificationManager();
            }
        }

        DropModificationManager()
        {
            StateResetter.Subscribe(() =>
            {
                _instance = null;
            });

            DropModifiers.Add(ModifierLoaderEpicLoot.MagicItem);
        }

        public void ApplyModifications(GameObject item, DropExtended extended)
        {
            if(item is null || extended is null)
            {
                return;
            }

#if DEBUG
            Log.LogDebug($"Applying modifiers to item {item.name}");
#endif

            foreach (var modifier in DropModifiers)
            {
                modifier?.Modify(item, extended);
            }
        }
    }
}

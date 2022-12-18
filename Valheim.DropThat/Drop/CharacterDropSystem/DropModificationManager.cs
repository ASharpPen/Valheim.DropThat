using System;
using System.Collections.Generic;
using UnityEngine;
using DropThat.Core;
using DropThat.Drop.CharacterDropSystem.Caches;
using DropThat.Drop.CharacterDropSystem.Modifiers;
using DropThat.Drop.CharacterDropSystem.Modifiers.ModSpecific;
using DropThat.Reset;
using DropThat.Utilities;

namespace DropThat.Drop.CharacterDropSystem
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

            DropModifiers.AddNullSafe(ModifierSetQualityLevel.Instance);
            DropModifiers.AddNullSafe(ModifierLoaderEpicLoot.MagicItem);

            // Should run AFTER epic loot modifier
            DropModifiers.AddNullSafe(ModifierSetDurability.Instance);
        }

        public void ApplyModifications(GameObject item, DropExtended extended, Vector3 pos)
        {
            if (item is null || extended is null)
            {
                return;
            }

#if DEBUG
            Log.LogDebug($"Applying modifiers to item {item.name}");
#endif

            var context = new DropContext
            {
                Item = item,
                Extended = extended,
                Pos = pos
            };

            foreach (var modifier in DropModifiers)
            {
                try
                {
                    modifier?.Modify(context);
                }
                catch (Exception e)
                {
                    Log.LogError($"Error while attempting to modify item drop {item.name}", e);
                }
            }
        }
    }
}

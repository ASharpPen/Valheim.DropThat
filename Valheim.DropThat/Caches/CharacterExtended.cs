﻿using System.Runtime.CompilerServices;

namespace Valheim.DropThat.Caches
{
    public class CharacterExtended
    {
        public bool? HasMonsterAI { get; set; }

        public MonsterAI MonsterAI { get; set; }

        public bool? HasInventory { get; set; }

        public Inventory Inventory { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DropThat.Drop.CharacterDropSystem.Caches;

namespace DropThat.Drop.CharacterDropSystem
{
    public class DropContext
    {
        public GameObject Item { get; set; }

        public DropExtended Extended { get; set; }

        public Vector3 Pos { get; set; }
    }
}

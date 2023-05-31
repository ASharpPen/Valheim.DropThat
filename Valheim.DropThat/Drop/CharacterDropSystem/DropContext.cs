using UnityEngine;
using Valheim.DropThat.Drop.CharacterDropSystem.Caches;

namespace Valheim.DropThat.Drop.CharacterDropSystem
{
    public class DropContext
    {
        public GameObject Item { get; set; }

        public DropExtended Extended { get; set; }

        public Vector3 Pos { get; set; }
    }
}

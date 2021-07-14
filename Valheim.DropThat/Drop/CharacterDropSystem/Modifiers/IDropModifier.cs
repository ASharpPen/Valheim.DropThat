using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Drop.CharacterDropSystem;

namespace Valheim.DropThat.Drop.CharacterDropSystem.Modifiers
{
    public interface IDropModifier
    {
        void Modify(DropContext context);
    }
}

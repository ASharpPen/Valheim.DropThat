using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Valheim.DropThat.Drop.DropTableSystem.Modifiers
{
    public interface IDropTableModifier
    {
        void Modify(DropModificationContext context);

        void Modify(ref ItemDrop.ItemData drop, DropTemplate template, Vector3 position);
    }
}

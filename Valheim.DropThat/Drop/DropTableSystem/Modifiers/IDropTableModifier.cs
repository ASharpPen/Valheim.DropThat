using UnityEngine;

namespace Valheim.DropThat.Drop.DropTableSystem.Modifiers
{
    public interface IDropTableModifier
    {
        void Modify(DropModificationContext context);

        void Modify(ref ItemDrop.ItemData drop, DropTemplate template, Vector3 position);
    }
}

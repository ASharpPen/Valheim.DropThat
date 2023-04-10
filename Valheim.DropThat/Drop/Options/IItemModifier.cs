using UnityEngine;

namespace DropThat.Drop.Options;

public interface IItemModifier
{
    void Modify(ItemModifierContext<GameObject> drop);

    void Modify(ItemModifierContext<ItemDrop.ItemData> drop);
}

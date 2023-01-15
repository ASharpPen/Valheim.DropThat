using UnityEngine;

namespace DropThat.Drop.Options;

public interface IItemModifier
{
    void Modify(GameObject drop);
}

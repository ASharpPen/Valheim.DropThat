using UnityEngine;

namespace DropThat.Drop.Options;

public sealed class ItemModifierContext<T>
    where T : class
{
    public T Item { get; set; }

    public Vector3 Position { get; set; }
}
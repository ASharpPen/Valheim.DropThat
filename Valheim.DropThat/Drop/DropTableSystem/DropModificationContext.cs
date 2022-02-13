using UnityEngine;

namespace Valheim.DropThat.Drop.DropTableSystem
{
    public class DropModificationContext
    {
        public GameObject Drop { get; }

        public DropTemplate Template { get; }

        public DropModificationContext(GameObject drop, DropTemplate template)
        {
            Drop = drop;
            Template = template;

            ItemDrop = new(drop);
        }

        public CachedComponent<ItemDrop> ItemDrop { get; }

        public class CachedComponent<T> where T : Component
        {
            private T Component;

            private GameObject Source;

            private bool HasChecked;

            public CachedComponent(GameObject gameObject)
            {
                Source = gameObject;
            }

            public static implicit operator T(CachedComponent<T> cache)
            {
                if (!cache.HasChecked)
                {
                    cache.Component = cache.Source.GetComponent<T>();
                    cache.HasChecked = true;
                }

                return cache.Component;
            }
        }
    }
}

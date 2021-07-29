using System.Runtime.CompilerServices;
using UnityEngine;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Drop.DropTableSystem.Wrapper
{
    public static class WrapperGameObjectExtensions
    {
        public const string WrapperName = "SpawnThat_Wrapper";

        private static ManagedCache<GameObject> WrapperTable { get; } = new();

        public static GameObject Wrap(this GameObject gameObject)
        {
            if (WrapperTable.TryGet(gameObject, out GameObject cached))
            {
                return cached;
            }

            GameObject wrapper = new GameObject(WrapperName + ";" + gameObject.name.GetStableHashCode());
            wrapper.AddComponent<WrapperComponent>();

            WrapperTable.Set(wrapper, gameObject);

            return wrapper;
        }

        public static GameObject Unwrap(this GameObject gameObject)
        {
            if (gameObject.name.StartsWith(WrapperName))
            {
                if (WrapperTable.TryGet(gameObject, out GameObject wrapped))
                {
                    return wrapped;
                }
            }

            return gameObject;
        }
    }
}

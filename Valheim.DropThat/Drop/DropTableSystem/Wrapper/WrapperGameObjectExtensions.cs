using System.Runtime.CompilerServices;
using UnityEngine;

namespace Valheim.DropThat.Drop.DropTableSystem.Wrapper
{
    public static class WrapperGameObjectExtensions
    {
        public const string WrapperName = "SpawnThat_Wrapper";

        private static ConditionalWeakTable<GameObject, GameObject> WrapperTable = new();

        public static GameObject Wrap(this GameObject gameObject)
        {
            if (WrapperTable.TryGetValue(gameObject, out GameObject cached))
            {
                return cached;
            }

            GameObject wrapper = new GameObject(WrapperName + ";" + gameObject.name.GetStableHashCode());
            wrapper.AddComponent<WrapperComponent>();

            WrapperTable.Add(wrapper, gameObject);

            return wrapper;
        }

        public static GameObject Unwrap(this GameObject gameObject)
        {
            if (gameObject.name.StartsWith(WrapperName))
            {
                if (WrapperTable.TryGetValue(gameObject, out GameObject wrapped))
                {
                    return wrapped;
                }
            }

            return gameObject;
        }
    }
}

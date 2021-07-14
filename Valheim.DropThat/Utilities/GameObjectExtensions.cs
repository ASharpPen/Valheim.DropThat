using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Valheim.DropThat.Utilities
{
    public static class GameObjectExtensions
    {
        public const string WrapperName = "SpawnThat_Wrapper";

        private static ConditionalWeakTable<GameObject, GameObject> WrapperTable = new();

        public static GameObject Wrap(this GameObject gameObject)
        {
            if(WrapperTable.TryGetValue(gameObject, out GameObject cached))
            {
                return cached;
            }

            GameObject wrapper = new GameObject(WrapperName);

            WrapperTable.Add(wrapper, gameObject);

            return wrapper;
        }

        public static GameObject Unwrap(this GameObject gameObject)
        {
            if (gameObject.name == WrapperName)
            {
                if(WrapperTable.TryGetValue(gameObject, out GameObject wrapped))
                {
                    return wrapped;
                }
            }

            return gameObject;
        }

        public static string GetCleanedName(this GameObject gameObject)
        {
            string cleanedName = gameObject.name
                .Split(new char[] { '(' }, System.StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault()?
                .Trim();

            return string.IsNullOrWhiteSpace(cleanedName)
                ? gameObject.name
                : cleanedName;
        }
    }
}

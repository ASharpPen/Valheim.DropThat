using System.Linq;
using UnityEngine;

namespace Valheim.DropThat.Utilities
{
    public static class GameObjectExtensions
    {
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

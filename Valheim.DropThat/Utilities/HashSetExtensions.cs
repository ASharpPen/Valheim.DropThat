using System.Collections.Generic;

namespace Valheim.DropThat.Utilities;

internal static class HashSetExtensions
{
    /// <summary>
    /// Adds candidate to hashset, if hashset and candidate is not null.
    /// </summary>
    public static void AddNullSafe<T>(this HashSet<T> set, T candidate)
        where T : class
    {
        if (set is null)
        {
            return;
        }

        if (candidate is null)
        {
            return;
        }

        set.Add(candidate);
    }
}

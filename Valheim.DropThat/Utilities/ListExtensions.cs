using System.Collections.Generic;

namespace DropThat.Utilities;

internal static class ListExtensions
{
    /// <summary>
    /// Adds candidate to list, if list and candidate is not null.
    /// </summary>
    public static void AddNullSafe<T>(this List<T> list, T candidate)
        where T : class
    {
        if (list is null)
        {
            return;
        }

        if (candidate is null)
        {
            return;
        }

        list.Add(candidate);
    }
}

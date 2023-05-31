using System;
using System.Collections.Generic;
using System.Linq;

namespace Valheim.DropThat.Utilities;

public static class StringExtensions
{
    private static char[] Comma = new[] { ',' };

    public static List<string> SplitBy(this string value, char splitChar, bool toUpper = false)
    {
        var split = value.Split(new[] { splitChar }, StringSplitOptions.RemoveEmptyEntries);

        if ((split?.Length ?? 0) == 0)
        {
            return new List<string>();
        }

        return split.Select(x => Clean(x, toUpper)).ToList();
    }

    public static List<string> SplitByComma(this string value, bool toUpper = false)
    {
        var split = value.Split(Comma, StringSplitOptions.RemoveEmptyEntries);

        if ((split?.Length ?? 0) == 0)
        {
            return new List<string>();
        }

        return split.Select(x => Clean(x, toUpper)).ToList();
    }

    private static string Clean(string x, bool toUpper)
    {
        var result = x.Trim();
        if (toUpper)
        {
            return result.ToUpperInvariant();
        }
        return result;
    }

    public static bool TryConvertToEnum<T>(this IEnumerable<string> strings, out List<T> enums)
        where T : struct
    {
        enums = new List<T>();

        foreach (var type in strings)
        {
            if (Enum.TryParse<T>(type, true, out T entity))
            {
                enums.Add(entity);
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}

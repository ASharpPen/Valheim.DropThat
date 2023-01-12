using System.Linq;
using System.Text.RegularExpressions;

namespace Valheim.DropThat.Utilities;

public static class UnityObjectExtensions
{
    private static Regex NameRegex = new Regex(@"^[^$(]*(?=$|[(])", RegexOptions.Compiled);

    public static bool IsNull(this UnityEngine.Object obj)
    {
        if (obj == null || !obj)
        {
            return true;
        }

        return false;
    }

    public static bool IsNotNull(this UnityEngine.Object obj)
    {
        if (obj != null && obj)
        {
            return true;
        }

        return false;
    }

    public static string GetCleanedName(this UnityEngine.Object obj, bool toUpper = false)
    {
        if (obj.IsNull())
        {
            return null;
        }

        string cleanedName = obj.name
            .Split(new char[] { '(' }, System.StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault()?
            .Trim();

        return string.IsNullOrWhiteSpace(cleanedName)
            ? obj.name
            : cleanedName;

        // TODO: Figure out why/if the below solution causes problems.
        /*
        if (obj.IsNull())
        {
            return null;
        }

        var match = NameRegex.Match(obj.name);

        if (!match.Success)
        {
            return null;
        }

        string cleanedName = match
            .Value
            .Trim();

        if (toUpper)
        {
            cleanedName = cleanedName.ToUpperInvariant();
        }

        return cleanedName;
        */
    }
}

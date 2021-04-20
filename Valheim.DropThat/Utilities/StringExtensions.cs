using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valheim.DropThat.Utilities
{
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
    }
}

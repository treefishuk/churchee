using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace System
{
    public static partial class StringExtensions
    {
        public static string ToTitleCase(this string str)
        {
            var cultureInfo = Thread.CurrentThread.CurrentCulture;

            var textInfo = cultureInfo.TextInfo;

            return textInfo.ToTitleCase(str);
        }

        public static string ToSentence(this string variableName)
        {
            char[] chars = variableName.ToCharArray();

            return new string(chars.SelectMany((c, i) => i != 0 && char.IsUpper(c) && !char.IsUpper(chars[i - 1]) ? [' ', c] : new char[] { c }).ToArray());
        }

        public static string ToCamelCase(this string str)
        {
            if (str == null)
            {
                return null;
            }

            if (str.Contains(' '))
            {
                str = str.ToPascalCase();
            }

            if (str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str[1..];
            }

            return str;
        }

        public static string ToPascalCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            var info = Thread.CurrentThread.CurrentCulture.TextInfo;

            str = info.ToTitleCase(str);

            string[] parts = str.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);

            return string.Concat(parts);
        }

        public static string ToDevName(this string text)
        {
            return ToCamelCase(text);
        }

        [GeneratedRegex(@"[^a-zA-Z0-9\-]", RegexOptions.None, matchTimeoutMilliseconds: 2000)]
        private static partial Regex InvalidUrlCharactersRegex();

        public static string ToURL(this string text)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(text);

            string ampersandReplaced = Regex.Replace(text, "&", "and", RegexOptions.None, TimeSpan.FromSeconds(2));

            string dashed = Regex.Replace(ampersandReplaced, @"\s+", "-", RegexOptions.None, TimeSpan.FromSeconds(2));

            string result = InvalidUrlCharactersRegex().Replace(dashed, string.Empty);

            result = Regex.Replace(result, "-{2,}", "-", RegexOptions.None, TimeSpan.FromSeconds(2));

            return HttpUtility.UrlEncode(result.ToLowerInvariant());
        }

        public static string AddSuffix(this string text)
        {
            if (int.TryParse(text, out int day))
            {
                return day switch
                {
                    1 or 21 or 31 => text + "st",
                    2 or 22 => text + "nd",
                    3 or 23 => text + "rd",
                    _ => text + "th",
                };
            }

            return string.Empty;
        }
    }
}

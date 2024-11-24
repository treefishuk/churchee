using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace System
{
    public static partial class StringExtentions
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
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
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

        [GeneratedRegex(@"[^\p{L}0-9 -\.]", RegexOptions.None, matchTimeoutMilliseconds: 2000)]
        private static partial Regex InvalidUrlCharactersRegex();

        public static string ToURL(this string text)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(text);

            string dashed = Regex.Replace(text, @"\s+", "-", RegexOptions.None, TimeSpan.FromSeconds(2));

            string result = InvalidUrlCharactersRegex().Replace(dashed, string.Empty);

            return HttpUtility.UrlEncode(result.ToLowerInvariant());
        }

        public static string AddSuffix(this string text)
        {
            if (int.TryParse(text, out int day))
            {
                switch (day)
                {
                    case 1:
                    case 21:
                    case 31:
                        return text + "st";
                    case 2:
                    case 22:
                        return text + "nd";
                    case 3:
                    case 23:
                        return text + "rd";
                    default:
                        return text + "th";
                }
            }

            return string.Empty;
        }
    }
}

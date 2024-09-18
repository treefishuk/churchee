using System.Text.RegularExpressions;

namespace Churchee.Module.Site.Features.Pages.Validation
{
    internal static class PageValidation
    {

        internal static bool DoesNotContainScriptTags(List<KeyValuePair<Guid, string>> content)
        {
            foreach (var item in content)
            {

                if (ContainsScriptTags(item.Value))
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool DoesNotContainEmbedTags(List<KeyValuePair<Guid, string>> content)
        {
            foreach (var item in content)
            {
                if (ContainsNonYouTubeEmbeds(item.Value))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ContainsScriptTags(string content)
        {
            bool containsStyleTags = Regex.IsMatch(content, @"<script>", RegexOptions.IgnoreCase);

            if (containsStyleTags)
            {
                return true;
            }

            return false;
        }


        internal static bool ContainsNonYouTubeEmbeds(string content)
        {
            var embedPattern = @"<(iframe|object|embed)[^>]*>";
            var matches = Regex.Matches(content, embedPattern, RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                var tag = match.Value;
                if (tag.Contains("youtube-nocookie.com/embed"))
                {
                    continue;
                }
                return true;
            }

            return false;
        }



    }
}

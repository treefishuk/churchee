using System.Text.RegularExpressions;

namespace Churchee.Module.Site.Features.Pages.Validation
{
    internal static class PageValidation
    {

        internal static bool DoesNotContainScriptTags(List<KeyValuePair<Guid, string>> content)
        {
            return !content.Any(item => ContainsScriptTags(item.Value));
        }

        internal static bool DoesNotContainEmbedTags(List<KeyValuePair<Guid, string>> content)
        {
            return !content.Any(item => ContainsNonYouTubeEmbeds(item.Value));
        }

        private static bool ContainsScriptTags(string content)
        {
            return Regex.IsMatch(content, @"<script>", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(2));
        }

        internal static bool ContainsNonYouTubeEmbeds(string content)
        {
            string embedPattern = @"<(iframe|object|embed)[^>]*>";

            var matches = Regex.Matches(content, embedPattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(2));

            foreach (string tag in matches.Select(s => s.Value))
            {
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

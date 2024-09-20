using System.Text.RegularExpressions;

namespace Churchee.Module.Site.Features.Templates.Validation
{
    internal static class TemplateValidation
    {
        internal static bool NotContainsDisallowedContent(string viewContent)
        {
            var disallowedPatterns = new List<string>
            {
                "DbContext",
                "SiteDBContext",
                "Microsoft.Data.SqlClient",
                "System.Data.SqlClient",
                "Npgsql",
                "MySql.Data.MySqlClient",
                "this.Context",
                @"@functions",
                @"@Html.Raw",
                @"@code",
                @"@ViewBag",
                @"@ViewData",
                @"@TempData",
                @"@Configuration",
                @"@Environment",
            };

            // Check for disallowed patterns
            foreach (var pattern in disallowedPatterns)
            {
                if (Regex.IsMatch(viewContent, pattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(2)))
                {
                    return false;
                }
            }

            return true;
        }


        internal static bool NotContainsDisallowedInjects(string viewContent)
        {
            var allowedInjectServices = new List<string>
            {
                @"IMediaItemService",
                @"IQueryParamsService"
            };

            // Check for disallowed @inject statements
            var injectPattern = @"@inject\s+(\w+)\s+(\w+)";
            var injectMatches = Regex.Matches(viewContent, injectPattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(2));
            foreach (Match match in injectMatches)
            {
                var serviceType = match.Groups[1].Value;
                if (!allowedInjectServices.Contains(serviceType))
                {
                    return false;
                }
            }

            return true;
        }


        internal static bool NotContainStyleTags(string content)
        {
            bool containsStyleTags = Regex.IsMatch(content, @"<style>", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(2));

            if (containsStyleTags)
            {
                return false;
            }

            return true;
        }

        internal static bool NotContainsDisallowedEmbeds(string content)
        {
            var embedPattern = @"<(iframe|object|embed)[^>]*>";
            var matches = Regex.Matches(content, embedPattern, RegexOptions.IgnoreCase, TimeSpan.FromSeconds(2));

            foreach (Match match in matches)
            {
                var tag = match.Value;
                if (tag.Contains("youtube-nocookie.com/embed"))
                {
                    continue; // Allow YouTube embeds
                }
                return false; // Disallow other embeds
            }

            return true;
        }

    }


}

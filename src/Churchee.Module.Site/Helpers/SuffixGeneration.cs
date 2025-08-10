using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Helpers
{
    public static class SuffixGeneration
    {
        public static void AddUniqueSuffixIfNeeded(WebContent webContent, IDataStore dataStore)
        {
            var repo = dataStore.GetRepository<WebContent>();

            // Ensure unique URL by adding/incrementing a numeric suffix if needed
            string baseUrl = webContent.Url;
            string uniqueUrl = baseUrl;
            int suffix = 2;

            while (repo.GetQueryable().Any(a => a.Url == uniqueUrl))
            {
                // If baseUrl already ends with -number, increment it
                int lastDash = baseUrl.LastIndexOf('-');
                if (lastDash > 0 && int.TryParse(baseUrl[(lastDash + 1)..], out int existingNumber))
                {
                    baseUrl = baseUrl[..lastDash];
                    suffix = existingNumber + 1;
                }
                uniqueUrl = $"{baseUrl}-{suffix}";
                suffix++;
            }

            webContent.Url = uniqueUrl;
        }

    }
}

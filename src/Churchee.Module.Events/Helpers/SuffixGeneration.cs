using Churchee.Common.Storage;
using Churchee.Module.Events.Entities;

namespace Churchee.Module.Events.Helpers
{
    public static class SuffixGeneration
    {
        public static void AddUniqueSuffixIfNeeded(Event newEvent, IDataStore dataStore)
        {
            var repo = dataStore.GetRepository<Event>();

            // Ensure unique URL by adding/incrementing a numeric suffix if needed
            string baseUrl = newEvent.Url;
            string uniqueUrl = baseUrl;
            int suffix = 1;

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

            newEvent.Url = uniqueUrl;
        }

    }
}

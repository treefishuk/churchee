namespace Churchee.Common.Validation
{
    public static class UrlValidation
    {
        public static bool IsSafeRelativeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            // Must start with /
            if (!url.StartsWith('/'))
            {
                return false;
            }

            // Must not start with //
            return !url.StartsWith("//");
        }

    }
}

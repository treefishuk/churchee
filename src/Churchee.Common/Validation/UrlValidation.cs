using System;

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

            // Must start with a single /
            if (!url.StartsWith('/'))
            {
                return false;
            }

            // Reject protocol-relative URLs
            if (url.StartsWith("//"))
            {
                return false;
            }

            // Reject absolute URIs
            return !Uri.TryCreate(url, UriKind.Absolute, out _);
        }

    }
}

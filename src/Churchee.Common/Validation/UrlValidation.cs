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

            url = Uri.UnescapeDataString(url).Trim();

            // Must start with a single forward slash
            if (!url.StartsWith('/'))
            {
                return false;
            }

            // Reject protocol-relative URLs like //evil.com
            if (url.StartsWith("//"))
            {
                return false;
            }

            // Reject absolute URLs
            return !Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}

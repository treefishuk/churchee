using Bunit;
using Microsoft.AspNetCore.Components;

namespace Churchee.Test.Helpers.Blazor
{
    public static class BunitExtensions
    {
        // Wait until the provided selector has at least `min` nodes or timeout elapses.
        public static void WaitForNodes<T>(this IRenderedComponent<T> cut, string selector, int min = 1, TimeSpan? timeout = null) where T : IComponent
        {
            timeout ??= TimeSpan.FromSeconds(5);
            cut.WaitForState(() => cut.FindAll(selector).Count >= min, timeout.Value);
        }

        // Convenience for waiting for data rows in a typical Radzen grid
        public static void WaitForGridRows<T>(this IRenderedComponent<T> cut, int min = 1, TimeSpan? timeout = null) where T : IComponent
        {
            // Adjust selector if your grid markup differs
            const string gridRowSelector = "table tbody tr";
            cut.WaitForNodes(gridRowSelector, min, timeout);
        }
    }
}

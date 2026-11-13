using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Churchee.Common.Helpers
{
    public static class EmbeddedResourceHelper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string ReadTemplate(string templateName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(templateName);

            var assembly = Assembly.GetCallingAssembly();
            string resourceSuffix = $".Templates.{templateName}.cshtml";
            string[] resourceNames = assembly.GetManifestResourceNames()
                .Where(name => name.EndsWith(resourceSuffix, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (resourceNames.Length == 0)
            {
                throw new InvalidOperationException($"Embedded template resource '{resourceSuffix}' was not found in assembly '{assembly.FullName}'.");
            }

            if (resourceNames.Length > 1)
            {
                throw new InvalidOperationException($"Multiple embedded template resources ending with '{resourceSuffix}' were found in assembly '{assembly.FullName}'.");
            }

            using var stream = assembly.GetManifestResourceStream(resourceNames[0])
                ?? throw new InvalidOperationException($"Embedded template resource '{resourceNames[0]}' could not be opened.");
            using var reader = new StreamReader(stream, Encoding.UTF8);

            return reader.ReadToEnd();
        }
    }
}

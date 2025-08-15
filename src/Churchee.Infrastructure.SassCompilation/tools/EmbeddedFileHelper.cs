namespace Churchee.Infrastructure.SassCompilation.tools
{
    public static class EmbeddedFileHelper
    {
        public static async Task ExtractEmbeddedDirectoryToTempAsync(string resourcePrefix, string tempDir)
        {
            Directory.CreateDirectory(tempDir);

            var assembly = typeof(EmbeddedFileHelper).Assembly;

            string[] resources = assembly.GetManifestResourceNames();

            var filteredResources = assembly.GetManifestResourceNames()
                                    .Where(r => r.StartsWith(resourcePrefix.Replace("\\", ".")));

            foreach (string? resource in filteredResources)
            {
                // Remove the namespace prefix, keep the real file name
                string relativeName = resource.Substring(resourcePrefix.Replace("\\", ".").Length + 1);

                // Only replace dots in folder path, not in file name
                int lastDotIndex = relativeName.LastIndexOf('.');
                string folderPath = lastDotIndex > 0 ? relativeName.Substring(0, lastDotIndex) : relativeName;
                string fileName = lastDotIndex > 0 ? relativeName.Substring(lastDotIndex + 1) : "";

                string folderPathClean = folderPath.Replace('.', Path.DirectorySeparatorChar);
                string fullPath = Path.Combine(tempDir, folderPathClean + (string.IsNullOrEmpty(fileName) ? "" : "." + fileName));

                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

                await using var stream = assembly.GetManifestResourceStream(resource);
                using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                await stream.CopyToAsync(fs);
            }
        }




    }
}

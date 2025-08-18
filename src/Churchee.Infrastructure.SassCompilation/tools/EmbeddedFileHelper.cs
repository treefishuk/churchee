using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Churchee.Infrastructure.SassCompilation.tools
{
    public static class EmbeddedFileHelper
    {
        private static readonly string ScssResourcePathPrefix = "Churchee.Infrastructure.SassCompilation.wwwroot.lib.bootstrap.scss";
        private static readonly string Win64ResourcePathPrefix = "Churchee.Infrastructure.SassCompilation.tools.dart_sass.win_x64";
        private static readonly string Linux64ResourcePathPrefix = "Churchee.Infrastructure.SassCompilation.tools.dart_sass.linux_x64";

        public static async Task ExtractEmbeddedSassDirectoryToTempAsync(string bootstrapPath)
        {
            if (Directory.Exists(bootstrapPath))
            {
                return;
            }

            Directory.CreateDirectory(bootstrapPath);

            var resources = GetResourcePathsByPrefix(ScssResourcePathPrefix);

            foreach (string resource in resources)
            {
                await CreateScssFile(bootstrapPath, resource);
            }
        }

        public static async Task<string> ExtractEmbeddedToolsAndReturnPathAsync(string tempDir)
        {
            Directory.CreateDirectory(tempDir);

            string resourcePrefix = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Win64ResourcePathPrefix : Linux64ResourcePathPrefix;

            string binaryName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "sass.bat" : "sass";

            string binaryPath = Path.Combine(tempDir, binaryName);

            if (File.Exists(binaryPath))
            {
                return binaryPath;
            }

            var resources = GetResourcePathsByPrefix(resourcePrefix);

            foreach (string resource in resources)
            {
                await CreateToolFile(tempDir, resourcePrefix, resource);
            }

            MakeFilesExecutable(binaryPath);

            return binaryPath;
        }

        private static async Task CreateScssFile(string bootstrapPath, string resource)
        {
            var assembly = typeof(EmbeddedFileHelper).Assembly;

            // Remove the namespace prefix, keep the real file name
            string relativeName = resource.Substring(ScssResourcePathPrefix.Replace("\\", ".").Length + 1);

            // Only replace dots in folder path, not in file name
            int lastDotIndex = relativeName.LastIndexOf('.');
            string folderPath = lastDotIndex > 0 ? relativeName.Substring(0, lastDotIndex) : relativeName;
            string fileName = lastDotIndex > 0 ? relativeName.Substring(lastDotIndex + 1) : "";

            string folderPathClean = folderPath.Replace('.', Path.DirectorySeparatorChar);
            string fullPath = Path.Combine(bootstrapPath, folderPathClean + (string.IsNullOrEmpty(fileName) ? "" : "." + fileName));

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            // Log the file being created
            Console.WriteLine($"[CreateScssFile] Creating file: {fullPath}");

            await using var stream = assembly.GetManifestResourceStream(resource) ?? throw new InvalidOperationException($"Resource '{resource}' not found.");

            using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write);

            await stream.CopyToAsync(fs);
        }

        private static IEnumerable<string> GetResourcePathsByPrefix(string resourcePrefix)
        {
            var assembly = typeof(EmbeddedFileHelper).Assembly;
            string prefix = resourcePrefix.Replace("\\", ".") + ".";
            return assembly.GetManifestResourceNames().Where(r => r.StartsWith(prefix));
        }

        private static void MakeFilesExecutable(string binaryPath)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                static void MakeExecutableRecursively(string path)
                {
                    foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                    {
                        var chmod = Process.Start("chmod", $"+x \"{file}\"");
                        chmod.WaitForExit();
                    }
                }

                MakeExecutableRecursively(Path.GetDirectoryName(binaryPath)!);
            }
        }

        private static async Task CreateToolFile(string tempDir, string prefix, string resource)
        {
            var assembly = typeof(EmbeddedFileHelper).Assembly;

            // Remove the prefix
            string relativeName = resource.Substring(prefix.Length + 1);

            string fullPath;

            if (relativeName.StartsWith("src."))
            {
                // Remove the "src." part and put under src folder
                string filePath = relativeName.Substring(4);
                fullPath = Path.Combine(tempDir, "src", filePath);
            }
            else
            {
                fullPath = Path.Combine(tempDir, relativeName);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            await using var stream = assembly.GetManifestResourceStream(resource) ?? throw new InvalidOperationException($"Resource '{resource}' not found.");
            using var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fs);
        }
    }
}

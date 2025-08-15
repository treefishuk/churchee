using Churchee.Common.Abstractions.Utilities;
using Churchee.Infrastructure.SassCompilation.tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Churchee.Infrastructure.SassCompilation
{
    public sealed class SassCliCompiler : ISassComplier
    {
        private readonly string _sassTempDir;
        private readonly string _sassBinaryPath;
        private readonly string _bootstrapTempDir;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(20);

        public SassCliCompiler(IWebHostEnvironment env, IConfiguration config)
        {
            _sassTempDir = Path.Combine(Path.GetTempPath(), "sass-temp");
            Directory.CreateDirectory(_sassTempDir);

            // 1. Extract Dart Sass binary recursively
            string sassResourcePrefix = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "Churchee.Infrastructure.SassCompilation.tools.dart_sass.win_x64"   // embed the full folder
                : "Churchee.Infrastructure.SassCompilation.tools.dart_sass.linux_x64"; // embed the single binary

            _sassBinaryPath = ExtractDartSass(sassResourcePrefix).Result;

            // 2. Extract Bootstrap SCSS recursively
            _bootstrapTempDir = Path.Combine(_sassTempDir, "bootstrap");
            if (!Directory.Exists(_bootstrapTempDir))
            {
                Task.Run(() => EmbeddedFileHelper.ExtractEmbeddedDirectoryToTempAsync(
                    "Churchee.Infrastructure.SassCompilation.wwwroot.lib.bootstrap.scss", _bootstrapTempDir
                )).Wait();
            }

        }

        private async Task<string> ExtractDartSass(string resourcePrefix)
        {
            string tempDir = Path.Combine(_sassTempDir, "dart-sass");
            if (!Directory.Exists(tempDir))
            {
                await EmbeddedFileHelper.ExtractEmbeddedDirectoryToTempAsync(resourcePrefix, tempDir);
            }

            string binaryName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "sass.bat" : "sass";
            string binaryPath = Path.Combine(tempDir, binaryName);

            if (!File.Exists(binaryPath))
                throw new FileNotFoundException($"Dart Sass binary not found at {binaryPath}");

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var chmod = Process.Start("chmod", $"+x {binaryPath}");
                chmod.WaitForExit();
            }

            return binaryPath;
        }


        public async Task<string> CompileStringAsync(string scss, bool compressed = true, CancellationToken ct = default)
        {
            // "-" tells sass to read from stdin
            string args = BuildArgs("-", compressed);
            return await RunAsync(args, scss, ct);
        }

        private string BuildArgs(string input, bool compressed)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(input).Append(' ');
            sb.Append("--no-source-map ")
              .Append("--silence-deprecation=import ")
              .Append("--silence-deprecation=global-builtin ")
              .Append("--silence-deprecation=color-functions ")
              .Append("--verbose ")
              .Append($"--load-path={_bootstrapTempDir} ")
              .Append(compressed ? "--style=compressed " : "--style=expanded ");

            return sb.ToString();
        }

        private async Task<string> RunAsync(string args, string? stdin, CancellationToken ct)
        {

            var psi = new ProcessStartInfo
            {
                FileName = _sassBinaryPath,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = stdin != null,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = new Process { StartInfo = psi };

            if (!File.Exists(_sassBinaryPath))
            {
                throw new FileNotFoundException("Sass binary not found", _sassBinaryPath);
            }

            if (!Directory.Exists(_bootstrapTempDir))
            {
                throw new FileNotFoundException("bootstrap Sass path not found", _bootstrapTempDir);
            }


            proc.Start();

            if (stdin != null)
            {
                await proc.StandardInput.WriteAsync(stdin.AsMemory(), ct);
                proc.StandardInput.Close();
            }

            var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(_timeout);

            var stdOutTask = proc.StandardOutput.ReadToEndAsync(ct);
            var stdErrTask = proc.StandardError.ReadToEndAsync(ct);

            await Task.WhenAny(Task.Run(() => proc.WaitForExit(), cts.Token), Task.Delay(Timeout.Infinite, cts.Token))
                      .ConfigureAwait(false);

            if (!proc.HasExited)
            {
                try { proc.Kill(entireProcessTree: true); } catch { /* ignore */ }
                throw new TimeoutException("Sass compilation timed out.");
            }

            string css = await stdOutTask;
            string err = await stdErrTask;

            if (proc.ExitCode != 0 || !string.IsNullOrWhiteSpace(err))
            {
                throw new Exception($"Sass error (code {proc.ExitCode}): {err}");
            }

            return css;
        }
    }

}

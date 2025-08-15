using Churchee.Common.Abstractions.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Churchee.Infrastructure.SassCompilation
{
    public sealed class SassCliCompiler : ISassComplier
    {
        private readonly string _sassPath;
        private readonly string _contentRoot;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(20);

        public SassCliCompiler(IWebHostEnvironment env, IConfiguration config)
        {
            _contentRoot = env.ContentRootPath;
            // Allow override via env var or appSettings; else use bundled binary
            _sassPath = config["Sass:BinaryPath"]
                        ?? Path.Combine(_contentRoot,
                            "tools",
                            "dart-sass",
                            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win-x64" : "linux-x64",
                            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "sass.bat" : "sass");
        }

        public async Task<string> CompileFileAsync(
            string inputFile,
            IEnumerable<string>? loadPaths = null,
            bool compressed = true,
            CancellationToken ct = default)
        {
            string args = BuildArgs(inputFile, compressed, loadPaths);
            return await RunAsync(args, null, ct);
        }

        public async Task<string> CompileStringAsync(
            string scss,
            IEnumerable<string>? loadPaths = null,
            bool compressed = true,
            CancellationToken ct = default)
        {
            // "-" tells sass to read from stdin
            string args = BuildArgs("-", compressed, loadPaths);
            return await RunAsync(args, scss, ct);
        }

        private static string BuildArgs(string input,
                                        bool compressed,
                                        IEnumerable<string>? loadPaths)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(input).Append(' ');
            sb.Append("--no-source-map ")
              .Append("--silence-deprecation=import ")
              .Append("--silence-deprecation=global-builtin ")
              .Append("--silence-deprecation=color-functions ")
              .Append(compressed ? "--style=compressed " : "--style=expanded ");
            if (loadPaths != null)
            {
                foreach (string p in loadPaths)
                {
                    // multiple --load-path flags are allowed
                    sb.Append("--load-path=").Append('"').Append(p).Append('"').Append(' ');
                }
            }
            return sb.ToString();
        }

        private async Task<string> RunAsync(string args, string? stdin, CancellationToken ct)
        {
            var psi = new ProcessStartInfo
            {
                FileName = _sassPath,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = stdin != null,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = new Process { StartInfo = psi };

            if (!File.Exists(_sassPath))
            {
                throw new FileNotFoundException("Sass binary not found", _sassPath);
            }

            proc.Start();

            if (stdin != null)
            {
                await proc.StandardInput.WriteAsync(stdin.AsMemory(), ct);
                proc.StandardInput.Close();
            }

            var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(_timeout);

            var stdOutTask = proc.StandardOutput.ReadToEndAsync();
            var stdErrTask = proc.StandardError.ReadToEndAsync();

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

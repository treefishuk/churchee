using Churchee.Common.Abstractions.Utilities;
using Churchee.Infrastructure.SassCompilation.Exceptions;
using Churchee.Infrastructure.SassCompilation.tools;
using System.Diagnostics;

namespace Churchee.Infrastructure.SassCompilation
{
    public sealed class SassCliCompiler : ISassComplier
    {
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(20);


        public async Task<string> CompileStringAsync(string scss, bool compressed, CancellationToken cancelationToken)
        {
            string sassTempDir = Path.Combine(Path.GetTempPath(), "sass-temp");

            string bootstrapTempDir = Path.Combine(sassTempDir, "bootstrap");

            await EmbeddedFileHelper.ExtractEmbeddedSassDirectoryToTempAsync(bootstrapTempDir);

            string binaryPath = await EmbeddedFileHelper.ExtractEmbeddedToolsAndReturnPathAsync(sassTempDir);

            // "-" tells sass to read from stdin
            string args = BuildArgs("-", compressed, bootstrapTempDir);

            return await RunAsync(args, scss, binaryPath, cancelationToken);
        }

        private static string BuildArgs(string input, bool compressed, string bootstrapTempDir)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(input).Append(' ');
            sb.Append("--no-source-map ")
              .Append("--silence-deprecation=import ")
              .Append("--silence-deprecation=global-builtin ")
              .Append("--silence-deprecation=color-functions ")
              .Append("--verbose ")
              .Append($"--load-path={bootstrapTempDir} ")
              .Append(compressed ? "--style=compressed " : "--style=expanded ");

            return sb.ToString();
        }

        private async Task<string> RunAsync(string args, string? stdin, string binaryPath, CancellationToken ct)
        {

            var psi = new ProcessStartInfo
            {
                FileName = binaryPath,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = stdin != null,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = new Process { StartInfo = psi };

            if (!File.Exists(binaryPath))
            {
                throw new FileNotFoundException("Sass binary not found", binaryPath);
            }

            proc.Start();

            if (stdin != null)
            {
                await proc.StandardInput.WriteAsync(stdin.AsMemory(), ct);
                proc.StandardInput.Close();
            }

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
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
                throw new SassCompilationException($"Sass error (code {proc.ExitCode}): {err}");
            }

            return css;
        }
    }

}

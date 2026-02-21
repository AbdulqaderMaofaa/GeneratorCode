using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeneratorCode.Core.Helpers
{
    public class ProcessResult
    {
        public int ExitCode { get; set; }
        public string StandardOutput { get; set; } = string.Empty;
        public string StandardError { get; set; } = string.Empty;
        public bool Success => ExitCode == 0;
        public TimeSpan Elapsed { get; set; }
    }

    public class ProcessRunner
    {
        public static async Task<ProcessResult> RunAsync(
            string command,
            string arguments,
            string workingDirectory = null,
            int timeoutMs = 120000,
            IProgress<string> progress = null,
            CancellationToken cancellationToken = default)
        {
            ValidatePath(workingDirectory);
            ValidateCommand(command);

            var sw = System.Diagnostics.Stopwatch.StartNew();
            var stdout = new StringBuilder();
            var stderr = new StringBuilder();

            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data == null) return;
                stdout.AppendLine(e.Data);
                progress?.Report(e.Data);
            };

            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data == null) return;
                stderr.AppendLine(e.Data);
                progress?.Report($"[ERROR] {e.Data}");
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            using var timeoutCts = new CancellationTokenSource(timeoutMs);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            try
            {
                await process.WaitForExitAsync(linkedCts.Token);
            }
            catch (OperationCanceledException)
            {
                try { process.Kill(true); } catch { }

                if (timeoutCts.IsCancellationRequested)
                    throw new TimeoutException($"العملية تجاوزت المهلة المحددة ({timeoutMs / 1000} ثانية)");

                throw;
            }

            sw.Stop();

            return new ProcessResult
            {
                ExitCode = process.ExitCode,
                StandardOutput = stdout.ToString(),
                StandardError = stderr.ToString(),
                Elapsed = sw.Elapsed
            };
        }

        private static void ValidatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;

            var normalized = Path.GetFullPath(path);
            if (normalized.Contains("..") || normalized.Contains('\0'))
                throw new ArgumentException($"مسار غير آمن: {path}");
        }

        private static void ValidateCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException("الأمر مطلوب");

            var dangerousChars = new[] { '|', '&', ';', '`', '$', '>', '<' };
            foreach (var c in dangerousChars)
            {
                if (command.Contains(c))
                    throw new ArgumentException($"الأمر يحتوي على حرف غير مسموح: {c}");
            }
        }
    }
}

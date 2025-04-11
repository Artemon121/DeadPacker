using DeadPacker;
using Spectre.Console;
using System.Diagnostics;

namespace DeadPacker
{
    internal class ResourceCompiler
    {
        private readonly CompileConfig config;

        public ResourceCompiler(CompileConfig config) {
            if (config.CompilerPath == null)
            {
                throw new ArgumentException("resource_compiler_path is not specified");
            }
            if (config.CompilerPath == null)
            {
                throw new ArgumentException("addon_content_directory is not specified");
            }
            this.config = config;
        }

        private Process GetProcess()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = config.CompilerPath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            return process;
        }

        public async Task CompileContentDirectory()
        {
            await Task.Delay(100); // Slight delay to give some time for file locks to be released
            Log.Info($"Compiling directory {Log.FormatPath(config.ContentDirectory!)}");

            using var process = GetProcess();
            process.StartInfo.Arguments = $"-r -i \"{config.ContentDirectory!.TrimEnd('/', '\\')}\\**\"";

            AnsiConsole.WriteLine();
         
            process.Start();
            process.BeginOutputReadLine();
            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    AnsiConsole.WriteLine(e.Data);
                }
            };
            await process.WaitForExitAsync();
            
            AnsiConsole.WriteLine();

            Log.Info($"Compiled {Log.FormatPath(config.ContentDirectory!)}");
        }
    }
}

using Spectre.Console;
using Tomlyn;
using DeadPacker.Properties;
using System.Text;
using System.Text.Unicode;
using System.Diagnostics;

namespace DeadPacker
{
    internal class Program
    {
        private static async Task ProcessSteps(ConfigModel config)
        {
            foreach (var step in config.Steps)
            {
                try
                {
                    if (step.Copy != null)
                    {
                        if (string.IsNullOrWhiteSpace(step.Copy.From)) throw new ArgumentException("Copy 'from' is required");
                        if (string.IsNullOrWhiteSpace(step.Copy.To)) throw new ArgumentException("Copy 'to' is required");
                        FileCopyHelper.Copy(step.Copy.From!, step.Copy.To!, true);
                    }
                    else if (step.Compile != null)
                    {
                        if (string.IsNullOrWhiteSpace(step.Compile.CompilerPath)) throw new ArgumentException("Compiler path is required");
                        if (string.IsNullOrWhiteSpace(step.Compile.ContentDirectory)) throw new ArgumentException("Content directory is required");
                        var resourceCompiler = new ResourceCompiler(step.Compile);
                        await resourceCompiler.CompileContentDirectory();
                    }
                    else if (step.Pack != null)
                    {
                        if (string.IsNullOrWhiteSpace(step.Pack.InputDirectory)) throw new ArgumentException("Watch directory is required");
                        if (string.IsNullOrWhiteSpace(step.Pack.OutputPath)) throw new ArgumentException("Output path is required");
                        var packer = new Packer(step.Pack);
                        await packer.Pack();
                    }
                    else if (step.LaunchDeadlock != null)
                    {
                        DeadlockLauncher.LaunchDeadlock(step.LaunchDeadlock);
                    }
                    else if (step.CloseDeadlock != null)
                    {
                        await DeadlockLauncher.CloseDeadlock();
                    }
                    else
                    {
                        Log.Error("Step with no valid operation configuration");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error processing step: {ex.Message}", ex);
                    Console.ReadKey();
                    Environment.Exit(1);
                }
            }
        }

        private static ConfigModel? LoadConfig(string configPath)
        {
            ConfigModel config;
            try
            {
                config = Toml.ToModel<ConfigModel>(File.ReadAllText(configPath));
            }
            catch (Exception exc)
            {
                Log.Error($"Config error: {exc.Message}", exc);
                Console.ReadKey();
                Environment.Exit(1);
                return null;
            }

            if (config.Steps.Count == 0)
            {
                Log.Error("No steps found in config");
                Console.ReadKey();
                Environment.Exit(1);
                return null;
            }

            return config;
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine();
            var font = FigletFont.Parse(Encoding.UTF8.GetString(Resources.font));
            AnsiConsole.Write(
                new FigletText(font, "DeadPacker")
                    .Centered()
                    .Color(Color.SpringGreen1));

            try
            {
                while (true)
                {
                    var configPath = (args.Length >= 1) ? args[0] : "deadpacker.toml";
                    var config = LoadConfig(configPath);
                    
                    Log.Info($"Starting DeadPacker with config: {Log.FormatPath(configPath)}");
                    await ProcessSteps(config!);

                    AnsiConsole.WriteLine();
                    Log.Info("[springgreen2 bold]Done![/] Press [white on grey] ENTER [/] to re-run the program");
                    Console.ReadLine();
                    AnsiConsole.Write(new Rule() { Border = BoxBorder.Heavy });
                    AnsiConsole.WriteLine();
                }
            } finally
            {
                Console.ReadKey();
            }
        }
    }
}

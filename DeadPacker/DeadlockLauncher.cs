using Spectre.Console;
using System.Diagnostics;

namespace DeadPacker
{
    internal static class DeadlockLauncher
    {

        public static readonly int DEADLOCK_APPID = 1422450;

        public static async Task CloseDeadlock()
        {
            Log.Info("Closing Deadlock...");
            using Process? process = Process.GetProcessesByName("deadlock")?.FirstOrDefault();
            if (process == null)
            {
                Log.Info("Deadlock is not currently running");
                return;
            }
            Log.Debug($"Killing process: [silver]{process.ProcessName}[/] (PID: [silver]{process.Id}[/])");
            process.Kill();
            await process.WaitForExitAsync();
            Log.Info("Closed Deadlock");

            Log.Debug("Waiting for Steam to recognize that the game is no longer running...");

            string registryPath = $@"Software\Valve\Steam\Apps\{DEADLOCK_APPID}";
            string valueName = "Running";
            while (true)
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryPath))
                {
                    if (key != null)
                    {
                        object? value = key.GetValue(valueName);
                        if (value is int intValue && intValue == 0)
                        {
                            break;
                        }
                    }
                }
                await Task.Delay(100);
            }
        }
        public static void LaunchDeadlock(LaunchDeadlockConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.LaunchParams))
            {
                Log.Info("Launching Deadlock");
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"steam://launch/{DEADLOCK_APPID}/dialog",
                    UseShellExecute = true,
                });
            }
            else
            {
                Log.Info($"Launching Deadlock with parameters: [deepskyblue2]{Markup.Escape(config.LaunchParams)}[/]");
                Process.Start(new ProcessStartInfo
                {
                    FileName = $"steam://run/{DEADLOCK_APPID}//{config.LaunchParams}",
                    UseShellExecute = true,
                });
            }
        }
    }
}

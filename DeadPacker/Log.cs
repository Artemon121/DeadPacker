using Spectre.Console;
using Spectre.Console.Rendering;

namespace DeadPacker
{
    internal static class Log
    {
        public static string FormatPath(string path)
        {
            var parts = path.Replace("\\", "/").TrimEnd('/').Split('/').ToList();
            if (Directory.Exists(path)) parts.Add("");
            var new_path = string.Join("[springgreen4]/[/]", parts);
            return $"[springgreen2]{new_path}[/]";
        }

        private static void PrintToConsole(string level, string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            AnsiConsole.MarkupLine(
                $"[grey][[[silver]{timestamp}[/] {level}]][/] [white]{message}[/]"
            );
        }

        public static void PrintToConsole(string level, params object[] args)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            AnsiConsole.Markup(
                $"[grey][[[silver]{timestamp}[/] {level}]][/] "
            );
            foreach (var item in args)
            {
                if (item is string stringItem)
                {
                    AnsiConsole.Markup($"[white]{stringItem}[/]");
                }
                else if (item is IRenderable renderable)
                {
                    AnsiConsole.Write(renderable);
                }
                else
                {
                    AnsiConsole.Write(item?.ToString());
                }
            }
            AnsiConsole.WriteLine();
        }

        public static void Info(string message) => PrintToConsole("[white]INFO[/]", message);
        public static void Info(params object[] args) => PrintToConsole("[white]INFO[/]", args);
        public static void Debug(string message) => PrintToConsole("[silver]DEBUG[/]", message);
        public static void Debug(params object[] args) => PrintToConsole("[silver]DEBUG[/]", args);
        public static void Error(string message, Exception? exc = null)
        {
            PrintToConsole("[red]ERROR[/]", message);

            if (exc != null)
            {
                AnsiConsole.WriteException(exc);
            }
        }
        public static void Warn(string message, Exception? exc = null)
        {
            PrintToConsole("[deepskyblue2]WARN[/]", message);

            if (exc != null)
            {
                AnsiConsole.WriteException(exc);
            }
        }
    }
}


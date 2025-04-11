using Microsoft.Extensions.FileSystemGlobbing;
using SteamDatabase.ValvePak;

namespace DeadPacker
{
    internal class Packer
    {
        private readonly PackConfig config;
        private readonly Matcher excludeMatcher;

        public Packer(PackConfig config) {
            if (string.IsNullOrEmpty(config.InputDirectory))
            {
                throw new ArgumentException("watch_directory is required");
            }
            if (string.IsNullOrEmpty(config.OutputPath))
            {
                throw new ArgumentException("output_file is required");
            }
            this.config = config;
            excludeMatcher = new Matcher();
            excludeMatcher.AddIncludePatterns(config.ExcludePatterns ?? Enumerable.Empty<string>()); // Adding globs to exclude
        }

        public async Task Pack()
        {
            await Task.Delay(500); // Slight delay to give some time for file locks to be released
            Log.Info($"Packing [deepskyblue2]{Path.GetFileName(config.OutputPath)}[/]");

            using var vpk = new Package();
            int count = 0;
            foreach (var file in Directory.EnumerateFiles(config.InputDirectory!, "*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(config.InputDirectory!, file);
                if (excludeMatcher.Match(relativePath).HasMatches) continue;

                try { 
                    Log.Debug($"Adding file [silver]{relativePath}[/] to vpk");
                    vpk.AddFile(relativePath, await File.ReadAllBytesAsync(file));
                }
                catch (IOException exc)
                {
                    Log.Error($"Unable to add file to vpk: {exc.Message}", exc);
                    return;
                }
                count++;
            }
            if (count == 0) return;
            try { 
                vpk.Write(config.OutputPath);
            }
            catch (IOException exc)
            {
                Log.Error($"Unable to write vpk: {exc.Message}", exc);
                return;
            }
            Log.Info($"Packed [deepskyblue2]{count}[/] files from {Log.FormatPath(config.InputDirectory!)} to {Log.FormatPath(config.OutputPath!)}");
        }
    }
}

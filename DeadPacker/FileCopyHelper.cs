namespace DeadPacker;

public static class FileCopyHelper
{
    public static void Copy(string sourcePath, string destinationPath, bool overwrite)
    {
        try
        {
            var (isSingleFile, sourceDir, filePattern) = ParseSourcePath(sourcePath);
            destinationPath = DetermineDestinationPath(sourcePath, destinationPath, isSingleFile);

            if (isSingleFile)
            {
                CopySingleFile(sourcePath, destinationPath, overwrite);
            }
            else
            {
                CopyWithWildcards(sourceDir!, filePattern!, destinationPath, overwrite);
            }
        }
        catch (Exception exc)
        {
            Log.Error($"Error while copying {sourcePath} to {destinationPath}: {exc.Message}", exc);
        }
    }

    private static (bool isSingleFile, string? sourceDir, string? filePattern) ParseSourcePath(string sourcePath)
    {
        if (ContainsWildcards(sourcePath))
        {
            var sourceDir = Path.GetDirectoryName(sourcePath);
            if (string.IsNullOrEmpty(sourceDir)) sourceDir = Directory.GetCurrentDirectory();
            if (!Directory.Exists(sourceDir)) throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");

            return (false, sourceDir, Path.GetFileName(sourcePath));
        }

        if (Directory.Exists(sourcePath))
        {
            return (false, sourcePath, "*");
        }

        if (File.Exists(sourcePath))
        {
            return (true, null, null);
        }

        throw new FileNotFoundException("Source path not found", sourcePath);
    }

    private static bool ContainsWildcards(string path) => path.Contains('*') || path.Contains('?');

    private static string DetermineDestinationPath(string sourcePath, string destinationPath, bool isSingleFile)
    {

        if (!isSingleFile)
        {
            if (File.Exists(destinationPath)) throw new IOException("Destination path must be a directory when copying multiple files");
            return EnsureDirectoryDestination(destinationPath);
        }

        if (Directory.Exists(destinationPath) || EndsWithDirectorySeparator(destinationPath))
        {
            return Path.Combine(EnsureDirectoryDestination(destinationPath), Path.GetFileName(sourcePath));
        }

        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
        return destinationPath;
    }

    private static string EnsureDirectoryDestination(string path)
    {
        var normalized = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                          + Path.DirectorySeparatorChar;
        Directory.CreateDirectory(normalized);
        return normalized;
    }

    private static bool EndsWithDirectorySeparator(string path) =>
        path.EndsWith(Path.DirectorySeparatorChar.ToString()) ||
        path.EndsWith(Path.AltDirectorySeparatorChar.ToString());

    private static void CopySingleFile(string sourceFile, string destFile, bool overwrite)
    {
        try
        {
            File.Copy(sourceFile, destFile, overwrite);
        }
        catch (Exception exc)
        {
            Log.Error($"Failed to copy file from {sourceFile} to {destFile}: {exc.Message}", exc);
        }
        Log.Info($"Copied file from {Log.FormatPath(sourceFile)} to {Log.FormatPath(destFile)}");
    }

    private static void CopyWithWildcards(string sourceDir, string pattern, string destDir, bool overwrite)
    {
        var count = 0;
        foreach (var file in Directory.EnumerateFiles(sourceDir, pattern, SearchOption.AllDirectories))
        {
            try
            {
                var relativePath = GetRelativePath(file, sourceDir);
                var destPath = Path.Combine(destDir, relativePath);
                var destDirectory = Path.GetDirectoryName(destPath);

                Directory.CreateDirectory(destDirectory);
                File.Copy(file, destPath, overwrite);
                count++;
            }
            catch (Exception exc)
            {
                Log.Warn($"Skipped '{file}': {exc.Message}", exc);
            }
        }
        Log.Info($"Copied [deepskyblue2]{count}[/] files from {Log.FormatPath(sourceDir)} to {destDir}");
    }

    private static string GetRelativePath(string fullPath, string basePath)
    {
        var baseUri = new Uri(basePath + Path.DirectorySeparatorChar);
        var fullUri = new Uri(fullPath);
        return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fullUri).ToString())
                  .Replace('/', Path.DirectorySeparatorChar);
    }
}
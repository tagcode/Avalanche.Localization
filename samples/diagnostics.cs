using Avalanche.Localization;
using Avalanche.Utilities;
using static System.Console;

class diagnostics
{
    public static void Run()
    {
        {
            // Get localization context
            ILocalization localization = Localization.Default;
            // Add parse error printer
            localization.AddErrorHandler((ILocalizationError e) => Console.WriteLine(e));
        }
        {
            // Get localization context
            ILocalization localization = Localization.Default;
            // Try get text
            if (localization.LocalizedTextCached.TryGetValue(("", "Namespace.Apples"), out ILocalizedText text))
            {
                // Print any parse errors
                foreach (ILocalizationError error in text.Errors) WriteLine(error);
            }
        }
        {
            // Get localization context
            ILocalization localization = Localization.Default;
            // Print files that are visible to localization
            if (localization.FileQueryCached.TryGetValue((null, null), out IEnumerable<ILocalizationFile> files))
                foreach (ILocalizationFile file in files)
                    WriteLine(file.Key);
        }
        {
            // Get localization context
            ILocalization localization = Localization.Default;
            // Try each file provider
            foreach (var fp in localization.Files.FileProvidersCached)
            {
                // Try query files
                if (!fp.TryGetValue((null, null), out IEnumerable<ILocalizationFile> files)) continue;
                // Print file provider composition
                WriteLine(fp);
                // Print files
                foreach (ILocalizationFile file in files) WriteLine(file.Key);
            }
        }
        {
            // Get localization context
            ILocalization localization = Localization.Default;
            // Print files that are visible to each file system
            foreach (ILocalizationFileSystem fs in localization.Files.FileSystems)
            {
                // Visit tree
                foreach (var file in fs.VisitTree(""))
                {
                    // Print text
                    string print = file.Path == "" ? fs.ToString()! : file.ToString(LocalizationFileSystemPrintTreeExtensions.PrintFormat.DefaultLong);
                    // Write to console
                    WriteLine(print);
                }
            }
        }
        {
            // Get localization context
            ILocalization localization = Localization.Default;
            // Print lines that are visible to localization
            if (localization.LineQueryCached.TryGetValue((null, null), out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines))
                foreach (var line in lines)
                    WriteLine(string.Join(", ", line.Select(a => $"{a.Key}={a.Value.AsString}")));
        }
    }
}

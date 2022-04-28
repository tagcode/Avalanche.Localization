using System.Globalization;
using System.Reflection;
using Avalanche.Localization;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;
using Microsoft.Extensions.FileProviders;
using static System.Console;

class localizationfilesystem
{
    public static void Run()
    {
        {
            // Create filesystem
            ILocalizationFileSystem fs = LocalizationFileSystem.ApplicationRoot;
            // Create localization
            ILocalization localization = new Localization().AddFileSystem(fs);
        }
        {
            // Create filesystem
            ILocalizationFileSystem fs = LocalizationFileSystem.ApplicationRoot.Cached(cacheLists: true, cacheFiles: true);
            // Create localization
            ILocalization localization = new Localization().AddFileSystem(fs);
        }
        {
            ILocalization localization = new Localization().AddFileSystem(LocalizationFileSystem.ApplicationRootCached);
        }
        {
            Localization.Default.Files.FileSystems.Remove(LocalizationFileSystem.ApplicationRoot);
        }
        {
            Localization.Default.Files.FileSystems.Remove(LocalizationFileSystem.ApplicationRoot);
            Localization.Default.Files.FileSystems.Add(LocalizationFileSystem.ApplicationRootCached);
        }
        {
            // Create filesystem
            ILocalizationFileSystem fs = new LocalizationFileSystem("C:\\", name: "Root");
            // Create localization
            ILocalization localization = new Localization().AddFileSystem(fs);
        }

        {
            // Get assembly reference
            Assembly assembly = typeof(localizationfilesystem).Assembly;
            // Create filesystem
            ILocalizationFileSystem fs = new LocalizationFileSystemEmbedded(assembly)
                .Cached(cacheLists: true, cacheFiles: true);
            // Create localization
            ILocalization localization = new Localization()
                .AddFileSystem(fs)
                .AddFilePatterns("*/*.Resources.{Key}", "*/*.Resources.{Culture}.{Key}", "*/{Key}", "*/{Key}.{Culture}");
            WriteLine(fs.PrintTree("", format: LocalizationFileSystemPrintTreeExtensions.PrintFormat.DefaultLong));
            // Open each file in assembly
            foreach (string resourceName in fs.ListAllFiles("")) fs.ReadFully(resourceName);
        }
        {
            // Create filesystem
            ILocalizationFileSystem fs = LocalizationFileSystemEmbedded.AppDomain;
            // Create localization
            ILocalization localization = new Localization().AddFileSystem(fs);
            WriteLine(fs.PrintTree("", format: LocalizationFileSystemPrintTreeExtensions.PrintFormat.DefaultLong));
            // Open each file in appdomain
            foreach (string resourceName in fs.ListAllFiles("")) fs.ReadFully(resourceName);
        }
        {
            // Create localization
            ILocalization localization = new Localization()
                .AddFileFormat(LocalizationFileFormatYaml.Instance)
                .AddFileSystemWithPattern(LocalizationFileSystemEmbedded.AppDomain, "*/*.Resources.{Key}", "*/*.Resources.{Culture}.{Key}", "*/{Key}", "*/{Key}.{Culture}");
            // Print Text
            WriteLine(localization.LocalizedTextCached[("", "Namespace.Apples")].Print(CultureInfo.InvariantCulture, null)); // "Hello World"
        }

        {
            foreach (string filename in LocalizationFileSystem.ApplicationRoot.ListFiles("")!)
                WriteLine(filename);
        }
        {
            foreach (string filename in LocalizationFileSystem.ApplicationRoot.ListAllFiles("")!)
                WriteLine(filename);
        }
        {
            foreach (string filename in LocalizationFileSystem.ApplicationRoot.ListDirectores("")!)
                WriteLine(filename);
        }
        {
            ILocalizationFileSystem fs = LocalizationFileSystem.ApplicationRoot.Cached(true, true);
            foreach (string filename in fs.ListAllDirectories("")!)
                WriteLine(filename);
        }
        WriteLine(); WriteLine();
        {
            ILocalizationFileSystem fs = LocalizationFileSystem.ApplicationRoot.Cached(true, true);
            foreach (var line in fs.VisitTree(""))
                WriteLine(line);
        }
        WriteLine(); WriteLine();
        {
            ILocalizationFileSystem fs = LocalizationFileSystem.ApplicationRoot.Cached(true, true);
            WriteLine(fs.PrintTree(""));
        }
        WriteLine(); WriteLine();
        {
            ILocalizationFileSystem fs = LocalizationFileSystem.ApplicationRoot.Cached(true, true);
            WriteLine(fs.PrintTree("", format: LocalizationFileSystemPrintTreeExtensions.PrintFormat.DefaultLong));
        }
        WriteLine(); WriteLine();
        {
            ILocalizationFileSystem fs = LocalizationFileSystem.ApplicationRoot;
            using Stream? stream = fs.Open("Resources/Namespace.yaml");
        }
        {
            ILocalizationFileSystem fs = LocalizationFileSystem.ApplicationRoot;
            byte[] data = fs.ReadFully("Resources/Namespace.yaml");
        }

    }
}

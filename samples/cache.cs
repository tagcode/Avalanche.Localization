using System.Reflection;
using Avalanche.Localization;
using Avalanche.Utilities;
using static System.Console;

class cache
{
    public static void Run()
    {
        {
            // Create localization
            ILocalization localization = Localization.CreateDefault();
            // Create text using caches
            ILocalizedText text = localization.LocalizableTextCached["Namespace.Apples"];
            // Print
            WriteLine(text.Print(null, new object[] { 3 }));
            // Flush cache
            (localization as Avalanche.Utilities.ICached)?.InvalidateCache(deep: true);
        }
        {
            // Create localization
            ILocalization localization = Localization.CreateDefault()
                .AddFileSystem(LocalizationFileSystem.ApplicationRoot)
                .AddFileFormats(LocalizationFileFormatYaml.Instance, LocalizationFileFormatXml.Instance, LocalizationFileFormatJson.Instance)
                .AddFilePatterns(LocalizationFilePatterns.ResourcesFolder) // "Resources/{Key}", "Resources/{Culture}/{Key}"
                .AddFileSystemWithPattern(LocalizationFileSystemEmbedded.AppDomain, LocalizationFilePatterns.ResourcesEmbedded) // "*/*.Resources.{Key}", "*/*.Resources.{Culture}.{Key}", "*/{Key}", "*/{Key}.{Culture}"
                .AddResourceManagerProvider();
            // Add cache flush on assembly load
            AppDomain.CurrentDomain.AssemblyLoad += (object? sender, AssemblyLoadEventArgs args) => (localization as ICached)?.InvalidateCache(true);

            // Create text using caches
            ILocalizedText text = localization.LocalizableTextCached["Namespace.Apples"];
            // Print
            WriteLine(text.Print(null, new object[] { 3 }));

            // Load assembly and have localization cache flushed
            Assembly a = Assembly.Load("System.xml");
        }
        {
            // Create localization
            ILocalization localization = Localization.CreateDefault();
            // Create text without any caches
            ILocalizedText text = localization.LocalizableText["Namespace.Apples"];
            // Print
            WriteLine(text.Print(null, new object[] { 3 }));
        }
    }
}

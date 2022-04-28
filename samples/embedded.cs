using System.Globalization;
using System.Reflection;
using Avalanche.Localization;
using Avalanche.Utilities;
using static System.Console;

class embedded
{
    public static void Run()
    {
        {
            // Add to localization
            ILocalization localization = Localization.Default;
            // Print
            ILocalizedText text = localization.LocalizableTextCached["Namespace.Apples"];
            string print = text.Print(CultureInfo.InvariantCulture, new object[] { 1 });
            WriteLine(print);
        }        
        {
            // Add to localization
            ILocalization localization = new Localization()
                .AddFileFormats(LocalizationFileFormatYaml.Instance, LocalizationFileFormatXml.Instance, LocalizationFileFormatJson.Instance)
                .AddFileSystemWithPattern(LocalizationFileSystemEmbedded.AppDomain, "*/*.Resources.{Key}", "*/*.Resources.{Culture}.{Key}", "*/{Key}", "*/{Key}.{Culture}");
            // Print
            ILocalizedText text = localization.LocalizableTextCached["Namespace.Apples"];
            string print = text.Print(CultureInfo.InvariantCulture, new object[] { 1 });
            WriteLine(print);
        } 
        {
            // Add to localization
            ILocalization localization = new Localization()
                .AddFileFormats(LocalizationFileFormatYaml.Instance, LocalizationFileFormatXml.Instance, LocalizationFileFormatJson.Instance)
                .AddFileSystemWithPattern(LocalizationFileSystemEmbedded.AppDomain, LocalizationFilePatterns.ResourcesEmbedded);
            // Print
            ILocalizedText text = localization.LocalizableTextCached["Namespace.Apples"];
            string print = text.Print(CultureInfo.InvariantCulture, new object[] { 1 });
            WriteLine(print);
        }
        {
            // Get assembly
            Assembly assembly = typeof(embedded).Assembly;
            // Add to localization
            ILocalization localization = new Localization()
                .AddFileFormats(LocalizationFileFormatYaml.Instance, LocalizationFileFormatXml.Instance, LocalizationFileFormatJson.Instance)
                .AddFileSystemWithPattern(new LocalizationFileSystemEmbedded(assembly), "*/*.Resources.{Key}", "*/*.Resources.{Culture}.{Key}", "*/{Key}", "*/{Key}.{Culture}");
            // Print
            ILocalizedText text = localization.LocalizableTextCached["Namespace.Apples"];
            string print = text.Print(CultureInfo.InvariantCulture, new object[] { 1 });
            WriteLine(print);
        }
        {
            // Print format
            var format = LocalizationFileSystemPrintTreeExtensions.PrintFormat.DefaultLong | LocalizationFileSystemPrintTreeExtensions.PrintFormat.DirectorySlash;
            // Print to tree
            string tree = LocalizationFileSystemEmbedded.AppDomain.PrintTree(format: format);
            // Write to console
            WriteLine(tree);
        }

        {
            // Create file reference
            ILocalizationFile localizationFile = new LocalizationFile
            {
                FileName = "samples/samples.Resources.Namespace.yaml",
                FileSystem = LocalizationFileSystemEmbedded.AppDomain,
                FileFormat = LocalizationFileFormatYaml.Instance,
                Culture = "",
                Key = "Namespace"
            }.SetReadOnly();
            // Add to localization
            ILocalization localization = new Localization().AddFile(localizationFile);
            // Print
            ILocalizedText text = localization.LocalizableTextCached["Namespace.Apples"];
            string print = text.Print(CultureInfo.InvariantCulture, new object[] { 1 });
            WriteLine(print);
        }

        {
            // Get assembly
            Assembly assembly = typeof(embedded).Assembly;
            // Get data
            using Stream stream = assembly.GetManifestResourceStream("samples.Resources.Namespace.yaml")!;
            // Create reader
            var reader = new LocalizationReaderYaml.FromStream(stream);
            // Add to localization
            ILocalization localization = new Localization().AddLines(reader);
            // Print
            ILocalizedText text = localization.LocalizableTextCached["Namespace.Apples"];
            string print = text.Print(CultureInfo.InvariantCulture, new object[] { 1 });
            WriteLine(print);
        }

    }
}

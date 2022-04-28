using Avalanche.Localization;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

public class localizationfiles
{
    public static void Run()
    {
        {
            ILocalization localization = new Localization();
            ILocalizationFiles localizationFiles = localization.Files;
        }
        {
            ILocalization localization = new Localization();
            ILocalizationFileFormat fileformat = LocalizationFileFormatYaml.Instance;
            localization.Files.FileFormats.Add(fileformat);
        }
        {
            ILocalization localization = new Localization();
            ILocalizationFileSystem filesystem = new LocalizationFileSystem(AppDomain.CurrentDomain.BaseDirectory, "ApplicationRoot");
            localization.Files.FileSystems.Add(filesystem);
        }
        {
            ILocalization localization = new Localization();
            ITemplateFormatPrintable pattern = new TemplateText("Resources/{Culture}/{Key}", TemplateFormat.BraceAlphaNumeric);
            localization.Files.FilePatterns.Add(pattern);
        }
        {
            ILocalization localization = new Localization();
            // Create file reference
            ILocalizationFile localizationFile = new LocalizationFile
            {
                FileName = "Resources/localization.yaml",
                FileSystem = LocalizationFileSystem.ApplicationRoot,
                FileFormat = LocalizationFileFormatYaml.Instance,
                Culture = "",
                Key = "Namespace"
            }.SetReadOnly();
            // Add localization file (reference)
            localization.Files.Files.Add(localizationFile);
        }
        {
            // Create localization
            ILocalization localization = new Localization();
            // Create provider
            IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> provider = 
                new ResourceManagerFileProvider(samples.Resources.Resource1.ResourceManager);
            // Add provider (both are required)
            localization.Files.FileProviders.Add(provider);
            localization.Files.FileProvidersCached.Add(provider.ValueResultCaptured().Cached().ValueResultOpened());
            // Get file reference
            ILocalizationFile localizationFile = localization.FileQueryCached[("", "samples.Resources.Resource1.logo")].FirstOrDefault()!;
            // Read file
            byte[] data = localizationFile.ReadFully();
        }
        {
            // Get default localization
            ILocalization localization = Localization.Default;
            // Query
            IEnumerable<ILocalizationFile> files = localization.Files.QueryCached[("en", "Namespace.Apple")];
        }
        {
            // Get default localization
            ILocalization localization = Localization.Default;
            // Query
            IEnumerable<ILocalizationFile> files = localization.Files.QueryCached[("en", "Namespace.Apple")];
        }
    }
}

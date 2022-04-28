using Avalanche.Localization;
using Avalanche.Utilities;
using static System.Console;

class localizationfileformat
{
    public static void Run()
    {
        {
            ILocalizationFileFormat fileFormat1 = LocalizationFileFormatYaml.Instance;
            ILocalizationFileFormat fileFormat2 = LocalizationFileFormatXml.Instance;
            ILocalizationFileFormat fileFormat3 = LocalizationFileFormatJson.Instance;
        }
        {
            // Create localization
            ILocalization localization = Localization.CreateDefault();
            // Modify localization file formats
            localization.Files.FileFormats.Remove(LocalizationFileFormatXml.Instance);
            localization.Files.FileFormats.Remove(LocalizationFileFormatJson.Instance);
        }
        {
            // Create xml file reader
            IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> localizationLines = new LocalizationReaderXml.File(@"localizationfileformat\localization1.xml");
            // Read and print lines
            foreach (var line in localizationLines) WriteLine(string.Join(", ", line.Select(a => $"{a.Key}={a.Value.AsString}")));
        }
        {
            // Create intermediate format of localization lines
            IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> localizationLines =
                new Dictionary<string, MarkedText>[]
                {
                    new Dictionary<string, MarkedText>{ { "TemplateFormat", "BraceNumeric" }, { "Culture", "en" }, { "Key", "Namespace.Apples" }, { "Text", "You've got {0} apple(s)." } },
                    new Dictionary<string, MarkedText>{ { "TemplateFormat", "BraceNumeric" }, { "Culture", "en" }, { "Key", "Namespace.ExitRoundabout" }, { "Text", "Take exit number {0}." } },
                };
            // Print lines
            foreach (var line in localizationLines) WriteLine(string.Join(", ", line.Select(a => $"{a.Key}={a.Value.AsString}")));
        }
    }
}

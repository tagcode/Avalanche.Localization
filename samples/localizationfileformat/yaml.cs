using System.Globalization;
using Avalanche.Localization;
using Avalanche.Utilities;
using Microsoft.Extensions.FileProviders;
using YamlDotNet.RepresentationModel;
using static System.Console;

class yaml
{
    public static void Run()
    {
        {
            // Filename
            string filename = "localizationfileformat/localization1.yaml";
            // Create reader
            IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader = new LocalizationReaderYaml.File(filename);
            // Read and print lines
            foreach (var line in reader)
                WriteLine(string.Join(", ", line.Select(a => $"{a.Key}={a.Value.AsString}")));
        }
        {
            IEnumerable<KeyValuePair<string, MarkedText>>[] lines = new LocalizationReaderYaml.File("localizationfileformat/localization1.yaml").ToArray();
        }
        {
            var reader = new LocalizationReaderYaml.File("localizationfileformat/localization1.yaml").AnnotateFilename("localization.yml");
        }
        {
string text = @"TemplateFormat: BraceNumeric
PluralRules: Unicode.CLDR41
English:
- Culture: en
  Items:
  - Key: Namespace.Apples
    Text: ""You've got {0} apple(s).""";
// Create reader
IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader = new LocalizationReaderYaml.Text(text);
// Read and print lines
foreach (var line in reader)
    WriteLine(string.Join(", ", line.Select(a => $"{a.Key}={a.Value.AsString}")));
        }

        {
            // Create document
            YamlDocument document = new YamlDocument("");
            // Create reader
            IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader = new LocalizationReaderYaml.Document(document);
        }

    }
}

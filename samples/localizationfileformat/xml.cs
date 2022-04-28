using System.Globalization;
using System.Xml.Linq;
using Avalanche.Localization;
using Avalanche.Utilities;
using Microsoft.Extensions.FileProviders;
using YamlDotNet.RepresentationModel;
using static System.Console;

class xml
{
    public static void Run()
    {
        {
            // Filename
            string filename = "localizationfileformat/localization1.xml";
            // Create reader
            IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader = new LocalizationReaderXml.File(filename);
            // Read and print lines
            foreach (var line in reader)
                WriteLine(string.Join(", ", line.Select(a => $"{a.Key}={a.Value.AsString}")));
        }
        {
            IEnumerable<KeyValuePair<string, MarkedText>>[] lines = new LocalizationReaderXml.File("localizationfileformat/localization1.xml").ToArray();
        }
        {
            var reader = new LocalizationReaderXml.File("localizationfileformat/localization1.xml").AnnotateFilename("localization.xml");
        }
        {
            // Filename
            string filename = "localizationfileformat/localization1c.xml";
            // Create reader
            IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader = new LocalizationReaderXml.File(filename);
            // Read and print lines
            foreach (var line in reader)
                WriteLine(string.Join(", ", line.Select(a => $"{a.Key}={a.Value.AsString}")));
        }
        {
string text = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Localization TemplateFormat=""BraceNumeric"" PluralRules=""Unicode.CLDR40"">
    <N Culture=""en"">
        <N Key=""Namespace.Apples"">
            <Text Plurals=""0:cardinal:one"">You've got an apple.</Text>
            <Text Plurals=""0:cardinal:other"">You've got {0} apples.</Text>
        </N>
        <N Key=""Namespace.ExitRoundabout"">
            <Text Plurals=""0:ordinal:one"">Take first exit.</Text>
            <Text Plurals=""0:ordinal:two"">Take second exit.</Text>
            <Text Plurals=""0:ordinal:few"">Take third exit.</Text>
            <Text Plurals=""0:ordinal:other"">Take {0}th exit.</Text>
        </N>        
    </N>
</Localization>
";
// Create reader
IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader = new LocalizationReaderXml.Text(text);
// Read and print lines
foreach (var line in reader)
    WriteLine(string.Join(", ", line.Select(a => $"{a.Key}={a.Value.AsString}")));
        }

        {
            // Create document
            XDocument document = new XDocument();
            // Create reader
            IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader = new LocalizationReaderXml.Document(document);
        }

    }
}

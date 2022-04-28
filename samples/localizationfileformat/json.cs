using System.Text.Json.Nodes;
using Avalanche.Localization;
using Avalanche.Utilities;
using Microsoft.Extensions.FileProviders;
using static System.Console;

class json
{
    public static void Run()
    {
        {
            // Filename
            string filename = "localizationfileformat/localization1.json";
            // Create reader
            IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader = new LocalizationReaderJson.File(filename);
            // Read and print lines
            foreach (var line in reader)
                WriteLine(string.Join(", ", line.Select(a => $"{a.Key}={a.Value.AsString}")));
        }
        {
            IEnumerable<KeyValuePair<string, MarkedText>>[] lines = new LocalizationReaderJson.File("localizationfileformat/localization1.json").ToArray();
        }
        {
            var reader = new LocalizationReaderJson.File("localizationfileformat/localization1.json").AnnotateFilename("localization.json");
        }
        {
string text = @"
{
  ""TemplateFormat"": ""BraceNumeric"",
  ""PluralRules"": ""Unicode.CLDR40"",
  ""English"": {
    ""Culture"": ""en"",
    ""Items"": [
      {
        ""Key"": ""Namespace.Apples"",
        ""Items"": [
          {
            ""Text"": ""You've got no apples."",
            ""Plurals"": ""0:cardinal:zero""
          },
          {
            ""Text"": ""You've got an apple."",
            ""Plurals"": ""0:cardinal:one""
          },
          {
            ""Text"": ""You've got {0} apples."",
            ""Plurals"": ""0:cardinal:other""
          }
        ]
      },
      {
        ""Key"": ""Namespace.ExitRoundabout"",
        ""Items"": [
          {
            ""Text"": ""Take first exit."",
            ""Plurals"": ""0:ordinal:one""
          },
          {
            ""Text"": ""Take second exit."",
            ""Plurals"": ""0:ordinal:two""
          },
          {
            ""Text"": ""Take third exit."",
            ""Plurals"": ""0:ordinal:few""
          },
          {
            ""Text"": ""Take {0}th exit."",
            ""Plurals"": ""0:ordinal:other""
          }
        ]
      }
    ]
  }
}
";
// Create reader
IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader = new LocalizationReaderJson.Text(text);
// Read and print lines
foreach (var line in reader)
    WriteLine(string.Join(", ", line.Select(a => $"{a.Key}={a.Value.AsString}")));
        }

        {
            // Create document
            JsonNode document = JsonNode.Parse("{}")!;
            // Create reader
            IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader = new LocalizationReaderJson.Node(document);
        }

    }
}

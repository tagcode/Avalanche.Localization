using System.Globalization;
using Avalanche.Localization;
using Avalanche.Template;
using Avalanche.Utilities;
using static System.Console;

class localizationline
{
    public static void Run()
    {
        {
            ILocalization localization = new Localization()
                .AddLine("", "Namespace.Apples", "BraceNumeric", "You've got an apple.", "Unicode.CLDR41", "0:cardinal:one:en")
                .AddLine("", "Namespace.Apples", "BraceNumeric", "You've got {0} apples.", "Unicode.CLDR41", "0:cardinal:other:en")
                .AddLine("fi", "Namespace.Apples", "BraceNumeric", "Sinulla on omena.", "Unicode.CLDR41", "0:cardinal:one")
                .AddLine("fi", "Namespace.Apples", "BraceNumeric", "Sinulla on {0} omenaa.", "Unicode.CLDR41", "0:cardinal:other")
                .AddLine("sv", "Namespace.Apples", "BraceNumeric", "Du har ett äpple.", "Unicode.CLDR41", "0:cardinal:one")
                .AddLine("sv", "Namespace.Apples", "BraceNumeric", "Du har {0} äpplen.", "Unicode.CLDR41", "0:cardinal:other");

            ITemplateFormatPrintable printable = localization.LocalizableTextCached["Namespace.Apples"];
            WriteLine(printable.Print(CultureInfo.GetCultureInfo("en"), new object[] { 2 })); // You've got 2 apples.
            WriteLine(printable.Print(CultureInfo.GetCultureInfo("fi"), new object[] { 2 })); // Sinulla on 2 omenaa.
        }
        {
            // Create intermediate format of localization lines
            IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines =
                new Dictionary<string, MarkedText>[]
                {
                    new Dictionary<string, MarkedText>{ { "TemplateFormat", "BraceNumeric" }, { "Culture", "" }, { "Key", "Namespace.Apples" }, { "Text", "You've got {0} apple(s)." } },
                    new Dictionary<string, MarkedText>{ { "TemplateFormat", "BraceNumeric" }, { "Culture", "" }, { "Key", "Namespace.ExitRoundabout" }, { "Text", "Take exit number {0}." } },
                };
            // Print lines
            foreach (var line in lines) WriteLine(string.Join(", ", line.Select(a => $"{a.Key}={a.Value.AsString}")));

            // Create localization context
            ILocalization localization = new Localization();
            // Add to localization
            localization.AddLines(lines);
            // Print localization
            ITemplateFormatPrintable printable = localization.LocalizableTextCached["Namespace.Apples"];
            WriteLine(printable.Print(CultureInfo.InvariantCulture, new object[] { 2 })); // You've got 2 apples.
        }
        {
            // Create xml file reader
            IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines = new LocalizationReaderYaml.File("Localization/localization.yaml");
            // Read and print lines
            foreach (var line in lines) WriteLine(string.Join(", ", line.Select(a => $"{a.Key}={a.Value.AsString}")));

            // Create localization context
            ILocalization localization = new Localization();
            // Add to localization
            localization.AddLines(lines);
            // Print localization
            ITemplateFormatPrintable printable = localization.LocalizableTextCached["Namespace.Apples"];
            WriteLine(printable.Print(CultureInfo.GetCultureInfo("en"), new object[] { 2 })); // You've got 2 apples.
        }
    }
}

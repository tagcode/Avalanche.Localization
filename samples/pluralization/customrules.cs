using System.Globalization;
using Avalanche.Localization;
using Avalanche.Utilities.Provider;
using static System.Console;

class customrules
{
    public static void Run()
    {
        {
            // Read file
            var lines = new LocalizationReaderYaml.File("pluralization/customrules2.yaml");
            // Create line databse
            ILocalizationLines db = new LocalizationLines().AddLines(lines);
            // Create localized text provider
            var localizedTextProvider = new LocalizedTextProvider(db.Query).Cached();
            // Get localized text
            ILocalizedText localizedText = localizedTextProvider[("", "Namespace.Apples")];
            // Get culture info
            CultureInfo cultureInfo = CultureInfo.InvariantCulture;
            // Print text
            for (int apples = 0; apples < 4; apples++)
                WriteLine(localizedText.Print(cultureInfo, new object?[] { apples }));
        }
    }
}

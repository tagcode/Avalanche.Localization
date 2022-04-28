using System.Globalization;
using Avalanche.Localization;
using Avalanche.Localization.Pluralization;
using Avalanche.Utilities;
using static System.Console;

class pluralization_invariantculture
{
    public static void Run()
    {
        {
            // Load lines from file
            Localization.Default.Lines.Lines.AddRange(new LocalizationReaderYaml.File("pluralization/invariantculture.yaml"));
            // Get localizable text
            var localizableText = Localization.Default.LocalizableText["Namespace.ExitRoundabout"];
            // Get culture info
            CultureInfo cultureInfo = CultureInfo.InvariantCulture;
            // Print text
            for (int exit = 1; exit <= 4; exit++) WriteLine(localizableText.Print(cultureInfo, new object?[] { exit }));
            // Take first exit.
            // Take second exit.
            // Take third exit.
            // Take 4th exit.
        }
    }
}

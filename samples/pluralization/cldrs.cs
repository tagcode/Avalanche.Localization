using System.Globalization;
using Avalanche.Localization;
using Avalanche.Localization.Pluralization;
using Avalanche.Utilities;
using static System.Console;

class cldrs
{
    public static void Run()
    {
        {
            IPluralRules rules = CLDRs.CLDR;
        }
        {
            IPluralRules rules = CLDRs.CLDR40;
        }
        {
            IPluralRules rules = CLDRs.All;
        }

        {
            IEnumerable<IPluralRule> reader = CLDRs.PluralReaders(".");
            IPluralRules rules = new PluralRules(reader);
        }
        {
            // Choose newest ruleset
            string ruleset = PluralRuleInfo.NEWEST;
            // Get 'de' rules evaluator for 'cardinal' values
            IPluralRulesEvaluator evaluator = CLDRs.All.EvaluatorCached[(ruleset, "cardinal", "de", null, null)];
            // Get culture
            IFormatProvider culture = CultureInfo.GetCultureInfo("de");
            // Create text
            TextNumber textNumber = new TextNumber("100", culture);
            // Evaluate plurality
            IPluralRule[]? matches = evaluator.Evaluate<TextNumber>(textNumber);
            // Get best matching plurality case
            string? pluralityCase = matches?[0]?.Info.Case; // "other"
        }

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

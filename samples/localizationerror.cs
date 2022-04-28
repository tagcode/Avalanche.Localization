using Avalanche.Localization;
using Avalanche.Localization.Internal;
using Avalanche.Message;
using Avalanche.Template;
using Avalanche.Utilities;
using static System.Console;

class localizationerror
{
    public static void Run()
    {
        {
            WriteLine(LocalizationMessages.Instance.ReadSummaryXmls());
        }
        {
            // File reader
            var lines = new LocalizationReaderYaml.File(@"error.yaml");
            // Create line database
            LocalizationLines db = new LocalizationLines().AddLines(lines);
            // Create provider
            var localizedTextProvider = new LocalizedTextProvider(db.Query, LocalizationLinesInfoProvider.Cached);
            // Get localization text
            ILocalizedText localizedText = localizedTextProvider[("en", "Namespace.CatsDogs")];
            // Print parse errors
            foreach (ILocalizationError error in localizedText.Errors)
                WriteLine(error);
            foreach (ILocalizationError error in localizedText.Errors)
            {
                WriteLine(error.Code);
                WriteLine(error.Message);
                WriteLine(error.Key);
                WriteLine(error.Culture);
                WriteLine(error.Text.Position.FileName);
                WriteLine(error.Text.Position.Start.Line);
                WriteLine(error.Text.Position.Start.Column);
                WriteLine(error.Text.Position.End.Line);
                WriteLine(error.Text.Position.End.Column);
            }
        }
        {
            // Create localization
            ILocalization localization = new Localization().AddLines(new LocalizationReaderYaml.File(@"error.yaml"));
            // Create error handler (Typically logger)
            ILocalizationErrorHandler errorHandler = new LocalizationErrorHandler(error => Console.WriteLine(error));
            // Add error handler
            localization.ErrorHandlers.Add(errorHandler);
            // Get localization text
            ILocalizedText text = localization.LocalizedTextCached[("en", "Namespace.CatsDogs")];
            // Print errors
            foreach (ILocalizationError error in text.Errors) WriteLine(error);
        }
        {
            // Create localization
            ILocalization localization = new Localization().AddLines(new LocalizationReaderYaml.File(@"error.yaml"));
            // Validate all sources
            IEnumerable<ILocalizationError> errors = localization.LocalizationLinesInfosQuery[(culture: null, key: null)].SelectMany(info => info.Errors);
            // Print errors
            foreach (ILocalizationError error in errors) WriteLine(error);
        }
        {
            // Create localization
            ILocalization localization = new Localization().AddLines(new LocalizationReaderYaml.File(@"error.yaml"));
            // Create error handler (Typically logger)
            ILocalizationErrorHandler errorHandler = new LocalizationErrorHandler(error => Console.WriteLine(error));
            // Add error handler
            localization.ErrorHandlers.Add(errorHandler);
            // Get localization text
            ILocalizedText text = localization.LocalizedTextCached[("en", "Namespace.CatsDogs")];
            // Print parse errors
            foreach (ILocalizationError error in text.Errors)
            {
                IMessage message = error.AsMessage();
                WriteLine(message); // TextMalformed Malformed Text="{cats} cats and {dogs dogs" en Namespace.CatsDogs localizationfile\error.yaml [Ln 9, Col 27, Ix 174 - Ln 9, Col 28, Ix 175]
            }
        }
    }
}

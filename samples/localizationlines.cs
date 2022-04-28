using System.Globalization;
using Avalanche.Localization;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;
using static System.Console;

class localizationlines
{
    public static void Run()
    {
        {
            ILocalization localization = new Localization();
            ILocalizationLines localizationLines = localization.Lines;
        }

        {
            ILocalization localization = new Localization();
            var line = new Dictionary<string, MarkedText> { { "TemplateFormat", "BraceNumeric" }, { "Culture", "" }, { "Key", "Namespace.Apples" }, { "Text", "You've got {0} apple(s)." } };
            localization.Lines.AddLine(line);
        }
        {
            ILocalization localization = new Localization();
            localization.Lines.AddLine(culture: "", key: "Namespace.Apples", templateFormat: "BraceNumeric", text: "You've got {0} apple(s).");
        }
        {
            // Create localization
            ILocalization localization = new Localization();
            // Create provider
            IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> provider =
                new ResourceManagerLineProvider(samples.Resources.Resource1.ResourceManager);
            // Add provider (both are required)
            localization.Lines.LineProviders.Add(provider);
            localization.Lines.LineProvidersCached.Add(provider.ValueResultCaptured().Cached().ValueResultOpened());
            // Get text
            ILocalizedText? text = localization.LocalizableTextCached["samples.Resources.Resource1.Apples"];
            // Print text
            WriteLine(text.Print(CultureInfo.InvariantCulture, new object[] { 1 }));
        }
        {
            ILocalization localization = Localization.Default;
            IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines = localization.Lines.QueryCached[("fi", "Namespace.Apples")];
        }
        {
            ILocalization localization = Localization.Default;
            IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines = localization.Lines.QueryCached[("fi", "Namespace.Apples")];
        }
    }
}

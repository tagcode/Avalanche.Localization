using System.Globalization;
using Avalanche.Localization;
using Avalanche.Utilities.Provider;
using static System.Console;

class fallbackcultureprovider
{
    public static void Run()
    {        
        {
            // Get culture provider
            IProvider<string, string[]> fallbackCultureProvider = Localization.Default.FallbackCultureProvider;
            // Retrieve fallback cultures
            string[] fallbackCultures = fallbackCultureProvider["fi-FI"];
            // Print fallback cultures in evaluation order
            WriteLine($"\"{String.Join("\" → \"", fallbackCultures)}\""); // "fi-FI" → "fi" → ""
        }
        {
            // Get culture provider
            IProvider<string, string[]> fallbackCultureProvider = Localization.Default.FallbackCultureProvider;
            // Retrieve fallback cultures
            string[] fallbackCultures = fallbackCultureProvider["fi-FI"];
            // Print fallback cultures in evaluation order
            WriteLine($"\"{String.Join("\" → \"", fallbackCultures)}\""); // "fi-FI" → "fi" → "en-UK" → "en" → ""
        }
        {
            // Create localization
            ILocalization localization = new Localization()
                .SetFallbackCultureProvider(new FallbackCultureProvider("sv"));
            // Get fallback cultures
            string[] fallbackCultures = localization.FallbackCultureProvider["fi-FI"];
            // Print fallback cultures in evaluation order
            WriteLine($"\"{String.Join("\" → \"", fallbackCultures)}\""); // "fi-FI" → "fi" → "sv" → ""
        }
        {
            // Create localization
            ILocalization localization = new Localization();
            // Assign culture provider "en"
            localization.FallbackCultureProvider = FallbackCultureProvider.Get("en");
            // Get fallback cultures
            string[] fallbackCultures = localization.FallbackCultureProvider["fi-FI"];
            // Print fallback cultures in evaluation order
            WriteLine($"\"{String.Join("\" → \"", fallbackCultures)}\""); // "fi-FI" → "fi" → "en" → ""
        }
        {
            // Create localization
            ILocalization localization = new Localization();
            // Assign culture provider "en"
            localization.FallbackCultureProvider = FallbackCultureProvider.Invariant;
            // Get fallback cultures
            string[] fallbackCultures = localization.FallbackCultureProvider["fi-FI"];
            // Print fallback cultures in evaluation order
            WriteLine($"\"{String.Join("\" → \"", fallbackCultures)}\""); // "fi-FI" → "fi" → "en" → ""
        }
        {
            // Create fallback provider function
            Func<string, string[]> func = culture => new string[] { culture, "en", "" }.Distinct().ToArray();
            // Create provider
            IProvider<string, string[]> fallbackCultureProvider = Providers.Func(func).Cached();
            // Get fallback cultures
            string[] fallbackCultures = fallbackCultureProvider["fi"];
            // Print fallback cultures in evaluation order
            WriteLine($"\"{String.Join("\" → \"", fallbackCultures)}\""); // "fi" → "en" → ""
        }
        {
            // Get culture provider
            IProvider<string, string[]> fallbackCultureProvider = FallbackCultureProvider.NoFallback;
            // Retrieve fallback cultures
            string[] fallbackCultures = fallbackCultureProvider["fi-FI"];
            // Print fallback cultures in evaluation order
            WriteLine($"\"{String.Join("\" → \"", fallbackCultures)}\""); // "fi-FI"
        }
        {
            // Create localization
            ILocalization localization = new Localization()
                .SetFallbackCultureProvider(FallbackCultureProvider.Default)
                .AddLines(new LocalizationReaderYaml.File("fallbackcultureprovider.yaml"));
            // Get localizable text
            ILocalizableText localizable = localization.LocalizableTextCached["Namespace.HelloCasual"];
            // Cultures to print with
            foreach (string requested_culture in new string[] { "en", "en-NZ", "en-AU", "en-UK", "en-US", "en-CA" })
            {
                // Localize to culture
                ILocalizedText localized = localizable.Localize(requested_culture)?.Value!;
                // Get the culture where fallback landed to
                string supplied_culture = localized.Culture;
                // Print text
                WriteLine($"{requested_culture,-5} {supplied_culture,-5}: {localized.Print(null)}");
            }
            // en    en   : Hello fella
            // en-NZ en   : Hello fella
            // en-AU en-AU: Hello mate
            // en-UK en-UK: Hello chap
            // en-US en-US: Hello dude
            // en-CA en-CA: Hello buddy
        }
    }
}

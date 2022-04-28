using System.Globalization;
using Avalanche.Localization;
using static System.Console;

class alphabet
{
    public static void Run()
    {
        {
            // Get fallback cultures for "sr-Cyrl-RS"
            string[] fallbackCultures = FallbackCultureProvider.Default["sr-Cyrl-RS"];
            // Print 
            WriteLine($"\"{String.Join("\" → \"", fallbackCultures)}\""); // "sr-Cyrl-RS" → "sr-Cyrl" → "sr" → ""
        }
        {
            // Get fallback cultures for "sr-Latn-RS"
            string[] fallbackCultures = FallbackCultureProvider.Default["sr-Latn-RS"];
            // Print
            WriteLine($"\"{String.Join("\" → \"", fallbackCultures)}\""); // "sr-Latn-RS" → "sr-Latn" → "sr" → ""
        }
        {
            // Create localization
            ILocalization localization = Localization.CreateDefault();
            // Create localizable text for "HelloWorld"
            ILocalizableText hello = localization.LocalizableTextCached["Localization.Example.HelloWorld"];
            // Print for "en" which fallbacks to invariant ""
            WriteLine(hello.Print(CultureInfo.GetCultureInfo("en"), null)); // "Hello World"
            // Print for "sr"
            WriteLine(hello.Print(CultureInfo.GetCultureInfo("sr"), null)); // "Zdravo Svete"
            // Print for "sr-Latn-RS" which fallbacks to "sr"
            WriteLine(hello.Print(CultureInfo.GetCultureInfo("sr-Latn-RS"), null)); // "Zdravo Svete"
            // Print for "sr-Cyrl-RS" which fallbacks to "sr-Cyrl"
            WriteLine(hello.Print(CultureInfo.GetCultureInfo("sr-Cyrl-RS"), null)); // "Здраво Свете"
        }
        {
            // Create localization
            ILocalization localization = Localization.CreateDefault();
            // Create localizable text for "HelloWorld"
            ILocalizableText hello = localization.LocalizableTextCached["Localization.Example.HelloWorld"];
            // Localize to cyrillic
            ILocalizedText hello_cyrl = hello.Localize("sr-Cyrl-RS")?.Value!;
            // Get the culture where fallback landed to
            string hello_cyrl_culture = hello_cyrl.Culture;
            // Print culture
            WriteLine(hello_cyrl_culture); // "sr-Cyrl"
        }
    }
}

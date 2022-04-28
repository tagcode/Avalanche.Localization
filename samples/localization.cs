using System.Globalization;
using Avalanche.Localization;
using Avalanche.Template;
using static System.Console;

class localization_index
{
    public static void Run()
    {
        {
            // Get default localization
            ILocalization localization = Localization.Default;
        }
        {
            // Create localization
            ILocalization localization = Localization.CreateDefault();
        }
        {
            ILocalization localization = new Localization()
                .AddLine("", "Namespace.Apples", "BraceNumeric", "You've got {0} apple(s).")
                .AddLine("en", "Namespace.Apples", "BraceNumeric", "You've got an apple.", "Unicode.CLDR40", "0:cardinal:one:en")
                .AddLine("en", "Namespace.Apples", "BraceNumeric", "You've got {0} apples.", "Unicode.CLDR40", "0:cardinal:other:en")
                .AddLine("fi", "Namespace.Apples", "BraceNumeric", "Sinulla on omena.", "Unicode.CLDR40", "0:cardinal:one:fi")
                .AddLine("fi", "Namespace.Apples", "BraceNumeric", "Sinulla on {0} omenaa.", "Unicode.CLDR40", "0:cardinal:other:fi");

            ITemplateFormatPrintable printable = localization.LocalizableText["Namespace.Apples"];
            WriteLine(printable.Print(CultureInfo.GetCultureInfo("en"), new object[] { 2 })); // You've got 2 apples.
            WriteLine(printable.Print(CultureInfo.GetCultureInfo("fi"), new object[] { 2 })); // Sinulla on 2 omenaa.
        }
        {
            ILocalization localization =
                Localization.CreateDefault()
                    .AddFilePatterns("Localization/{Key}", "Localization/{Culture}/{Key}");
        }
        {
            ILocalization localization =
                Localization.CreateDefault()
                .AddFilePatterns("Localization/**/{Key}", "Localization/**/{Culture}/{Key}");
        }
        {
            ILocalization localization = new Localization()
                .AddLine("", "Namespace.Apples", "BraceNumeric", "You've got {0} apple(s).")
                .AddLine("en", "Namespace.Apples", "BraceNumeric", "You've got an apple.", "Unicode.CLDR40", "0:cardinal:one:en")
                .AddLine("en", "Namespace.Apples", "BraceNumeric", "You've got {0} apples.", "Unicode.CLDR40", "0:cardinal:other:en")
                .AddLine("fi", "Namespace.Apples", "BraceNumeric", "Sinulla on omena.", "Unicode.CLDR40", "0:cardinal:one:fi")
                .AddLine("fi", "Namespace.Apples", "BraceNumeric", "Sinulla on {0} omenaa.", "Unicode.CLDR40", "0:cardinal:other:fi");
            // Get localizable text
            ILocalizedText text = localization.LocalizableTextCached["Namespace.Apples"];
            // Create arguments
            object[] arguments = { 2 };
            // Print
            WriteLine(text.Print(CultureInfo.GetCultureInfo("fi"), arguments)); // "Sinulla on 2 omenaa."
        }
        {
            // Create localization (Reads "Resources/Namespace.yml")
            ILocalization localization = new Localization()
                .SetFallbackCultureProvider(FallbackCultureProvider.Invariant)
                .AddFileSystem(LocalizationFileSystem.ApplicationRoot)
                .AddFilePatterns("Resources/{Key}", "Resources/{Culture}/{Key}")
                .AddFileFormats(LocalizationFileFormatYaml.Instance, LocalizationFileFormatXml.Instance, LocalizationFileFormatJson.Instance);
            // Get text that uses current culture
            ILocalizedText text = localization.LocalizingTextCached[(CultureProvider.CurrentCulture.Instance, "Namespace.Apples")];
            // Assign current culture
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            // Create arguments
            object[] arguments = { 2 };
            // Print with current culture
            WriteLine(text.Print(arguments)); // "Sinulla on 2 omenaa."
        }
        {
            // Create localization (Reads "Resources/Namespace.yml")
            ILocalization localization = new Localization()
                .SetFallbackCultureProvider(FallbackCultureProvider.Invariant)
                .AddFileSystem(LocalizationFileSystem.ApplicationRoot)
                .AddFilePatterns("Resources/{Key}", "Resources/{Culture}/{Key}")
                .AddFileFormats(LocalizationFileFormatYaml.Instance, LocalizationFileFormatXml.Instance, LocalizationFileFormatJson.Instance);
            // Get text that uses specific culture
            ILocalizedText text = localization.LocalizedTextCached[("fi", "Namespace.Apples")];
            // Assign current culture to english (ignored)
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en");
            // Create arguments
            object[] arguments = { 2 };
            // Print with the assigned culture "fi"
            WriteLine(text.Print(arguments)); // "Sinulla on 2 omenaa."
        }
        {
            // Create text
            ITemplateText text = new TemplateText("You've got {0} apple(s).", TemplateFormat.BraceNumeric);
            // Decorate text to localize to language in 'formatProvider'
            text = Localization.Default.Localize(text, "Namespace.Apples");

            // Print
            object[] arguments = { 2 };
            WriteLine(text.Print(null, arguments));     // "You've got {0} apples."
            WriteLine(text.Print(formatProvider: CultureInfo.GetCultureInfo("en"), arguments)); // "You've got 2 apples."
            WriteLine(text.Print(formatProvider: CultureInfo.GetCultureInfo("fi"), arguments)); // "Sinulla on 2 omenaa."
            WriteLine(text.Print(formatProvider: CultureInfo.GetCultureInfo("sv"), arguments)); // "Du har 2 äpplen."
        }
        {
            // Create text
            ITemplateText text = new TemplateText("You've got {0} apple(s).", TemplateFormat.BraceNumeric);
            // Get active culture provider (CurrentThread)
            ICultureProvider cultureProvider = CultureProvider.Default;
            // Decorate text to localize to the active culture
            text = Localization.Default.Localize(text, "Namespace.Apples", cultureProvider);

            // Print
            object[] arguments = { 2 };
            cultureProvider.Culture = "en";
            WriteLine(text.Print(formatProvider: null, arguments));                              // "You've got 2 apple(s)."
            cultureProvider.Culture = "sv";
            WriteLine(text.Print(formatProvider: null, arguments));                              // "Du har 2 äpplen."
            cultureProvider.Culture = "fi";
            WriteLine(text.Print(formatProvider: null, arguments));                              // "Sinulla on 2 omenaa."
        }
    }
}

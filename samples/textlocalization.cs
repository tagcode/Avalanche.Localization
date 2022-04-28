using System.Globalization;
using Avalanche.Localization;
using static System.Console;

class textlocalization
{
    public static void Run()
    {
        {
            // Get text localized to a language
            ILocalizedText text = Localization.Default.LocalizedTextCached[(culture: "en", key: "Namespace.Today")];
            // Print with 'fi' format provider
            string print = text.Print(formatProvider: CultureInfo.GetCultureInfo("fi"), new object[] { DateTime.Now });
            // Write to console
            WriteLine(print); // "Today is 24.3.2022 13.34.07."
        }
        {
            // Get text localized to a language and format provider
            ILocalizedText text = Localization.Default.FormatLocalizedTextCached[((culture: "en", format: CultureInfo.GetCultureInfo("fi")), key: "Namespace.Today")];
            // Print with the pre-assigned format provider ("fi")
            string print = text.Print(formatProvider: null, new object[] { DateTime.Now });
            // Write to console
            WriteLine(print); // "Today is 24.3.2022 13.34.07."
        }
        {
            // Get text
            ILocalizableText localizable = Localization.Default.LocalizableTextCached[key: "Namespace.Today"];

            // Get format
            IFormatProvider format = CultureInfo.GetCultureInfo("fi");
            // Print to language indicated by format
            string print = localizable.Print(formatProvider: format, new object[] { DateTime.Now });
            // Write to console
            WriteLine(print); // "Tänään on 24.3.2022 13.34.07."

            // Localize to "fi"
            ILocalizedText localized_fi = localizable.Localize("fi")!.Value;
            // Print
            WriteLine(localized_fi.Print(new object[] { DateTime.Now })); // "Tänään on 24.3.2022 13.34.07."
        }
        {
            // Create cultrue provider
            ICultureProvider cultureProvider = new CultureProvider("en");
            // Get automatically localizing text
            ILocalizingText localizing = Localization.Default.LocalizingTextCached[(cultureProvider, key: "Namespace.Today")];
            // Assign language
            cultureProvider.Culture = "en";
            // Assign format
            cultureProvider.Format = CultureInfo.GetCultureInfo("fi");

            // Print to active culture
            string print = localizing.Print(new object[] { DateTime.Now });
            // Write to console
            WriteLine(print); // "Today is 24.3.2022 13.34.07."

            // Localize to active culture (culture="en", format="fi")
            ILocalizedText localized = localizing.Value!.Value;
            // Print
            WriteLine(localized.Print(new object[] { DateTime.Now })); // "Today is 24.3.2022 13.34.07."
        }
        {
            // Create culture provider
            ICultureProvider cultureProvider = new CultureProvider("en");
            // Create text localizer
            ITextLocalizer textLocalizer = new TextLocalizer(Localization.Default, cultureProvider);
            // Assign language
            cultureProvider.Culture = "en";
            // Assign format
            cultureProvider.Format = CultureInfo.GetCultureInfo("fi");
            // Create localized text
            ILocalizedText text = textLocalizer["Namespace.Today"]!;
            // Print to active culture (culture="en", format="fi")
            string print = text.Print(new object[] { DateTime.Now }); 
            // Write
            WriteLine(print); // "Today is 24.3.2022 13.34.07."
        }
        {
            // Create cultrue provider
            ICultureProvider cultureProvider = new CultureProvider("en");
            // Create text localizer
            ITextLocalizer textLocalizer = new TextLocalizer(Localization.Default, cultureProvider, "Namespace");
            // Assign language
            cultureProvider.Culture = "en";
            // Assign format
            cultureProvider.Format = CultureInfo.GetCultureInfo("fi");
            // Create localized text (culture="en", format="fi")
            ILocalizedText text = textLocalizer["Today"]!;
            // Print to active culture
            string print = text.Print(new object[] { DateTime.Now });
            // Write
            WriteLine(print); // "Today is 24.3.2022 13.34.07."
        }
    }
}

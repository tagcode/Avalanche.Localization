using System.Globalization;
using Avalanche.Localization;
using Avalanche.Template;
using Avalanche.Utilities;
using static System.Console;

class index
{
    public static void Run()
    {
        {
            // Create text
            ILocalizedText text = Localization.Default.LocalizableTextCached["Namespace.Apples"];
            // Get culture
            IFormatProvider culture = CultureInfo.GetCultureInfo("en");
            // Create arguments
            object[] arguments = { 2 };
            // Print
            WriteLine(text.Print(culture, arguments)); // "You've got 2 apples."
        }
        {
            // Create text
            ILocalizedText text = Localization.Default.LocalizableTextCached["Namespace.Apples"];
            // Get culture
            IFormatProvider culture = CultureInfo.GetCultureInfo("sv");
            // Create arguments
            object[] arguments = { 2 };
            // Print
            WriteLine(text.Print(culture, arguments)); // "Du har 2 äpplen."
        }
        {
            // Get active culture provider (CurrentThread)
            ICultureProvider cultureProvider = CultureProvider.CurrentCulture.Instance;
            // Get localizing text
            ILocalizedText text = Localization.Default.LocalizingTextCached[(cultureProvider, "Namespace.Apples")];
            // Assign language
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("sv");
            // Assign format provider
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("sv");
            // Print to active culture
            WriteLine(text.Print(new object[] { 2 })); // "Du har 2 äpplen."
        }
    }
}

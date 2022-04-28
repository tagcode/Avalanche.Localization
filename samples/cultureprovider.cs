using System.Globalization;
using Avalanche.Localization;
using Avalanche.Template;
using Avalanche.Utilities;
using static System.Console;

class cultureprovider
{
    public static void Run()
    {
        {
            // Create provider
            CultureProvider cultureProvider = new CultureProvider("en");
            // Re-assign culture
            cultureProvider.Culture = "fi";
            cultureProvider.Format = CultureInfo.GetCultureInfo("fi");
        }
        {
            ICultureProvider cultureProvider = new CultureProvider("en").SetReadOnly();
        }
        {
            ICultureProvider cultureProvider = CultureProvider.En.Instance;
        }
        {
            ICultureProvider cultureProvider = CultureProvider.InvariantCulture.Instance;
        }
        {
            // Get culture provider
            ICultureProvider cultureProvider = CultureProvider.CurrentCulture.Instance;
            // Assign language
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("fi");
            // Assign format
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            // Get active (language, format)
            (string language, IFormatProvider format) = cultureProvider.Set();
        }
        {
            // Get default culture provider (same as CultureInfo.CurrentCulture)
            ICultureProvider cultureProvider = CultureProvider.Default;
            // Assign application's current language
            cultureProvider.Culture = "fi";
            // Assign application's current format
            cultureProvider.Format = CultureInfo.GetCultureInfo("fi");
            // Get active (language, format)
            (string language, IFormatProvider format) = cultureProvider.Set();
        }
        {
            // Get culture provider
            ICultureProvider cultureProvider = CultureProvider.CurrentThread.Instance;
            // Assign language
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("fi");
            // Assign format
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            // Get active (language, format)
            (string language, IFormatProvider format) = cultureProvider.Set();
        }
        {
            // Get culture provider
            ICultureProvider cultureProvider = CultureProvider.DefaultThread.Instance;
            // Get default thread culture (language, format)
            (string language, IFormatProvider format) = cultureProvider.Set();
        }
        {
            ICultureProvider cultureProvider = new CultureProvider.Func(() => (CultureInfo.CurrentUICulture.Name, CultureInfo.CurrentCulture));
        }
        {
            ICultureProvider cultureProvider = new CultureProvider("en");
            ILocalizedText text = Localization.Default.LocalizingTextCached[(cultureProvider, "Namespace.Apples")];
            WriteLine(text.Print(new object[] { 2 })); // "You've got 2 apples." 
            cultureProvider.Culture = "fi";
            WriteLine(text.Print(new object[] { 2 })); // "Sinulla on 2 omenaa."
        }
        {
            ICultureProvider cultureProvider = new CultureProvider("en");
            ITemplateText text = new TemplateText("You've got {0} apple(s).");
            text = Localization.Default.Localize(text, "Namespace.Apples", cultureProvider);
            WriteLine(text.Print(null, new object[] { 2 })); // "You've got 2 apples." 
            cultureProvider.Culture = "fi";
            WriteLine(text.Print(null, new object[] { 2 })); // "Sinulla on 2 omenaa."
        }
    }
}

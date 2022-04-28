using System.Globalization;
using Avalanche.Localization;
using static System.Console;

class unitprefix
{
    public static void Run()
    {
        {
            // Create localization context
            ILocalization localization = Localization.CreateDefault();
            // Localize to 'fr' 
            ILocalizedText pommes = localization.LocalizedTextCached[("fr", "Namespace.Apples")];

            // 1200000
            string print = pommes.Pluralize(CultureInfo.InvariantCulture, new object[] { "1200000" }).Print(null, new object[] { "1200000" });
            WriteLine(print); // "Vous avez 1200000 pommes."

            // 1,2M
            print = pommes.Pluralize(CultureInfo.InvariantCulture, new object[] { "1.2c6" }).Print(null, new object[] { "1,2M" });
            WriteLine(print); // "Vous avez 1,2M de pommes."
        }
    }
}

using System.Globalization;
using Avalanche.Localization;
using Avalanche.Template;
using static System.Console;

class emplacement  // emplacement
{
    public static void Run()
    {
        {
            // Create localization
            ILocalization localization = Localization.CreateDefault();
            // Get texts
            ILocalizableText and = localization.LocalizableTextCached["samples.emplacement.and"]; // "{0} and {1}"
            ILocalizableText cat = localization.LocalizableTextCached["samples.emplacement.cat"]; // "{count} cats"
            ILocalizableText dog = localization.LocalizableTextCached["samples.emplacement.dog"]; // "{count} dogs"

            // "{0_count} cats and {1_count} dogs"
            ILocalizableText catAndDog = (ILocalizableText) and.Place("0", cat, "1", dog);

            // Print "en"
            CultureInfo en = CultureInfo.GetCultureInfo("en");
            foreach (int catCount in new int[] { 0, 1, 2 })
                foreach(int dogCount in new int[] { 0, 1, 2 })
                    WriteLine(catAndDog.Print(en, new object[] { catCount, dogCount }));
            WriteLine();

            // Print "fi"
            CultureInfo fi = CultureInfo.GetCultureInfo("fi");
            foreach (int catCount in new int[] { 0, 1, 2 })
                foreach(int dogCount in new int[] { 0, 1, 2 })
                    WriteLine(catAndDog.Print(fi, new object[] { catCount, dogCount })); 
        }
        {
            // Create localization
            ILocalization localization = Localization.CreateDefault();
            // Get texts
            ILocalizableText and  = localization.LocalizableTextCached["samples.emplacement.and"];   // "{0} and {1}"
            ILocalizableText cat  = localization.LocalizableTextCached["samples.emplacement.cat"];   // "{count} cats"
            ILocalizableText pony = localization.LocalizableTextCached["samples.emplacement.pony"];  // "a pony"

            // "{0_count} cats and a pony"
            ILocalizableText catAndPony = (ILocalizableText) and.Place("0", cat, "1", pony);

            // Print "en"
            CultureInfo en = CultureInfo.GetCultureInfo("en");
            foreach (int catCount in new int[] { 0, 1, 2 })
                WriteLine(catAndPony.Print(en, new object[] { catCount }));
            WriteLine();

            // Print "fi"
            CultureInfo fi = CultureInfo.GetCultureInfo("fi");
            foreach (int catCount in new int[] { 0, 1, 2 })
                WriteLine(catAndPony.Print(fi, new object[] { catCount }));
        }
        {
            // Create localization
            ILocalization localization = Localization.CreateDefault();
            // Get texts
            ILocalizableText and  = localization.LocalizableTextCached["samples.emplacement.and"];   // "{0} and {1}"
            ILocalizableText cat  = localization.LocalizableTextCached["samples.emplacement.cat"];   // "{count} cats"
            ILocalizableText pony = localization.LocalizableTextCached["samples.emplacement.pony"];  // "a pony"

            // "{0_count} cats and {1}"
            ILocalizableText catAnd = (ILocalizableText) and.Place("0", cat);

            // Print "en"
            CultureInfo en = CultureInfo.GetCultureInfo("en");
            foreach (int catCount in new int[] { 0, 1, 2 })
                WriteLine(catAnd.Print(en, new object[] { catCount, pony }));
            WriteLine();

            // Print "fi"
            CultureInfo fi = CultureInfo.GetCultureInfo("fi");
            foreach (int catCount in new int[] { 0, 1, 2 })
                WriteLine(catAnd.Print(fi, new object[] { catCount, pony }));
        }
        {
            // Create localization
            ILocalization localization = Localization.CreateDefault();
            // Get texts
            ILocalizableText and = localization.LocalizableTextCached["samples.emplacement.and"]; // "{0} and {1}"
            ILocalizableText cat = localization.LocalizableTextCached["samples.emplacement.cat"]; // "{count} cats"
            ILocalizableText dog = localization.LocalizableTextCached["samples.emplacement.dog"]; // "{count} dogs"

            // Create localizable "2 cats", "3 dogs"
            ILocalizableText cats2 = (ILocalizableText)cat.Place("count", new ParameterlessText("2"));
            ILocalizableText dogs3 = (ILocalizableText)dog.Place("count", new ParameterlessText("3"));

            // Print "{0} and {1}"
            WriteLine(and.Print(null, new object[] { cats2, dogs3 })); // "2 cats and 3 dogs"
            CultureInfo fi = CultureInfo.GetCultureInfo("fi");
            WriteLine(and.Print(fi, new object[] { cats2, dogs3 }));   // "2 kissaa ja 3 koiraa"
        }
    }
}

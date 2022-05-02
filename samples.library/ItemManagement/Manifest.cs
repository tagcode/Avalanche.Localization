namespace samples.library;
using System.Globalization;
using System.Text;
using static System.Console;

public class Manifest : List<Manifest.Item>
{
    /// <summary>Manifest item</summary>
    public record Item(string Identity, int Count);

    /// <summary>Header text</summary>
    static ILocalizableText Header = Localization.Default.LocalizableText["samples.library.ItemManagement.Manifest.Header"];
    /// <summary>Spacer</summary>
    static ILocalizableText Spacer = Localization.Default.LocalizableText["samples.library.ItemManagement.Manifest.Spacer"];
    /// <summary>Line</summary>
    static ILocalizableText Line = Localization.Default.LocalizableText["samples.library.ItemManagement.Manifest.Line"];

    /// <summary>Print items in manifest in language specified in <paramref name="format"/>.</summary>
    public string ToString(IFormatProvider format)
    {
        //
        StringBuilder sb = new StringBuilder();
        // Append header
        Header.AppendTo(sb, format); sb.AppendLine();
        // Append spacer
        Spacer.AppendTo(sb, format); sb.AppendLine();
        // Append each item
        foreach (Item item in this)
        {
            Line.AppendTo(sb, format, new object[] { item.Identity, item.Count });
            sb.AppendLine();
        }
        // Print table
        return sb.ToString();
    }

    /// <summary>Print items in manifest. Uses current UI culture.</summary>
    public override string ToString() => ToString(CultureInfo.CurrentUICulture);

    /// <summary>Test run for debugging.</summary>
    public static void Run()
    {
        // Create manifest
        Manifest manifest = new Manifest();
        manifest.Add(new Item("#5435", 10));
        manifest.Add(new Item("#9639", 20));

        // Print manifest
        WriteLine(manifest.ToString(CultureInfo.GetCultureInfo("en")));
        // Print manifest
        WriteLine(manifest.ToString(CultureInfo.GetCultureInfo("fi")));
        /*
        Identity | Count
        --------------------
           #5435 |        10
           #9639 |        20

        Nimike   | Lukumäärä
        --------------------
           #5435 |        10
           #9639 |        20
         */
    }
}

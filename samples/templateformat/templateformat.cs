using System.Globalization;
using Avalanche.Localization;
using Avalanche.Template;
using YamlDotNet.RepresentationModel;
using static System.Console;

class templateformat
{
    public static void Run()
    {
        {
            // Get template format
            ITemplateFormat dash = DashTemplateFormat.Instance;
            // Create localization
            ILocalization localization = new Localization()
                .AddTemplateFormat(dash)
                .AddLines(new LocalizationReaderYaml.File("templateformat/templateformat6.yaml"));
            // Get text
            ITemplateFormatPrintable text = localization.LocalizableTextCached["Namespace.Apples"];
            // Print
            WriteLine(text.Print(CultureInfo.InvariantCulture, new object[] { 3 })); // You've got 3 apple(s).
        }
        {
            // Get template format
            ITemplateFormat dash = DashTemplateFormat.Instance;
            // Create localization
            ILocalization localization = new Localization()
                .AddTemplateFormat(dash)
                .AddLines(new LocalizationReaderYaml.File("templateformat/templateformat7.yaml"));
            // Get text
            ITemplateFormatPrintable text = localization.LocalizableTextCached["Namespace.Apples"];
            // Print
            WriteLine(text.Print(CultureInfo.InvariantCulture, new object[] { 3 })); // You've got 3 apple(s).
        }
    }
}

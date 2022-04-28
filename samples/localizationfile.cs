using System.Globalization;
using Avalanche.Localization;
using Avalanche.Template;
using Avalanche.Utilities;
using Microsoft.Extensions.FileProviders;
using static System.Console;

public class localizationfile
{
    public static void Run()
    {
        {
            // Create localization
            ILocalization localization = new Localization();
            // Create file reference
            ILocalizationFile localizationFile = new LocalizationFile
            {
                FileSystem = LocalizationFileSystem.ApplicationRoot,
                FileName = "Resources/Namespace.yaml",
                FileFormat = LocalizationFileFormatYaml.Instance,
                Culture = "",
                Key = "Namespace"
            }.SetReadOnly();
            // Add localization file (reference)
            localization.Files.Files.Add(localizationFile);

            // Query line
            ITemplateFormatPrintable printable = localization.LocalizableTextCached["Namespace.Apples"];
            // Print
            WriteLine(printable.Print(CultureInfo.GetCultureInfo("en"), new object[] { 2 })); // You've got 2 apples.
            WriteLine(printable.Print(CultureInfo.GetCultureInfo("fi"), new object[] { 2 })); // Sinulla on 2 omenaa.
        }


        {
            // Create localization
            ILocalization localization = new Localization();
            // Create file reference
            ILocalizationFile localizationFile = new LocalizationFileInMemory(new byte[] { 0, 1, 2, 3 })
            {
                FileName = "Resources/en/logo.svg",
                FileFormat = new LocalizationFileFormat(".svg"),
                Culture = "en",
                Key = "logo"
            }.SetReadOnly();
            // Add localization file (explicit)
            localization.Files.Files.Add(localizationFile);
            {
                // Query file
                IEnumerable<ILocalizationFile> files = localization.Files.QueryCached[("en", "logo")];
            }
            {
                // Query file
                ILocalizationFile? file = localization.Files.QueryCached[("en", "logo")].FirstOrDefault();
                // Read file
                if (file!.TryOpen(out Stream? stream)) 
                {
                    // ...

                    // Close 
                    stream.Dispose();
                }
            }
            {
                // Query file
                ILocalizationFile? file = localization.Files.QueryCached[("en", "logo")].FirstOrDefault();
                // Read file
                using Stream stream = file!.Open();
            }
            {
                // Query file
                ILocalizationFile? file = localization.Files.QueryCached[("en", "logo")].FirstOrDefault();
                // Read file
                byte[] data = file!.ReadFully();
            }
        }
    }
}

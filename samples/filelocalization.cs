using Avalanche.Localization;
using static System.Console;

class filelocalization
{
    public static void Run()
    {
        {
            // Get files localized to a language
            IEnumerable<ILocalizationFile> files = Localization.Default.FileQueryCached[(culture: "fi", key: "image")].ToArray();
            // Print file names
            foreach (var file in files)
                WriteLine($"\"{file.Culture}\": {file.FileName}"); // "fi":Resources/fi/image.png, "": Resources/image.png
        }
        {
            // Get file
            ILocalizable<ILocalizationFile> localizable = Localization.Default.LocalizableFileCached[key: "image"];
            // Localize to "fi"
            ILocalized<ILocalizationFile> file_fi = localizable.Localize("fi")!;
            // Print file name
            WriteLine($"\"{file_fi.Culture}\": {file_fi.Value.FileName}"); // "fi":Resources/fi/image.png            
        }
        {
            // Get cultrue provider
            ICultureProvider cultureProvider = new CultureProvider("en");
            // Get automatically localizing file
            ILocalizing<ILocalizationFile> localizing = Localization.Default.LocalizingFileCached[(cultureProvider, key: "image")];
            // Assign another language
            cultureProvider.Culture = "fi";
            // Localize to "fi"
            ILocalized<ILocalizationFile> file_fi = localizing.Value!;
            // Print file name
            WriteLine($"\"{file_fi.Culture}\": {file_fi.Value.FileName}"); // "fi":Resources/fi/image.png            
        }
        {
            // Get cultrue provider
            ICultureProvider cultureProvider = new CultureProvider("en");
            // Create file localizer
            IFileLocalizer localizer = new FileLocalizer(Localization.Default, cultureProvider, @namespace: null);
            // Assign another language
            cultureProvider.Culture = "fi";
            // Get localized file(s)
            ILocalized<ILocalizationFile[]> files = localizer["image"]!;
            foreach (var file in files.Value)
                WriteLine($"\"{file.Culture}\": {file.FileName}"); // "fi":Resources/fi/image.png, "": Resources/image.png
        }
    }
}

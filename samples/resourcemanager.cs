using System.Globalization;
using System.Resources;
using Avalanche.Localization;
using Avalanche.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using static System.Console;

class resourcemanager
{
    public static void Run()
    {
        {
            // Create localization and add resource manager provider
            ILocalization localization = new Localization()
                .AddResourceManagerProvider();
        }
        {
            // Create localization and add resource manager provider
            ILocalization localization = new Localization()
                .AddLineProvider(ResourceManagerLineProvider.AssemblyLoader, ResourceManagerLineProvider.AssemblyLoaderCached);
            // Get localizable text
            ILocalizedText text = localization.LocalizableTextCached["samples.Resources.Resource1.Apples"];
            // Print to localization
            WriteLine(text.Print(CultureInfo.InvariantCulture, new object[] { 1 })); // "You've got 1 apple(s)."
        }
        {
            // Create localization and add resource manager provider
            ILocalization localization = new Localization()
                .AddFileProvider(ResourceManagerFileProvider.AssemblyLoader, ResourceManagerFileProvider.AssemblyLoaderCached);
            // Get localizable file
            ILocalizable<ILocalizationFile> localizableFile = localization.LocalizableFileCached["samples.Resources.Resource1.logo"];
            // Localize to invariant ""
            ILocalizationFile? localizationFile = localizableFile.Localize("")?.Value;
            // Read file 
            byte[] data = localizationFile!.ReadFully();
        }        
        {
            // Create localization and add resource manager provider
            ILocalization localization = new Localization()
                .AddLineProvider(ResourceManagerLineProvider.AssemblyLoader, ResourceManagerLineProvider.AssemblyLoaderCached);
            // Get localizable text
            IEnumerable<ILocalizedText> texts = localization.LocalizableTextsQueryCached[null];
            // Print texts
            foreach(var text in texts)
                WriteLine(text.Print(CultureInfo.InvariantCulture, new object[] { 1 })); // "You've got 1 apple(s)."
        }
        {
            // Create localization and add resource manager provider
            ILocalization localization = new Localization()
                .AddFileProvider(ResourceManagerFileProvider.AssemblyLoader, ResourceManagerFileProvider.AssemblyLoaderCached);
            // Get localizable file
            IEnumerable<ILocalizable<ILocalizationFile>> localizableFiles = localization.LocalizableFilesQueryCached[null];
            // Visit each file
            foreach (var localizableFile in localizableFiles)
            {
                // Localize to invariant ""
                ILocalizationFile? localizationFile = localizableFile.Localize("")?.Value;
                // Read file 
                byte[] data = localizationFile!.ReadFully();
            }
        }


        {
            // Get resource manager
            ResourceManager resourcemanager = samples.Resources.Resource1.ResourceManager;
            // Create localization and add resource manager as provider
            ILocalization localization = new Localization().AddResourceManager(resourcemanager);
        }
        {
            // Get resource manager
            ResourceManager resourceManager = samples.Resources.Resource1.ResourceManager;
            // Add resource manager to localization singleton
            Localization.Default.AddResourceManager(resourceManager);
        }
        {
            ILocalization localization = new Localization().AddResourceManager(samples.Resources.Resource1.ResourceManager);
            ILocalizedText? text = localization.LocalizableTextCached["samples.Resources.Resource1.Apples"];
            WriteLine(text.Print(CultureInfo.InvariantCulture, new object[] { 1 }));
        }
        {
            ILocalization localization = new Localization().AddResourceManager(samples.Resources.Resource1.ResourceManager, "Namespace");
            ILocalizedText? text = localization.LocalizableTextCached["Namespace.Apples"];
            WriteLine(text.Print(CultureInfo.InvariantCulture, new object[] { 1 }));
        }
        {
            // Create Localization
            ILocalization localization = new Localization().AddResourceManager(samples.Resources.Resource1.ResourceManager);
            // Get file reference
            ILocalizationFile localizationFile = localization.FileQueryCached[("", "samples.Resources.Resource1.logo")].FirstOrDefault()!;
            // Read file
            byte[] data = localizationFile.ReadFully();
        }
        {
            // Create Localization
            ILocalization localization = new Localization().AddResourceManager(samples.Resources.Resource1.ResourceManager);
            // Get all files
            ILocalizationFile[] allLocalizationFiles = localization.FileQueryCached[(null, null)].ToArray()!;
        }
        {
            // Create Localization
            ILocalization localization = new Localization().AddResourceManager(samples.Resources.Resource1.ResourceManager);
            // Get localizable file
            ILocalizable<ILocalizationFile> localizable1 = localization.LocalizableFileCached["samples.Resources.Resource1.logo"];
            // Localize to invariant ""
            ILocalizationFile? localizationFile = localizable1.Localize("")?.Value;
            // Read file 
            byte[] data = localizationFile!.ReadFully();
        }
        {
            // Create Localization
            ILocalization localization = new Localization().AddResourceManager(samples.Resources.Resource1.ResourceManager);
            // Create localizing file
            ILocalizing<ILocalizationFile> localizing = localization.LocalizingFileCached[(CultureProvider.CurrentCulture.Instance, "samples.Resources.Resource1.logo")];
            // Localize to CultureProvider.CurrentCulture
            ILocalizationFile? localizationFile = localizing.Value?.Value;
            // Read file
            byte[] data = localizationFile!.ReadFully();
        }

    }
}

using System.Globalization;
using System.Resources;
using Avalanche.Localization;
using Avalanche.Localization.Pluralization;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using static System.Console;

class microsoft_extensions_dependencyinjection
{
    public static void Run()
    {
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddSingleton<IProvider<string, string[]>>(new FallbackCultureProvider("en"));
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get Localization
            ILocalization localization = service.GetService<ILocalization>()!;
            // Get text
            ILocalizedText localizedText = localization.LocalizedTextCached[("", "Namespace.Apples.Count")];
            // Print text
            WriteLine(localizedText.Print(new object[] { 2 })); // "You've got 2 apples."
        }
        {
            // Assign fallback provider
            Localization.Default.FallbackCultureProvider = FallbackCultureProvider.En;
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationInstance(Localization.Default);
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get Localization
            ILocalization localization = service.GetService<ILocalization>()!;
            // Get text
            ILocalizedText localizedText = localization.LocalizedTextCached[("", "Namespace.Apples.Count")];
            // Print text
            WriteLine(localizedText.Print(new object[] { 2 })); // "You've got 2 apples."
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationResourceManagerProvider();
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get Localization
            ILocalization localization = service.GetService<ILocalization>()!;

            // Get text
            ILocalizedText localizedText = localization.LocalizedTextCached[("", "samples.Resources.Resource1.Apples")];
            // Print text
            WriteLine(localizedText.Print(new object[] { 2 })); // "You've got 2 apple(s)."

            // Get localizable file
            ILocalizable<ILocalizationFile> localizableFile = localization.LocalizableFileCached["samples.Resources.Resource1.logo"];
            // Localize to invariant ""
            ILocalizationFile? localizationFile = localizableFile.Localize("")?.Value;
            // Read file 
            byte[] data = localizationFile!.ReadFully();
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationInstance(Localization.Default)
                .AddAvalancheLocalizationResourceManagerProvider();
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get Localization
            ILocalization localization = service.GetService<ILocalization>()!;

            // Get text
            ILocalizedText localizedText = localization.LocalizedTextCached[("", "samples.Resources.Resource1.Apples")];
            // Print text
            WriteLine(localizedText.Print(new object[] { 2 })); // "You've got 2 apple(s)."

            // Get localizable file
            ILocalizable<ILocalizationFile> localizableFile = localization.LocalizableFileCached["samples.Resources.Resource1.logo"];
            // Localize to invariant ""
            ILocalizationFile? localizationFile = localizableFile.Localize("")?.Value;
            // Read file 
            byte[] data = localizationFile!.ReadFully();
        }
        {
            // Get ResourceManager
            ResourceManager resourceManager = samples.Resources.Resource1.ResourceManager;
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddSingleton(typeof(ResourceManager), resourceManager)
                .AddAvalancheLocalizationService();
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get Localization
            ILocalization localization = service.GetService<ILocalization>()!;
            // Get text
            ILocalizedText localizedText = localization.LocalizedTextCached[("", "samples.Resources.Resource1.Apples")];
            // Print text
            WriteLine(localizedText.Print(new object[] { 2 }));
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddSingleton<IProvider<string, string[]>>(new FallbackCultureProvider("en"));
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get Localization
            ILocalization localization = service.GetService<ILocalization>()!;
            // Get text
            ILocalizedText localizedText = localization.LocalizedTextCached[("", "Namespace.Apples.Count")];
            // Print text
            WriteLine(localizedText.Print(new object[] { 2 }));
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddSingleton(typeof(ILocalizationFileSystem), LocalizationFileSystem.ApplicationRoot)
                .AddSingleton<IProvider<string, string[]>>(new FallbackCultureProvider("en"));
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get Localization
            ILocalization localization = service.GetService<ILocalization>()!;
            // Get text
            ILocalizedText localizedText = localization.LocalizedTextCached[("", "Namespace.Apples.Count")];
            // Print text
            WriteLine(localizedText.Print(new object[] { 2 }));
        }        
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationToUseFileProvider()
                .AddSingleton(typeof(IFileProvider), new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory))
                .AddSingleton<IProvider<string, string[]>>(new FallbackCultureProvider("en"));
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get Localization
            ILocalization localization = service.GetService<ILocalization>()!;
            // Get text
            ILocalizedText localizedText = localization.LocalizedTextCached[("", "Namespace.Apples.Count")];
            // Print text
            WriteLine(localizedText.Print(new object[] { 2 }));
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddSingleton(typeof(IEnumerable<KeyValuePair<string, MarkedText>>), new Dictionary<string, MarkedText> { { "TemplateFormat", "BraceNumeric" }, { "Culture", "" }, { "Key", "Namespace.Example" }, { "Text", "Example: {0}." } });
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get Localization
            ILocalization localization = service.GetService<ILocalization>()!;
            // Get text
            ILocalizedText localizedText = localization.LocalizedTextCached[("", "Namespace.Example")];
            // Print text
            WriteLine(localizedText.Print(new object[] { "Hello world" })); // "Example: Hello world."
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddLine(templateFormat: "BraceNumeric", culture: "", key: "Namespace.Example", text: "Example: {0}.");
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get Localization
            ILocalization localization = service.GetService<ILocalization>()!;
            // Get text
            ILocalizedText localizedText = localization.LocalizedTextCached[("", "Namespace.Example")];
            // Print text
            WriteLine(localizedText.Print(new object[] { "Hello world" })); // "Example: Hello world."
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddSingleton<IProvider<string, string[]>>(new FallbackCultureProvider("en"))
                .AddSingleton(typeof(IProvider<PluralRuleInfo, IPluralRule[]>), Avalanche.Localization.CLDRs.CLDR40.RulesCached);
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get Localization
            ILocalization localization = service.GetService<ILocalization>()!;
            // Get text
            ILocalizedText localizedText = localization.LocalizedTextCached[("", "Namespace.Apples.Count")];
            // Print text
            WriteLine(localizedText.Print(new object[] { 2 }));
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddSingleton<IProvider<string, string[]>>(new FallbackCultureProvider("en"))
                .AddSingleton(typeof(ILocalizationFileFormat), new LocalizationFileFormat(".html"));
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get Localization
            ILocalization localization = service.GetService<ILocalization>()!;
            // Get text
            ILocalizedText localizedText = localization.LocalizedTextCached[("", "Namespace.Apples.Count")];
            // Print text
            WriteLine(localizedText.Print(new object[] { 2 }));
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddSingleton<IProvider<string, string[]>>(new FallbackCultureProvider("en"))
                .AddSingleton(typeof(ILocalizationErrorHandler), new LocalizationErrorHandler(e => WriteLine(e)));
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get Localization
            ILocalization localization = service.GetService<ILocalization>()!;
            // Get text
            ILocalizedText localizedText = localization.LocalizedTextCached[("", "Example.ErrorExample")];
            // Print text
            WriteLine(localizedText.Print(new object[] { 2 }));
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddSingleton<IProvider<string, string[]>>(new FallbackCultureProvider("en"))
                .AddLogging(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Trace).AddConsole());
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get Localization
            ILocalization localization = service.GetService<ILocalization>()!;
            // Get text
            ILocalizedText localizedText = localization.LocalizedTextCached[("", "Example.ErrorExample")];
            // Print text
            WriteLine(localizedText.Print(new object[] { 2 }));
        }
        {
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddSingleton<IProvider<string, string[]>>(new FallbackCultureProvider("en"))
                .AddSingleton(typeof(ILocalizationFilePatterns), new LocalizationFilePatterns("Localization/{Key}", "Localization/{Culture}/{Key}"));
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get Localization
            ILocalization localization = service.GetService<ILocalization>()!;
            // Get text
            ILocalizedText localizedText = localization.LocalizedTextCached[("", "Namespace.Apples.Count")];
            // Print text
            WriteLine(localizedText.Print(new object[] { 2 }));
        }
        {
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddSingleton<IProvider<string, string[]>>(new FallbackCultureProvider("en"))
                .AddSingleton(typeof(ILocalizationFilePatterns), new LocalizationFilePatterns("Localization/{Key}", "Localization/{Culture}/{Key}"));
            // Removes service descriptors for "Resources/{Key}", "Resources/{Culture}/{Key}"
            serviceCollection.Remove(LocalizationServiceDescriptors.Instance.LocalizationFilePatterns);
        }
        {
            ILocalizationFileSystem fs = LocalizationFileSystemEmbedded.AppDomain;
            WriteLine(fs.PrintTree("", format: LocalizationFileSystemPrintTreeExtensions.PrintFormat.DefaultLong));
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddSingleton<IProvider<string, string[]>>(new FallbackCultureProvider("en"))
                .AddAvalancheLocalizationEmbeddedResourceProviderDefault();
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get Localization
            ILocalization localization = service.GetService<ILocalization>()!;
            // Print
            ILocalizedText text = localization.LocalizableTextCached["Namespace.Apples.Count"];
            string print = text.Print(CultureInfo.GetCultureInfo("sv"), new object[] { 1 });
            WriteLine(print);
        }
        {
            ILocalizationFileSystem fs = LocalizationFileSystemEmbedded.AppDomain;
            WriteLine(fs.PrintTree("", format: LocalizationFileSystemPrintTreeExtensions.PrintFormat.DefaultLong));
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddSingleton<IProvider<string, string[]>>(new FallbackCultureProvider("en"))
                .AddAvalancheLocalizationEmbeddedResourceProvider("*/*.Resources.{Key}", "*/*.Resources.{Culture}.{Key}", "*/{Key}", "*/{Key}.{Culture}");
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get Localization
            ILocalization localization = service.GetService<ILocalization>()!;
            // Print
            ILocalizedText text = localization.LocalizableTextCached["Namespace.Apples.Count"];
            string print = text.Print(CultureInfo.GetCultureInfo("sv"), new object[] { 1 });
            WriteLine(print);
        }
    }
}

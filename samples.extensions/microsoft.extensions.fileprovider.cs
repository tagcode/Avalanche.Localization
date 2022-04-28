using Avalanche.Localization;
using Avalanche.Utilities.Provider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using static System.Console;

class microsoft_extensions_fileprovider
{
    public static void Run()
    {
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
            WriteLine(localizedText.Print(new object[] { 2 })); // "You've got 2 apples."
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileProvider(new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory))
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
    }
}

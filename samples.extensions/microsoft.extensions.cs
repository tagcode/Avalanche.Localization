using Avalanche.Localization;
using Avalanche.Utilities.Provider;
using Microsoft.Extensions.DependencyInjection;
using static System.Console;

class microsoft_extensions
{
    public static void Run()
    {
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddAvalancheLocalizationEmbeddedResourceProviderDefault()
                .AddAvalancheLocalizationResourceManagerProvider()
                .AddSingleton<IProvider<string, string[]>>(new FallbackCultureProvider("en"))
                .AddSingleton<ICultureProvider>(CultureProvider.CurrentCulture.Instance)
                .AddAvalancheStringLocalizer();
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

namespace Namespace
{
    public class Apples { }
}

using Avalanche.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static System.Console;

class microsoft_extensions_logging
{
    public static void Run()
    {
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
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
    }
}

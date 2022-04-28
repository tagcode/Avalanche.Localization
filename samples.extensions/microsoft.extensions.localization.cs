using System.Globalization;
using Avalanche.Localization;
using Avalanche.Utilities.Provider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using static System.Console;

class microsoft_extensions_localization
{
    public static void Run()
    {
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddSingleton<IProvider<string, string[]>>(FallbackCultureProvider.En)
                .AddAvalancheStringLocalizer();
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            {
                // Get string localizer for key "Assembly[.Resources].Namespace.Type."
                IStringLocalizer<Namespace.Apples> stringLocalizer = service.GetService<IStringLocalizer<Namespace.Apples>>()!;
                // Assign active culture to "fi"
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("fi");
                // Print a string for "samples.Namespace.Apples.Count"
                WriteLine(stringLocalizer["Count", 2]); // "Sinulla on 2 omenaa."
            }
            {
                // Get string localizer for key "Assembly[.Resources].Namespace.".
                IStringLocalizer<Namespace.Apples> stringLocalizer = service.GetService<IStringLocalizer<Namespace.Apples>>()!;
                // Assign active culture to "fi"
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("fi");
                // Print all strings for "samples.Namespace." for "" culture.
                foreach (var line in stringLocalizer.GetAllStrings(includeParentCultures: true)) WriteLine($"{line.Name} = {line.Value}");
            }
            {
                // Get string localizer factory
                IStringLocalizerFactory stringLocalizerFactory = service.GetService<IStringLocalizerFactory>()!;
                // Assign active culture to ""
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
                // Create string localizer for key "Namespace".
                IStringLocalizer stringLocalizer = stringLocalizerFactory.Create("", "samples.Resources.Namespace.Apples");
                // Print a string for "Namespace.Apples.Count"
                WriteLine(stringLocalizer["Count", 2]); // "You've got 2 apples."
            }
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddAvalancheStringLocalizer()
                .Configure<Microsoft.Extensions.Localization.LocalizationOptions>(a => a.ResourcesPath = "Resources");
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get string localizer for key "Assembly.Resources.Namespace.Type."
            IStringLocalizer<Namespace.Apples> stringLocalizer = service.GetService<IStringLocalizer<Namespace.Apples>>()!;
            // Assign active culture to ""
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("fi");
            // Print a string for "samples.Resources.Namespace.Apples.Count"
            WriteLine(stringLocalizer["Count", 2]); // "Sinulla on 2 omenaa."
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddAvalancheLocalizationResourceManagerProvider()
                .AddAvalancheStringLocalizer()
                .Configure<Microsoft.Extensions.Localization.LocalizationOptions>(a => a.ResourcesPath = "Resources");
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get string localizer for key "Assembly.Resources.Namespace.Type."
            IStringLocalizer<Namespace.Apples> stringLocalizer = service.GetService<IStringLocalizer<Namespace.Apples>>()!;
            // Assign active culture to ""
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("fi");
            // Print a string for "samples.Resources.Namespace.Apples.Count"
            WriteLine(stringLocalizer["Count", 2]); // "Sinulla on 2 omenaa."
        }

        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddLogging(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Trace).AddConsole())
                .AddAvalancheStringLocalizer();
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get string localizer
            IStringLocalizer stringLocalizer = service.GetService<IStringLocalizer<Namespace.Apples>>()!;
            // Try get non-existent string
            var text = stringLocalizer["NonExistent"];
            // Get string localizer
            stringLocalizer = service.GetService<IStringLocalizer>()!;
            // Try get non-existent string
            text = stringLocalizer["NonExistent"];
        }
    }
}

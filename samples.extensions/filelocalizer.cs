using System.Globalization;
using Avalanche.Localization;
using Avalanche.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using static System.Console;

class filelocalizer
{
    public static void Run()
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en");
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en");
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddSingleton(typeof(ILocalizationFilePatterns), new LocalizationFilePatterns("filelocalizer/{Key}", "filelocalizer/{Culture}/{Key}"));
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get file localizer (Uses Thread.CurrentThread.CurrentUICulture)
            IFileLocalizer fileLocalizer = service.GetService<IFileLocalizer>()!;
            // Assign active culture to "fi"
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            // Get file "filelocalizer/fi/Namespace.Apples.logo.svg" or "filelocalizer/Namespace.Apples.logo.svg"
            ILocalizationFile file = fileLocalizer["Namespace.Apples.logo"]?.Value[0]!;
            // Print filename
            WriteLine(file.FileName); // "filelocalizer/fi/Namespace.Apples.logo.svg"
            // Read file
            byte[] data = file.ReadFully();
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddSingleton(typeof(ILocalizationFilePatterns), new LocalizationFilePatterns("filelocalizer/{Key}", "filelocalizer/{Culture}/{Key}"));
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get file localizer (Uses Thread.CurrentThread.CurrentUICulture)
            IFileLocalizer fileLocalizer = service.GetService<IFileLocalizer<Namespace.Apples>>()!;
            // Assign active culture to "fi"
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            // Get file "filelocalizer/fi/Namespace.Apples.logo.svg" or "filelocalizer/Namespace.Apples.logo.svg"
            ILocalizationFile file = fileLocalizer["logo"]?.Value[0]!;
            // Print filename
            WriteLine(file.FileName); // "filelocalizer/fi/Namespace.Apples.logo.svg"
            // Read file
            byte[] data = file.ReadFully();
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddSingleton(typeof(ILocalizationFilePatterns), new LocalizationFilePatterns("filelocalizer/{Key}", "filelocalizer/{Culture}/{Key}"));
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get file localizer (Uses Thread.CurrentThread.CurrentUICulture)
            IFileLocalizer fileLocalizer = service.GetService<IFileLocalizer<Namespace.Apples, CultureProvider.CurrentCulture>>()!;
            // Assign active culture to "fi"
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            // Get file "filelocalizer/fi/Namespace.Apples.logo.svg" or "filelocalizer/Namespace.Apples.logo.svg"
            ILocalizationFile file = fileLocalizer["logo"]?.Value[0]!;
            // Print filename
            WriteLine(file.FileName); // "filelocalizer/fi/Namespace.Apples.logo.svg"
            // Read file
            byte[] data = file.ReadFully();
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddSingleton(typeof(ICultureProvider), CultureProvider.CurrentCulture.Instance)
                .AddSingleton(typeof(ILocalizationFilePatterns), new LocalizationFilePatterns("filelocalizer/{Key}", "filelocalizer/{Culture}/{Key}"));
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get file localizer (Uses Thread.CurrentThread.CurrentUICulture)
            IFileLocalizer fileLocalizer = service.GetService<IFileLocalizer>()!;
            // Assign active culture to "fi"
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            // Get file "filelocalizer/fi/Namespace.Apples.logo.svg" or "filelocalizer/Namespace.Apples.logo.svg"
            ILocalizationFile file = fileLocalizer["Namespace.Apples.logo"]?.Value[0]!;
            // Print filename
            WriteLine(file.FileName); // "filelocalizer/fi/Namespace.Apples.logo.svg"
            // Read file
            byte[] data = file.ReadFully();
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddLogging(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Trace).AddConsole());
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get file localizer (Uses Thread.CurrentThread.CurrentUICulture)
            IFileLocalizer fileLocalizer = service.GetService<IFileLocalizer>()!;
            // Assign active culture to "fi"
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            // Try get non-existent file
            var file = fileLocalizer["NonExistent"];
        }
    }
}

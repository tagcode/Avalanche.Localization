using System.Globalization;
using Avalanche.Localization;
using Avalanche.Template;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using static System.Console;

class textlocalizer
{
    public static void Run()
    {
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot();
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get ITextLocalizer (Uses Thread.CurrentThread.CurrentUICulture)
            ITextLocalizer textLocalizer = service.GetService<ITextLocalizer>()!;
            // Read "Resources/en/Namespace.Apples.yaml" or "Resources/Namespace.Apples.yaml"
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            ITemplatePrintable? printable = textLocalizer["Namespace.Apples.Count"];
            WriteLine(printable!.Print(new object[] { 1 })); // "You've got an apple."
            // Read "Resources/fi/Namespace.Apples.yaml" or "Resources/Namespace.Apples.yaml"
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            ITemplatePrintable? printablefi = textLocalizer["Namespace.Apples.Count"];
            WriteLine(printablefi!.Print(new object[] { 1 })); // "Sinulla on yksi omena."
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot();
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get ITextLocalizer (Uses Thread.CurrentThread.CurrentUICulture)
            ITextLocalizer textLocalizer = service.GetService<ITextLocalizer<Namespace.Apples>>()!;
            // Read "Resources/en/Namespace.Apples.yaml" or "Resources/Namespace.Apples.yaml"
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            ITemplatePrintable? printable = textLocalizer["Count"];
            WriteLine(printable!.Print(new object[] { 1 })); // "You've got an apple."
            // Read "Resources/fi/Namespace.Apples.yaml" or "Resources/Namespace.Apples.yaml"
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            ITemplatePrintable? printablefi = textLocalizer["Count"];
            WriteLine(printablefi!.Print(new object[] { 1 })); // "Sinulla on yksi omena."
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot();
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get ITextLocalizer for namespace "System.Collections.Generic.IList<System.String>"
            ITextLocalizer textLocalizer = service.GetService<ITextLocalizer<System.Collections.Generic.IList<System.String>>>()!;
            // Read "Resources/en/System.Collections.Generic.yaml" or "Resources/System.Collections.Generic.yaml"
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            ITemplatePrintable? printable = textLocalizer[null];
            WriteLine(printable!.Print(null)); // "List of strings"
            // Read "Resources/fi/System.Collections.Generic.yaml" or "Resources/System.Collections.Generic.yaml"
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            ITemplatePrintable? printablefi = textLocalizer[null];
            WriteLine(printablefi!.Print(null)); // "Lista merkkijonoja"
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot();
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get ITextLocalizer, active culture from CultureInfo.CurrentCulture
            ITextLocalizer textLocalizer = service.GetService<ITextLocalizer<Namespace.Apples, CultureProvider.CurrentCulture>>()!;
            // Read "Resources/en/Namespace.Apples.yaml" or  "Resources/Namespace.Apples.yaml"
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en");
            ITemplatePrintable? printable = textLocalizer["Count"];
            WriteLine(printable!.Print(new object[] { 1 })); // "You've got an apple."
            // Read "Resources/fi/Namespace.Apples.yaml" or "Resources/Namespace.Apples.yaml"
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            ITemplatePrintable? printablefi = textLocalizer["Count"];
            WriteLine(printablefi!.Print(new object[] { 1 })); // "Sinulla on yksi omena."
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddSingleton(typeof(ICultureProvider), CultureProvider.CurrentCulture.Instance);
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get ITextLocalizer (Uses Thread.CurrentThread.CurrentUICulture, Thread.CurrentThread.CurrentCulture)
            ITextLocalizer textLocalizer = service.GetService<ITextLocalizer>()!;
            // Read "Resources/en/Namespace.Apples.yaml" or "Resources/Namespace.Apples.yaml"
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en");
            ITemplatePrintable? printable = textLocalizer["Namespace.Apples.Count"];
            WriteLine(printable!.Print(new object[] { 1 })); // "You've got an apple."
            // Read "Resources/fi/Namespace.Apples.yaml" or "Resources/Namespace.Apples.yaml"
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fi");
            ITemplatePrintable? printablefi = textLocalizer["Namespace.Apples.Count"];
            WriteLine(printablefi!.Print(new object[] { 1 })); // "Sinulla on yksi omena."
        }
        {
            // Add service descriptors
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddAvalancheLocalizationService()
                .AddAvalancheLocalizationFileSystemApplicationRoot()
                .AddLogging(loggingBuilder => loggingBuilder.SetMinimumLevel(LogLevel.Trace).AddConsole());
            // Build service
            using ServiceProvider service = serviceCollection.BuildServiceProvider();
            // Get ITextLocalizer (Uses Thread.CurrentThread.CurrentUICulture)
            ITextLocalizer textLocalizer = service.GetService<ITextLocalizer>()!;
            // Assign active culture
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            // Try to get non-existent text
            ITemplatePrintable? printable = textLocalizer["NonExistant"];
        }
    }
}

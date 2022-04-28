// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Localization.Internal;
using Avalanche.Localization.Pluralization;
using Avalanche.Utilities.Provider;
using System.Resources;
using System.Collections.Generic;

/// <summary>Dependency injection service descriptors.</summary>
public record LocalizationServiceDescriptors : RecordPropertiesOf<ServiceDescriptor>
{
    /// <summary>Singleton</summary>
    static readonly Lazy<LocalizationServiceDescriptors> instance = new Lazy<LocalizationServiceDescriptors>();
    /// <summary>Singleton</summary>
    public static LocalizationServiceDescriptors Instance => instance.Value;

    /// <summary><see cref="ILocalization"/></summary>
    public ServiceDescriptor Localization { get; init; } = new ServiceDescriptor(typeof(ILocalization), typeof(DI.Localization), ServiceLifetime.Singleton);

    /// <summary><see cref="ILocalizationLines"/></summary>
    public ServiceDescriptor LocalizationLines { get; init; } = new ServiceDescriptor(typeof(ILocalizationLines), typeof(DI.LocalizationLines), ServiceLifetime.Singleton);
    /// <summary>Default fallback culture provider</summary>
    public ServiceDescriptor FallbackCultureProvider { get; init; } = new ServiceDescriptor(typeof(IProvider<string, string[]>), Avalanche.Localization.FallbackCultureProvider.Default);
    /// <summary>No fallback culture</summary>
    public ServiceDescriptor NoFallbackCulture { get; init; } = new ServiceDescriptor(typeof(IProvider<string, string[]>), Avalanche.Localization.FallbackCultureProvider.NoFallback);
    /// <summary>Adds builtin template formats, Brace, BraceNumeric, Parameterless, etc</summary>
    public ServiceDescriptor TemplateFormats { get; init; } = new ServiceDescriptor(typeof(IEnumerable<ITemplateFormat>), Avalanche.Template.TemplateFormats.All.AllFormats);
    /// <summary>Built-in plural rules, e.g. CLDR40, CLDR35, ...</summary>
    public ServiceDescriptor PluralRules { get; init; } = new ServiceDescriptor(typeof(IProvider<PluralRuleInfo, IPluralRule[]>), Avalanche.Localization.Pluralization.PluralRulesProvider.Cached);
    /// <summary>Adapts <see cref="ResourceManager"/> to a line provider.</summary>
    public ServiceDescriptor ResourceManagerLineProvider { get; init; } = new ServiceDescriptor(typeof(IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>), typeof(DI.ResourceManagerLineProvider), ServiceLifetime.Singleton);

    /// <summary><see cref="ILocalizationFiles"/></summary>
    public ServiceDescriptor LocalizationFiles { get; init; } = new ServiceDescriptor(typeof(ILocalizationFiles), typeof(DI.LocalizationFiles), ServiceLifetime.Singleton);
    /// <summary>.yml and .yaml localization file</summary>
    public ServiceDescriptor LocalizationFileFormatYaml { get; init; } = new ServiceDescriptor(typeof(ILocalizationFileFormat), Avalanche.Localization.LocalizationFileFormatYaml.Instance);
    /// <summary>.xml localization file</summary>
    public ServiceDescriptor LocalizationFileFormatXml { get; init; } = new ServiceDescriptor(typeof(ILocalizationFileFormat), Avalanche.Localization.LocalizationFileFormatXml.Instance);
    /// <summary>.json localization file</summary>
    public ServiceDescriptor LocalizationFileFormatJson { get; init; } = new ServiceDescriptor(typeof(ILocalizationFileFormat), Avalanche.Localization.LocalizationFileFormatJson.Instance);

    /// <summary><see cref="ILocalizationFileSystem"/> at application root.</summary>
    public ServiceDescriptor LocalizationFileSystemApplicationRoot { get; init; } = new ServiceDescriptor(typeof(ILocalizationFileSystem), Avalanche.Localization.LocalizationFileSystem.ApplicationRoot);
    /// <summary><see cref="ILocalizationFileSystem"/> at application root with filelist cache.</summary>
    public ServiceDescriptor LocalizationFileSystemApplicationRootCached { get; init; } = new ServiceDescriptor(typeof(ILocalizationFileSystem), Avalanche.Localization.LocalizationFileSystem.ApplicationRootCached);
    /// <summary>Adapts <see cref="IFileProvider"/> into <see cref="ILocalizationFileSystem"/></summary>
    public ServiceDescriptor LocalizationFileSystemFromFileProvider { get; init; } = new ServiceDescriptor(typeof(ILocalizationFileSystem), typeof(LocalizationMsFileSystem), ServiceLifetime.Singleton);
    /// <summary>Adapts <see cref="IFileProvider"/> into <see cref="ILocalizationFileSystem"/> with filelist cache</summary>
    public ServiceDescriptor LocalizationFileSystemFromFileProviderCached { get; init; } = new ServiceDescriptor(typeof(ILocalizationFileSystem), factory: sp => { IFileProvider? fp = sp.GetService(typeof(IFileProvider)) as IFileProvider; return (fp == null ? new LocalizationMsFileSystem() : new LocalizationMsFileSystem(fp)).Cached(true, false); }, ServiceLifetime.Singleton);

    /// <summary>File pattern for "Resources/{Key}", "Resources/{Culture}/{Key}"</summary>
    public ServiceDescriptor LocalizationFilePatterns { get; init; } = new ServiceDescriptor(typeof(ILocalizationFilePatterns), Avalanche.Localization.LocalizationFilePatterns.ResourcesFolder);
    /// <summary>Adapts <see cref="ResourceManager"/> to a file provider.</summary>
    public ServiceDescriptor ResourceManagerFileProvider { get; init; } = new ServiceDescriptor(typeof(IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>), typeof(DI.ResourceManagerFileProvider), ServiceLifetime.Singleton);

    /// <summary>Searches for <see cref="ResourceManager"/>s in AppDomain.</summary>
    public ServiceDescriptor ResourceManagerProvider { get; init; } = new ServiceDescriptor(typeof(IProvider<string?, (string @namespace, ResourceManager resourceManager)[]>), sp=>Avalanche.Localization.ResourceManagerProvider.CreateCached(), ServiceLifetime.Singleton);
    /// <summary>Adapts <see cref="ResourceManagerProvider"/> to a line provider.</summary>
    public ServiceDescriptor ResourceManagerAssemblyLoaderLineProvider { get; init; } = new ServiceDescriptor(typeof(IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>), typeof(Avalanche.Localization.ResourceManagerLineProvider), ServiceLifetime.Singleton);
    /// <summary>Adapts <see cref="ResourceManagerProvider"/> to a file provider.</summary>
    public ServiceDescriptor ResourceManagerAssemblyLoaderFileProvider { get; init; } = new ServiceDescriptor(typeof(IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>), typeof(Avalanche.Localization.ResourceManagerFileProvider), ServiceLifetime.Singleton);

    /// <summary><see cref="ITextLocalizer"/></summary>
    public ServiceDescriptor TextLocalizer { get; init; } = new ServiceDescriptor(typeof(ITextLocalizer), typeof(DI.TextLocalizer), ServiceLifetime.Singleton);
    /// <summary><see cref="ITextLocalizer{Namespace}"/></summary>
    public ServiceDescriptor TextLocalizerOfNamespace { get; init; } = new ServiceDescriptor(typeof(ITextLocalizer<>), typeof(DI.TextLocalizer<>), ServiceLifetime.Singleton);
    /// <summary><see cref="ITextLocalizer{Namespace, CultureProvider}"/></summary>
    public ServiceDescriptor TextLocalizerOfNamespaceAndCultureProvider { get; init; } = new ServiceDescriptor(typeof(ITextLocalizer<,>), typeof(DI.TextLocalizer<,>), ServiceLifetime.Singleton);
    /// <summary><see cref="IFileLocalizer"/></summary>
    public ServiceDescriptor FileLocalizer { get; init; } = new ServiceDescriptor(typeof(IFileLocalizer), typeof(DI.FileLocalizer), ServiceLifetime.Singleton);
    /// <summary><see cref="IFileLocalizer{Namespace}"/></summary>
    public ServiceDescriptor FileLocalizerOfNamespace { get; init; } = new ServiceDescriptor(typeof(IFileLocalizer<>), typeof(DI.FileLocalizer<>), ServiceLifetime.Singleton);
    /// <summary><see cref="IFileLocalizer{Namespace, CultureProvider}"/></summary>
    public ServiceDescriptor FileLocalizerOfNamespaceAndCultureProvider { get; init; } = new ServiceDescriptor(typeof(IFileLocalizer<,>), typeof(DI.FileLocalizer<,>), ServiceLifetime.Singleton);

    /// <summary>Adapts <see cref="ILoggerFactory"/> to <see cref="ILocalizationErrorHandler"/></summary>
    public ServiceDescriptor LocalizationErrorLogger { get; init; } = new ServiceDescriptor(typeof(ILocalizationErrorHandler), typeof(LocalizationErrorLogger), ServiceLifetime.Singleton);

    /// <summary>Default embedded resources in "*.Resources.{Key}" and "*.Resources.{Key}.{Culture}"</summary>
    public ServiceDescriptor EmbeddedResourcesFileProvider { get; init; } = CreateEmbeddedResourcesFileProvider(null!);

    /// <summary>Adds descriptor that adds support for embedded resources in "*.Path.{Key}" and "*.Path.{Culture}.{Key}"</summary>
    public static ServiceDescriptor CreateEmbeddedResourcesFileProvider(params string[] paths)
    {
        // Create patterns
        IEnumerable<ITemplateFormatPrintable> filePatterns = paths == null ? Avalanche.Localization.LocalizationFilePatterns.ResourcesEmbedded.Patterns :
            new LocalizationFilePatterns(paths).Patterns;
        // Create descriptor
        var sd = new ServiceDescriptor(typeof(IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>), (IServiceProvider sp) =>
        {
            // Get file formats
            IEnumerable<ILocalizationFileFormat> fileFormats = sp.GetService<IEnumerable<ILocalizationFileFormat>>() ?? Array.Empty<ILocalizationFileFormat>();
            // Get file systems
            IEnumerable<ILocalizationFileSystem> fileSystems = new ILocalizationFileSystem[] { Avalanche.Localization.LocalizationFileSystemEmbedded.AppDomain };
            // Create querier
            var fileProvider = new LocalizationFilesQuerierWithPattern(fileFormats, fileSystems, filePatterns);
            // Return provider
            return fileProvider;
        }, ServiceLifetime.Singleton);
        // Return descriptor
        return sd;
    }

    /// <summary>
    /// Create service descriptor that adds <paramref name="localization"/> and imports following services, if they are new (and modifies <paramref name="localization"/>):
    ///     <![CDATA[IProvider<string, string[]>]]> for fallback culture provider,
    ///     <![CDATA[IProvider<PluralRuleInfo, IPluralRule[]>]]> for plural rule provider,
    ///     <see cref="ITemplateFormat"/>,
    ///     <see cref="ILocalizationFileFormat"/> for localization file formats,
    ///     <see cref="IFileProvider"/> for localization resource file provider,
    ///     <see cref="ILocalizationFilePatterns"/> for additional file patterns,
    ///     <see cref="ILogger"/> for logging localization errors,
    ///     <see cref="System.Resources.ResourceManager"/>.
    /// </summary>
    /// <param name="localization">Localization instance, e.g. <see cref="Localization.Default"/>.</param>
    public static ServiceDescriptor LocalizationInstance(ILocalization localization)
    {
        // Create loader delegate
        Func<IServiceProvider, object> loader = sp =>
        {
            //
            ILocalization _localization = localization ?? new Localization();
            // Add fallback culture provider
            IProvider<string, string[]>? fallbackCultureProvider = sp.GetService<IProvider<string, string[]>>();
            if (fallbackCultureProvider != null) _localization.FallbackCultureProvider = fallbackCultureProvider;
            // Add template foramts
            IEnumerable<ITemplateFormat>? templateFormats = sp.GetService<IEnumerable<ITemplateFormat>>();
            if (templateFormats != null) foreach (ITemplateFormat templateFormat in templateFormats) _localization.AddTemplateFormats(templateFormat);
            // Add file systems
            IEnumerable<ILocalizationFileSystem>? filesystems = sp.GetService<IEnumerable<ILocalizationFileSystem>>();
            if (filesystems != null) foreach (ILocalizationFileSystem filesystem in filesystems) _localization.Files.FileSystems.AddIfNew(filesystem);
            // Add file provider
            IFileProvider? fileprovider = sp.GetService<IFileProvider>();
            if (fileprovider != null) _localization.Files.FileSystems.AddIfNew(new LocalizationMsFileSystem(fileprovider));
            // Add file patterns
            IEnumerable<ILocalizationFilePatterns>? filepatterns = sp.GetService<IEnumerable<ILocalizationFilePatterns>>();
            if (filepatterns != null)
                foreach (ILocalizationFilePatterns patterns in filepatterns)
                    foreach (var pattern in patterns.Patterns)
                        _localization.Files.FilePatterns.AddIfNew(pattern);
            // Add logger
            ILoggerFactory? loggerFactory = sp.GetService<ILoggerFactory>();
            if (loggerFactory != null)
            {
                ILogger logger = loggerFactory.CreateLogger(typeof(Avalanche.Localization.ILocalization));
                ILocalizationErrorHandler errorLogger = Avalanche.Localization.LocalizationErrorLogger.Create(logger);
                _localization.ErrorHandlers.AddIfNew(errorLogger);
            }
            // Add resource managers
            IEnumerable<ResourceManager>? resourceManagers = sp.GetService<IEnumerable<ResourceManager>>();
            if (resourceManagers != null && resourceManagers.Any())
            {
                _localization.AddResourceManagers(resourceManagers.ToArray());
            }
            // Add lines provider
            var lineProviders = sp.GetService<IEnumerable<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>>>();
            if (lineProviders != null)
            {
                foreach (var lineProvider in lineProviders)
                {
                    _localization.Lines.LineProviders.Add(lineProvider);
                    _localization.Lines.LineProvidersCached.Add(lineProvider.ValueResultCaptured().Cached().ValueResultOpened());
                }
            }
            // Add files provider
            var fileProviders = sp.GetService<IEnumerable<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>>>();
            if (fileProviders != null)
            {
                foreach (var fileProvider in fileProviders)
                {
                    _localization.Files.FileProviders.Add(fileProvider);
                    _localization.Files.FileProvidersCached.Add(fileProvider.ValueResultCaptured().Cached().ValueResultOpened());
                }
            }

            // Return localization
            return _localization;
        };
        // Create descriptor
        ServiceDescriptor serviceDescriptor = new ServiceDescriptor(typeof(ILocalization), loader, ServiceLifetime.Singleton);
        // Return
        return serviceDescriptor;
    }

    // <summary></summary>
    //public ServiceDescriptor X = new ServiceDescriptor(typeof(), typeof(), ServiceLifetime.Singleton);
}


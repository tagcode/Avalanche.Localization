// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using Avalanche.Template;
using Avalanche.Utilities;

/// <summary>Extension methods for <see cref="IServiceCollection"/></summary>
public static class LocalizationDependencyInjectionExtensions
{
    /// <summary>
    /// Add service for <see cref="Avalanche.Localization.ILocalization"/>.
    /// 
    /// For this service to work, a <see cref="IFileProvider"/> must be installed.
    /// Searches localization files from "Resources/{Key}", "Resources/{Culture}/{Key}" paths.
    /// Uses .yaml, .yaml, .xml and .json localization lines files.
    /// 
    /// Imports following services:
    ///     <![CDATA[IProvider<string, string[]>]]> for fallback culture provider,
    ///     <![CDATA[IProvider<PluralRuleInfo, IPluralRule[]>]]> for plural rule provider,
    ///     <see cref="ITemplateFormat"/>,
    ///     <see cref="ILocalizationFileFormat"/> for localization file formats,
    ///     <see cref="IFileProvider"/> for localization resource file provider,
    ///     <see cref="ILocalizationFilePatterns"/> for additional file patterns,
    ///     <see cref="ILogger"/> for logging localization errors,
    ///     <see cref="System.Resources.ResourceManager"/>.
    ///     
    /// </summary>
    public static IServiceCollection AddAvalancheLocalizationService(this IServiceCollection serviceCollection)
    {
        // Add service descriptions
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.Localization);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.LocalizationLines);
        //serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.FallbackCultureProvider);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.TemplateFormats);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.PluralRules);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.ResourceManagerLineProvider);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.LocalizationFiles);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.LocalizationFileFormatYaml);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.LocalizationFileFormatXml);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.LocalizationFileFormatJson);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.LocalizationFileSystemApplicationRoot);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.LocalizationFilePatterns);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.ResourceManagerFileProvider);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.LocalizationErrorLogger);

        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.TextLocalizer);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.TextLocalizerOfNamespace);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.TextLocalizerOfNamespaceAndCultureProvider);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.FileLocalizer);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.FileLocalizerOfNamespace);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.FileLocalizerOfNamespaceAndCultureProvider);
        // Return service collection
        return serviceCollection;
    }

    /// <summary>
    /// Adds service descriptor that adapts <see cref="IFileProvider"/> into <see cref="ILocalizationFileSystem"/>
    /// making file provider available as localization resource source.
    /// </summary>
    public static IServiceCollection AddAvalancheLocalizationToUseFileProvider(this IServiceCollection serviceCollection)
    {
        // Add
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.LocalizationFileSystemFromFileProvider);
        // Return service collection
        return serviceCollection;
    }

    /// <summary>Adds <paramref name="fileProvider"/> as <see cref="ILocalizationFileSystem"/> making it available as localization resource source.</summary>
    public static IServiceCollection AddAvalancheLocalizationFileProvider(this IServiceCollection serviceCollection, IFileProvider fileProvider)
    {
        // Create file system
        ILocalizationFileSystem fs = new LocalizationMsFileSystem(fileProvider);
        // Create service descriptor
        ServiceDescriptor sd = new ServiceDescriptor(typeof(ILocalizationFileSystem), fs);
        // Add to collection
        serviceCollection.AddIfNew(sd);
        // Return service collection
        return serviceCollection;
    }

    /// <summary>Adds <see cref="ILocalizationFileSystem"/> that uses localization resources from application root.</summary>
    public static IServiceCollection AddAvalancheLocalizationFileSystemApplicationRoot(this IServiceCollection serviceCollection)
    {
        // Add to collection
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.LocalizationFileSystemApplicationRoot);
        // Return service collection
        return serviceCollection;
    }

    /// <summary></summary>
    public static IServiceCollection AddAvalancheLocalizationResourceManagerProvider(this IServiceCollection serviceCollection)
    {
        // Add service descriptions
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.ResourceManagerProvider);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.ResourceManagerAssemblyLoaderLineProvider);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.ResourceManagerAssemblyLoaderFileProvider);
        // Return service collection
        return serviceCollection;
    }

    /// <summary></summary>
    public static IServiceCollection AddAvalancheLocalizationEmbeddedResourceProviderDefault(this IServiceCollection serviceCollection)
    {
        // Get-or-create service descriptor
        ServiceDescriptor sd = LocalizationServiceDescriptors.Instance.EmbeddedResourcesFileProvider;
        // Add service descriptions
        serviceCollection.AddIfNew(sd);
        // Return service collection
        return serviceCollection;
    }

    /// <summary></summary>
    public static IServiceCollection AddAvalancheLocalizationEmbeddedResourceProvider(this IServiceCollection serviceCollection, params string[] paths)
    {
        // Get-or-create service descriptor
        ServiceDescriptor sd = paths == null ? LocalizationServiceDescriptors.Instance.EmbeddedResourcesFileProvider : LocalizationServiceDescriptors.CreateEmbeddedResourcesFileProvider(paths);
        // Add service descriptions
        serviceCollection.AddIfNew(sd);
        // Return service collection
        return serviceCollection;
    }

    /// <summary>
    /// Add already configured <paramref name="localization"/> as service description. 
    /// 
    /// Imports following services and modifies <paramref name="localization"/>:
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
    public static IServiceCollection AddAvalancheLocalizationInstance(this IServiceCollection serviceCollection, ILocalization localization)
    {
        // Add service descriptor for 'localization'
        serviceCollection.Add(LocalizationServiceDescriptors.LocalizationInstance(localization));
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.TextLocalizer);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.TextLocalizerOfNamespace);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.TextLocalizerOfNamespaceAndCultureProvider);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.FileLocalizer);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.FileLocalizerOfNamespace);
        serviceCollection.AddIfNew(LocalizationServiceDescriptors.Instance.FileLocalizerOfNamespaceAndCultureProvider);
        // Return service collection
        return serviceCollection;
    }

    /// <summary>
    /// Add following services: <see cref="IStringLocalizer"/>, <see cref="IStringLocalizerFactory"/> and <see cref="IStringLocalizer{T}"/>.
    /// 
    /// This requires service for <see cref="Avalanche.Localization.ILocalization"/>, e.g. with <see cref="AddAvalancheLocalizationInstance"/> or <see cref="AddAvalancheLocalizationService"/>.
    /// 
    /// Do not use with <![CDATA[Microsoft.Extensions.DependencyInjection.LocalizationServiceCollectionExtensions.AddLocalization]]>.
    /// </summary>
    public static IServiceCollection AddAvalancheStringLocalizer(this IServiceCollection serviceCollection)
    {
        // Add service descriptors
        serviceCollection.AddIfNew(StringLocalizerServiceDescriptors.Instance.StringLocalizerFactory);
        serviceCollection.AddIfNew(StringLocalizerServiceDescriptors.Instance.StringLocalizer);
        serviceCollection.AddIfNew(StringLocalizerServiceDescriptors.Instance.StringLocalizerOfT);
        // Return service collection
        return serviceCollection;
    }

    /// <summary>Add explicit line.</summary>
    public static IServiceCollection AddLine(this IServiceCollection serviceCollection, string culture, string key, string templateFormat, string text, string? pluralRules = null, string? plurals = null)
    {
        // Create key-value pair map
        StructList6<KeyValuePair<string, MarkedText>> line = new();
        // Add key-values
        if (culture != null) line.Add(new KeyValuePair<string, MarkedText>("Culture", culture));
        if (key != null) line.Add(new KeyValuePair<string, MarkedText>("Key", key));
        if (text != null) line.Add(new KeyValuePair<string, MarkedText>("Text", text));
        if (templateFormat != null) line.Add(new KeyValuePair<string, MarkedText>("TemplateFormat", templateFormat));
        if (pluralRules != null) line.Add(new KeyValuePair<string, MarkedText>("PluralRules", pluralRules));
        if (plurals != null) line.Add(new KeyValuePair<string, MarkedText>("Plurals", plurals));
        // Create service descriptor
        ServiceDescriptor serviceDescriptor = new ServiceDescriptor(typeof(IEnumerable<KeyValuePair<string, MarkedText>>), line.ToArray());
        // Add to service collection
        serviceCollection.Add(serviceDescriptor);
        // Return service collection
        return serviceCollection;
    }
}

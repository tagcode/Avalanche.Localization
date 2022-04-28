// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Resources;
using Avalanche.Utilities.Provider;

/// <summary>Additional extension methods for <see cref="ILocalization"/>.</summary>
public static class ResourceManagerExtensions_
{
    /// <summary>Add <see cref="ResourceManager"/> file and lines provider</summary>
    public static L AddResourceManagerProvider<L>(this L localization) where L : ILocalization
    {
        // Add providers
        localization
            .AddLineProvider(ResourceManagerLineProvider.AssemblyLoader, ResourceManagerLineProvider.AssemblyLoaderCached)
            .AddFileProvider(ResourceManagerFileProvider.AssemblyLoader, ResourceManagerFileProvider.AssemblyLoaderCached);
        // Return
        return localization;
    }
    /*
    /// <summary>Add <paramref name="resourceManager"/> as file provider.</summary>
    /// <param name="resourceManager"></param>
    /// <param name="namespace">Explicit namespace</param>
    public static L AddResourceManager<L>(this L localization, ResourceManager resourceManager, string? @namespace = null) where L : ILocalizationLines
    {
        // Create resource manager provider
        IProvider<string?, (string @namespace, ResourceManager resourceManager)[]> resourceManagerProvider = new ResourceManagerList(resourceManager, @namespace);
        // Create line provider
        var lineProvider = new ResourceManagerLineProvider(resourceManagerProvider);
        // Add line provider
        if (!localization.LineProviders.Contains(lineProvider))
        {
            localization.LineProviders.Add(lineProvider);
            localization.LineProvidersCached.Add(lineProvider.ValueResultCaptured().Cached().ValueResultOpened());
        }
        // Return self
        return localization;
    }

    /// <summary>Add <paramref name="resourceManager"/> as text provider.</summary>
    public static L AddResourceManager<L>(this L localization, ResourceManager resourceManager, string? @namespace = null) where L : ILocalizationFiles
    {
        // Create resource manager provider
        IProvider<string?, (string @namespace, ResourceManager resourceManager)[]> resourceManagerProvider = new ResourceManagerList(resourceManager, @namespace);
        // Create file provider
        var fileProvider = new ResourceManagerFileProvider(resourceManagerProvider);
        // Add file provider
        if (!localization.FileProviders.Contains(fileProvider))
        {
            localization.FileProviders.Add(fileProvider);
            localization.FileProvidersCached.Add(fileProvider.ValueResultCaptured().Cached().ValueResultOpened());
        }

        // Return self
        return localization;
    }
    */
    /// <summary>Add <paramref name="resourceManager"/> as text and file provider.</summary>
    /// <param name="resourceManager"></param>
    /// <param name="namespace">Explicit namespace</param>
    public static L AddResourceManager<L>(this L localization, ResourceManager resourceManager, string? @namespace = null) where L : ILocalization
    {
        // Create resource manager provider
        IProvider<string?, (string @namespace, ResourceManager resourceManager)[]> resourceManagerProvider = new ResourceManagerList(resourceManager, @namespace);
        // Create line provider
        var lineProvider = new ResourceManagerLineProvider(resourceManagerProvider);
        // Add line provider
        if (!localization.Lines.LineProviders.Contains(lineProvider))
        {
            localization.Lines.LineProviders.Add(lineProvider);
            localization.Lines.LineProvidersCached.Add(lineProvider.ValueResultCaptured().Cached().ValueResultOpened());
        }

        // Create file provider
        var fileProvider = new ResourceManagerFileProvider(resourceManagerProvider);
        // Add file provider
        if (!localization.Files.FileProviders.Contains(fileProvider))
        {
            localization.Files.FileProviders.Add(fileProvider);
            localization.Files.FileProvidersCached.Add(fileProvider.ValueResultCaptured().Cached().ValueResultOpened());
        }

        // Return self
        return localization;
    }

    /// <summary>Add <paramref name="resourceManagers"/> as text and file provider.</summary>
    public static L AddResourceManagers<L>(this L localization, ResourceManager[] resourceManagers, string?[]? namespaces = null) where L : ILocalization
    {
        // Create resource manager provider
        IProvider<string?, (string @namespace, ResourceManager resourceManager)[]> resourceManagerProvider = new ResourceManagerList(resourceManagers, @namespaces);
        // Create line provider
        var lineProvider = new ResourceManagerLineProvider(resourceManagerProvider);
        // Add line provider
        if (!localization.Lines.LineProviders.Contains(lineProvider))
        {
            localization.Lines.LineProviders.Add(lineProvider);
            localization.Lines.LineProvidersCached.Add(lineProvider.ValueResultCaptured().Cached().ValueResultOpened());
        }

        // Create file provider
        var fileProvider = new ResourceManagerFileProvider(resourceManagerProvider);
        // Add file provider
        if (!localization.Files.FileProviders.Contains(fileProvider))
        {
            localization.Files.FileProviders.Add(fileProvider);
            localization.Files.FileProvidersCached.Add(fileProvider.ValueResultCaptured().Cached().ValueResultOpened());
        }

        // Return self
        return localization;
    }

}


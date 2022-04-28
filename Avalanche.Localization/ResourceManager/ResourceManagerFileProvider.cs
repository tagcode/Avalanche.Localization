// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using Avalanche.Localization.Internal;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Provides files resources from one or multiple <see cref="ResourceManager"/>s.</summary>
public class ResourceManagerFileProvider : ProviderBase<(string? culture, string? key), IEnumerable<ILocalizationFile>>
{
    /// <summary>Loads resource manager from assembly, non-cached.</summary>
    static readonly IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> assemblyLoader = new ResourceManagerFileProvider(ResourceManagerProvider.Instance);
    /// <summary>Loads resource manager from assembly, cached.</summary>
    static readonly IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> assemblyLoaderCached = new ResourceManagerFileProvider(ResourceManagerProvider.Cached).ValueResultCaptured().Cached().ValueResultOpened();
    /// <summary>Loads resource manager from assembly, non-cached.</summary>
    public static IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> AssemblyLoader => assemblyLoader;
    /// <summary>Loads resource manager from assembly, cached.</summary>
    public static IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> AssemblyLoaderCached => assemblyLoaderCached;
    /// <summary>Create new resource manager from-assembly loader, cached.</summary>
    public static IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> CreateAssemblyLoaderCached() => new ResourceManagerFileProvider(ResourceManagerProvider.CreateCached());

    /// <summary>Resource managers</summary>
    protected IProvider<string?, (string @namespace, ResourceManager resourceManager)[]> resourceManagerProviders;
    /// <summary>Resource managers</summary>
    public IProvider<string?, (string @namespace, ResourceManager resourceManager)[]> ResourceManagerProviders => resourceManagerProviders;

    /// <summary></summary>
    public ResourceManagerFileProvider(ResourceManager resourceManager) : this(new ResourceManagerList(resourceManager)) { }
    /// <summary></summary>
    public ResourceManagerFileProvider(ResourceManager resourceManager, string? @namespace) : this(new ResourceManagerList(resourceManager, @namespace)) { }
    /// <summary></summary>
    public ResourceManagerFileProvider(ResourceManager[] resourceManagers) : this(new ResourceManagerList(resourceManagers)) { }
    /// <summary></summary>
    public ResourceManagerFileProvider(ResourceManager[] resourceManagers, string?[]? namespaces) : this(new ResourceManagerList(resourceManagers, namespaces)) { }
    /// <summary></summary>
    public ResourceManagerFileProvider(IProvider<string?, (string @namespace, ResourceManager resourceManager)[]> resourceManagerProvider)
    {
        this.resourceManagerProviders = resourceManagerProvider ?? throw new ArgumentNullException(nameof(resourceManagerProvider));
    }

    /// <summary>Try get files.</summary>
    public override bool TryGetValue((string? culture, string? key) query, out IEnumerable<ILocalizationFile> files)
    {
        // Place here resource managers
        StructList8<(string @namespace, ResourceManager resourceManager)> resourceManagers = new();
        // Query all resource managers
        if (query.key == null)
        {
            // Query all resource managers
            if (!ResourceManagerProviders.TryGetValue(null, out (string @namespace, ResourceManager resourceManager)[] _resourceManagers)) { files = null!; return false; }
            // Add resource managers
            resourceManagers.AddRange(_resourceManagers);
        }
        // Query by all keys, until false is returned
        else {
            // Get snapshot
            var _resourceManagerProviders = ResourceManagerProviders;
            // Get all providers
            foreach ((string? @namespace, string? name) in LocalizationUtilities.GetAllNamespaceAndNameDotPermutations(query.key))
            {
                // No namespace
                if (@namespace == null) continue;
                // Query all resource managers
                if (!_resourceManagerProviders.TryGetValue(@namespace, out (string @namespace, ResourceManager resourceManager)[] _resourceManagers)) continue;
                // Add resource managers
                resourceManagers.AddRange(_resourceManagers);
            }
        }
        // No resource managers
        if (resourceManagers.Count == 0) { files = null!; return false; }
        // Get culture info
        CultureInfo cultureInfo = query.culture == null ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(query.culture);
        // Place result here
        StructList8<ILocalizationFile> result = new();
        // Try each resource manager
        for (int i = 0; i < resourceManagers.Count; i++)
        {
            // Get resource manager and namespace
            (string @namespace, ResourceManager resourceManager) = resourceManagers[i];
            // Get resource set
            ResourceSet? resourceSet = resourceManager.GetResourceSet(cultureInfo, createIfNotExists: true, tryParents: false);
            // No resource set
            if (resourceSet == null) continue;

            // Query all keys (for null return for invariant culture, because it is the only one that is queryable from ResourceManager)
            if (query.key == null)
            {
                // Iterate lists
                foreach (DictionaryEntry entry in resourceSet)
                {
                    // Not data
                    if (entry.Value is not byte[] data) continue;
                    // Create file
                    ILocalizationFile file = new LocalizationFileInMemory
                    {
                        Culture = cultureInfo.Name,
                        Data = data,
                        Key = @namespace + "." + entry.Key,
                        FileFormat = LocalizationFileFormat.NoExtension,
                        FileName = entry.Key.ToString()
                    };
                    // Add to result
                    result.Add(file);
                }
            }
            // Query single key
            else
            {
                // Does not begin with a namespace
                if (query.key.Length <= @namespace.Length || !query.key.StartsWith(@namespace, StringComparison.InvariantCulture)) continue;
                // Get name
                string name = query.key.Substring(@namespace.Length + 1);

                // Get data
                byte[]? data = resourceSet.GetObject(name) as byte[];
                // No data
                if (data == null) continue;
                // Create data file
                ILocalizationFile file = new LocalizationFileInMemory
                {
                    Culture = cultureInfo.Name,
                    Data = data,
                    Key = query.key,
                    FileFormat = LocalizationFileFormat.NoExtension,
                    FileName = name
                };
                // Add to result
                result.Add(file);
            }
        }
        // Return
        files = result.ToArray();
        return result.Count > 0;
    }

    /// <summary></summary>
    public override int GetHashCode() => resourceManagerProviders.GetHashCode() ^ 0x23345;
    /// <summary></summary>
    public override bool Equals(object? obj)
    {
        // Cast
        if (obj is not ResourceManagerFileProvider other) return false;
        // Compare
        if (!other.ResourceManagerProviders.Equals(this.ResourceManagerProviders)) return false;
        // Equals
        return true;
    }
    /// <summary>Print information</summary>
    public override string ToString() => ResourceManagerProviders.ToString()??GetType().Name;
}

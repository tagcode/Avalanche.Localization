// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Localization.Internal;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

/// <summary>Provides line resources from one or multiple <see cref="ResourceManager"/>s.</summary>
public class ResourceManagerLineProvider : ProviderBase<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>
{
    /// <summary>Loads resource manager from assembly, non-cached.</summary>
    static readonly IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> assemblyLoader = new ResourceManagerLineProvider(ResourceManagerProvider.Instance);
    /// <summary>Loads resource manager from assembly, cached.</summary>
    static readonly IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> assemblyLoaderCached = new ResourceManagerLineProvider(ResourceManagerProvider.Cached);
    /// <summary>Loads resource manager from assembly, non-cached.</summary>
    public static IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> AssemblyLoader => assemblyLoader;
    /// <summary>Loads resource manager from assembly, cached.</summary>
    public static IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> AssemblyLoaderCached => assemblyLoaderCached;
    /// <summary>Create new resource manager from-assembly loader, cached.</summary>
    public static IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> CreateAssemblyLoaderCached() => new ResourceManagerLineProvider(ResourceManagerProvider.CreateCached());

    /// <summary>Resource managers</summary>
    protected IProvider<string?, (string @namespace, ResourceManager resourceManager)[]> resourceManagerProviders;
    /// <summary>Resource managers</summary>
    public IProvider<string?, (string @namespace, ResourceManager resourceManager)[]> ResourceManagerProviders => resourceManagerProviders;

    /// <summary></summary>
    public ResourceManagerLineProvider(ResourceManager resourceManager) : this(new ResourceManagerList(resourceManager)) { }
    /// <summary></summary>
    public ResourceManagerLineProvider(ResourceManager resourceManager, string? @namespace) : this(new ResourceManagerList(resourceManager, @namespace)) { }
    /// <summary></summary>
    public ResourceManagerLineProvider(ResourceManager[] resourceManagers) : this(new ResourceManagerList(resourceManagers)) { }
    /// <summary></summary>
    public ResourceManagerLineProvider(ResourceManager[] resourceManagers, string?[]? namespaces) : this(new ResourceManagerList(resourceManagers, namespaces)) { }
    /// <summary></summary>
    public ResourceManagerLineProvider(IProvider<string?, (string @namespace, ResourceManager resourceManager)[]> resourceManagerProvider)
    {
        this.resourceManagerProviders = resourceManagerProvider ?? throw new ArgumentNullException(nameof(resourceManagerProvider));
    }

    /// <summary>Try get lines</summary>
    public override bool TryGetValue((string? culture, string? key) query, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines)
    {
        // Place here resource managers
        StructList8<(string @namespace, ResourceManager resourceManager)> resourceManagers = new();
        // Query all resource managers
        if (query.key == null)
        {
            // Query all resource managers
            if (!ResourceManagerProviders.TryGetValue(null, out (string @namespace, ResourceManager resourceManager)[] _resourceManagers)) { lines = null!; return false; }
            // Add resource managers
            resourceManagers.AddRange(_resourceManagers);
        }
        // Query by all keys, until false is returned
        else
        {
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
        if (resourceManagers.Count == 0) { lines = null!; return false; }

        // Place result here
        StructList8<IEnumerable<KeyValuePair<string, MarkedText>>> result = new();
        // Get culture
        CultureInfo culture = query.culture == null ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(query.culture);
        //
        for (int i=0; i<resourceManagers.Count; i++)
        {
            // Get resource manager and namespace
            (string @namespace, ResourceManager resourceManager) = resourceManagers[i];
            // Get resource set
            ResourceSet? resourceSet = resourceManager.GetResourceSet(culture, createIfNotExists: true, tryParents: false);
            // No resource set
            if (resourceSet == null) continue;

            // Query all keys (for null return for invariant culture, because it is the only one that is queryable from ResourceManager)
            if (query.key == null)
            {
                // Iterate lists
                foreach (DictionaryEntry entry in resourceSet)
                {
                    // Not string
                    if (entry.Value is not string text) continue;
                    // Create line
                    var line = new Dictionary<string, MarkedText> {
                        { "TemplateFormat", "BraceNumeric" },
                        { "Culture", "" },
                        { "Key", @namespace + "." + entry.Key },
                        { "Text", new MarkedText(text, resourceManager.BaseName) }
                    };
                    // Add to result
                    result.Add(line);
                }
            }
            // Query single key
            else
            {
                // Does not begin with a namespace
                if (query.key.Length <= @namespace.Length || !query.key.StartsWith(@namespace, StringComparison.InvariantCulture)) continue;
                // Get name
                string name = query.key.Substring(@namespace.Length + 1);
                // Get text
                string? text = resourceSet.GetString(name);
                // No text
                if (text == null) { lines = null!; return false; }
                // Create line
                var line = new Dictionary<string, MarkedText>
                {
                    { "TemplateFormat", "BraceNumeric" },
                    { "Culture", "" },
                    { "Key", query.key },
                    { "Text", new MarkedText(text, resourceManager.BaseName) }
                };
                // Add to result
                result.Add(line);
            }
        }
        // Return
        lines = result.ToArray();
        return result.Count > 0;
    }

    /// <summary></summary>
    public override int GetHashCode() => resourceManagerProviders.GetHashCode() ^ 0xab2f;
    /// <summary></summary>
    public override bool Equals(object? obj)
    {
        // Cast
        if (obj is not ResourceManagerLineProvider other) return false;
        // Compare
        if (!other.ResourceManagerProviders.Equals(this.ResourceManagerProviders)) return false;
        // Equals
        return true;
    }
    /// <summary>Print information</summary>
    public override string ToString() => ResourceManagerProviders.ToString() ?? GetType().Name;
}

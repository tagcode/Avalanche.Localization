// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System;
using System.Linq;
using System.Resources;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>
/// Manages a list of <see cref="ResourceManager"/>. 
/// 
/// Provides resource managers for namespace query.
/// </summary>
public class ResourceManagerList : ProviderBase<string?, (string @namespace, ResourceManager resourceManager)[]>
{
    /// <summary>Resource managers and associated namespaces</summary>
    protected (string @namespace, ResourceManager resourceManager)[] resourceManagers;
    /// <summary></summary>
    protected int hashcode;

    /// <summary>Resource managers and associated namespaces</summary>
    public virtual (string @namespace, ResourceManager resourceManager)[] ResourceManagers => resourceManagers;
    /// <summary>Resource manager count</summary>
    public virtual int Count => ResourceManagers.Length;

    /// <summary></summary>
    public ResourceManagerList(ResourceManager resourceManager)
    {
        this.resourceManagers = MakeArray(new ResourceManager[] { resourceManager ?? throw new ArgumentNullException(nameof(resourceManager)) }, null);
        this.hashcode = MakeHashCode(this.resourceManagers);
    }

    /// <summary></summary>
    public ResourceManagerList(ResourceManager resourceManager, string? @namespace)
    {
        this.resourceManagers = MakeArray(new ResourceManager[] { resourceManager ?? throw new ArgumentNullException(nameof(resourceManager)) }, new string?[] { @namespace });
        this.hashcode = MakeHashCode(this.resourceManagers);
    }

    /// <summary></summary>
    public ResourceManagerList(ResourceManager[] resourceManagers)
    {
        this.resourceManagers = MakeArray(resourceManagers ?? throw new ArgumentNullException(nameof(resourceManagers)), null);
        this.hashcode = MakeHashCode(this.resourceManagers);
    }

    /// <summary></summary>
    public ResourceManagerList(ResourceManager[] resourceManagers, string?[]? namespaces)
    {
        this.resourceManagers = MakeArray(resourceManagers ?? throw new ArgumentNullException(nameof(resourceManagers)), namespaces);
        this.hashcode = MakeHashCode(this.resourceManagers);
    }

    /// <summary>Derive namespace+ResourceManager array</summary>
    static (string, ResourceManager)[] MakeArray(ResourceManager[] resourceManagers, string?[]? namespaces)
    {
        // Count number of resource managers
        int count = resourceManagers.Length;
        // Place here namespaces
        (string, ResourceManager)[] result = new (string, ResourceManager)[count];
        // Add each
        for (int i = 0; i < count; i++)
        {
            // Get resource manager
            ResourceManager resourceManager = resourceManagers[i];
            // Derive namespace
            string @namespace = (namespaces == null ? null : i >= namespaces.Length ? null : namespaces[i]) ?? resourceManager.BaseName;
            // Add to result
            result[i] = (@namespace, resourceManager);
        }
        // Return result namespaces
        return result;
    }

    /// <summary>Query resource managers and namespaces</summary>
    /// <param name="namespaceQuery">Query key. If null returns all.</param>
    public override bool TryGetValue(string? namespaceQuery, out (string @namespace, ResourceManager resourceManager)[] result)
    {
        // Return all
        if (namespaceQuery == null) { result = ResourceManagers; return true; }
        // Place filtered result here
        StructList4<(string, ResourceManager)> list = new();
        // Get array
        var _resourceManagers = ResourceManagers;
        // Filter
        foreach ((string @namespace, ResourceManager resource) in _resourceManagers)
            if (@namespace == namespaceQuery) list.Add((@namespace, resource));
        // No resource managers
        if (list.Count == 0) { result = null!; return false; }
        // Nothing was filtered out, return original array reference
        if (list.Count == _resourceManagers.Length) { result = _resourceManagers; return true; }
        // Return filtered list
        result = list.ToArray();
        return true;
    }

    /// <summary></summary>
    public override int GetHashCode() => hashcode;
    /// <summary></summary>
    protected virtual int MakeHashCode((string @namespace, ResourceManager resourceManager)[] _resourcemanagers)
    {
        // Init
        int hash = 0x3465478;
        // Hash each order-insensitive with xor
        for (int i = 0; i < _resourcemanagers.Length; i++) hash ^= _resourcemanagers[i].resourceManager.GetHashCode() ^ _resourcemanagers[i].@namespace.GetHashCode();
        // Return
        return hash;
    }

    /// <summary></summary>
    public override bool Equals(object? obj)
    {
        // Cast
        if (obj is not ResourceManagerList other) return false;
        // Get snapshot
        (string @namespace, ResourceManager resourceManager)[] _resourcemanagers1 = this.ResourceManagers, _resourcemanagers2 = other.ResourceManagers;
        // Compare each
        foreach ((string @namespace, ResourceManager resourceManager) _ in _resourcemanagers1) if (!_resourcemanagers2.Contains(_)) return false;
        foreach ((string @namespace, ResourceManager resourceManager) _ in _resourcemanagers2) if (!_resourcemanagers1.Contains(_)) return false;
        // Equals
        return true;
    }

    /// <summary>Print information</summary>
    public override string ToString() => GetType().Name + String.Join<ResourceManager>(", ", resourceManagers.Select(_ => _.resourceManager));
}

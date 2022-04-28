// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System;
using System.Reflection;
using System.Resources;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Provides <see cref="ResourceManager"/>s by searching for for type.</summary>
/// <remarks>This class is used as provider for <see cref="ResourceManagerFileProvider"/> and <see cref="ResourceManagerLineProvider"/>.</remarks>
public class ResourceManagerProvider : ProviderBase<string?, (string @namespace, ResourceManager resourceManager)[]>, ICached
{
    /// <summary>Resource manager provider singleton, non-cached</summary>
    static readonly IProvider<string?, (string @namespace, ResourceManager resourceManager)[]> instance = new ResourceManagerProvider(Avalanche.Utilities.Provider.TypeProvider.Instance);
    /// <summary>Resource manager provider singleton, cached</summary>
    static readonly IProvider<string?, (string @namespace, ResourceManager resourceManager)[]> cached = new ResourceManagerProvider(Avalanche.Utilities.Provider.TypeProvider.Cached).ValueResultCaptured().CachedNullableKey().ValueResultOpened();
    /// <summary>Resource manager provider singleton, non-cached</summary>
    public static IProvider<string?, (string @namespace, ResourceManager resourceManager)[]> Instance => instance;
    /// <summary>Resource manager provider singleton, cached</summary>
    public static IProvider<string?, (string @namespace, ResourceManager resourceManager)[]> Cached => cached;
    /// <summary>Create resource manager provider with new cache.</summary>
    public static IProvider<string?, (string @namespace, ResourceManager resourceManager)[]> CreateCached() => new ResourceManagerProvider(Avalanche.Utilities.Provider.TypeProvider.CreateCached()).ValueResultCaptured().CachedNullableKey().ValueResultOpened();

    /// <summary></summary>
    protected IProvider<string?, Type[]> typeProvider;
    /// <summary></summary>
    public virtual IProvider<string?, Type[]> TypeProvider => typeProvider;

    /// <summary></summary>
    public bool IsCached { get => typeProvider is ICached cached ? cached.IsCached : false; set => throw new InvalidOperationException(); }

    /// <summary></summary>
    public ResourceManagerProvider() : this(Avalanche.Utilities.Provider.TypeProvider.CreateCached())
    {
    }

    /// <summary></summary>
    public ResourceManagerProvider(IProvider<string?, Type[]> typeProvider)
    {
        this.typeProvider = typeProvider ?? throw new ArgumentNullException(nameof(typeProvider));
    }

    /// <summary>Evaluates whether class is a code-generated resource class</summary>
    /// <param name="type"></param>
    protected virtual bool IsResourcesClass(Type type)
    {
        // Count number of expected attributes
        int score = 0;
        // Get properties
        PropertyInfo[] properties = type.GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        // Find "ResourceManager" property
        PropertyInfo? resourceManagerProperty = null;
        foreach (PropertyInfo pi in properties) if (pi.Name == "ResourceManager" && pi.PropertyType.Equals(typeof(ResourceManager))) { resourceManagerProperty = pi; break; }
        // Got resource manager property
        if (resourceManagerProperty != null) score += 2;
        // Estimate each attribute
        foreach (object attribute in type.GetCustomAttributes(true))
        {
            // Get attribute type
            Type attributeType = attribute.GetType();

            // [GeneratedCode]
            if (attribute is System.CodeDom.Compiler.GeneratedCodeAttribute generatedCode)
            {
                if (generatedCode.Tool != null && generatedCode.Tool.Contains("Resource")) score++;
                continue;
            }
            // [DebuggerNonUserCode]
            if (attributeType.Equals(typeof(System.Diagnostics.DebuggerNonUserCodeAttribute)))
            {
                score++;
                continue;
            }

            // [CompilerGenerated]
            if (attributeType.Equals(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)))
            {
                score++;
            }
        }
        // Estimate heuristically
        return score >= 3;
    }

    /// <summary></summary>
    public override bool TryGetValue(string? typeName, out (string @namespace, ResourceManager resourceManager)[] result)
    {
        // Get reference
        var _typeProvider = TypeProvider;
        // Try load type
        if (!_typeProvider.TryGetValue(typeName, out Type[]? types) || types == null) { result = null!; return false; }
        // Build result here
        StructList1<(string @namespace, ResourceManager resourceManager)> list = new();
        // Eastimate each type
        foreach(Type type in types)
        {
            try
            {
                // Not resources
                if (!IsResourcesClass(type)) continue;
                // Create resource manager
                ResourceManager resourceManager = new ResourceManager(type);
                // Derive namespace
                string @namespace = typeName ?? type.FullName ?? type.Name;
                // Add to result
                list.Add((@namespace, resourceManager));
            }
            catch (Exception) { 
            }
        }
        // Return array
        result = list.ToArray();
        return list.Count>0;
    }

    /// <summary></summary>
    public virtual void InvalidateCache(bool deep = false)
    {
        if (typeProvider is ICached cached) cached.InvalidateCache(deep);
    }

    /// <summary></summary>
    public override int GetHashCode() => GetType().GetHashCode() ^ TypeProvider.GetHashCode();
    /// <summary></summary>
    public override bool Equals(object? obj)
    {
        // Not resource manager
        if (obj is not ResourceManagerProvider other) return false;
        // Compare type providers
        return other.TypeProvider.Equals(this.TypeProvider);
    }
}

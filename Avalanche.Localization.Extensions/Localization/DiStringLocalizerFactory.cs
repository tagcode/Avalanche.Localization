// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Internal;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary></summary>
public class DiStringLocalizerFactory : IStringLocalizerFactory
{
    /// <summary>Construct <see cref="DiStringLocalizerFactory"/> to use <paramref name="namespace"/>.</summary>
    public static DiStringLocalizerFactory Create(ILocalization localization, string @namespace, string? culture, ILoggerFactory? loggerFactory = null) => new DiStringLocalizerFactory(localization, @namespace, culture, loggerFactory);

    /// <summary></summary>
    protected ILocalization localization;
    /// <summary></summary>
    protected string @namespace;
    /// <summary>Explicit culture</summary>
    protected string? culture;
    /// <summary>Optional logger factory</summary>
    protected ILoggerFactory? loggerFactory;
    /// <summary>Cache for <see cref="Create(Type)"/></summary>
    protected Dictionary<Type, IStringLocalizer> typeStringLocalizers = new();
    /// <summary>Cache for <see cref="Create(string, string)"/></summary>
    protected Dictionary<(string, string), IStringLocalizer> resourceLocalizers = new();
    /// <summary>Lock for <see cref="typeStringLocalizers"/>.</summary>
    protected ReaderWriterLock typeStringLocalizersLock = new ReaderWriterLock();
    /// <summary>Lock for <see cref="resourceLocalizers"/>.</summary>
    protected ReaderWriterLock resourceLocalizersLock = new ReaderWriterLock();
    /// <summary>Assembly loader with cache</summary>
    protected IProvider<string, ValueResult<Assembly>> assemblyLoader = Providers.Func<string, Assembly>(Assembly.Load).ValueResultCaptured();

    /// <summary>Localization context</summary>
    public ILocalization Localization => localization;
    /// <summary>Key namespace</summary>
    public string Namespace => @namespace;
    /// <summary>Explicit culture</summary>
    public string? Culture => culture;

    /// <summary></summary>
    public DiStringLocalizerFactory(ILocalization localization)
    {
        this.localization = localization ?? throw new ArgumentNullException(nameof(localization));
        this.@namespace = "";
        this.culture = null;
    }

    /// <summary></summary>
    /// <param name="localizationOptions"></param>
    public DiStringLocalizerFactory(ILocalization localization, IOptions<LocalizationOptions> localizationOptions)
    {
        this.localization = localization ?? throw new ArgumentNullException(nameof(localization));
        this.@namespace = localizationOptions.Value.ResourcesPath ?? "";
        // Replace separator
        this.@namespace = this.@namespace.Replace(Path.DirectorySeparatorChar, '.').Replace(Path.AltDirectorySeparatorChar, '.');
        this.culture = null;
    }

    /// <summary></summary>
    public DiStringLocalizerFactory(ILocalization localization, ILoggerFactory loggerFactory)
    {
        this.localization = localization ?? throw new ArgumentNullException(nameof(localization));
        this.@namespace = "";
        this.culture = null;
        this.loggerFactory = loggerFactory;
    }

    /// <summary></summary>
    /// <param name="localizationOptions"></param>
    public DiStringLocalizerFactory(ILocalization localization, IOptions<LocalizationOptions> localizationOptions, ILoggerFactory loggerFactory)
    {
        this.localization = localization ?? throw new ArgumentNullException(nameof(localization));
        this.@namespace = localizationOptions.Value.ResourcesPath ?? "";
        // Replace separator
        this.@namespace = this.@namespace.Replace(Path.DirectorySeparatorChar, '.').Replace(Path.AltDirectorySeparatorChar, '.');
        this.culture = null;
        this.loggerFactory = loggerFactory;
    }

    /// <summary>Create for specific <paramref name="namespace"/>. This constructor is hidden from dependency injection to not confuse it, thereof this is called from <see cref="Create(ILocalization, string, string?, ILoggerFactory?)"/>.</summary>
    protected DiStringLocalizerFactory(ILocalization localization, string @namespace, string? culture, ILoggerFactory? loggerFactory)
    {
        this.localization = localization ?? throw new ArgumentNullException(nameof(localization));
        this.@namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        this.culture = culture;
        this.loggerFactory = loggerFactory;
    }

    /// <summary>Get namespace of <paramref name="typeInfo"/></summary>
    protected virtual string GetNamespace(TypeInfo typeInfo)
    {
        // Get namespace
        string baseNamespace = GetRootNamespace(typeInfo.Assembly);
        // Get resource path
        string? resourcesRelativePath = GetResourceNamespace(typeInfo.Assembly);
        // Get namespace
        string @namespace = GetNamespace(typeInfo ?? throw new ArgumentNullException(nameof(typeInfo)), baseNamespace, resourcesRelativePath);
        // Return
        return @namespace;
    }

    /// <summary>Get namespace of <paramref name="typeInfo"/></summary>
    /// <param name="typeInfo">Type to look up</param>
    /// <param name="baseNamespace">The base namespace of the application.</param>
    /// <param name="resourcesRelativePath">The folder containing all resources.</param>
    /// <returns>"baseNamespace[.resourceRelativePath].TypeName</returns>
    protected virtual string GetNamespace(TypeInfo typeInfo, string baseNamespace, string? resourcesRelativePath)
    {
        // Assert not empty
        if (string.IsNullOrEmpty(baseNamespace)) throw new ArgumentNullException(nameof(baseNamespace));
        // No relative path, return full name
        if (string.IsNullOrEmpty(resourcesRelativePath)) return typeInfo.FullName!;
        // Return with relative path
        else
        {
            // Formulate assembly name
            string assemblyName = new AssemblyName(typeInfo.Assembly.FullName!).Name!;
            // Create builder
            StringBuilder sb = new StringBuilder(255);
            // Append namespace
            sb.Append(baseNamespace);
            // Append resourcesRelativePath
            if (!string.IsNullOrEmpty(resourcesRelativePath))
            {
                // '.'
                if (sb.Length>0) sb.Append('.');
                // Append resourcesRelativePath
                sb.Append(resourcesRelativePath);
            }
            // Formulate typename
            string typename = TrimPrefix(typeInfo.FullName ?? "", assemblyName + ".");
            // Append 'typename'
            if (!string.IsNullOrEmpty(typename))
            {
                // '.'
                if (sb.Length > 0) sb.Append('.');
                // Append resourcesRelativePath
                sb.Append(typename);
            }
            // Print
            return sb.ToString();
        }
    }

    /// <summary>Derive resource namespace. Utilizes assembly name, <see cref="RootNamespaceAttribute"/> and <see cref="ResourceLocationAttribute"/>.</summary>
    /// <param name="baseResourceName">The namespace of the resource to be looked up, e.g. "Namespace.Resources"</param>
    /// <param name="baseNamespace">Assembly namespace.</param>
    /// <returns>Namespace</returns>
    protected virtual string GetResourceNamespace(string baseResourceName, string baseNamespace)
    {
        // Assert not empty
        if (string.IsNullOrEmpty(baseNamespace)) throw new ArgumentNullException(nameof(baseNamespace));

        // Try to read assembly
        Assembly? assembly = !string.IsNullOrEmpty(baseNamespace) && assemblyLoader.TryGetValue(baseNamespace, out ValueResult<Assembly> assemblyResult) && assemblyResult.Value != null ? assemblyResult.Value : null;

        // Derive resource name
        if (assembly != null)
        {
            // Derive namespace
            string rootNamespace = GetRootNamespace(assembly);
            string resourceLocation = GetResourceNamespace(assembly);
            string locationPath = rootNamespace + "." + resourceLocation;
            string @namespace = locationPath + TrimPrefix(baseResourceName, baseNamespace + ".");
            // Return namespace
            return @namespace;
        }
        // Derive without assembly
        {
            // Make namespace
            string @namespace = baseNamespace + (string.IsNullOrEmpty(baseResourceName) ? "" : "." + baseResourceName);
            // Trim
            @namespace = TrimPrefix(@namespace, baseNamespace + ".");
            // Return
            return @namespace;
        }
    }

    /// <summary>Derive root namespace from assembly or <see cref="RootNamespaceAttribute"/>.</summary>
    /// <returns>Namespace with '.' separators</returns>
    protected virtual string GetRootNamespace(Assembly assembly)
    {
        // Get [RootNamespace()] attribute.
        RootNamespaceAttribute? rootNamespaceAttribute = assembly.GetCustomAttribute<RootNamespaceAttribute>();
        // Derive namespace
        return rootNamespaceAttribute?.RootNamespace ?? assembly.GetName().Name ?? "";
    }

    /// <summary>Derive resource namespace from assembly name or <see cref="ResourceLocationAttribute"/> attribute.</summary>
    /// <returns>Namespace with '.' separators</returns>
    protected virtual string GetResourceNamespace(Assembly assembly)
    {
        // Get [ResourceLocation()] attribute.
        ResourceLocationAttribute? resourceLocationAttribute = assembly.GetCustomAttribute<ResourceLocationAttribute>();
        // Derive resource location from attribute or relative path
        string resourceLocation = resourceLocationAttribute != null ? resourceLocationAttribute.ResourceLocation : @namespace;
        // Replace separator with '.'
        resourceLocation = resourceLocation.Replace(Path.DirectorySeparatorChar, '.').Replace(Path.AltDirectorySeparatorChar, '.');
        // Retrun namespace
        return resourceLocation;
    }

    /// <summary>Remove <paramref name="prefix"/> from beginning of <paramref name="name"/></summary>
    public static string TrimPrefix(string name, string prefix)
    {
        // Return without prefix
        if (name.StartsWith(prefix, StringComparison.Ordinal)) return name.Substring(prefix.Length);
        // Return as is
        return name;
    }

    /// <summary>Get-or-create localizer for <paramref name="resourceSource"/>.</summary>
    public IStringLocalizer Create(Type resourceSource)
    {
        // Assert not null
        if (resourceSource == null) throw new ArgumentNullException(nameof(resourceSource));
        // Lock
        typeStringLocalizersLock.AcquireReaderLock(int.MaxValue);
        try
        {
            // Get existing
            if (typeStringLocalizers.TryGetValue(resourceSource, out IStringLocalizer? stringLocalizer)) return stringLocalizer;
            // 
            TypeInfo typeInfo = resourceSource.GetTypeInfo();
            // Get namespace
            string localizerNamespace = GetNamespace(resourceSource.GetTypeInfo());
            // Place logger here
            ILogger ? logger = null;
            // Assign logger
            if (loggerFactory != null) logger = loggerFactory.CreateLogger(typeof(DiStringLocalizer<>).MakeGenericType(resourceSource));
            // Create StringLocalizer<T>
            stringLocalizer = DiStringLocalizer.Create(resourceSource, localization, localizerNamespace, culture, logger);
            // Add within write
            LockCookie cookie = typeStringLocalizersLock.UpgradeToWriterLock(int.MaxValue);
            try
            {
                // Try again 
                if (typeStringLocalizers.TryGetValue(resourceSource, out IStringLocalizer? stringLocalizer2)) return stringLocalizer2;
                // Add under write lock
                typeStringLocalizers[resourceSource] = stringLocalizer!;
                // Return
                return stringLocalizer!;
            }
            finally
            {
                typeStringLocalizersLock.DowngradeFromWriterLock(ref cookie);
            }
        }
        finally
        {
            typeStringLocalizersLock.ReleaseReaderLock();
        }
    }

    /// <summary></summary>
    public IStringLocalizer Create(string baseName, string location)
    {
        // Assert not null
        if (baseName == null) throw new ArgumentNullException(nameof(baseName));
        if (location == null) throw new ArgumentNullException(nameof(location));
        // Lock
        resourceLocalizersLock.AcquireReaderLock(int.MaxValue);
        try
        {
            // Create cache key
            (string, string) cacheKey = (baseName, location);
            // Get existing
            if (resourceLocalizers.TryGetValue(cacheKey, out IStringLocalizer? stringLocalizer)) return stringLocalizer;
            // Derive namespace
            string localizerNamespace = GetResourceNamespace(baseName, location);
            // Create localizer
            stringLocalizer = new DiStringLocalizer(localization, localizerNamespace, culture);
            // Add within write
            LockCookie cookie = resourceLocalizersLock.UpgradeToWriterLock(int.MaxValue);
            try
            {
                // Try again 
                if (resourceLocalizers.TryGetValue(cacheKey, out IStringLocalizer? stringLocalizer2)) return stringLocalizer2;
                // Add under write lock
                resourceLocalizers[cacheKey] = stringLocalizer!;
                // Return
                return stringLocalizer!;
            }
            finally
            {
                resourceLocalizersLock.DowngradeFromWriterLock(ref cookie);
            }
        }
        finally
        {
            resourceLocalizersLock.ReleaseReaderLock();
        }
    }


    /// <summary></summary>
    public override bool Equals(object? obj)
    {
        // Cast
        if (obj is not DiStringLocalizerFactory other) return false;
        //
        if (this.Culture != other.Culture) return false;
        if (this.Namespace != other.Namespace) return false;
        if (this.Localization != other.Localization) return false;
        // Equals
        return true;
    }

    /// <summary></summary>
    public override int GetHashCode()
    {
        //
        FNVHash32 hash = new();
        hash.HashIn(this.Culture);
        hash.HashIn(this.Namespace);
        hash.HashIn(this.Localization);
        //
        return hash.Hash;
    }

    /// <summary>Print information</summary>
    public override string ToString() => @namespace;
}


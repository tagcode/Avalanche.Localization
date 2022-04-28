// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>File format description</summary>
public class LocalizationFileFormat : ILocalizationFileFormat
{
    /// <summary>Provider that returns binary file format. Must contain separator, e.g. ".xml"</summary>
    static readonly IProvider<string, ILocalizationFileFormat> constructor = Providers.Func<string, ILocalizationFileFormat>(ext => new LocalizationFileFormat(ext));
    /// <summary>Cached provider that returns binary file format. Must contain separator, e.g. ".xml"</summary>
    static readonly IProvider<string, ILocalizationFileFormat> cached = constructor.Cached();
    /// <summary>Provider that returns binary file format. Must contain separator, e.g. ".xml"</summary>
    public static IProvider<string, ILocalizationFileFormat> Constructor => constructor;
    /// <summary>Cached provider that returns binary file format. Must contain separator, e.g. ".xml"</summary>
    public static IProvider<string, ILocalizationFileFormat> Cached => cached;
    /// <summary>No extension file format ""</summary>
    public static ILocalizationFileFormat NoExtension => Cached[""];

    /// <summary>Extensions</summary>
    protected string[] extensions;
    /// <summary>Extensions</summary>
    public virtual string[] Extensions => extensions;

    /// <summary></summary>
    public LocalizationFileFormat(params string[] extensions)
    {
        this.extensions = extensions ?? throw new ArgumentNullException(nameof(extensions));
    }

    /// <summary></summary>
    public override bool Equals(object? obj) 
    {
        if (obj is not ILocalizationFileFormat other) return false;
        string[] exts1 = other.Extensions, exts2 = this.Extensions;
        if (exts1.Length != exts2.Length) return false;
        foreach (string ext in exts2) if (!exts1.Contains(ext)) return false;
        foreach (string ext in exts1) if (!exts2.Contains(ext)) return false;
        return true;            
    }

    /// <summary></summary>
    public override int GetHashCode() 
    {
        FNVHash32 hash = new();
        hash.HashIn(GetType());
        foreach (string ext in extensions)
            hash.HashIn(ext);
        return hash.Hash;
    }

    /// <summary></summary>
    public override string ToString() => String.Join(',', Extensions);
}

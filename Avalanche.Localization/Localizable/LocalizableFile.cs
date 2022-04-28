// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary></summary>
public class LocalizableFile : ILocalizable<ILocalizationFile>
{
    /// <summary></summary>
    protected ILocalized<ILocalizationFile>? @default;
    /// <summary></summary>
    protected string key;
    /// <summary></summary>
    protected IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> fileQuery;
    /// <summary></summary>
    protected int hash;

    /// <summary></summary>
    public ILocalized<ILocalizationFile>? Default => @default ??= Localize("");
    /// <summary></summary>
    public string Key { get => key; set => throw new InvalidOperationException(); }

    /// <summary></summary>
    public LocalizableFile(string key, IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> fileQuery)
    {
        this.key = key ?? throw new ArgumentNullException(nameof(key));
        this.fileQuery = fileQuery ?? throw new ArgumentNullException(nameof(fileQuery));

        FNVHash32 hash = new FNVHash32();
        hash.HashInObject(key);
        hash.HashInObject(fileQuery);
        this.hash = hash.Hash;
    }

    /// <summary></summary>
    public ILocalized<ILocalizationFile>? Localize(string language)
    {
        // Query files
        if (!fileQuery.TryGetValue((language, key), out IEnumerable<ILocalizationFile> files)) return null;
        // Get first
        ILocalizationFile? file = files.FirstOrDefault();
        // No file
        if (file == null) return null;
        // Convert to localized
        ILocalized<ILocalizationFile> localizedFile = Localized<ILocalizationFile>.GetOrCreate(key, language, file);
        // Return localized file
        return localizedFile;
    }

    /// <summary></summary>
    public override bool Equals(object? obj)
    {
        //
        if (obj is not LocalizableFile other) return false;
        //
        if (other.Key == this.Key && this.fileQuery == other.fileQuery) return true;
        //
        return false;
    }

    /// <summary></summary>
    public override int GetHashCode() => hash;
    /// <summary>Print information</summary>
    public override string ToString() => Key;
}

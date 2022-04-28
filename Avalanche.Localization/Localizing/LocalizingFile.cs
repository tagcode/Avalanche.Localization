// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>File that localizes to the current culture in an assigned <see cref="ICultureProvider"/>.</summary>
public class LocalizingFile : ILocalizing<ILocalizationFile>
{
    /// <summary></summary>
    protected string key;
    /// <summary></summary>
    protected ICultureProvider cultureProvider;
    /// <summary>Querier for localized texts</summary>
    protected IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> fileQuery = null!;
    /// <summary>Default text for invariant culture</summary>
    protected ILocalizationFile? defaultFile;

    /// <summary></summary>
    public ICultureProvider CultureProvider { get => cultureProvider; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string Key { get => key; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public virtual ILocalized<ILocalizationFile>? Value
    {
        get
        {
            // Get language
            if (!cultureProvider.TryGetCulture(out string language)) return null!;
            // Query file
            if (fileQuery != null && fileQuery.TryGetValue((language, key), out IEnumerable<ILocalizationFile> files))
            {
                // Get file
                ILocalizationFile? file = files.FirstOrDefault();
                // No file
                if (file == null) return null!;
                // Wrap into localized
                ILocalized<ILocalizationFile> localized = Localized<ILocalizationFile>.GetOrCreate(key, language, file);
                // Return
                return localized;
            }
            // No file
            return null!;
        }
    }

    /// <summary></summary>
    public LocalizingFile(string key, ICultureProvider cultureProvider, IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> fileQuery)
    {
        this.key = key ?? throw new ArgumentNullException(nameof(key));
        this.cultureProvider = cultureProvider ?? throw new ArgumentNullException(nameof(cultureProvider));
        this.fileQuery = fileQuery ?? throw new ArgumentNullException(nameof(fileQuery));
    }

    /// <summary></summary>
    public override bool Equals(object? obj)
    {
        // Not same
        if (obj is not ILocalizing<ILocalizationFile> other) return false;
        //
        if (!this.CultureProvider.Equals(other.CultureProvider)) return false;
        //
        if (!this.Key.Equals(other.Key)) return false;
        //
        if (other is LocalizingFile other2 && (this.fileQuery != other2.fileQuery)) return false;
        //
        return true;
    }

    /// <summary></summary>
    public override int GetHashCode()
    {
        FNVHash32 hash = new();
        hash.HashIn(Key);
        hash.HashIn(CultureProvider);
        if (fileQuery != null) hash.HashIn(fileQuery);
        return hash.Hash;
    }

    /// <summary></summary>
    public override string ToString() => Value?.ToString() ?? Key;
}

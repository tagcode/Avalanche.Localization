// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System;
using System.Collections.Generic;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Applies queries into <![CDATA[IEnumerable<ILocalizationFile>]]>.</summary>
public class LocalizationFilesQuerier : ProviderBase<(string? culture, string? key), IEnumerable<ILocalizationFile>>
{
    /// <summary>File provider</summary>
    protected IEnumerable<ILocalizationFile> files;
    /// <summary>File provider</summary>
    public virtual IEnumerable<ILocalizationFile> Files => files;

    /// <summary></summary>
    public LocalizationFilesQuerier(IEnumerable<ILocalizationFile> files)
    {
        this.files = files ?? throw new ArgumentNullException(nameof(files));
    }

    /// <summary></summary>
    public override bool TryGetValue((string? culture, string? key) query, out IEnumerable<ILocalizationFile> resultFiles)
    {
        // Get snapshots
        IList<ILocalizationFile> _files = ArrayUtilities.GetSnapshot(Files);
        // Place results here
        StructList10<ILocalizationFile> result = new();
        // Iterate
        foreach (ILocalizationFile file in _files)
        {
            // Disqualify by culture
            if (query.culture != null && file.Culture != query.culture) continue;
            // Disqualify by key
            if (query.key != null && file.Key != query.key) continue;
            // Add to result
            result.Add(file);
        }
        // Return
        resultFiles = result.ToArray();
        return true;
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"{GetType().Name}({files})";
}


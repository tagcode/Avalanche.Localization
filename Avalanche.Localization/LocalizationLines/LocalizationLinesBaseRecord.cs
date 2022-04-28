// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Collections.Generic;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Record of line query settings and providers</summary>
public class LocalizationLinesBaseRecord : ReadOnlyAssignableClass, ILocalizationLines, ICached
{
    /// <summary>The provider that converts <see cref="ILocalizationFile"/> into lines.</summary>
    protected IProvider<ILocalizationFile, IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> fileReader = null!;
    /// <summary>The provider that converts <see cref="ILocalizationFile"/> into lines and caches.</summary>
    protected IProvider<ILocalizationFile, IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> fileReaderCached = null!;
    /// <summary>Providers that return files that contain localization lines (Have <see cref="ILocalizationLineFileFormat"/> as format).</summary>
    protected IList<IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>>> fileProviders = null!;
    /// <summary>Providers that return files that contain localization lines (Have <see cref="ILocalizationLineFileFormat"/> as format).</summary>
    protected IList<IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>>> fileProvidersCached = null!;
    /// <summary>Explictly added lines</summary>
    protected IList<IEnumerable<KeyValuePair<string, MarkedText>>> lines = null!;
    /// <summary>Line query provider. Culture and Key are constraints, if null then less is filtered and more is passed.</summary>
    protected IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> query = null!;
    /// <summary>Line query provider. Culture and Key are constraints, if null then less is filtered and more is passed.</summary>
    protected IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> queryCached = null!;

    /// <summary>Line query providers.</summary>
    protected IList<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>> lineProviders = null!;
    /// <summary>Line query providers.</summary>
    protected IList<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>> lineProvidersCached = null!;

    /// <summary>The provider that converts <see cref="ILocalizationFile"/> into lines.</summary>
    public virtual IProvider<ILocalizationFile, IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> FileReader { get => fileReader; set => this.AssertWritable().fileReader = value; }
    /// <summary>The provider that converts <see cref="ILocalizationFile"/> into lines and caches.</summary>
    public virtual IProvider<ILocalizationFile, IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> FileReaderCached { get => fileReaderCached; set => this.AssertWritable().fileReaderCached = value; }
    /// <summary>Providers that return files that contain localization lines (Have <see cref="ILocalizationLineFileFormat"/> as format).</summary>
    public virtual IList<IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>>> FileProviders { get => fileProviders; set => this.AssertWritable().fileProviders = value; }
    /// <summary>Providers that return files that contain localization lines (Have <see cref="ILocalizationLineFileFormat"/> as format).</summary>
    public virtual IList<IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>>> FileProvidersCached { get => fileProvidersCached; set => this.AssertWritable().fileProvidersCached = value; }
    /// <summary>Explictly added lines</summary>
    public virtual IList<IEnumerable<KeyValuePair<string, MarkedText>>> Lines { get => lines; set => this.AssertWritable().lines = value; }
    /// <summary>Line query provider. Culture and Key are constraints, if null then less is filtered and more is passed.</summary>
    public virtual IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> Query { get => query; set => this.AssertWritable().query = value; }
    /// <summary>Line query provider. Culture and Key are constraints, if null then less is filtered and more is passed.</summary>
    public virtual IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> QueryCached { get => queryCached; set => this.AssertWritable().queryCached = value; }
    /// <summary>Line query providers.</summary>
    public virtual IList<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>> LineProviders { get => lineProviders; set => this.AssertWritable().lineProviders = value; }
    /// <summary>Line query providers.</summary>
    public virtual IList<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>> LineProvidersCached { get => lineProvidersCached; set => this.AssertWritable().lineProvidersCached = value; }
    
    /// <summary></summary>
    public virtual void InvalidateCache(bool deep = false)
    {
        FileReaderCached.InvalidateCache(true);
        QueryCached.InvalidateCache(true);
        if (deep)
        {
            foreach (var o0 in ArrayUtilities.GetSnapshot(FileProviders)) if (o0 is ICached cached0) cached0.InvalidateCache(deep);
            foreach (var o0 in ArrayUtilities.GetSnapshot(FileProvidersCached)) if (o0 is ICached cached0) cached0.InvalidateCache(deep);
            foreach (var o1 in ArrayUtilities.GetSnapshot(LineProviders)) if (o1 is ICached cached0) cached0.InvalidateCache(deep);
        }
    }

    /// <summary></summary>
    bool ICached.IsCached { get => true; set => throw new InvalidOperationException(); }
}


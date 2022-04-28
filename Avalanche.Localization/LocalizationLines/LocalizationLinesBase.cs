// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Record of line query settings and providers</summary>
public class LocalizationLinesBase : LocalizationLinesBaseRecord
{
    /// <summary></summary>
    public LocalizationLinesBase() : base()
    {
        this.FileReader = LocalizationLineFileReader.Instance;
        this.FileReaderCached = this.FileReader.ValueResultCaptured().Cached().ValueResultOpened();
        this.FileProviders = new ArrayList<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>>();
        this.FileProvidersCached = new ArrayList<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>>();
        this.Lines = new ArrayList<IEnumerable<KeyValuePair<string, MarkedText>>>();

        // List for providers
        this.LineProviders = new ArrayList<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>>();
        this.LineProvidersCached = new ArrayList<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>>();
        // Add provider that reads from the .Lines field
        var staticLinesQuerier = new LocalizationLineQuerier(this.LinesField());
        this.LineProviders.Add(staticLinesQuerier);
        this.LineProvidersCached.Add(staticLinesQuerier.ValueResultCaptured().Cached().ValueResultOpened());
        // Add provider that reads from file lines (with cache)
        this.LineProviders.Add(new LocalizationFileLineQuerier(this.FileProvidersField(), Providers.Indirect(() => this.FileReader)));
        this.LineProvidersCached.Add(new LocalizationFileLineQuerier(this.FileProvidersCachedField(), Providers.Indirect(() => this.FileReaderCached)));
        
        // Queries from 'Providers'
        this.Query = Avalanche.Utilities.Provider.Providers.EnumerableConcat(this.LineProvidersField(), distinctValues: true, LocalizationLineEqualityComparer.All);
        // Cached queries
        this.QueryCached = Avalanche.Utilities.Provider.Providers.EnumerableConcat(this.LineProvidersCachedField(), distinctValues: true, LocalizationLineEqualityComparer.All).ValueResultCaptured().Cached().ValueResultOpened();
    }

    /// <summary>Deep read-only assignment.</summary>
    protected override void setReadOnly()
    {
        this.FileProviders = this.FileProviders.ToArray();
        this.FileProvidersCached = this.FileProvidersCached.ToArray();
        this.Lines = this.Lines.ToArray();
        this.LineProviders = this.LineProviders.ToArray();
        this.LineProvidersCached = this.LineProvidersCached.ToArray();
        base.setReadOnly();
    }

    /// <summary></summary>
    public override void InvalidateCache(bool deep = false)
    {
        // Parent invalidation
        base.InvalidateCache(deep);
    }
}

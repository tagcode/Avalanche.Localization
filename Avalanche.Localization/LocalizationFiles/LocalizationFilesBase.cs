// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Record of localization file properties and providers with initial providers.</summary>
public class LocalizationFilesBase : LocalizationFilesBaseRecord
{
    /// <summary></summary>
    public LocalizationFilesBase() : base()
    {
        // Lists
        this.FileFormats = new ArrayList<ILocalizationFileFormat>();
        this.FileSystems = new ArrayList<ILocalizationFileSystem>();
        this.FilePatterns = new ArrayList<ITemplateFormatPrintable>();
        this.Files = new ArrayList<ILocalizationFile>();
        this.FileProviders = new ArrayList<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>>();
        this.FileProvidersCached = new ArrayList<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>>();
        this.FileSystemsListCached = new SnapshotProviderDecorator<ILocalizationFileSystem>(this.FileSystemsField(), selector: fs => LocalizationFileSystemCached.FileListCacheDecorator[fs]);
        // Providers
        var filesQuerier = new LocalizationFilesQuerier(this.FilesField());
        this.FileProviders.Add(filesQuerier);
        this.FileProvidersCached.Add(filesQuerier);
        this.FileProviders.Add(new LocalizationFilesQuerierWithPattern(this.FileFormatsField(), this.FileSystemsField(), this.FilePatternsField()));
        this.FileProvidersCached.Add(new LocalizationFilesQuerierWithPattern(this.FileFormatsField(), this.FileSystemsListCachedField(), this.FilePatternsField()));
        // Concatenate localization file queriers
        this.Query = Providers.EnumerableConcat(this.FileProvidersField(), true);
        // Query provider that decorates each provider to use cached file lists
        this.QueryCached = Providers.EnumerableConcat(this.FileProvidersCachedField(), true).ValueResultCaptured().Cached().ValueResultOpened();
        /*
        this.QueryCached =             
            Providers.ResultConcat(
                new SnapshotProviderDecorator<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>>(this.FileProvidersField(), 
                selector: provider => (IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>) (provider is ILocalizationFileSystemProvider lfsp ? lfsp.RecreateWith(new SnapshotProviderDecorator<ILocalizationFileSystem>(lfsp.FileSystems, selector: fs => LocalizationFileSystemCached.FileListCacheDecorator[fs])) : provider)
                ), 
                true);
        */
    }

    /// <summary>Deep read-only assignment.</summary>
    protected override void setReadOnly()
    {
        this.FileFormats = this.FileFormats.ToArray();
        this.FileSystems = this.FileSystems.ToArray();
        this.FilePatterns = this.FilePatterns.ToArray();
        this.Files = this.Files.ToArray();
        this.FileProviders = this.FileProviders.ToArray();
        this.FileProvidersCached = this.FileProvidersCached.ToArray();
        base.setReadOnly();
    }

    /// <summary></summary>
    public override void InvalidateCache(bool deep = false)
    {
        // Invalidate file-list caches.
        foreach (var fs in filesystemsListCached) if (fs is ICached cached) cached.InvalidateCache(true);
        // Parent invalidation
        base.InvalidateCache(deep);
    }
}

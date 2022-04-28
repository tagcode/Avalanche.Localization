// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Record of localization file properties and providers.</summary>
public class LocalizationFilesBaseRecord : ReadOnlyAssignableClass, ILocalizationFiles, ICached
{
    /// <summary>Localization file formats.</summary>
    protected IList<ILocalizationFileFormat> fileFormats = null!;
    /// <summary>File systems</summary>
    protected IList<ILocalizationFileSystem> fileSystems = null!;
    /// <summary>File name patterns without extension, e.g. "{Culture}/{Namespace}". Template text can uses parameters: "Culture", "Namespace", "Name", "Key"</summary>
    protected IList<ITemplateFormatPrintable> filePatterns = null!;
    /// <summary>Explicit localization files</summary>
    protected IList<ILocalizationFile> files = null!;
    /// <summary>File providers that provide by culture+key</summary>
    protected IList<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>> fileProviders = null!;
    /// <summary>File providers that provide by culture+key</summary>
    protected IList<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>> fileProvidersCached = null!;
    /// <summary>Culture+key query provider. (no caching)</summary>
    protected IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> query = null!;
    /// <summary>Culture+key query provider. (cached)</summary>
    protected IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> queryCached = null!;
    /// <summary>Returns file-list cached versions of file-systems in FileSystems property. Is invalidated with <see cref="InvalidateCache(bool)"/>.</summary>
    protected IEnumerable<ILocalizationFileSystem> filesystemsListCached = null!;

    /// <summary>Localization file formats.</summary>
    public virtual IList<ILocalizationFileFormat> FileFormats { get => fileFormats; set => this.AssertWritable().fileFormats = value; }
    /// <summary>File systems</summary>
    public virtual IList<ILocalizationFileSystem> FileSystems { get => fileSystems; set => this.AssertWritable().fileSystems = value; }
    /// <summary>File name patterns without extension, e.g. "{Culture}/{Namespace}". Template text can uses parameters: "Culture", "Namespace", "Name", "Key"</summary>
    public virtual IList<ITemplateFormatPrintable> FilePatterns { get => filePatterns; set => this.AssertWritable().filePatterns = value; }
    /// <summary>Explicit localization files</summary>
    public virtual IList<ILocalizationFile> Files { get => files; set => this.AssertWritable().files = value; }
    /// <summary>File providers that provide by culture+key</summary>
    public virtual IList<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>> FileProviders { get => fileProviders; set => this.AssertWritable().fileProviders = value; }
    /// <summary>File providers that provide by culture+key</summary>
    public virtual IList<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>> FileProvidersCached { get => fileProvidersCached; set => this.AssertWritable().fileProvidersCached = value; }

    /// <summary>Returns file-list cached versions of file-systems in FileSystems property. Is invalidated with <see cref="InvalidateCache(bool)"/>.</summary>
    public virtual IEnumerable<ILocalizationFileSystem> FileSystemsListCached { get => filesystemsListCached; set => this.AssertWritable().filesystemsListCached = value; }

    /// <summary>Culture+key query provider. (no caching)</summary>
    public virtual IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> Query { get => query; set => this.AssertWritable().query = value; }
    /// <summary>Culture+key query provider. (cached)</summary>
    public virtual IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> QueryCached { get => queryCached; set => this.AssertWritable().queryCached = value; }
    /// <summary>Invalidates caches.</summary>
    public virtual void InvalidateCache(bool deep = false) 
    {
        QueryCached.InvalidateCache(true);
        if (deep)
        {
            foreach (var o0 in ArrayUtilities.GetSnapshot(FileFormats)) if (o0 is ICached cached0) cached0.InvalidateCache(deep);
            foreach (var o1 in ArrayUtilities.GetSnapshot(FileSystems)) if (o1 is ICached cached1) cached1.InvalidateCache(deep);
            foreach (var o1 in ArrayUtilities.GetSnapshot(FileSystemsListCached)) if (o1 is ICached cached1) cached1.InvalidateCache(deep);
            foreach (var o2 in ArrayUtilities.GetSnapshot(FilePatterns)) if (o2 is ICached cached2) cached2.InvalidateCache(deep);
            foreach (var o3 in ArrayUtilities.GetSnapshot(Files)) if (o3 is ICached cached3) cached3.InvalidateCache(deep);
            foreach (var o4 in ArrayUtilities.GetSnapshot(FileProviders)) if (o4 is ICached cached4) cached4.InvalidateCache(deep);
            foreach (var o4 in ArrayUtilities.GetSnapshot(FileProvidersCached)) if (o4 is ICached cached4) cached4.InvalidateCache(deep);
            foreach (var o5 in ArrayUtilities.GetSnapshot(FileProviders)) if (o5 is ICached cached5) cached5.InvalidateCache(deep);
            if (Query is ICached cached6) cached6.InvalidateCache(deep);
        }
    }
    /// <summary></summary>
    bool ICached.IsCached { get => true; set => throw new InvalidOperationException(); }
}


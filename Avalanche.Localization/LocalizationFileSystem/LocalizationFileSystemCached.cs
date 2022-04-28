// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary></summary>
public class LocalizationFileSystemCached : ILocalizationFileSystem, IDecoration, ICache
{
    /// <summary>Decorator that adds file-listing caching to file-systems. </summary>
    protected static IProvider<ILocalizationFileSystem, ILocalizationFileSystem> fileListCacheDecorator = Providers.Func<ILocalizationFileSystem, ILocalizationFileSystem>(fs => new LocalizationFileSystemCached(fs, true, false)).WeakCached();
    /// <summary>Decorator that adds caches files and file-listing caching to file-systems. </summary>
    protected static IProvider<ILocalizationFileSystem, ILocalizationFileSystem> cacheDecorator = Providers.Func<ILocalizationFileSystem, ILocalizationFileSystem>(fs => new LocalizationFileSystemCached(fs, true, true)).WeakCached();
    /// <summary>Decorator that adds file-listing caching to file-systems. </summary>
    public static IProvider<ILocalizationFileSystem, ILocalizationFileSystem> FileListCacheDecorator => fileListCacheDecorator;
    /// <summary>Decorator that adds caches files and file-listing caching to file-systems. </summary>
    public static IProvider<ILocalizationFileSystem, ILocalizationFileSystem> CacheDecorator => cacheDecorator;

    /// <summary></summary>
    protected ILocalizationFileSystem filesystem;

    /// <summary></summary>
    protected ConcurrentDictionary<string, ValueResult<string[]>>? filelistCache;
    /// <summary></summary>
    protected ConcurrentDictionary<string, ValueResult<string[]>>? directorylistCache;
    /// <summary></summary>
    protected ConcurrentDictionary<string, ValueResult<byte[]>>? fileCache;

    /// <summary>Provides file lists</summary>
    protected IProvider<string, string[]> fileListProvider;
    /// <summary>Provides directory lists</summary>
    protected IProvider<string, string[]> directoryListProvider;
    /// <summary>Provides files</summary>
    protected IProvider<string, byte[]> fileProvider;

    /// <summary>Provides file lists</summary>
    public IProvider<string, string[]> FileList => fileListProvider;
    /// <summary>Provides directory lists</summary>
    public IProvider<string, string[]> DirectoryList => directoryListProvider;
    /// <summary>Provides files</summary>
    public IProvider<string, byte[]> File => fileProvider;

    /// <summary></summary>
    public bool CacheFileLists => /*fileListProvider is ICached cached ? cached.IsCached : false*/ filelistCache != null;
    /// <summary></summary>
    public bool CacheFiles => /*fileProvider is ICached cached ? cached.IsCached : false*/ fileCache != null;

    /// <summary></summary>
    public LocalizationFileSystemCached(ILocalizationFileSystem filesystem, bool cacheLists, bool cacheFiles)
    {
        this.filesystem = filesystem ?? throw new ArgumentNullException(nameof(filesystem));
        this.fileListProvider = new FileListProvider(filesystem);
        this.directoryListProvider = new DirectoryListProvider(filesystem);
        this.fileProvider = new FileProvider(filesystem);
        if (cacheLists)
        {
            filelistCache = new ConcurrentDictionary<string, ValueResult<string[]>>();
            fileListProvider = fileListProvider.ValueResultCaptured().Cached(filelistCache).ValueResultOpened();
            directorylistCache = new ConcurrentDictionary<string, ValueResult<string[]>>();
            directoryListProvider = directoryListProvider.ValueResultCaptured().Cached(directorylistCache).ValueResultOpened();
        }
        if (cacheFiles)
        {
            fileCache = new ConcurrentDictionary<string, ValueResult<byte[]>>();
            fileProvider = fileProvider.ValueResultCaptured().Cached(fileCache).ValueResultOpened();
        }
    }

    /// <summary></summary>
    public bool IsCache { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public bool IsCached { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public bool IsDecoration { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public object? Decoree { get => filesystem; set => throw new InvalidOperationException(); }

    /// <summary></summary>
    public void InvalidateCache(bool deep = false)
    {
        directorylistCache?.Clear();
        filelistCache?.Clear();
        fileCache?.Clear();
    }

    /// <summary></summary>
    public bool TryListFiles(string path, [NotNullWhen(true)] out string[]? files)
    {
        // No caching, forward to source
        if (filelistCache == null) return filesystem.TryListFiles(path, out files);
        // 
        return this.fileListProvider.TryGetValue(path, out files);
    }

    /// <summary></summary>
    public bool TryListDirectories(string path, [NotNullWhen(true)] out string[]? directories)
    {
        // No caching, forward to source
        if (directorylistCache == null) return filesystem.TryListDirectories(path, out directories);
        // 
        return this.directoryListProvider.TryGetValue(path, out directories);
    }

    /// <summary></summary>
    public bool TryOpen(string filename, [NotNullWhen(true)] out Stream stream)
    {
        // No caching, forward to source
        if (fileCache == null) return filesystem.TryOpen(filename, out stream);
        // 
        if (!this.fileProvider.TryGetValue(filename, out byte[] data)) { stream = null!; return false; }
        // 
        stream = new MemoryStream(data);
        return true;
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"{filesystem}.Cached({FileList}, {File})";

    /// <summary></summary>
    public class FileListProvider : ProviderBase<string, string[]>
    {
        /// <summary></summary>
        protected ILocalizationFileSystem filesystem;

        /// <summary></summary>
        public FileListProvider(ILocalizationFileSystem filesystem)
        {
            this.filesystem = filesystem ?? throw new ArgumentNullException(nameof(filesystem));
        }

        /// <summary></summary>
        public override bool TryGetValue(string path, out string[] files)
        {
            // No listing
            if (!filesystem.TryListFiles(path, out files!)) { files = null!; return false; }
            // Got listing
            return true;
        }
        /// <summary>Print information</summary>
        public override string ToString() => $"{filesystem}.{GetType().Name}";
    }

    /// <summary></summary>
    public class DirectoryListProvider : ProviderBase<string, string[]>
    {
        /// <summary></summary>
        protected ILocalizationFileSystem filesystem;

        /// <summary></summary>
        public DirectoryListProvider(ILocalizationFileSystem filesystem)
        {
            this.filesystem = filesystem ?? throw new ArgumentNullException(nameof(filesystem));
        }

        /// <summary></summary>
        public override bool TryGetValue(string path, out string[] directories)
        {
            // No listing
            if (!filesystem.TryListDirectories(path, out directories!)) { directories = null!; return false; }
            // Got listing
            return true;
        }
        /// <summary>Print information</summary>
        public override string ToString() => $"{filesystem}.{GetType().Name}";
    }

    /// <summary></summary>
    public class FileProvider : ProviderBase<string, byte[]>
    {
        /// <summary></summary>
        protected ILocalizationFileSystem filesystem;

        /// <summary></summary>
        public FileProvider(ILocalizationFileSystem filesystem)
        {
            this.filesystem = filesystem ?? throw new ArgumentNullException(nameof(filesystem));
        }

        /// <summary></summary>
        public override bool TryGetValue(string filename, out byte[] data)
        {
            // File did not open
            if (!filesystem.TryOpen(filename, out Stream stream)) { data = null!; return false; }
            try
            {
                // Read data
                data = stream.ReadFully()!;
                // Done
                return true;
            }
            finally
            {
                stream?.Dispose();
            }
        }
        /// <summary>Print information</summary>
        public override string ToString() => $"{filesystem}.{GetType().Name}";
    }

}

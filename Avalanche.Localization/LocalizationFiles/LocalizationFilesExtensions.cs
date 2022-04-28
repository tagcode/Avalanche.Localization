// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Localization.Internal;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;
using System.Collections;

/// <summary>Additional extension methods for <see cref="ILocalizationFiles"/>.</summary>
public static class LocalizationFilesExtensions_
{
    /// <summary>Add file patterns without extension, e.g. "{Culture}/{Namespace}". Template text can uses parameters: "Culture", "Namespace", "Name", "Key".</summary>
    public static L AddFilePattern<L>(this L localization, string bracePattern) where L : ILocalizationFiles
    {
        // Add file pattern
        localization.FilePatterns.Add(TemplateFormat.BraceAlphaNumeric.Breakdown[bracePattern]);
        // Return
        return localization;
    }

    /// <summary>Add file patterns without extension, e.g. "{Culture}/{Namespace}". Template text can uses parameters: "Culture", "Namespace", "Name", "Key".</summary>
    public static L AddFilePatterns<L>(this L localization, params string[] bracePatterns) where L : ILocalizationFiles
    {
        // Add file pattern
        foreach (string pattern in bracePatterns)
        {
            // Create pattern
            var template = TemplateFormat.BraceAlphaNumeric.Breakdown[pattern];
            // Add template
            localization.FilePatterns.Add(template);
        }
        // Return
        return localization;
    }

    /// <summary>Add file patterns without extension, e.g. "{Culture}/{Namespace}". Template text can uses parameters: "Culture", "Namespace", "Name", "Key".</summary>
    public static L AddFilePatterns<L>(this L localization, params ITemplateFormatPrintable[] patterns) where L : ILocalizationFiles
    {
        // Add file patterns
        localization.FilePatterns.AddRange(patterns);
        // Return
        return localization;
    }

    /// <summary>Add file patterns without extension, e.g. "{Culture}/{Namespace}". Template text can uses parameters: "Culture", "Namespace", "Name", "Key".</summary>
    public static L AddFilePatterns<L>(this L localization, ILocalizationFilePatterns filePatterns) where L : ILocalizationFiles
    {
        // Add each
        foreach (var _pattern in filePatterns.Patterns)
        {
            // Add template
            localization.FilePatterns.AddIfNew(_pattern);
        }
        // Return
        return localization;
    }

    /// <summary>Create enumerable that reads a snapshot of <see cref="ILocalizationFiles.FileFormats"/> field.</summary>
    public static IEnumerable<ILocalizationFileFormat> FileFormatsField(this ILocalizationFiles localizationFiles) => new FileFormatsFieldEnumerable(localizationFiles);
    /// <summary>Create enumerable that reads a snapshot of <see cref="ILocalizationFiles.FileSystems"/> field.</summary>
    public static IEnumerable<ILocalizationFileSystem> FileSystemsField(this ILocalizationFiles localizationFiles) => new FileSystemsFieldEnumerable(localizationFiles);
    /// <summary>Create enumerable that reads a snapshot of <see cref="ILocalizationFiles.FileSystemsListCached"/> field.</summary>
    public static IEnumerable<ILocalizationFileSystem> FileSystemsListCachedField(this ILocalizationFiles localizationFiles) => new FileSystemsListCachedFieldEnumerable(localizationFiles);
    /// <summary>Create enumerable that reads a snapshot of <see cref="ILocalizationFiles.FilePatterns"/> field.</summary>
    public static IEnumerable<ITemplateFormatPrintable> FilePatternsField(this ILocalizationFiles localizationFiles) => new FilePatternsFieldEnumerable(localizationFiles);
    /// <summary>Create enumerable that reads a snapshot of <see cref="ILocalizationFiles.Files"/> field.</summary>
    public static IEnumerable<ILocalizationFile> FilesField(this ILocalizationFiles localizationFiles) => new FilesFieldEnumerable(localizationFiles);
    /// <summary>Create enumerable that reads a snapshot of <see cref="ILocalizationFiles.FileProviders"/> field.</summary>
    public static IEnumerable<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>> FileProvidersField(this ILocalizationFiles localizationFiles) => new FileProvidersFieldEnumerable(localizationFiles);
    /// <summary>Create enumerable that reads a snapshot of <see cref="ILocalizationFiles.FileProviders"/> field.</summary>
    public static IEnumerable<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>> FileProvidersCachedField(this ILocalizationFiles localizationFiles) => new FileProvidersCachedFieldEnumerable(localizationFiles);

    /// <summary>Decorates <paramref name="filesQuerier"/> to use <paramref name="fallbackCultureProvider"/>.</summary>
    /// <returns>Decorated provider</returns>
    public static IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> WithFallbackLookup(this IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> filesQuerier, IProvider<string, string[]> fallbackCultureProvider)
    {
        // Null 
        if (fallbackCultureProvider == null) return filesQuerier;
        // Return decoration
        return new CultureFallbackDecoration<IEnumerable<ILocalizationFile>>(filesQuerier, fallbackCultureProvider);
    }

    /// <summary>Add file system with specific pattern. File system is added as file provider to <see cref="ILocalizationFiles.FileProviders"/>.</summary>
    public static L AddFileSystemWithPattern<L>(this L localization, ILocalizationFileSystem filesystem, ILocalizationFilePatterns patterns, IEnumerable<ILocalizationFileFormat>? formats = null) where L : ILocalizationFiles
    {
        // Assign formats from localization.Files
        if (formats == null) formats = localization.FileFormatsField();
        // Create file provider
        var fileProvider = new LocalizationFilesQuerierWithPattern(formats, new ILocalizationFileSystem[] { filesystem }, patterns.Patterns);
        // Add file provider
        localization.AddFileProvider(fileProvider, fileProvider.ValueResultCaptured().Cached().ValueResultOpened());
        // Return
        return localization;
    }

    /// <summary>Add file system with specific pattern. File system is added as file provider to <see cref="ILocalizationFiles.FileProviders"/>.</summary>
    public static L AddFileSystemWithPattern<L>(this L localization, ILocalizationFileSystem filesystem, params string[] patterns) where L : ILocalizationFiles
    {
        // Assign formats from localization.Files
        IEnumerable<ILocalizationFileFormat> formats = localization.FileFormatsField();
        // Create patterns
        ILocalizationFilePatterns _patterns = new LocalizationFilePatterns(patterns);
        // Create file provider
        var fileProvider = new LocalizationFilesQuerierWithPattern(formats, new ILocalizationFileSystem[] { filesystem }, _patterns.Patterns);
        // Add file provider
        localization.AddFileProvider(fileProvider, fileProvider.ValueResultCaptured().Cached().ValueResultOpened());
        // Return
        return localization;
    }

    /// <summary>Returns snapshot value of <see cref="ILocalizationFiles.FileProviders"/>.</summary>
    internal class FileFormatsFieldEnumerable : IEnumerable<ILocalizationFileFormat>, ISnapshotProvider<ILocalizationFileFormat>
    {
        /// <summary></summary>
        public readonly ILocalizationFiles LocalizationFiles;
        /// <summary></summary>
        public ILocalizationFileFormat[] Snapshot { get => ArrayUtilities.GetSnapshotArray(LocalizationFiles.FileFormats); set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public FileFormatsFieldEnumerable(ILocalizationFiles localizationFiles) => this.LocalizationFiles = localizationFiles ?? throw new ArgumentNullException(nameof(localizationFiles));
        /// <summary></summary>
        public IEnumerator<ILocalizationFileFormat> GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationFiles.FileFormats).GetEnumerator();
        /// <summary></summary>
        IEnumerator IEnumerable.GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationFiles.FileFormats).GetEnumerator();
        /// <summary>Print information</summary>
        public override string ToString() => $"{LocalizationFiles}.{nameof(ILocalizationFiles.FileFormats)}";
    }

    /// <summary>Returns snapshot value of <see cref="ILocalizationFiles.FileSystems"/>.</summary>
    internal class FileSystemsFieldEnumerable : IEnumerable<ILocalizationFileSystem>, ISnapshotProvider<ILocalizationFileSystem>
    {
        /// <summary></summary>
        public readonly ILocalizationFiles LocalizationFiles;
        /// <summary></summary>
        public ILocalizationFileSystem[] Snapshot { get => ArrayUtilities.GetSnapshotArray(LocalizationFiles.FileSystems); set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public FileSystemsFieldEnumerable(ILocalizationFiles localizationFiles) => this.LocalizationFiles = localizationFiles ?? throw new ArgumentNullException(nameof(localizationFiles));
        /// <summary></summary>
        public IEnumerator<ILocalizationFileSystem> GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationFiles.FileSystems).GetEnumerator();
        /// <summary></summary>
        IEnumerator IEnumerable.GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationFiles.FileSystems).GetEnumerator();
        /// <summary>Print information</summary>
        public override string ToString() => $"{LocalizationFiles}.{nameof(ILocalizationFiles.FileSystems)}";
    }

    /// <summary>Returns snapshot value of <see cref="ILocalizationFiles.FileSystemsListCached"/>.</summary>
    internal class FileSystemsListCachedFieldEnumerable : IEnumerable<ILocalizationFileSystem>, ISnapshotProvider<ILocalizationFileSystem>
    {
        /// <summary></summary>
        public readonly ILocalizationFiles LocalizationFiles;
        /// <summary></summary>
        public ILocalizationFileSystem[] Snapshot { get => ArrayUtilities.GetSnapshotArray(LocalizationFiles.FileSystemsListCached); set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public FileSystemsListCachedFieldEnumerable(ILocalizationFiles localizationFiles) => this.LocalizationFiles = localizationFiles ?? throw new ArgumentNullException(nameof(localizationFiles));
        /// <summary></summary>
        public IEnumerator<ILocalizationFileSystem> GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationFiles.FileSystemsListCached).GetEnumerator();
        /// <summary></summary>
        IEnumerator IEnumerable.GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationFiles.FileSystemsListCached).GetEnumerator();
        /// <summary>Print information</summary>
        public override string ToString() => $"{LocalizationFiles}.{nameof(ILocalizationFiles.FileSystemsListCached)}";
    }

    /// <summary>Returns snapshot value of <see cref="ILocalizationFiles.FilePatterns"/>.</summary>
    internal class FilePatternsFieldEnumerable : IEnumerable<ITemplateFormatPrintable>, ISnapshotProvider<ITemplateFormatPrintable>
    {
        /// <summary></summary>
        public readonly ILocalizationFiles LocalizationFiles;
        /// <summary></summary>
        public ITemplateFormatPrintable[] Snapshot { get => ArrayUtilities.GetSnapshotArray(LocalizationFiles.FilePatterns); set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public FilePatternsFieldEnumerable(ILocalizationFiles localizationFiles) => this.LocalizationFiles = localizationFiles ?? throw new ArgumentNullException(nameof(localizationFiles));
        /// <summary></summary>
        public IEnumerator<ITemplateFormatPrintable> GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationFiles.FilePatterns).GetEnumerator();
        /// <summary></summary>
        IEnumerator IEnumerable.GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationFiles.FilePatterns).GetEnumerator();
        /// <summary>Print information</summary>
        public override string ToString() => $"{LocalizationFiles}.{nameof(ILocalizationFiles.FilePatterns)}";
    }

    /// <summary>Returns snapshot value of <see cref="ILocalizationFiles.Files"/>.</summary>
    internal class FilesFieldEnumerable : IEnumerable<ILocalizationFile>, ISnapshotProvider<ILocalizationFile>
    {
        /// <summary></summary>
        public readonly ILocalizationFiles LocalizationFiles;
        /// <summary></summary>
        public ILocalizationFile[] Snapshot { get => ArrayUtilities.GetSnapshotArray(LocalizationFiles.Files); set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public FilesFieldEnumerable(ILocalizationFiles localizationFiles) => this.LocalizationFiles = localizationFiles ?? throw new ArgumentNullException(nameof(localizationFiles));
        /// <summary></summary>
        public IEnumerator<ILocalizationFile> GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationFiles.Files).GetEnumerator();
        /// <summary></summary>
        IEnumerator IEnumerable.GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationFiles.Files).GetEnumerator();
        /// <summary>Print information</summary>
        public override string ToString() => $"{LocalizationFiles}.{nameof(ILocalizationFiles.Files)}";
    }

    /// <summary>Returns snapshot value of <see cref="ILocalizationFiles.FileProviders"/>.</summary>
    internal class FileProvidersFieldEnumerable : IEnumerable<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>>, ISnapshotProvider<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>>
    {
        /// <summary></summary>
        public readonly ILocalizationFiles LocalizationFiles;
        /// <summary></summary>
        public IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>[] Snapshot { get => ArrayUtilities.GetSnapshotArray(LocalizationFiles.FileProviders); set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public FileProvidersFieldEnumerable(ILocalizationFiles localizationFiles) => this.LocalizationFiles = localizationFiles ?? throw new ArgumentNullException(nameof(localizationFiles));
        /// <summary></summary>
        public IEnumerator<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>> GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationFiles.FileProviders).GetEnumerator();
        /// <summary></summary>
        IEnumerator IEnumerable.GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationFiles.FileProviders).GetEnumerator();
        /// <summary>Print information</summary>
        public override string ToString() => $"{LocalizationFiles}.{nameof(ILocalizationFiles.FileProviders)}";
    }

    /// <summary>Returns snapshot value of <see cref="ILocalizationFiles.FileProvidersCached"/>.</summary>
    internal class FileProvidersCachedFieldEnumerable : IEnumerable<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>>, ISnapshotProvider<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>>
    {
        /// <summary></summary>
        public readonly ILocalizationFiles LocalizationFiles;
        /// <summary></summary>
        public IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>[] Snapshot { get => ArrayUtilities.GetSnapshotArray(LocalizationFiles.FileProvidersCached); set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public FileProvidersCachedFieldEnumerable(ILocalizationFiles localizationFiles) => this.LocalizationFiles = localizationFiles ?? throw new ArgumentNullException(nameof(localizationFiles));
        /// <summary></summary>
        public IEnumerator<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>> GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationFiles.FileProvidersCached).GetEnumerator();
        /// <summary></summary>
        IEnumerator IEnumerable.GetEnumerator() => ArrayUtilities.GetSnapshot(LocalizationFiles.FileProvidersCached).GetEnumerator();
        /// <summary>Print information</summary>
        public override string ToString() => $"{LocalizationFiles}.{nameof(ILocalizationFiles.FileProvidersCached)}";
    }

}


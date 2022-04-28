// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;
using Avalanche.Utilities.Provider;

/// <summary>Extension methods for <see cref="ILocalizationFiles"/></summary>
public static class LocalizationFilesExtensions
{
    /// <summary>Add file</summary>
    public static L AddFile<L>(this L localization, ILocalizationFile file) where L : ILocalizationFiles
    {
        // Add file 
        localization.Files.AddIfNew(file);
        // Return
        return localization;
    }

    /// <summary>Add file patterns without extension, e.g. "{Culture}/{Namespace}". Template text can uses parameters: "Culture", "Namespace", "Name", "Key".</summary>
    public static L AddFilePattern<L>(this L localization, ITemplateFormatPrintable pattern) where L : ILocalizationFiles
    {
        // Add file pattern
        localization.FilePatterns.AddIfNew(pattern);
        // Return
        return localization;
    }

    /// <summary>Add file patterns without extension, e.g. "{Culture}/{Namespace}". Template text can uses parameters: "Culture", "Namespace", "Name", "Key".</summary>
    public static L AddFilePatterns<L>(this L localization, params ITemplateFormatPrintable[] patterns) where L : ILocalizationFiles
    {
        // Add file pattern
        foreach (var pattern in patterns)
        {
            // Add template
            localization.FilePatterns.AddIfNew(pattern);
        }
        // Return
        return localization;
    }

    /// <summary>Add file system</summary>
    public static L AddFileSystem<L>(this L localization, ILocalizationFileSystem fileSystem) where L : ILocalizationFiles
    {
        // Add file format
        localization.FileSystems.AddIfNew(fileSystem);
        // Return
        return localization;
    }

    /// <summary>Add file system</summary>
    public static L AddFileSystem<L>(this L localization, params ILocalizationFileSystem[] filesystems) where L : ILocalizationFiles
    {
        // Add file format
        foreach (ILocalizationFileSystem filesystem in filesystems)
            localization.FileSystems.AddIfNew(filesystem);
        // Return
        return localization;
    }

    /// <summary>Add file format</summary>
    public static L AddFileFormat<L>(this L localization, ILocalizationFileFormat fileFormat) where L : ILocalizationFiles
    {
        // Add file format
        localization.FileFormats.AddIfNew(fileFormat);
        // Return
        return localization;
    }

    /// <summary>Add file formats</summary>
    public static L AddFileFormats<L>(this L localization, params ILocalizationFileFormat[] fileformats) where L : ILocalizationFiles
    {
        // Add file format
        foreach (ILocalizationFileFormat fileformat in fileformats)
            localization.FileFormats.AddIfNew(fileformat);
        // Return
        return localization;
    }

    /// <summary>Add <paramref name="provider"/> to <paramref name="localizationFiles"/>.</summary>
    public static L AddFileProvider<L>(this L localizationFiles,
        IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> provider,
        IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> providerCached) where L : ILocalizationFiles
    {
        // Add file provider
        localizationFiles.FileProviders.AddIfNew(provider);
        localizationFiles.FileProvidersCached.AddIfNew(providerCached);
        // Return lines
        return localizationFiles;
    }
}


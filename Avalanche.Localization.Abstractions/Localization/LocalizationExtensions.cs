// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Extension methods for <see cref="ILocalization"/></summary>
public static class LocalizationExtensions
{
    /// <summary>Fallback cultures provider</summary>
    public static L SetFallbackCultureProvider<L>(this L localization, IProvider<string, string[]> fallbackCultureProvider) where L : ILocalization
    {
        // Assign
        localization.FallbackCultureProvider = fallbackCultureProvider;
        // Return
        return localization;
    }

    /// <summary>Add file patterns without extension, e.g. "{Culture}/{Namespace}". Template text can uses parameters: "Culture", "Namespace", "Name", "Key".</summary>
    public static L AddFilePattern<L>(this L localization, ITemplateFormatPrintable pattern) where L : ILocalization
    {
        // Add file pattern
        localization.Files.FilePatterns.AddIfNew(pattern);
        // Return
        return localization;
    }

    /// <summary>Add file</summary>
    public static L AddFile<L>(this L localization, ILocalizationFile file) where L : ILocalization
    {
        // Add file 
        localization.Files.Files.AddIfNew(file);
        // Return
        return localization;
    }

    /// <summary>Add file patterns without extension, e.g. "{Culture}/{Namespace}". Template text can uses parameters: "Culture", "Namespace", "Name", "Key".</summary>
    public static L AddFilePatterns<L>(this L localization, params ITemplateFormatPrintable[] patterns) where L : ILocalization
    {
        // Add file pattern
        foreach (var pattern in patterns)
        {
            // Add template
            localization.Files.FilePatterns.AddIfNew(pattern);
        }
        // Return
        return localization;
    }

    /// <summary>Add file system</summary>
    public static L AddFileSystem<L>(this L localization, ILocalizationFileSystem fileSystem) where L : ILocalization
    {
        // Add file format
        localization.Files.FileSystems.AddIfNew(fileSystem);
        // Return
        return localization;
    }

    /// <summary>Add file system</summary>
    public static L AddFileSystem<L>(this L localization, params ILocalizationFileSystem[] filesystems) where L : ILocalization
    {
        // Add file format
        foreach(ILocalizationFileSystem filesystem in filesystems)
            localization.Files.FileSystems.AddIfNew(filesystem);
        // Return
        return localization;
    }

    /// <summary>Add file format</summary>
    public static L AddFileFormat<L>(this L localization, ILocalizationFileFormat fileFormat) where L : ILocalization
    {
        // Add file format
        localization.Files.FileFormats.AddIfNew(fileFormat);
        // Return
        return localization;
    }

    /// <summary>Add file formats</summary>
    public static L AddFileFormats<L>(this L localization, params ILocalizationFileFormat[] fileformats) where L : ILocalization
    {
        // Add file format
        foreach(ILocalizationFileFormat fileformat in fileformats) 
            localization.Files.FileFormats.AddIfNew(fileformat);
        // Return
        return localization;
    }

    /// <summary>Add <paramref name="provider"/> to <paramref name="localizationLines"/>.</summary>
    public static L AddLineProvider<L>(this L localizationLines,
        IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> provider,
        IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> providerCached) where L : ILocalization
    {
        // Add file provider
        localizationLines.Lines.LineProviders.AddIfNew(provider);
        localizationLines.Lines.LineProvidersCached.AddIfNew(providerCached);
        // Return lines
        return localizationLines;
    }

    /// <summary>Add <paramref name="fileProvider"/> to <paramref name="localizationLines"/>.</summary>
    public static L AddFileProvider<L>(this L localizationLines, IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>> fileProvider, IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>> fileProviderCached) where L : ILocalization
    {
        // Add file provider
        localizationLines.Files.FileProviders.AddIfNew(fileProvider);
        localizationLines.Files.FileProvidersCached.AddIfNew(fileProvider);
        // Return lines
        return localizationLines;
    }

    /// <summary>Add explicit <paramref name="line"/> to <paramref name="localizationLines"/>.</summary>
    public static L AddLine<L>(this L localizationLines, IEnumerable<KeyValuePair<string, MarkedText>> line) where L : ILocalization
    {
        // Add line
        localizationLines.Lines.Lines.Add(line);
        // Return lines
        return localizationLines;
    }

    /// <summary>Add explicit <paramref name="lines"/> to <paramref name="localizationLines"/>.</summary>
    public static L AddLines<L>(this L localizationLines, IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines) where L : ILocalization
    {
        // Get reference
        var _lines = localizationLines.Lines.Lines;
        // Add each line
        foreach (var line in lines)
            _lines.Add(line);
        // Return lines
        return localizationLines;
    }

    /// <summary>Add explicit line.</summary>
    public static L AddLine<L>(this L localizationLines, string culture, string key, string templateFormat, string text, string? pluralRules = null, string? plurals = null) where L : ILocalization
    {
        // Create line
        KeyValuePair<string, MarkedText>[] line = LocalizationLinesExtensions.CreateLine(culture, key, templateFormat, text, pluralRules, plurals);
        // Add line
        localizationLines.Lines.Lines.Add(line);
        // Return lines
        return localizationLines;
    }

    /// <summary>Add <paramref name="errorHandler"/>.</summary>
    public static L AddErrorHandler<L>(this L localization, ILocalizationErrorHandler errorHandler) where L : ILocalization
    {
        //
        localization.ErrorHandlers.AddIfNew(errorHandler);
        // Return lines
        return localization;
    }

    /// <summary>Assign template formats</summary>
    public static T SetAllTemplateFormats<T>(this T localization, params ITemplateFormat[] allformats) where T : ILocalization
    {
        // Assign
        localization.TemplateFormats.AllFormats = allformats;
        // Return
        return localization;
    }

    /// <summary>Assign template formats</summary>
    public static T AddTemplateFormat<T>(this T localization, ITemplateFormat format) where T : ILocalization
    {
        // Assign
        localization.TemplateFormats.AddTemplateFormat(format);
        // Return
        return localization;
    }

    /// <summary>Assign template formats</summary>
    public static T AddTemplateFormats<T>(this T localization, params ITemplateFormat[] formats) where T : ILocalization
    {
        // Assign
        localization.TemplateFormats.AddTemplateFormats(formats);
        // Return
        return localization;
    }

    /// <summary>Add <paramref name="element"/> to <paramref name="collection"/> if new.</summary>
    /// <returns>true if element was new and was added.</returns>
    internal static bool AddIfNew<E>(this ICollection<E> collection, E element, IEqualityComparer<E>? equalityComparer = default)
    {
        // Assert argument
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        // Find element in collection
        bool containsElement = equalityComparer == null ? collection.Contains(element) : collection.Contains(element, equalityComparer);
        // Existed, not added
        if (containsElement) return false;
        // Add
        collection.Add(element);
        // Was added
        return true;
    }

}


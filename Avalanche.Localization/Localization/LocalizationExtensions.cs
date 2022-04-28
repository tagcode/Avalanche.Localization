// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;
using Avalanche.Utilities;

/// <summary>Additional extension methods for <see cref="ILocalization"/>.</summary>
public static class LocalizationExtensions_
{
    /// <summary>Localize <paramref name="text"/> to use <paramref name="localization"/>.</summary>
    public static ILocalizedText Localize(this ILocalization localization, ITemplateText text, string key, ICultureProvider? cultureProvider = null)
    {
        // Create localizable text
        if (localization.LocalizableTextCached != null) return new LocalizableText(key, localization.FormatLocalizedTextCached, cultureProvider, text);
        // Create localizing text
        if (localization.LocalizedTextCached != null) return new LocalizableText(key, localization.LocalizedTextCached, cultureProvider, text);
        // Create localizable text
        if (localization.LocalizableText != null) return new LocalizableText(key, localization.FormatLocalizedText, cultureProvider, text);
        // Create localizing text
        if (localization.LocalizedText != null) return new LocalizableText(key, localization.LocalizedText, cultureProvider, text);
        // Localization is not initialized
        return new LocalizedText(text, null!, null!, key);
    }

    /// <summary>Add file patterns without extension, e.g. "{Culture}/{Namespace}". Template text can uses parameters: "Culture", "Namespace", "Name", "Key".</summary>
    public static L AddFilePattern<L>(this L localization, string bracePattern) where L : ILocalization
    {
        // Add file pattern
        localization.Files.FilePatterns.Add(TemplateFormat.BraceAlphaNumeric.Breakdown[bracePattern]);
        // Return
        return localization;
    }

    /// <summary>Add file patterns without extension, e.g. "{Culture}/{Namespace}". Template text can uses parameters: "Culture", "Namespace", "Name", "Key".</summary>
    public static L AddFilePatterns<L>(this L localization, params string[] bracePatterns) where L : ILocalization
    {
        // Add file pattern
        foreach (string pattern in bracePatterns)
        {
            // Create pattern
            var template = TemplateFormat.BraceAlphaNumeric.Breakdown[pattern];
            // Add template
            localization.Files.FilePatterns.Add(template);
        }
        // Return
        return localization;
    }

    /// <summary>Add file patterns without extension, e.g. "{Culture}/{Namespace}". Template text can uses parameters: "Culture", "Namespace", "Name", "Key".</summary>
    public static L AddFilePatterns<L>(this L localization, ILocalizationFilePatterns filePatterns) where L : ILocalization
    {
        // Add each
        foreach (var _pattern in filePatterns.Patterns)
        {
            // Add template
            localization.Files.FilePatterns.AddIfNew(_pattern);
        }
        // Return
        return localization;
    }

    /// <summary>Add explicit <paramref name="errorHandler"/> to <paramref name="localization"/>.</summary>
    public static L AddErrorHandler<L>(this L localization, Action<ILocalizationError> errorHandler) where L : ILocalization
    {
        //
        localization.ErrorHandlers.Add(new LocalizationErrorHandler(errorHandler));
        // Return lines
        return localization;
    }

    /// <summary>Add file system with specific pattern. File system is added as file provider to <see cref="ILocalizationFiles.FileProviders"/>.</summary>
    public static L AddFileSystemWithPattern<L>(this L localization, ILocalizationFileSystem filesystem, ILocalizationFilePatterns patterns, IEnumerable<ILocalizationFileFormat>? formats = null) where L : ILocalization
    {
        // Add
        localization.Files.AddFileSystemWithPattern(filesystem, patterns, formats);
        // Return
        return localization;
    }

    /// <summary>Add file system with specific pattern. File system is added as file provider to <see cref="ILocalizationFiles.FileProviders"/>.</summary>
    public static L AddFileSystemWithPattern<L>(this L localization, ILocalizationFileSystem filesystem, params string[] patterns) where L : ILocalization
    {
        // Add
        localization.Files.AddFileSystemWithPattern(filesystem, patterns);
        // Return
        return localization;
    }

}


// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Extension methods for <see cref="ILocalizationLines"/></summary>
public static class LocalizationLinesExtensions
{
    /// <summary>Add <paramref name="provider"/> to <paramref name="localizationLines"/>.</summary>
    public static L AddLineProvider<L>(this L localizationLines, 
        IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> provider,
        IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> providerCached) where L : ILocalizationLines
    {
        // Add file provider
        localizationLines.LineProviders.AddIfNew(provider);
        localizationLines.LineProvidersCached.AddIfNew(providerCached);
        // Return lines
        return localizationLines;
    }

    /// <summary>Add <paramref name="fileProvider"/> to <paramref name="localizationLines"/>.</summary>
    public static L AddFileProvider<L>(this L localizationLines, IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>> fileProvider, IProvider<(string? culture, string? @namespace), IEnumerable<ILocalizationFile>> fileProviderCached) where L : ILocalizationLines
    {
        // Add file provider
        localizationLines.FileProviders.Add(fileProvider);
        localizationLines.FileProvidersCached.Add(fileProvider);
        // Return lines
        return localizationLines;
    }

    /// <summary>Add explicit <paramref name="line"/> to <paramref name="localizationLines"/>.</summary>
    public static L AddLine<L>(this L localizationLines, IEnumerable<KeyValuePair<string, MarkedText>> line) where L : ILocalizationLines
    {
        // Add line
        localizationLines.Lines.Add(line);
        // Return lines
        return localizationLines;
    }

    /// <summary>Add explicit <paramref name="lines"/> to <paramref name="localizationLines"/>.</summary>
    public static L AddLines<L>(this L localizationLines, IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines) where L : ILocalizationLines
    {
        // Get reference
        var _lines = localizationLines.Lines;
        // Add each line
        foreach (var line in lines)
            _lines.Add(line);
        // Return lines
        return localizationLines;
    }

    /// <summary>Add explicit line.</summary>
    public static L AddLine<L>(this L localizationLines, string culture, string key, string templateFormat, string text, string? pluralRules = null, string? plurals = null) where L : ILocalizationLines
    {
        // Create line
        KeyValuePair<string, MarkedText>[] line = CreateLine(culture, key, templateFormat, text, pluralRules, plurals);
        // Add line
        localizationLines.Lines.Add(line);
        // Return lines
        return localizationLines;
    }

    /// <summary>Create line</summary>
    public static KeyValuePair<string, MarkedText>[] CreateLine(string culture, string key, string templateFormat, string text, string? pluralRules = null, string? plurals = null)
    {
        // Count
        int count = 0;
        if (culture != null) count++;
        if (key != null) count++;
        if (templateFormat != null) count++;
        if (text != null) count++;
        if (pluralRules != null) count++;
        if (plurals != null) count++;
        // Create line
        KeyValuePair<string, MarkedText>[] line = new KeyValuePair<string, MarkedText>[count];
        // Assign values
        int ix = 0;
        if (culture != null) line[ix++] = new KeyValuePair<string, MarkedText>("Culture", culture);
        if (key != null) line[ix++] = new KeyValuePair<string, MarkedText>("Key", key);
        if (templateFormat != null) line[ix++] = new KeyValuePair<string, MarkedText>("TemplateFormat", templateFormat);
        if (text != null) line[ix++] = new KeyValuePair<string, MarkedText>("Text", text);
        if (pluralRules != null) line[ix++] = new KeyValuePair<string, MarkedText>("PluralRules", pluralRules);
        if (plurals != null) line[ix++] = new KeyValuePair<string, MarkedText>("Plurals", plurals);
        // Return lines
        return line;
    }

}


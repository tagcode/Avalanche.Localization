// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Localization.Internal;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Extension methods for <![CDATA[IEnumerable<KeyValuePair<string, MarkedText>>]]></summary>
public static class LocalizationLineExtensions_
{
    /// <summary>Print line</summary>
    public static string Print(this IEnumerable<KeyValuePair<string, MarkedText>> line) => string.Join(", ", line.Select(a => $"{a.Key}={a.Value.AsString}"));
    /// <summary>Decorate <paramref name="reader"/> to append <paramref name="prefix"/>.</summary>
    public static IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> Prefix(this IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader, IEnumerable<KeyValuePair<string, MarkedText>> prefix) => new LocalizationLinePrefixer(reader, prefix);
    /// <summary>Decorate <paramref name="reader"/> to append <paramref name="prefix"/>.</summary>
    public static IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> Prefix(this IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader, params KeyValuePair<string, MarkedText>[] prefix) => new LocalizationLinePrefixer(reader, prefix);
    /// <summary>Decorate <paramref name="reader"/> to append <paramref name="prefix"/>.</summary>
    public static IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> Prefix(this IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader, params (string, MarkedText)[] prefix) => new LocalizationLinePrefixer(reader, prefix.Select(kv => new KeyValuePair<string, MarkedText>(kv.Item1, kv.Item2)).ToArray());
    /// <summary>Decorate <paramref name="reader"/> to append <paramref name="key"/> and <paramref name="value"/>.</summary>
    public static IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> Prefix(this IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader, string key, MarkedText value) => new LocalizationLinePrefixer(reader, new KeyValuePair<string, MarkedText>[] { new KeyValuePair<string, MarkedText>(key, value) });
    /// <summary>Decorate <paramref name="reader"/> to assign <paramref name="filename"/> to every <see cref="MarkedText"/>.</summary>
    public static IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> AnnotateFilename(this IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader, string filename) => new LocalizationLineFilenameAnnotator(reader, filename);

    /// <summary>Decorates <paramref name="linesQuerier"/> to use <paramref name="fallbackCultureProvider"/>.</summary>
    /// <returns>Decorated provider</returns>
    public static IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> WithFallbackLookup(this IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> linesQuerier, IProvider<string, string[]> fallbackCultureProvider)
    {
        // Null 
        if (fallbackCultureProvider == null) return linesQuerier;
        // Return decoration
        return new CultureFallbackDecoration<IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>(linesQuerier, fallbackCultureProvider);
    }
}


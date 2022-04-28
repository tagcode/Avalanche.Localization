// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary></summary>
public class LocalizedTextProvider : ProviderBase<(string? culture, string? key), ILocalizedText>
{
    /// <summary>Line provider</summary>
    protected IProvider<(string? culture, string? key), IEnumerable<ILocalizationLinesInfo>> localizationLinesInfoProvider;

    /// <summary>Line provider</summary>
    public virtual IProvider<(string? culture, string? key), IEnumerable<ILocalizationLinesInfo>> LocalizationLinesInfoProvider => localizationLinesInfoProvider;

    /// <summary>Create plurality info provider</summary>
    /// <param name="localizationLinesInfoProvider">source of lines</param>
    public LocalizedTextProvider(IProvider<(string? culture, string? key), IEnumerable<ILocalizationLinesInfo>> localizationLinesInfoProvider)
    {
        this.localizationLinesInfoProvider = localizationLinesInfoProvider ?? throw new ArgumentNullException(nameof(localizationLinesInfoProvider));
    }

    /// <summary>Create plurality info provider</summary>
    /// <param name="lineProvider">source of lines</param>
    public LocalizedTextProvider(
        IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> lineProvider,
        IProvider<IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>, IEnumerable<ILocalizationLinesInfo>>? lineInfoProvider = default)
    {
        this.localizationLinesInfoProvider = lineProvider.Concat(lineInfoProvider ?? Avalanche.Localization.LocalizationLinesInfoProvider.Instance);
    }

    /// <summary>Create plurality info provider</summary>
    /// <param name="lineProvider">source of lines</param>
    public LocalizedTextProvider(
        IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> lineProvider,
        IProvider<string, ITemplateFormat> templateFormatProvider,
        Action<ILocalizationError>? errorHandler = default)
    {
        this.localizationLinesInfoProvider = lineProvider.Concat(new LocalizationLinesInfoProvider(templateFormatProvider, errorHandler: errorHandler));
    }

    /// <summary></summary>
    public override bool TryGetValue((string? culture, string? key) query, out ILocalizedText value)
    {
        // Query
        if (!LocalizationLinesInfoProvider.TryGetValue(query, out IEnumerable<ILocalizationLinesInfo> texts)) { value = null!; return false; }
        //
        int count = texts.Count();
        // No texts
        if (count == 0) { value = null!; return false; }
        // Unexpected count
        if (count > 1) { throw new InvalidOperationException($"Expected one or zero {nameof(LocalizationLinesInfo)} for query {query}, but got {count} results."); }
        // Get line info
        ILocalizationLinesInfo lineInfo = texts.First();
        // Return
        value = new LocalizedTextFromInfo(lineInfo);
        return true;
    }
}


/// <summary></summary>
public class LocalizedTextProvider2 : ProviderBase<((string? culture, IFormatProvider format), string? key), ILocalizedText>
{
    /// <summary>Line provider</summary>
    protected IProvider<(string? culture, string? key), IEnumerable<ILocalizationLinesInfo>> localizationLinesInfoProvider;

    /// <summary>Line provider</summary>
    public virtual IProvider<(string? culture, string? key), IEnumerable<ILocalizationLinesInfo>> LocalizationLinesInfoProvider => localizationLinesInfoProvider;

    /// <summary>Create plurality info provider</summary>
    /// <param name="localizationLinesInfoProvider">source of lines</param>
    public LocalizedTextProvider2(IProvider<(string? culture, string? key), IEnumerable<ILocalizationLinesInfo>> localizationLinesInfoProvider)
    {
        this.localizationLinesInfoProvider = localizationLinesInfoProvider ?? throw new ArgumentNullException(nameof(localizationLinesInfoProvider));
    }

    /// <summary>Create plurality info provider</summary>
    /// <param name="lineProvider">source of lines</param>
    public LocalizedTextProvider2(
        IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> lineProvider,
        IProvider<IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>, IEnumerable<ILocalizationLinesInfo>>? lineInfoProvider = default)
    {
        this.localizationLinesInfoProvider = lineProvider.Concat(lineInfoProvider ?? Avalanche.Localization.LocalizationLinesInfoProvider.Instance);
    }

    /// <summary>Create plurality info provider</summary>
    /// <param name="lineProvider">source of lines</param>
    public LocalizedTextProvider2(
        IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> lineProvider,
        IProvider<string, ITemplateFormat> templateFormatProvider,
        Action<ILocalizationError>? errorHandler = default)
    {
        this.localizationLinesInfoProvider = lineProvider.Concat(new LocalizationLinesInfoProvider(templateFormatProvider, errorHandler: errorHandler));
    }

    /// <summary></summary>
    public override bool TryGetValue(((string? culture, IFormatProvider format), string? key) query, out ILocalizedText value)
    {
        // Query
        if (!LocalizationLinesInfoProvider.TryGetValue((query.Item1.culture, query.key), out IEnumerable<ILocalizationLinesInfo> texts)) { value = null!; return false; }
        //
        int count = texts.Count();
        // No texts
        if (count == 0) { value = null!; return false; }
        // Unexpected count
        if (count > 1) { throw new InvalidOperationException($"Expected one or zero {nameof(LocalizationLinesInfo)} for query {query}, but got {count} results."); }
        // Get line info
        ILocalizationLinesInfo lineInfo = texts.First();
        // Return
        value = new LocalizedTextFromInfo(lineInfo, query.Item1.format);
        return true;
    }
}


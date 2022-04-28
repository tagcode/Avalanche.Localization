// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System;
using Avalanche.Localization.Pluralization;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary></summary>
public abstract class LocalizationBase : LocalizationBaseRecord
{
    /// <summary></summary>
    public LocalizationBase() : base()
    {
        this.FallbackCultureProvider = Avalanche.Localization.FallbackCultureProvider.Default;
        this.FileQuery = Providers.Func<(string? culture, string? key), IEnumerable<ILocalizationFile>>(TryQueryFiles);
        this.FileQueryCached = Providers.Func<(string? culture, string? key), IEnumerable<ILocalizationFile>>(TryQueryFilesCached).ValueResultCaptured().Cached().ValueResultOpened();
        this.LineQuery = Providers.Func<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>(TryQueryLines);
        this.LineQueryCached = Providers.Func<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>(TryQueryLinesCached).ValueResultCaptured().Cached().ValueResultOpened();
        this.TemplateFormats = new TemplateFormats();
        this.PluralRuleProvider = Providers.Func<PluralRuleInfo, IPluralRule[]>(TryGetPluralRule);
        this.ErrorHandlers = new ArrayList<ILocalizationErrorHandler>();
        this.errorHandler = handleError;
        this.LocalizationLinesInfo = Providers.Func<(string culture, string key), ILocalizationLinesInfo>(TryGetLocalizationLinesInfo);
        this.LocalizationLinesInfoCached = Providers.Func<(string culture, string key), ILocalizationLinesInfo>(TryGetLocalizationLinesInfoCached).ValueResultCaptured().Cached().ValueResultOpened();
        this.LocalizationLinesInfosQuery = Providers.Func<(string? culture, string? key), IEnumerable<ILocalizationLinesInfo>>(TryQueryLocalizationLinesInfos);
        this.LocalizationLinesInfosQueryCached = Providers.Func<(string? culture, string? key), IEnumerable<ILocalizationLinesInfo>>(TryQueryLocalizationLinesInfosCached).ValueResultCaptured().Cached().ValueResultOpened();
        this.LocalizedText = Providers.Func<(string culture, string key), ILocalizedText>(TryGetLocalizedText);
        this.LocalizedTextCached = Providers.Func<(string culture, string key), ILocalizedText>(TryGetLocalizedTextCached).ValueResultCaptured().Cached().ValueResultOpened();
        this.FormatLocalizedText = Providers.Func<((string culture, IFormatProvider format), string key), ILocalizedText>(TryGetFormatLocalizedText);
        this.FormatLocalizedTextCached = Providers.Func<((string culture, IFormatProvider format), string key), ILocalizedText>(TryGetFormatLocalizedTextCached).ValueResultCaptured().Cached().ValueResultOpened();
        this.LocalizedTextsQuery = Providers.Func<(string? culture, string? key), IEnumerable<ILocalizedText>>(TryQueryLocalizedTexts);
        this.LocalizedTextsQueryCached = Providers.Func<(string? culture, string? key), IEnumerable<ILocalizedText>>(TryQueryLocalizedTextsCached).ValueResultCaptured().Cached().ValueResultOpened();
        this.LocalizableText = Providers.Func<string, ILocalizableText>(TryGetLocalizableText);
        this.LocalizableTextCached = Providers.Func<string, ILocalizableText>(TryGetLocalizableTextCached).ValueResultCaptured().Cached().ValueResultOpened();
        this.LocalizableTextsQuery = Providers.Func<string?, IEnumerable<ILocalizableText>>(TryQueryLocalizableTexts);
        this.LocalizableTextsQueryCached = Providers.Func<string?, IEnumerable<ILocalizableText>>(TryQueryLocalizableTextsCached).ValueResultCaptured().CachedNullableKey().ValueResultOpened();
        this.LocalizingText = Providers.Func<(ICultureProvider cultureProvider, string key), ILocalizingText>(TryGetLocalizingText);
        this.LocalizingTextCached = Providers.Func<(ICultureProvider cultureProvider, string key), ILocalizingText>(TryGetLocalizingTextCached).ValueResultCaptured().Cached().ValueResultOpened();

        this.LocalizableFile = Providers.Func<string, ILocalizable<ILocalizationFile>>(TryGetLocalizableFile);
        this.LocalizableFileCached = Providers.Func<string, ILocalizable<ILocalizationFile>>(TryGetLocalizableFileCached).ValueResultCaptured().Cached().ValueResultOpened();
        this.LocalizableFilesQuery = Providers.Func<string?, IEnumerable<ILocalizable<ILocalizationFile>>>(TryQueryLocalizableFiles);
        this.LocalizableFilesQueryCached = Providers.Func<string?, IEnumerable<ILocalizable<ILocalizationFile>>>(TryQueryLocalizableFilesCached).ValueResultCaptured().CachedNullableKey().ValueResultOpened();
        this.LocalizingFile = Providers.Func<(ICultureProvider cultureProvider, string key), ILocalizing<ILocalizationFile>>(TryGetLocalizingFile);
        this.LocalizingFileCached = Providers.Func<(ICultureProvider cultureProvider, string key), ILocalizing<ILocalizationFile>>(TryGetLocalizingFileCached).ValueResultCaptured().Cached().ValueResultOpened();
    }

    /// <summary></summary>
    protected Action<ILocalizationError> errorHandler;
    /// <summary>Handle error</summary>
    protected void handleError(ILocalizationError error)
    {
        foreach (ILocalizationErrorHandler errorHandler in ArrayUtilities.GetSnapshot(this.ErrorHandlers))
            errorHandler.Handle(error);
    }

    /// <summary>File querys. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected abstract bool TryQueryFiles((string? culture, string? key) query, out IEnumerable<ILocalizationFile> files);
    /// <summary>Files query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected abstract bool TryQueryFilesCached((string? culture, string? key) query, out IEnumerable<ILocalizationFile> files);

    /// <summary>Lines query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected abstract bool TryQueryLines((string? culture, string? key) query, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines);
    /// <summary>Lines query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected abstract bool TryQueryLinesCached((string? culture, string? key) query, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines);

    /// <summary>Queries and caches rules</summary>
    protected abstract bool TryGetPluralRule(PluralRuleInfo info, out IPluralRule[] rules);

    /// <summary>Localization text info provider, non-cached. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected abstract bool TryGetLocalizationLinesInfo((string culture, string key) query, out ILocalizationLinesInfo info);
    /// <summary>Localization text info provider, cached. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected abstract bool TryGetLocalizationLinesInfoCached((string culture, string key) query, out ILocalizationLinesInfo info);
    /// <summary>Querier for localization text infos, non-cached.</summary>
    protected abstract bool TryQueryLocalizationLinesInfos((string? culture, string? key) query, out IEnumerable<ILocalizationLinesInfo> infos);
    /// <summary>Querier for localization text infos, cached.</summary>
    protected abstract bool TryQueryLocalizationLinesInfosCached((string? culture, string? key) query, out IEnumerable<ILocalizationLinesInfo> infos);

    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected abstract bool TryGetLocalizedText((string culture, string key) query, out ILocalizedText text);
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected abstract bool TryGetLocalizedTextCached((string culture, string key) query, out ILocalizedText text);
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected abstract bool TryGetFormatLocalizedText(((string culture, IFormatProvider format), string key) query, out ILocalizedText text);
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected abstract bool TryGetFormatLocalizedTextCached(((string culture, IFormatProvider format), string key) query, out ILocalizedText text);
    /// <summary>Localized texts query</summary>
    protected abstract bool TryQueryLocalizedTexts((string? culture, string? key) query, out IEnumerable<ILocalizedText> texts);
    /// <summary>Localized texts query, cached</summary>
    protected abstract bool TryQueryLocalizedTextsCached((string? culture, string? key) query, out IEnumerable<ILocalizedText> texts);

    /// <summary>Localizable text provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected abstract bool TryGetLocalizableText(string key, out ILocalizableText text);
    /// <summary>Localizable text provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected abstract bool TryGetLocalizableTextCached(string key, out ILocalizableText text);
    /// <summary>Localizable texts query. Input argument is 'Key', and 'null' for all texts.</summary>
    protected abstract bool TryQueryLocalizableTexts(string? key, out IEnumerable<ILocalizableText> texts);
    /// <summary>Localizable texts query. Input argument is 'Key', and 'null' for all texts.</summary>
    protected abstract bool TryQueryLocalizableTextsCached(string? key, out IEnumerable<ILocalizableText> texts);

    /// <summary>Single localizable text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected abstract bool TryGetLocalizingText((ICultureProvider cultureProvider, string key) query, out ILocalizingText localizingText);
    /// <summary>Single localizable text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected abstract bool TryGetLocalizingTextCached((ICultureProvider cultureProvider, string key) query, out ILocalizingText localizingTextCached);

    /// <summary></summary>
    protected abstract bool TryGetLocalizableFile(string key, out ILocalizable<ILocalizationFile> localizableFile);
    /// <summary></summary>
    protected abstract bool TryGetLocalizableFileCached(string key, out ILocalizable<ILocalizationFile> localizableFile);
    /// <summary></summary>
    protected abstract bool TryQueryLocalizableFiles(string? keyQuery, out IEnumerable<ILocalizable<ILocalizationFile>> localizableFiles);
    /// <summary></summary>
    protected abstract bool TryQueryLocalizableFilesCached(string? keyQuery, out IEnumerable<ILocalizable<ILocalizationFile>> localizableFiles);
    /// <summary>Localizable file provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected abstract bool TryGetLocalizingFile((ICultureProvider cultureProvider, string key) query, out ILocalizing<ILocalizationFile> localizingFile);
    /// <summary>Localizable file  provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected abstract bool TryGetLocalizingFileCached((ICultureProvider cultureProvider, string key) query, out ILocalizing<ILocalizationFile> localizingFile);

    /// <summary>Print information</summary>
    public override string ToString() => GetType().Name;
}

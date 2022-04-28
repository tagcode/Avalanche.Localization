// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Localization.Pluralization;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

// <docs>
/// <summary>Localization settings and query providers</summary>
public interface ILocalization
{
    /// <summary>Fallback cultures provider</summary>
    IProvider<string, string[]> FallbackCultureProvider { get; set; }

    /// <summary>File configuration and providers</summary>
    ILocalizationFiles Files { get; set; }
    /// <summary>Files query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> FileQuery { get; set; }
    /// <summary>Files query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> FileQueryCached { get; set; }
    
    /// <summary>Line configuration and providers</summary>
    ILocalizationLines Lines { get; set; }
    /// <summary>Lines query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> LineQuery { get; set; }
    /// <summary>Lines query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> LineQueryCached { get; set; }

    /// <summary>Template formats</summary>
    ITemplateFormats TemplateFormats { get; set; }
    /// <summary>Queries and caches rules</summary>
    IProvider<PluralRuleInfo, IPluralRule[]> PluralRuleProvider { get; set; }
    /// <summary>Localization file parse error handlers</summary>
    IList<ILocalizationErrorHandler> ErrorHandlers { get; set; }

    /// <summary>Localization linfo info provider, non-cached. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<(string culture, string key), ILocalizationLinesInfo> LocalizationLinesInfo { get; set; }
    /// <summary>Localization linfo info provider, cached. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<(string culture, string key), ILocalizationLinesInfo> LocalizationLinesInfoCached { get; set; }
    /// <summary>Localization linfo infos query, non-cached. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<(string? culture, string? key), IEnumerable<ILocalizationLinesInfo>> LocalizationLinesInfosQuery { get; set; }
    /// <summary>Localization linfo infos query, cached. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<(string? culture, string? key), IEnumerable<ILocalizationLinesInfo>> LocalizationLinesInfosQueryCached { get; set; }
    
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<(string culture, string key), ILocalizedText> LocalizedText { get; set; }
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<(string culture, string key), ILocalizedText> LocalizedTextCached { get; set; }

    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<((string culture, IFormatProvider format), string key), ILocalizedText> FormatLocalizedText { get; set; }
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<((string culture, IFormatProvider format), string key), ILocalizedText> FormatLocalizedTextCached { get; set; }

    /// <summary>Localized texts query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<(string? culture, string? key), IEnumerable<ILocalizedText>> LocalizedTextsQuery { get; set; }
    /// <summary>Localized texts query, cached. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<(string? culture, string? key), IEnumerable<ILocalizedText>> LocalizedTextsQueryCached { get; set; }

    /// <summary>Localizable text provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<string, ILocalizableText> LocalizableText { get; set; }
    /// <summary>Localizable text provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<string, ILocalizableText> LocalizableTextCached { get; set; }

    /// <summary>Localizable texts query. Input argument is 'Key', and 'null' for all texts. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<string?, IEnumerable<ILocalizableText>> LocalizableTextsQuery { get; set; }
    /// <summary>Localizable texts query. Input argument is 'Key', and 'null' for all texts. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<string?, IEnumerable<ILocalizableText>> LocalizableTextsQueryCached { get; set; }

    /// <summary>Localizable text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<(ICultureProvider cultureProvider, string key), ILocalizingText> LocalizingText { get; set; }
    /// <summary>Localizable text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<(ICultureProvider cultureProvider, string key), ILocalizingText> LocalizingTextCached { get; set; }


    /// <summary>Localizable file provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<string, ILocalizable<ILocalizationFile>> LocalizableFile { get; set; }
    /// <summary>Localizable file provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<string, ILocalizable<ILocalizationFile>> LocalizableFileCached { get; set; }
    /// <summary>Localizable files query. Input argument is 'Key', and 'null' for all files. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<string?, IEnumerable<ILocalizable<ILocalizationFile>>> LocalizableFilesQuery { get; set; }
    /// <summary>Localizable files query. Input argument is 'Key', and 'null' for all files. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<string?, IEnumerable<ILocalizable<ILocalizationFile>>> LocalizableFilesQueryCached { get; set; }

    /// <summary>Localizable file provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<(ICultureProvider cultureProvider, string key), ILocalizing<ILocalizationFile>> LocalizingFile { get; set; }
    /// <summary>Localizable file  provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    IProvider<(ICultureProvider cultureProvider, string key), ILocalizing<ILocalizationFile>> LocalizingFileCached { get; set; }
}
// </docs>

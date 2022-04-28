// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Localization.Pluralization;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary></summary>
public class LocalizationBaseRecord : ReadOnlyAssignableClass, ILocalization, ICached
{
    /// <summary>Fallback cultures provider</summary>
    protected IProvider<string, string[]> fallbackCultureProvider = null!;
    /// <summary>File configuration and providers</summary>
    protected ILocalizationFiles files = null!;
    /// <summary>File querys. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> fileQuery = null!;
    /// <summary>Files query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> fileQueryCached = null!;
    /// <summary>Line configuration and providers</summary>
    protected ILocalizationLines lines = null!;
    /// <summary>Lines query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> lineQuery = null!;
    /// <summary>Lines query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> lineQueryCached = null!;
    /// <summary>Template formats</summary>
    protected ITemplateFormats templateFormats = null!;
    /// <summary>Queries and caches rules</summary>
    protected IProvider<PluralRuleInfo, IPluralRule[]> pluralRuleProvider = null!;
    /// <summary>Localization file parse error handlers</summary>
    protected IList<ILocalizationErrorHandler> errorHandlers = null!;
    /// <summary>Localization text info provider, non-cached. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<(string culture, string key), ILocalizationLinesInfo> localizationLinesInfo = null!;
    /// <summary>Localization text info provider, cached. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<(string culture, string key), ILocalizationLinesInfo> localizationLinesInfoCached = null!;
    /// <summary>Querier for localization text infos, non-cached.</summary>
    protected IProvider<(string? culture, string? key), IEnumerable<ILocalizationLinesInfo>> localizationLinesInfosQuery = null!;
    /// <summary>Querier for localization text infos, cached.</summary>
    protected IProvider<(string? culture, string? key), IEnumerable<ILocalizationLinesInfo>> localizationLinesInfosQueryCached = null!;
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<(string culture, string key), ILocalizedText> localizedText = null!;
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<(string culture, string key), ILocalizedText> localizedTextCached = null!;
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<((string culture, IFormatProvider format), string key), ILocalizedText> formatLocalizedText = null!;
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<((string culture, IFormatProvider format), string key), ILocalizedText> formatLocalizedTextCached = null!;
    /// <summary>Localized texts query</summary>
    protected IProvider<(string? culture, string? key), IEnumerable<ILocalizedText>> localizedTextsQuery = null!;
    /// <summary>Localized texts query, cached</summary>
    protected IProvider<(string? culture, string? key), IEnumerable<ILocalizedText>> localizedTextsQueryCached = null!;
    /// <summary>Localizable text provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<string, ILocalizableText> localizableText = null!;
    /// <summary>Localizable text provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<string, ILocalizableText> localizableTextCached = null!;
    /// <summary>Localizable texts query. Input argument is 'Key', and 'null' for all texts.</summary>
    protected IProvider<string?, IEnumerable<ILocalizableText>> localizableTextsQuery = null!;
    /// <summary>Localizable texts query. Input argument is 'Key', and 'null' for all texts.</summary>
    protected IProvider<string?, IEnumerable<ILocalizableText>> localizableTextsQueryCached = null!;
    /// <summary>Localizing text provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<(ICultureProvider cultureProvider, string key), ILocalizingText> localizingText = null!;
    /// <summary>Localizing text provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<(ICultureProvider cultureProvider, string key), ILocalizingText> localizingTextCached = null!;

    /// <summary>Localizable file provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<string, ILocalizable<ILocalizationFile>> localizableFile = null!;
    /// <summary>Localizable file provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<string, ILocalizable<ILocalizationFile>> localizableFileCached = null!;
    /// <summary>Localizable files query. Input argument is 'Key', and 'null' for all files. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<string?, IEnumerable<ILocalizable<ILocalizationFile>>> localizableFilesQuery = null!;
    /// <summary>Localizable files query. Input argument is 'Key', and 'null' for all files. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<string?, IEnumerable<ILocalizable<ILocalizationFile>>> localizableFilesQueryCached = null!;
    /// <summary>Localizing file provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<(ICultureProvider cultureProvider, string key), ILocalizing<ILocalizationFile>> localizingFile = null!;
    /// <summary>Localizing file  provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<(ICultureProvider cultureProvider, string key), ILocalizing<ILocalizationFile>> localizingFileCached = null!;

    /// <summary>Fallback cultures provider</summary>
    public virtual IProvider<string, string[]> FallbackCultureProvider { get => fallbackCultureProvider; set => this.AssertWritable().fallbackCultureProvider = value; }
    /// <summary>File configuration and providers</summary>
    public virtual ILocalizationFiles Files { get => files; set => this.AssertWritable().files = value; }
    /// <summary>File querys. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> FileQuery { get => fileQuery; set => this.AssertWritable().fileQuery = value; }
    /// <summary>Files query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> FileQueryCached { get => fileQueryCached; set => this.AssertWritable().fileQueryCached = value; }
    /// <summary>Line configuration and providers</summary>
    public virtual ILocalizationLines Lines { get => lines; set => this.AssertWritable().lines = value; }
    /// <summary>Lines query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> LineQuery { get => lineQuery; set => this.AssertWritable().lineQuery = value; }
    /// <summary>Lines query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>> LineQueryCached { get => lineQueryCached; set => this.AssertWritable().lineQueryCached = value; }
    /// <summary>Template formats</summary>
    public virtual ITemplateFormats TemplateFormats { get => templateFormats; set => this.AssertWritable().templateFormats = value; }
    /// <summary>Queries and caches rules</summary>
    public virtual IProvider<PluralRuleInfo, IPluralRule[]> PluralRuleProvider { get => pluralRuleProvider; set => this.AssertWritable().pluralRuleProvider = value; }
    /// <summary>Localization file parse error handlers</summary>
    public virtual IList<ILocalizationErrorHandler> ErrorHandlers { get => errorHandlers; set => this.AssertWritable().errorHandlers = value; }
    /// <summary>Localization text info provider, non-cached. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<(string culture, string key), ILocalizationLinesInfo> LocalizationLinesInfo { get => localizationLinesInfo; set => this.AssertWritable().localizationLinesInfo = value; }
    /// <summary>Localization text info provider, cached. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<(string culture, string key), ILocalizationLinesInfo> LocalizationLinesInfoCached { get => localizationLinesInfoCached; set => this.AssertWritable().localizationLinesInfoCached = value; }
    /// <summary>Querier for localization text infos, non-cached.</summary>
    public virtual IProvider<(string? culture, string? key), IEnumerable<ILocalizationLinesInfo>> LocalizationLinesInfosQuery { get => localizationLinesInfosQuery; set => this.AssertWritable().localizationLinesInfosQuery = value; }
    /// <summary>Querier for localization text infos, cached.</summary>
    public virtual IProvider<(string? culture, string? key), IEnumerable<ILocalizationLinesInfo>> LocalizationLinesInfosQueryCached { get => localizationLinesInfosQueryCached; set => this.AssertWritable().localizationLinesInfosQueryCached = value; }
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<(string culture, string key), ILocalizedText> LocalizedText { get => localizedText; set => this.AssertWritable().localizedText = value; }
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<(string culture, string key), ILocalizedText> LocalizedTextCached { get => localizedTextCached; set => this.AssertWritable().localizedTextCached = value; }
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<((string culture, IFormatProvider format), string key), ILocalizedText> FormatLocalizedText { get => formatLocalizedText; set => this.AssertWritable().formatLocalizedText = value; }
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<((string culture, IFormatProvider format), string key), ILocalizedText> FormatLocalizedTextCached { get => formatLocalizedTextCached; set => this.AssertWritable().formatLocalizedTextCached = value; }
    /// <summary>Localized texts query</summary>
    public virtual IProvider<(string? culture, string? key), IEnumerable<ILocalizedText>> LocalizedTextsQuery { get => localizedTextsQuery; set => this.AssertWritable().localizedTextsQuery = value; }
    /// <summary>Localized texts query, cached</summary>
    public virtual IProvider<(string? culture, string? key), IEnumerable<ILocalizedText>> LocalizedTextsQueryCached { get => localizedTextsQueryCached; set => this.AssertWritable().localizedTextsQueryCached = value; }
    /// <summary>Localizable text provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<string, ILocalizableText> LocalizableText { get => localizableText; set => this.AssertWritable().localizableText = value; }
    /// <summary>Localizable text provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<string, ILocalizableText> LocalizableTextCached { get => localizableTextCached; set => this.AssertWritable().localizableTextCached = value; }
    /// <summary>Localizable texts query. Input argument is 'Key', and 'null' for all texts.</summary>
    public virtual IProvider<string?, IEnumerable<ILocalizableText>> LocalizableTextsQuery { get => localizableTextsQuery; set => this.AssertWritable().localizableTextsQuery = value; }
    /// <summary>Localizable texts query. Input argument is 'Key', and 'null' for all texts.</summary>
    public virtual IProvider<string?, IEnumerable<ILocalizableText>> LocalizableTextsQueryCached { get => localizableTextsQueryCached; set => this.AssertWritable().localizableTextsQueryCached = value; }
    /// <summary>Localizable text provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<(ICultureProvider cultureProvider, string key), ILocalizingText> LocalizingText { get => localizingText; set => this.AssertWritable().localizingText = value; }
    /// <summary>Localizable text provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<(ICultureProvider cultureProvider, string key), ILocalizingText> LocalizingTextCached { get => localizingTextCached; set => this.AssertWritable().localizingTextCached = value; }

    /// <summary>Localizable file provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<string, ILocalizable<ILocalizationFile>> LocalizableFile { get => localizableFile; set => this.AssertWritable().localizableFile = value; }
    /// <summary>Localizable file provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<string, ILocalizable<ILocalizationFile>> LocalizableFileCached { get => localizableFileCached; set => this.AssertWritable().localizableFileCached = value; }
    /// <summary>Localizable files query. Input argument is 'Key', and 'null' for all files. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<string?, IEnumerable<ILocalizable<ILocalizationFile>>> LocalizableFilesQuery { get => localizableFilesQuery; set => this.AssertWritable().localizableFilesQuery = value; }
    /// <summary>Localizable files query. Input argument is 'Key', and 'null' for all files. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<string?, IEnumerable<ILocalizable<ILocalizationFile>>> LocalizableFilesQueryCached { get => localizableFilesQueryCached; set => this.AssertWritable().localizableFilesQueryCached = value; }
    /// <summary>Localizable file provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<(ICultureProvider cultureProvider, string key), ILocalizing<ILocalizationFile>> LocalizingFile { get => localizingFile; set => this.AssertWritable().localizingFile = value; }
    /// <summary>Localizable file  provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    public virtual IProvider<(ICultureProvider cultureProvider, string key), ILocalizing<ILocalizationFile>> LocalizingFileCached { get => localizingFileCached; set => this.AssertWritable().localizingFileCached = value; }

    /// <summary></summary>
    bool ICached.IsCached { get => true; set => throw new InvalidOperationException(); }

    /// <summary></summary>
    public virtual void InvalidateCache(bool deep)
    {
        if (FallbackCultureProvider is ICached c0) c0.InvalidateCache(deep);
        if (Files is ICached c1) c1.InvalidateCache(deep);
        if (FileQuery is ICached c2) c2.InvalidateCache(deep);
        if (FileQueryCached is ICached c3) c3.InvalidateCache(deep);
        if (Lines is ICached c4) c4.InvalidateCache(deep);
        if (LineQuery is ICached c5) c5.InvalidateCache(deep);
        if (LineQueryCached is ICached c6) c6.InvalidateCache(deep);
        if (TemplateFormats is ICached c19) c19.InvalidateCache(deep);
        if (PluralRuleProvider is ICached c20) c20.InvalidateCache(deep);
        if (ErrorHandlers is ICached c21) c21.InvalidateCache(deep);
        if (LocalizationLinesInfo is ICached c7) c7.InvalidateCache(deep);
        if (LocalizationLinesInfoCached is ICached c8) c8.InvalidateCache(deep);
        if (LocalizationLinesInfosQuery is ICached c9) c9.InvalidateCache(deep);
        if (LocalizationLinesInfosQueryCached is ICached c10) c10.InvalidateCache(deep);
        if (LocalizedText is ICached c11) c11.InvalidateCache(deep);
        if (LocalizedTextCached is ICached c12) c12.InvalidateCache(deep);
        if (FormatLocalizedText is ICached c11_) c11_.InvalidateCache(deep);
        if (FormatLocalizedTextCached is ICached c12_) c12_.InvalidateCache(deep);
        if (LocalizedTextsQuery is ICached c13) c13.InvalidateCache(deep);
        if (LocalizedTextsQueryCached is ICached c14) c14.InvalidateCache(deep);
        if (LocalizableText is ICached c15) c15.InvalidateCache(deep);
        if (LocalizableTextCached is ICached c16) c16.InvalidateCache(deep);
        if (LocalizableTextsQuery is ICached c17) c17.InvalidateCache(deep);
        if (LocalizableTextsQueryCached is ICached c18) c18.InvalidateCache(deep);
        if (LocalizingText is ICached c22) c22.InvalidateCache(deep);
        if (LocalizingTextCached is ICached c23) c23.InvalidateCache(deep);

        if (LocalizableFile is ICached c24) c24.InvalidateCache(deep);
        if (LocalizableFileCached is ICached c25) c25.InvalidateCache(deep);
        if (LocalizableFilesQuery is ICached c26) c26.InvalidateCache(deep);
        if (LocalizableFilesQueryCached is ICached c27) c27.InvalidateCache(deep);
        if (LocalizingFile is ICached c28) c28.InvalidateCache(deep);
        if (LocalizingFileCached is ICached c29) c29.InvalidateCache(deep);
    }
}

// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System;
using System.Collections.Generic;
using Avalanche.Localization.Internal;
using Avalanche.Localization.Pluralization;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary></summary>
public class Localization : LocalizationBase
{
    /// <summary>Singleton.</summary>
    static Lazy<Localization> instance = new(() =>
    {
        Localization localization = Localization.CreateDefault();
        return localization;
    });

    /// <summary>Create localization context with default settings.</summary>
    public static Localization CreateDefault()
    {
        // <default>
        // Create localization
        Localization localization = new Localization()
            .AddFileSystem(LocalizationFileSystem.ApplicationRoot)
            .AddFileFormats(LocalizationFileFormatYaml.Instance, LocalizationFileFormatXml.Instance, LocalizationFileFormatJson.Instance)            
            .AddFilePatterns(LocalizationFilePatterns.ResourcesFolder) // "Resources/{Key}", "Resources/{Culture}/{Key}"
            .AddFileSystemWithPattern(LocalizationFileSystemEmbedded.AppDomain, LocalizationFilePatterns.ResourcesEmbedded) // "*/*.Resources.{Key}", "*/*.Resources.{Culture}.{Key}", "*/{Key}", "*/{Key}.{Culture}"
            .AddResourceManagerProvider();
        // </default>
        // Return
        return localization;
    }

    /// <summary></summary>
    protected IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> fileQueryField;
    /// <summary></summary>
    protected IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>> fileQueryCachedField;

    /// <summary></summary>
    protected IProvider<(string culture, string key), ILocalizationLinesInfo> localizationLinesInfoField;
    /// <summary></summary>
    protected IProvider<(string culture, string key), ILocalizationLinesInfo> localizationLinesInfoCachedField;
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<(string culture, string key), ILocalizedText> localizedTextField;
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<(string culture, string key), ILocalizedText> localizedTextCachedField;
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<((string culture, IFormatProvider format), string key), ILocalizedText> formatLocalizedTextField;
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<((string culture, IFormatProvider format), string key), ILocalizedText> formatLocalizedTextCachedField;
    /// <summary>Localized texts query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<(string? culture, string? key), IEnumerable<ILocalizedText>> localizedTextsQueryField;
    /// <summary>Localized texts query, cached. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected IProvider<(string? culture, string? key), IEnumerable<ILocalizedText>> localizedTextsQueryCachedField;

    /// <summary></summary>
    protected LocalizationLinesInfoProvider localizationLinesInfoProvider;

    /// <summary></summary>
    public static Localization Default = instance.Value;

    /// <summary></summary>
    public Localization() : this(new LocalizationLines(), new LocalizationFiles(), Avalanche.Localization.FallbackCultureProvider.Default, Avalanche.Template.TemplateFormats.All.AllFormats, PluralRulesProvider.Cached)
    { 
    }

    /// <summary></summary>
    public Localization(ILocalizationLines localizationLines, ILocalizationFiles localizationFiles, IProvider<string, string[]> fallbackCultureProvider, IEnumerable<ITemplateFormat> templateFormats, IProvider<PluralRuleInfo, IPluralRule[]> pluralRuleProvider) : base()
    {
        this.FallbackCultureProvider = fallbackCultureProvider ?? throw new ArgumentNullException(nameof(fallbackCultureProvider));
        this.Lines = localizationLines ?? throw new ArgumentNullException(nameof(localizationLines));
        this.Files = localizationFiles ?? throw new ArgumentNullException(nameof(localizationFiles));
        this.TemplateFormats.AddTemplateFormats(templateFormats.ToArray());
        this.PluralRuleProvider = pluralRuleProvider ?? throw new ArgumentNullException(nameof(pluralRuleProvider));

        this.fileQueryField = Providers.Indirect(() => this.FileQuery);
        this.fileQueryCachedField = Providers.Indirect(() => this.FileQueryCached);

        // Non-fallback-queries
        //this.Lines.FileProviders.Add(this.Files.Query);
        //this.Lines.FileProvidersCached.Add(this.Files.QueryCached);
        // With-fallback-queries
        this.Lines.FileProviders.Add(this.FileQuery);
        this.Lines.FileProvidersCached.Add(this.FileQueryCached);
        //this.Lines.FileProviders.Add(fileQueryField);
        //this.Lines.FileProvidersCached.Add(fileQueryCachedField);

        this.localizationLinesInfoField = Providers.Indirect(() => this.LocalizationLinesInfo);
        this.localizationLinesInfoCachedField = Providers.Indirect(() => this.LocalizationLinesInfoCached);
        this.localizedTextField = Providers.Indirect(() => this.LocalizedText);
        this.localizedTextCachedField = Providers.Indirect(() => this.LocalizedTextCached);
        this.formatLocalizedTextField = Providers.Indirect(() => this.FormatLocalizedText);
        this.formatLocalizedTextCachedField = Providers.Indirect(() => this.FormatLocalizedTextCached);
        this.localizedTextsQueryField = Providers.Indirect(() => this.LocalizedTextsQuery);
        this.localizedTextsQueryCachedField = Providers.Indirect(() => this.LocalizedTextsQueryCached);

        this.localizationLinesInfoProvider = new LocalizationLinesInfoProvider(
            templateFormatProvider: Providers.Indirect<string, ITemplateFormat>(() => this.TemplateFormats.ByName),
            pluralsInfoProvider: PluralsInfo.Create,
            pluralRuleProvider: Providers.Indirect<PluralRuleInfo, IPluralRule[]>(() => this.PluralRuleProvider),
            errorHandler: this.errorHandler
            );
    }

    /// <summary>Files query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected override bool TryQueryFiles((string? culture, string? key) query, out IEnumerable<ILocalizationFile> files)
    {
        // Query from files
        var _filesQuerier = this.Files?.Query;
        // No querier
        if (_filesQuerier == null) { files = null!; return false; }
        // Get fallback provider
        var _fallbackCultureProvider = this.FallbackCultureProvider;
        // Query fallback cultures
        if (_fallbackCultureProvider == null || query.culture == null || !_fallbackCultureProvider.TryGetValue(query.culture, out string[] cultures))
        // Add exact culture
        {
            // Try query exact
            return _filesQuerier.TryGetValue(query, out files);
        }
        // Concat all fallback cultures
        else
        {
            //
            StructList8<ILocalizationFile> result = new();
            // Try with each cultures
            foreach (string culture in cultures)
            {
                // Get files
                if (!_filesQuerier.TryGetValue((culture, query.key), out IEnumerable<ILocalizationFile> _files)) continue;
                //
                result.AddRange(_files);
            }
            // Return
            files = result.ToArray();
            return result.Count > 0;
        }
    }

    /// <summary>Files query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected override bool TryQueryFilesCached((string? culture, string? key) query, out IEnumerable<ILocalizationFile> files)
    {
        // Query from files
        var _filesQuerier = this.Files?.QueryCached;
        // No querier
        if (_filesQuerier == null) { files = null!; return false; }
        // Get fallback provider
        var _fallbackCultureProvider = this.FallbackCultureProvider;
        // Query fallback cultures
        if (_fallbackCultureProvider == null || query.culture == null || !_fallbackCultureProvider.TryGetValue(query.culture, out string[] cultures))
        // Add exact culture
        {
            // Try query exact
            return _filesQuerier.TryGetValue(query, out files);
        }
        // Concat all fallback cultures
        else
        {
            //
            StructList8<ILocalizationFile> result = new();
            // Try with each cultures
            foreach (string culture in cultures)
            {
                // Get files
                if (!_filesQuerier.TryGetValue((culture, query.key), out IEnumerable<ILocalizationFile> _files)) continue;
                //
                result.AddRange(_files);
            }
            // Return
            files = result.ToArray();
            return result.Count > 0;
        }
    }

    /// <summary>Lines query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected override bool TryQueryLines((string? culture, string? key) query, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines)
    {
        // Query from lines
        var _linesQuerier = this.Lines?.Query;
        // No querier
        if (_linesQuerier == null) { lines = null!; return false; }
        // Try query exact
        if (_linesQuerier.TryGetValue(query, out lines)) return true;
        // Get fallback provider
        var _fallbackCultureProvider = this.FallbackCultureProvider;
        // Query fallback cultures
        if (_fallbackCultureProvider == null || query.culture == null || !_fallbackCultureProvider.TryGetValue(query.culture, out string[] cultures)) { lines = default!; return false; }
        // Try with each cultures
        foreach (string culture in cultures)
        {
            // Found lines
            if (_linesQuerier.TryGetValue((culture, query.key), out lines)) return true;
        }
        // No lines
        lines = default!;
        return false;
    }

    /// <summary>Lines query. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected override bool TryQueryLinesCached((string? culture, string? key) query, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines)
    {
        // Query from lines
        var _linesQuerier = this.Lines?.QueryCached;
        // No querier
        if (_linesQuerier == null) { lines = null!; return false; }
        // Try query exact
        if (_linesQuerier.TryGetValue(query, out lines)) return true;
        // Get fallback provider
        var _fallbackCultureProvider = this.FallbackCultureProvider;
        // Query fallback cultures
        if (_fallbackCultureProvider == null || query.culture == null || !_fallbackCultureProvider.TryGetValue(query.culture, out string[] cultures)) { lines = default!; return false; }
        // Try with each cultures
        foreach (string culture in cultures)
        {
            // Found lines
            if (_linesQuerier.TryGetValue((culture, query.key), out lines)) return true;
        }
        // No lines
        lines = default!;
        return false;
    }

    /// <summary>Queries and caches rules</summary>
    protected override bool TryGetPluralRule(PluralRuleInfo info, out IPluralRule[] rules) { rules = null!; return false; }

    /// <summary>Localization text info provider, non-cached. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected override bool TryGetLocalizationLinesInfo((string culture, string key) query, out ILocalizationLinesInfo info)
    {
        // No query for single text
        if (query.culture == null || query.key == null) { info = null!; return false; }
        // Query lines
        if (!this.LineQuery.TryGetValue(query, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines)) { info = null!; return false; }
        // Convert to localization text
        if (!localizationLinesInfoProvider.TryGetValue(lines, out IEnumerable<ILocalizationLinesInfo> infos)) { info = null!; return false; }
        // Get info
        info = infos.FirstOrDefault()!;
        return info != null;
    }

    /// <summary>Localization text info provider, cached. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected override bool TryGetLocalizationLinesInfoCached((string culture, string key) query, out ILocalizationLinesInfo info)
    {
        // No query for single text
        if (query.culture == null || query.key == null) { info = null!; return false; }
        // Query lines
        if (!this.LineQueryCached.TryGetValue(query, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines)) { info = null!; return false; }
        // Convert to localization text
        if (!localizationLinesInfoProvider.TryGetValue(lines, out IEnumerable<ILocalizationLinesInfo> infos)) { info = null!; return false; }
        // Get info
        info = infos.FirstOrDefault()!;
        return info != null;
    }

    /// <summary>Localization text infos query, non-cached.</summary>
    protected override bool TryQueryLocalizationLinesInfos((string? culture, string? key) query, out IEnumerable<ILocalizationLinesInfo> infos)
    {
        // Query lines
        if (!this.LineQuery.TryGetValue(query, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines)) { infos = null!; return false; }
        // Convert to localization text
        if (!localizationLinesInfoProvider.TryGetValue(lines, out infos)) { infos = null!; return false; }
        // 
        return true;
    }

    /// <summary>Localization text infos query, cached.</summary>
    protected override bool TryQueryLocalizationLinesInfosCached((string? culture, string? key) query, out IEnumerable<ILocalizationLinesInfo> infos)
    {
        // Query lines
        if (!this.LineQueryCached.TryGetValue(query, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines)) { infos = null!; return false; }
        // Convert to localization text
        if (!localizationLinesInfoProvider.TryGetValue(lines, out infos)) { infos = null!; return false; }
        // 
        return true;
    }

    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected override bool TryGetLocalizedText((string culture, string key) query, out ILocalizedText text)
    {
        // No query for single text
        if (query.culture == null || query.key == null) { text = null!; return false; }
        // Query lines
        if (!this.LineQuery.TryGetValue(query, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines)) { text = null!; return false; }
        // Convert to localization text
        if (!localizationLinesInfoProvider.TryGetValue(lines, out IEnumerable<ILocalizationLinesInfo> infos)) { text = null!; return false; }
        // Get fallback provider
        var _fallbackCultureProvider = this.FallbackCultureProvider;
        // Match in fallback culture order
        if (_fallbackCultureProvider != null && query.culture! != null && _fallbackCultureProvider.TryGetValue(query.culture, out string[] cultures))
        {
            foreach (string culture in cultures)
                foreach (ILocalizationLinesInfo _info in infos)
                    if (_info.Culture == culture) { text = new LocalizedTextFromInfo(_info); return true; }
        }
        // Get info
        ILocalizationLinesInfo? info = infos.FirstOrDefault()!;
        if (info == null) { text = null!; return false; }
        text = new LocalizedTextFromInfo(info);
        return true;
    }

    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected override bool TryGetLocalizedTextCached((string culture, string key) query, out ILocalizedText text)
    {
        // No query for single text
        if (query.culture == null || query.key == null) { text = null!; return false; }
        // Query lines
        if (!this.LineQueryCached.TryGetValue(query, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines)) { text = null!; return false; }
        // Convert to localization text
        if (!localizationLinesInfoProvider.TryGetValue(lines, out IEnumerable<ILocalizationLinesInfo> infos)) { text = null!; return false; }
        // Get fallback provider
        var _fallbackCultureProvider = this.FallbackCultureProvider;
        // Match in fallback culture order
        if (_fallbackCultureProvider != null && query.culture! != null && _fallbackCultureProvider.TryGetValue(query.culture, out string[] cultures))
        {
            // Return culture matching info
            foreach (string culture in cultures)
                foreach (ILocalizationLinesInfo _info in infos)
                    if (_info.Culture == culture) { text = new LocalizedTextFromInfo(_info); return true; }
        }
        // Get info
        ILocalizationLinesInfo? info = infos.FirstOrDefault()!;
        if (info == null) { text = null!; return false; }
        text = new LocalizedTextFromInfo(info);
        return true;
    }

    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected override bool TryGetFormatLocalizedText(((string culture, IFormatProvider format), string key) query, out ILocalizedText text)
    {
        // No query for single text
        if (query.Item1.culture == null || query.key == null) { text = null!; return false; }
        // Query lines
        if (!this.LineQuery.TryGetValue((query.Item1.culture, query.key), out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines)) { text = null!; return false; }
        // Convert to localization text
        if (!localizationLinesInfoProvider.TryGetValue(lines, out IEnumerable<ILocalizationLinesInfo> infos)) { text = null!; return false; }
        // Get fallback provider
        var _fallbackCultureProvider = this.FallbackCultureProvider;
        // Match in fallback culture order
        if (_fallbackCultureProvider != null && query.Item1.culture! != null && _fallbackCultureProvider.TryGetValue(query.Item1.culture, out string[] cultures))
        {
            foreach (string culture in cultures)
                foreach (ILocalizationLinesInfo _info in infos)
                    if (_info.Culture == culture) { text = new LocalizedTextFromInfo(_info, query.Item1.format); return true; }
        }
        // Get info
        ILocalizationLinesInfo? info = infos.FirstOrDefault()!;
        if (info == null) { text = null!; return false; }
        text = new LocalizedTextFromInfo(info, query.Item1.format);
        return true;
    }
    /// <summary>Localized text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected override bool TryGetFormatLocalizedTextCached(((string culture, IFormatProvider format), string key) query, out ILocalizedText text)
    {
        // No query for single text
        if (query.Item1.culture == null || query.key == null) { text = null!; return false; }
        // Query lines
        if (!this.LineQueryCached.TryGetValue((query.Item1.culture, query.key), out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines)) { text = null!; return false; }
        // Convert to localization text
        if (!localizationLinesInfoProvider.TryGetValue(lines, out IEnumerable<ILocalizationLinesInfo> infos)) { text = null!; return false; }
        // Get fallback provider
        var _fallbackCultureProvider = this.FallbackCultureProvider;
        // Match in fallback culture order
        if (_fallbackCultureProvider != null && query.Item1.culture! != null && _fallbackCultureProvider.TryGetValue(query.Item1.culture, out string[] cultures))
        {
            // Return culture matching info
            foreach (string culture in cultures)
                foreach (ILocalizationLinesInfo _info in infos)
                    if (_info.Culture == culture) { text = new LocalizedTextFromInfo(_info, query.Item1.format); return true; }
        }
        // Get info
        ILocalizationLinesInfo? info = infos.FirstOrDefault()!;
        if (info == null) { text = null!; return false; }
        text = new LocalizedTextFromInfo(info, query.Item1.format);
        return true;
    }

    /// <summary>Localized texts query</summary>
    protected override bool TryQueryLocalizedTexts((string? culture, string? key) query, out IEnumerable<ILocalizedText> texts)
    {
        // Query lines
        if (!this.LineQuery.TryGetValue(query, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines)) { texts = null!; return false; }
        // Convert to localization text
        if (!localizationLinesInfoProvider.TryGetValue(lines, out IEnumerable<ILocalizationLinesInfo> infos)) { texts = null!; return false; }
        // Get info
        texts = infos.Select(i => new LocalizedTextFromInfo(i)).ToArray();
        return true;
    }

    /// <summary>Localized texts query, cached</summary>
    protected override bool TryQueryLocalizedTextsCached((string? culture, string? key) query, out IEnumerable<ILocalizedText> texts)
    {
        // Query lines
        if (!this.LineQueryCached.TryGetValue(query, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines)) { texts = null!; return false; }
        // Convert to localization text
        if (!localizationLinesInfoProvider.TryGetValue(lines, out IEnumerable<ILocalizationLinesInfo> infos)) { texts = null!; return false; }
        // Get info
        texts = infos.Select(i => new LocalizedTextFromInfo(i)).ToArray();
        return true;
    }


    /// <summary>Localizable text provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected override bool TryGetLocalizableText(string key, out ILocalizableText text)
    {
        // Query
        if (!this.LocalizableTextsQuery.TryGetValue(key, out IEnumerable<ILocalizableText> texts)) { text = null!; return false; }
        // Get first text
        text = texts.FirstOrDefault()!;
        return text != null;
    }

    /// <summary>Localizable text provider. Input argument is 'Key'. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected override bool TryGetLocalizableTextCached(string key, out ILocalizableText text)
    {
        // Query
        if (!this.LocalizableTextsQueryCached.TryGetValue(key, out IEnumerable<ILocalizableText> texts)) { text = null!; return false; }
        // Get first text
        text = texts.FirstOrDefault()!;
        return text != null;
    }

    /// <summary>Localizable texts query. Input argument is 'Key', and 'null' for all texts.</summary>
    protected override bool TryQueryLocalizableTexts(string? key, out IEnumerable<ILocalizableText> texts)
    {
        // Query to get keys
        if (!this.LocalizationLinesInfosQuery.TryGetValue((null, key), out IEnumerable<ILocalizationLinesInfo> infos)) { texts = null!; return false; }
        // Get keys
        HashSet<string> keys = new();
        foreach (ILocalizationLinesInfo info in infos) if (info.Key != null) keys.Add(info.Key);
        // Create texts
        texts = keys.Select(key => new LocalizableText(key, formatLocalizedTextField/*localizedTextField*/)).ToArray();
        return true;
    }

    /// <summary>Localizable texts query. Input argument is 'Key', and 'null' for all texts.</summary>
    protected override bool TryQueryLocalizableTextsCached(string? key, out IEnumerable<ILocalizableText> texts)
    {
        // Query to get keys
        if (!this.LocalizationLinesInfosQueryCached.TryGetValue((null, key), out IEnumerable<ILocalizationLinesInfo> infos)) { texts = null!; return false; }
        // Get keys
        HashSet<string> keys = new();
        foreach (ILocalizationLinesInfo info in infos) if (info.Key != null) keys.Add(info.Key);
        // Create texts
        texts = keys.Select(key => new LocalizableText(key, formatLocalizedTextCachedField/*localizedTextCachedField*/)).ToArray();
        return true;
    }

    /// <summary>Localizable text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected override bool TryGetLocalizingText((ICultureProvider cultureProvider, string key) query, out ILocalizingText localizingText)
    {
        // Get provider
        var _provider = formatLocalizedText/*formatLocalizedTextField*/;
        // Return
        if (_provider != null) { localizingText = new LocalizingText(query.key, _provider, query.cultureProvider); return true; }
        // Get provider2
        var _provider2 = localizedText/*localizedTextField*/;
        // Return
        if (_provider2 != null) { localizingText = new LocalizingText(query.key, _provider2, query.cultureProvider); return true; }
        // No result
        localizingText = null!;
        return false;
    }

    /// <summary>Localizable text provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected override bool TryGetLocalizingTextCached((ICultureProvider cultureProvider, string key) query, out ILocalizingText localizingText)
    {
        // Get provider
        var _provider = formatLocalizedTextCached/*formatLocalizedTextCachedField*/;
        // Return
        if (_provider != null) { localizingText = new LocalizingText(query.key, _provider, query.cultureProvider); return true; }
        // Get provider2
        var _provider2 = localizedTextCached/*localizedTextCachedField*/;
        // Return
        if (_provider2 != null) { localizingText = new LocalizingText(query.key, _provider2, query.cultureProvider); return true; }
        // No result
        localizingText = null!;
        return false;
    }

    /// <summary></summary>
    protected override bool TryGetLocalizableFile(string key, out ILocalizable<ILocalizationFile> localizableFile)
    {
        // Query
        if (!this.LocalizableFilesQuery.TryGetValue(key, out IEnumerable<ILocalizable<ILocalizationFile>> localizableFiles)) { localizableFile = null!; return false; }
        // Get first text
        localizableFile = localizableFiles.FirstOrDefault()!;
        return localizableFile != null;
    }
    /// <summary></summary>
    protected override bool TryGetLocalizableFileCached(string key, out ILocalizable<ILocalizationFile> localizableFile)
    {
        // Query
        if (!this.LocalizableFilesQueryCached.TryGetValue(key, out IEnumerable<ILocalizable<ILocalizationFile>> localizableFiles)) { localizableFile = null!; return false; }
        // Get first text
        localizableFile = localizableFiles.FirstOrDefault()!;
        return localizableFile != null;
    }
    /// <summary></summary>
    protected override bool TryQueryLocalizableFiles(string? keyQuery, out IEnumerable<ILocalizable<ILocalizationFile>> localizableFiles)
    {
        // Open-ended query
        if (keyQuery == null)
        {
            // Query files
            if (!this.FileQuery.TryGetValue((null, null), out IEnumerable<ILocalizationFile> files)) { localizableFiles = null!; return false; }
            //
            StructList8<ILocalizable<ILocalizationFile>> _files = new();
            // Create 
            foreach(string key in files.Select(f => f.Key).Distinct())
            {
                _files.Add(new LocalizableFile(key, this.fileQueryField));
            }
            // Return
            localizableFiles = _files.ToArray();
            return _files.Count>0;
        }
        // Key specific query
        else 
        {
            // Query files
            if (!this.FileQuery.TryGetValue((null, keyQuery), out IEnumerable<ILocalizationFile> files)) { localizableFiles = null!; return false; }
            // Get file count
            int count = files.Count();
            // No files
            if (count == 0) { localizableFiles = null!; return false; }
            // 
            var localizableFile = new LocalizableFile(keyQuery, this.fileQueryField);
            //
            localizableFiles = new ILocalizable<ILocalizationFile>[] { localizableFile };
            return true;
        }
    }
    /// <summary></summary>
    protected override bool TryQueryLocalizableFilesCached(string? keyQuery, out IEnumerable<ILocalizable<ILocalizationFile>> localizableFiles)
    {
        // Open-ended query
        if (keyQuery == null)
        {
            // Query files
            if (!this.FileQueryCached.TryGetValue((null, null), out IEnumerable<ILocalizationFile> files)) { localizableFiles = null!; return false; }
            //
            StructList8<ILocalizable<ILocalizationFile>> _files = new();
            // Create 
            foreach (string key in files.Select(f => f.Key).Distinct())
            {
                _files.Add(new LocalizableFile(key, this.fileQueryCachedField));
            }
            // Return
            localizableFiles = _files.ToArray();
            return _files.Count > 0;
        }
        // Key specific query
        else
        {
            // Query files
            if (!this.FileQueryCached.TryGetValue((null, keyQuery), out IEnumerable<ILocalizationFile> files)) { localizableFiles = null!; return false; }
            // Get file count
            int count = files.Count();
            // No files
            if (count == 0) { localizableFiles = null!; return false; }
            // 
            var localizableFile = new LocalizableFile(keyQuery, this.fileQueryCachedField);
            //
            localizableFiles = new ILocalizable<ILocalizationFile>[] { localizableFile };
            return true;
        }
    }
    /// <summary>Localizable file  provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected override bool TryGetLocalizingFile((ICultureProvider cultureProvider, string key) query, out ILocalizing<ILocalizationFile> localizingFile)
    {
        // Return
        localizingFile = new LocalizingFile(query.key, query.cultureProvider, this.fileQueryField); 
        return true;
    }
    /// <summary>Localizable file  provider. Uses fallback cultures from <see cref="FallbackCultureProvider"/>.</summary>
    protected override bool TryGetLocalizingFileCached((ICultureProvider cultureProvider, string key) query, out ILocalizing<ILocalizationFile> localizingFile)
    {
        // Return
        localizingFile = new LocalizingFile(query.key, query.cultureProvider, this.fileQueryCachedField); 
        return true;
    }

}

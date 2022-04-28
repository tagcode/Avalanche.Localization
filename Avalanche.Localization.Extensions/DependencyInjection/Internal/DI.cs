// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Avalanche.Localization.Pluralization;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;
using Microsoft.Extensions.Logging;

/// <summary>Dependency injection versions of localization classes</summary>
public static class DI
{
    /// <summary></summary>
    public class Localization : Avalanche.Localization.Localization
    {
        /// <summary></summary>
        public Localization(ILocalizationLines localizationLines, ILocalizationFiles localizationFiles, IEnumerable<ITemplateFormat> templateFormats, IProvider<PluralRuleInfo, IPluralRule[]> pluralRuleProvider, IEnumerable<ILocalizationErrorHandler> localizationErrorHandlers) :
            this(localizationLines, localizationFiles, Avalanche.Localization.FallbackCultureProvider.Default, templateFormats, pluralRuleProvider, localizationErrorHandlers)
        { }

        /// <summary></summary>
        public Localization(ILocalizationLines localizationLines, ILocalizationFiles localizationFiles, IProvider<string, string[]> fallbackCultureProvider, IEnumerable<ITemplateFormat> templateFormats, IProvider<PluralRuleInfo, IPluralRule[]> pluralRuleProvider, IEnumerable<ILocalizationErrorHandler> localizationErrorHandlers) :
            base(localizationLines, localizationFiles, fallbackCultureProvider, templateFormats, pluralRuleProvider)
        {
            if (localizationErrorHandlers != null)
                foreach (var handler in localizationErrorHandlers)
                    this.ErrorHandlers.AddIfNew(handler);
        }
    }

    /// <summary></summary>
    public class LocalizationLines : Avalanche.Localization.LocalizationLines
    {
        /// <summary></summary>
        public LocalizationLines(IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines, IEnumerable<IProvider<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>> lineProviders)
        {
            this.Lines.AddRange(lines);
            foreach (var lineProvider in lineProviders)
            {
                if (this.LineProviders.Contains(lineProvider)) continue;
                this.LineProviders.Add(lineProvider);
                this.LineProvidersCached.Add(lineProvider.ValueResultCaptured().Cached().ValueResultOpened());
            }
        }
    }

    /// <summary></summary>
    public class LocalizationFiles : Avalanche.Localization.LocalizationFiles
    {
        /// <summary></summary>
        public LocalizationFiles(IEnumerable<ILocalizationFileFormat> fileFormats, IEnumerable<ILocalizationFileSystem> fileSystems, IEnumerable<ILocalizationFilePatterns> filePatterns, IEnumerable<IProvider<(string? culture, string? key), IEnumerable<ILocalizationFile>>> fileProviders) : base()
        {
            foreach (var fileFormat in fileFormats) this.FileFormats.AddIfNew(fileFormat);
            foreach (var fileSystem in fileSystems) this.FileSystems.AddIfNew(fileSystem);
            foreach (var _patterns in filePatterns)
                foreach (var _pattern in _patterns.Patterns)
                    this.FilePatterns.AddIfNew(_pattern);
            foreach (var fileProvider in fileProviders)
            {
                if (this.FileProviders.Contains(fileProvider)) continue;
                this.FileProviders.Add(fileProvider);
                this.FileProvidersCached.Add(fileProvider.ValueResultCaptured().Cached().ValueResultOpened());
            }
        }
    }

    /// <summary>Text localizer</summary>
    public class TextLocalizer : Avalanche.Localization.TextLocalizer
    {
        /// <summary>Optional logger</summary>
        protected ILogger? logger;
        /// <summary>Create text localizer</summary>
        public TextLocalizer(ILocalization localization) : base(localization, Avalanche.Localization.CultureProvider.CurrentThread.Instance, null) { this.SetReadOnly(); }
        /// <summary>Create text localizer</summary>
        public TextLocalizer(ILocalization localization, ICultureProvider cultureProvider) : base(localization, cultureProvider, null) { this.SetReadOnly(); }
        /// <summary>Create text localizer</summary>
        public TextLocalizer(ILocalization localization, ILogger<Avalanche.Localization.TextLocalizer> logger) : base(localization, Avalanche.Localization.CultureProvider.CurrentThread.Instance, null) { this.logger = logger; this.SetReadOnly(); }
        /// <summary>Create text localizer</summary>
        public TextLocalizer(ILocalization localization, ICultureProvider cultureProvider, ILogger<Avalanche.Localization.TextLocalizer> logger) : base(localization, cultureProvider, null) { this.logger = logger; this.SetReadOnly(); }
        /// <summary>Log</summary>
        protected override void SearchedLocation(string? key, string? culture, object? resource) { if (logger != null && resource == null) LoggingUtilities.LineNotFound(logger, key, culture, null); }
    }
    /// <summary>Text localizer</summary>
    public class TextLocalizer<Subject> : Avalanche.Localization.TextLocalizer<Subject>
    {
        /// <summary>Optional logger</summary>
        protected ILogger? logger;
        /// <summary>Create text localizer</summary>
        public TextLocalizer(ILocalization localization) : base(localization, Avalanche.Localization.CultureProvider.CurrentThread.Instance) { this.SetReadOnly(); }
        /// <summary>Create text localizer</summary>
        public TextLocalizer(ILocalization localization, ICultureProvider cultureProvider) : base(localization, cultureProvider) { this.SetReadOnly(); }
        /// <summary>Create text localizer</summary>
        public TextLocalizer(ILocalization localization, ILogger<Avalanche.Localization.TextLocalizer<Subject>> logger) : base(localization, Avalanche.Localization.CultureProvider.CurrentThread.Instance) { this.logger = logger; this.SetReadOnly(); }
        /// <summary>Create text localizer</summary>
        public TextLocalizer(ILocalization localization, ICultureProvider cultureProvider, ILogger<Avalanche.Localization.TextLocalizer<Subject>> logger) : base(localization, cultureProvider) { this.logger = logger; this.SetReadOnly(); }
        /// <summary>Log</summary>
        protected override void SearchedLocation(string? key, string? culture, object? resource) { if (logger != null && resource == null) LoggingUtilities.LineNotFound(logger, key, culture, null); }
    }
    /// <summary>Text localizer</summary>
    public class TextLocalizer<Subject, CultureProvider> : Avalanche.Localization.TextLocalizer<Subject, CultureProvider> where CultureProvider : ICultureProvider
    {
        /// <summary>Optional logger</summary>
        protected ILogger? logger;
        /// <summary>Create text localizer</summary>
        public TextLocalizer(ILocalization localization) : base(localization) { this.SetReadOnly(); }
        /// <summary>Create text localizer</summary>
        public TextLocalizer(ILocalization localization, IServiceProvider serviceProvider) : base(localization, (serviceProvider.GetService(typeof(CultureProvider)) as ICultureProvider) ?? Avalanche.Localization.CultureProvider.ByTypeCached[typeof(CultureProvider)]) { this.SetReadOnly(); }
        /// <summary>Create text localizer</summary>
        public TextLocalizer(ILocalization localization, ILogger<Avalanche.Localization.TextLocalizer<Subject, CultureProvider>> logger) : base(localization) { this.logger = logger; this.SetReadOnly(); }
        /// <summary>Create text localizer</summary>
        public TextLocalizer(ILocalization localization, IServiceProvider serviceProvider, ILogger<Avalanche.Localization.TextLocalizer<Subject, CultureProvider>> logger) : base(localization, (serviceProvider.GetService(typeof(CultureProvider)) as ICultureProvider) ?? Avalanche.Localization.CultureProvider.ByTypeCached[typeof(CultureProvider)]) { this.logger = logger; this.SetReadOnly(); }
        /// <summary>Log</summary>
        protected override void SearchedLocation(string? key, string? culture, object? resource) { if (logger != null && resource == null) LoggingUtilities.FileNotFound(logger, key, culture, null); }
    }
    /// <summary>File localizer</summary>
    public class FileLocalizer : Avalanche.Localization.FileLocalizer
    {
        /// <summary>Optional logger</summary>
        protected ILogger? logger;
        /// <summary>Create File localizer</summary>
        public FileLocalizer(ILocalization localization) : base(localization, Avalanche.Localization.CultureProvider.CurrentThread.Instance, null) { this.SetReadOnly(); }
        /// <summary>Create File localizer</summary>
        public FileLocalizer(ILocalization localization, ICultureProvider cultureProvider) : base(localization, cultureProvider, null) { this.SetReadOnly(); }
        /// <summary>Create File localizer</summary>
        public FileLocalizer(ILocalization localization, ILogger<Avalanche.Localization.FileLocalizer> logger) : base(localization, Avalanche.Localization.CultureProvider.CurrentThread.Instance, null) { this.logger = logger; this.SetReadOnly(); }
        /// <summary>Create File localizer</summary>
        public FileLocalizer(ILocalization localization, ICultureProvider cultureProvider, ILogger<Avalanche.Localization.FileLocalizer> logger) : base(localization, cultureProvider, null) { this.logger = logger; this.SetReadOnly(); }
        /// <summary>Log</summary>
        protected override void SearchedLocation(string? key, string? culture, object? resource) { if (logger != null && resource == null) LoggingUtilities.FileNotFound(logger, key, culture, null); }
    }
    /// <summary>File localizer</summary>
    public class FileLocalizer<Subject> : Avalanche.Localization.FileLocalizer<Subject>
    {
        /// <summary>Optional logger</summary>
        protected ILogger? logger;
        /// <summary>Create File localizer</summary>
        public FileLocalizer(ILocalization localization) : base(localization, Avalanche.Localization.CultureProvider.CurrentThread.Instance) { this.SetReadOnly(); }
        /// <summary>Create File localizer</summary>
        public FileLocalizer(ILocalization localization, ICultureProvider cultureProvider) : base(localization, cultureProvider) { this.SetReadOnly(); }
        /// <summary>Create File localizer</summary>
        public FileLocalizer(ILocalization localization, ILogger<Avalanche.Localization.FileLocalizer<Subject>> logger) : base(localization, Avalanche.Localization.CultureProvider.CurrentThread.Instance) { this.logger = logger; this.SetReadOnly(); }
        /// <summary>Create File localizer</summary>
        public FileLocalizer(ILocalization localization, ICultureProvider cultureProvider, ILogger<Avalanche.Localization.FileLocalizer<Subject>> logger) : base(localization, cultureProvider) { this.logger = logger; this.SetReadOnly(); }
        /// <summary>Log</summary>
        protected override void SearchedLocation(string? key, string? culture, object? resource) { if (logger != null && resource == null) LoggingUtilities.FileNotFound(logger, key, culture, null); }
    }
    /// <summary>File localizer</summary>
    public class FileLocalizer<Subject, CultureProvider> : Avalanche.Localization.FileLocalizer<Subject, CultureProvider> where CultureProvider : ICultureProvider
    {
        /// <summary>Optional logger</summary>
        protected ILogger? logger;
        /// <summary>Create File localizer</summary>
        public FileLocalizer(ILocalization localization) : base(localization) { this.SetReadOnly(); }
        /// <summary>Create File localizer</summary>
        public FileLocalizer(ILocalization localization, IServiceProvider serviceProvider) : base(localization, (serviceProvider.GetService(typeof(CultureProvider)) as ICultureProvider) ?? Avalanche.Localization.CultureProvider.ByTypeCached[typeof(CultureProvider)]) { this.SetReadOnly(); }
        /// <summary>Create File localizer</summary>
        public FileLocalizer(ILocalization localization, ILogger<Avalanche.Localization.FileLocalizer<Subject, CultureProvider>> logger) : base(localization) { this.logger = logger; this.SetReadOnly(); }
        /// <summary>Create File localizer</summary>
        public FileLocalizer(ILocalization localization, IServiceProvider serviceProvider, ILogger<Avalanche.Localization.FileLocalizer<Subject, CultureProvider>> logger) : base(localization, (serviceProvider.GetService(typeof(CultureProvider)) as ICultureProvider) ?? Avalanche.Localization.CultureProvider.ByTypeCached[typeof(CultureProvider)]) { this.logger = logger; this.SetReadOnly(); }
        /// <summary>Log</summary>
        protected override void SearchedLocation(string? key, string? culture, object? resource) { if (logger != null && resource == null) LoggingUtilities.FileNotFound(logger, key, culture, null); }
    }

    /// <summary></summary>
    public class ResourceManagerLineProvider : Avalanche.Localization.ResourceManagerLineProvider
    {
        /// <summary></summary>
        public ResourceManagerLineProvider(IEnumerable<ResourceManager> resourceManagers) : base(resourceManagers.ToArray()) { }
    }

    /// <summary></summary>
    public class ResourceManagerFileProvider : Avalanche.Localization.ResourceManagerFileProvider
    {
        /// <summary></summary>
        public ResourceManagerFileProvider(IEnumerable<ResourceManager> resourceManagers) : base(resourceManagers.ToArray()) { }
    }

    /// <summary></summary>
    public class StringLocalizer : DiStringLocalizer
    {
        /// <summary></summary>
        public StringLocalizer(ILocalization localization) : base(localization, @namespace: "", cultureInfo: null)
        {
        }
        /// <summary></summary>
        public StringLocalizer(ILocalization localization, ILogger<StringLocalizer> logger) : base(localization, @namespace: "", cultureInfo: null, logger)
        {
        }
    }
}

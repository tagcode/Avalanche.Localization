// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Internal;
using Avalanche.Template;
using Avalanche.Utilities;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;

/// <summary></summary>
public class DiStringLocalizer : IStringLocalizer
{
    /// <summary>Constructor</summary>
    static readonly ConstructorT<ILocalization, string, string?, ILogger?, DiStringLocalizer> constructor = new(typeof(DiStringLocalizer<>));
    /// <summary>Create <see cref="DiStringLocalizer{T}"/>.</summary>
    public static DiStringLocalizer Create(Type resourceSource, ILocalization localization, string @namespace, string? culture, ILogger? logger) => constructor.Create(resourceSource, localization, @namespace, culture, logger);

    /// <summary></summary>
    protected ILocalization localization;
    /// <summary></summary>
    protected string @namespace;
    /// <summary></summary>
    protected CultureInfo? culture;
    /// <summary>Optional logger</summary>
    protected ILogger? logger;

    /// <summary></summary>
    public ILocalization Localization => localization;
    /// <summary></summary>
    public string Namespace => @namespace;
    /// <summary></summary>
    public CultureInfo? Culture => culture;
    /// <summary>Optional logger</summary>
    public ILogger? Logger => logger;

    /// <summary>Get culture for formatting</summary>
    public virtual CultureInfo ActiveFormatCulture => this.culture ?? Thread.CurrentThread?.CurrentCulture ?? CultureInfo.CurrentCulture ?? CultureInfo.InvariantCulture;
    /// <summary>Get culture for language</summary>
    public virtual CultureInfo ActiveTextCulture => this.culture ?? Thread.CurrentThread?.CurrentUICulture ?? CultureInfo.CurrentUICulture ?? CultureInfo.InvariantCulture;

    /// <summary></summary>
    public virtual LocalizedString this[string name]
    {
        get
        {
            // Choose culture
            CultureInfo culture = ActiveTextCulture;
            // Get text
            ILocalizedText? text = GetLocalizedText(name, culture);
            // No text
            if (text == null) return new LocalizedString(name, name, true);
            // Get text
            string print = text.Text;
            // Return print
            return new LocalizedString(name, print);
        }
    }

    /// <summary></summary>
    public virtual LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            // Choose culture
            CultureInfo uiCulture = ActiveTextCulture;
            // Get text
            ILocalizedText? text = GetLocalizedText(name, uiCulture);
            // No text
            if (text == null) return new LocalizedString(name, name, true);
            // Pluralize
            ITemplateText pluralized = text.Pluralize(uiCulture, arguments);
            // Format
            CultureInfo formatCulture = ActiveFormatCulture;
            // Print
            string print = pluralized.Print(formatCulture, arguments);
            // Return print
            return new LocalizedString(name, print);
        }
    }

    /// <summary></summary>
    public DiStringLocalizer(ILocalization localization, string @namespace, string? culture)
    {
        this.localization = localization ?? throw new ArgumentNullException(nameof(localization));
        this.@namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        this.culture = culture == null ? null : CultureInfo.GetCultureInfo(culture);
    }

    /// <summary></summary>
    public DiStringLocalizer(ILocalization localization, string @namespace, CultureInfo? cultureInfo)
    {
        this.localization = localization ?? throw new ArgumentNullException(nameof(localization));
        this.@namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        this.culture = cultureInfo;
    }

    /// <summary></summary>
    public DiStringLocalizer(ILocalization localization, string @namespace, string? culture, ILogger logger)
    {
        this.localization = localization ?? throw new ArgumentNullException(nameof(localization));
        this.@namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        this.culture = culture == null ? null : CultureInfo.GetCultureInfo(culture);
        this.logger = logger;
    }

    /// <summary></summary>
    public DiStringLocalizer(ILocalization localization, string @namespace, CultureInfo? cultureInfo, ILogger logger)
    {
        this.localization = localization ?? throw new ArgumentNullException(nameof(localization));
        this.@namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
        this.culture = cultureInfo;
        this.logger = logger;
    }

    /// <summary>Get text for active culture</summary>
    ILocalizedText? GetLocalizedText(string name, CultureInfo culture)
    {
        // Create key
        string key = String.IsNullOrEmpty(@namespace) ? name : CreateKey(name)!;
        // Try get text
        localization.LocalizedTextCached.TryGetValue((culture.Name, key), out ILocalizedText? text);
        // Line was not found
        if (logger != null && text == null) LoggingUtilities.FileNotFound(logger, key, culture.Name, null);
        // Return
        return text;
    }

    /// <summary>Creates key by appending <see cref="Namespace"/> to <paramref name="name"/>.</summary>
    /// <returns>"Namespace.Name"</returns>
    protected virtual string? CreateKey(string? name)
    {
        // Get namespace snapshot
        string? _namespace = Namespace;
        // Namespace and Name are null
        if (name == null && _namespace == null) return null;
        // Only Name
        if (_namespace == null && name != null) return name;
        // Only Namespace
        if (_namespace != null && name == null) return _namespace;
        // Estimate length
        int length = _namespace!.Length + 1 + name!.Length;
        // Allocate chars
        Span<char> buf = length < 256 ? stackalloc char[length] : new char[length];
        // Copy chars
        _namespace.CopyTo(buf);
        buf[_namespace.Length] = '.';
        name.CopyTo(buf.Slice(_namespace.Length + 1));
        // Create string
        string str = new string(buf);
        // Return
        return str;
    }


    /// <summary></summary>
    public virtual IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        // Get active culture (as is in ResourceManagerStringLocalizer)
        CultureInfo culture = ActiveTextCulture;
        // Result list
        List<LocalizedString> result = new List<LocalizedString>();

        // Place cultures here
        List<string> cultures = new();
        // Add parent cultures
        if (includeParentCultures)
        {
            // Add cultures
            for (CultureInfo _culture = culture; !cultures.Contains(_culture.Name); _culture = _culture.Parent) cultures.Add(_culture.Name);
        }
        else
        {
            // Add culture
            cultures.Add(culture.Name);
        }
        // Create culture set
        HashSet<string> cultureSet = new(cultures);
        // Already added results
        HashSet<(string, string)> addedResults = new();

        // Visit all cultures
        foreach (string _culture in cultures)
        {
            // Get all localized texts            
            if (!localization.LocalizedTextsQueryCached.TryGetValue((_culture, null), out IEnumerable<ILocalizedText> texts)) continue;
            // Filter
            foreach (ILocalizedText text in texts)
            {
                // Culture not requested
                if (!cultureSet.Contains(text.Culture)) continue;
                //
                if (text.Key.Length <= @namespace.Length) continue;
                // 
                if (!text.Key.StartsWith(@namespace, StringComparison.Ordinal)) continue;
                //
                string key = text.Key.Substring(text.Key[@namespace.Length] == '.' ? @namespace.Length + 1 : @namespace.Length);
                //
                string _text = text.Text;
                // Already added
                if (!addedResults.Add((key, _text))) continue;
                // Create string
                LocalizedString localizedString = new LocalizedString(key, _text);
                // Add to result
                result.Add(localizedString);
            }
        }

        // Return
        return result == null ? Array.Empty<LocalizedString>() : result;
    }

    /// <summary></summary>
    public override bool Equals(object? obj)
    {
        // Cast
        if (obj is not DiStringLocalizer other) return false;
        //
        if (this.Culture != other.Culture) return false;
        if (this.Namespace != other.Namespace) return false;
        if (this.Localization != other.Localization) return false;
        // Equals
        return true;
    }

    /// <summary></summary>
    public override int GetHashCode()
    {
        //
        FNVHash32 hash = new();
        hash.HashIn(this.Culture);
        hash.HashIn(this.Namespace);
        hash.HashIn(this.Localization);
        //
        return hash.Hash;
    }

    /// <summary>Print information</summary>
    public override string ToString() => @namespace;
}

/// <summary></summary>
public class DiStringLocalizer<T> : DiStringLocalizer, IStringLocalizer<T>
{
    /// <summary></summary>
    public DiStringLocalizer(ILocalization localization, string @namespace, string? culture) : base(localization, @namespace, culture)
    {
    }
    /// <summary></summary>
    public DiStringLocalizer(ILocalization localization, string @namespace, string? culture, ILogger logger) : base(localization, @namespace, culture, logger)
    {
    }
}

/// <summary>Wraps <see cref="IStringLocalizer"/> as <see cref="IStringLocalizer{T}"/>.</summary>
public class StringLocalizerWrapper<T> : IStringLocalizer<T>
{
    /// <summary>Wrapped localizer</summary>
    protected IStringLocalizer localizer;

    /// <summary></summary>
    public StringLocalizerWrapper(IStringLocalizerFactory factory)
    {
        // Assert not null
        if (factory == null) throw new ArgumentNullException(nameof(factory));
        // Create underlying localizer
        localizer = factory.Create(typeof(T));
    }

    /// <summary></summary>
    public virtual LocalizedString this[string name] => localizer[name];
    /// <summary></summary>
    public virtual LocalizedString this[string name, params object[] arguments] => localizer[name, arguments];
    /// <summary></summary>
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => localizer.GetAllStrings(includeParentCultures);
}

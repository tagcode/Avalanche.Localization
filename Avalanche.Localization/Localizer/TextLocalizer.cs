// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Globalization;
using Avalanche.Template;

/// <summary>Text localizer</summary>
public class TextLocalizer : Localizer<ITemplatePrintable>, ITextLocalizer
{
    /// <summary>Element Type</summary>
    public override Type ResourceType => typeof(ITemplatePrintable);

    /// <summary></summary>
    public virtual new ILocalizedText? this[string? name] => (ILocalizedText?)GetLocalized(name);

    /// <summary>
    /// Get localized text for key "Namespace.Name".
    /// 
    /// If <see cref="ILocalizer.Namespace"/> or <paramref name="name"/> is null, then counter part is used as key without separator '.'.
    /// </summary>
    protected override ILocalized? GetLocalized(string? name)
    {
        // Get culture and format provider
        (string culture, IFormatProvider format) = this.CultureProvider.Set();
        // No culture
        if (culture == null) { SearchedLocation(null, null, null); return null!; }
        // Get key
        string? key = CreateKey(name);
        // No key
        if (key == null) { SearchedLocation(null, culture, null); return null!; }
        // Place text here
        ILocalizedText? text = null!;
        // Try get text with format provider
        if (localization.FormatLocalizedTextCached!=null) localization.FormatLocalizedTextCached.TryGetValue(((culture, format), key), out text);
        // Try get text
        if (text == null && localization.LocalizedTextCached!=null) localization.LocalizedTextCached.TryGetValue((culture, key), out text);
        // Handle result
        SearchedLocation(key, culture, text);
        // Return
        return text;
    }

    /// <summary></summary>
    public TextLocalizer() : base() { }
    /// <summary></summary>
    public TextLocalizer(ILocalization localization) : base(localization) { }
    /// <summary></summary>
    public TextLocalizer(ILocalization localization, ICultureProvider cultureProvider) : base(localization, cultureProvider) { }
    /// <summary></summary>
    public TextLocalizer(ILocalization localization, ICultureProvider cultureProvider, string? @namespace) : base(localization, cultureProvider, @namespace) { }
    /// <summary>Print information</summary>
    public override string ToString() => Namespace ?? GetType().Name;
}

/// <summary>Text localizer</summary>
/// <typeparam name="Namespace">Key namespace</typeparam>
public class TextLocalizer<Namespace> : TextLocalizer, ITextLocalizer<Namespace>
{
    /// <summary></summary>
    public TextLocalizer() : base(null!, null!, PrintNamespace<Namespace>()) { }
    /// <summary></summary>
    public TextLocalizer(ILocalization localization) : base(localization, null!, PrintNamespace<Namespace>()) { }
    /// <summary></summary>
    public TextLocalizer(ILocalization localization, ICultureProvider cultureProvider) : base(localization, cultureProvider, PrintNamespace<Namespace>()) { }
    /// <summary></summary>
    public TextLocalizer(ILocalization localization, ICultureProvider cultureProvider, string? @namespace) : base(localization, cultureProvider, @namespace) { }
}

/// <summary>Text localizer</summary>
/// <typeparam name="Namespace">Key namespace</typeparam>
/// <typeparam name="CultureProvider"></typeparam>
public class TextLocalizer<Namespace, CultureProvider> : TextLocalizer<Namespace>, ITextLocalizer<Namespace, CultureProvider> where CultureProvider : ICultureProvider
{
    /// <summary></summary>
    public TextLocalizer() : base(null!, Avalanche.Localization.CultureProvider.ByTypeCached[typeof(CultureProvider)]) { }
    /// <summary></summary>
    public TextLocalizer(ILocalization localization) : base(localization, Avalanche.Localization.CultureProvider.ByTypeCached[typeof(CultureProvider)]) { }
    /// <summary></summary>
    public TextLocalizer(ILocalization localization, ICultureProvider cultureProvider) : base(localization, cultureProvider, PrintNamespace<Namespace>()) { }
    /// <summary></summary>
    public TextLocalizer(ILocalization localization, ICultureProvider cultureProvider, string? @namespace) : base(localization, cultureProvider, @namespace) { }
}

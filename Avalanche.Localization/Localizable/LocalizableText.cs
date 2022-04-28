// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

/// <summary></summary>
public class LocalizableText : ILocalizableText, ITemplateEmplacementable
{
    /// <summary></summary>
    public string Key { get => key; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplatePart[] Parts { get => Default.Breakdown.Parts; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplatePlaceholderPart[] Placeholders { get => Default.Breakdown.Placeholders; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplateParameterPart?[] Parameters { get => Default.Breakdown.Parameters; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string Text { get => Default.Text; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplateFormat? TemplateFormat { get => Default.TemplateFormat; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplateBreakdown Breakdown { get => Default.Breakdown; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string?[] ParameterNames { get => Default.ParameterNames; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public IList<ILocalizationError> Errors { get => Default.Errors; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string Culture { get => Default.Culture; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public IFormatProvider Format { get => Default.Format; set => throw new InvalidOperationException(); }

    /// <summary></summary>
    protected string key;
    /// <summary>Localized text provider</summary>
    protected IProvider<((string culture, IFormatProvider formatProvider), string key), ILocalizedText>? textProvider1;
    /// <summary>Localized text provider</summary>
    protected IProvider<(string culture, string key), ILocalizedText>? textProvider2;
    /// <summary>Place here default value, invariat or fallback culture</summary>
    protected ValueResult<ILocalizedText> defaultText;
    /// <summary></summary>
    protected ICultureProvider? cultureProvider;
    /// <summary></summary>
    protected ITemplateParameterEmplacement[][]? emplacementSequences;
    /// <summary>Provider that provides pluralized+emplacement pluralized version. Key is array of emplacement texts, plus last index is pluralized version of .</summary>
    protected ConcurrentDictionaryCacheProvider<ITemplateText, ITemplateText>? emplacementedProvider;

    /// <summary></summary>
    public ILocalizedText Default
    {
        get
        {
            // Return cached value
            if (defaultText.Status == ResultStatus.Ok) return defaultText.Value;
            // Return cached
            if (defaultText.Status == ResultStatus.Error) throw ExceptionUtilities.Wrap(defaultText.Error!);
            // Place here text
            ILocalizedText localizedText = null!;
            // Acquire lines for invariant
            if (textProvider1 != null && textProvider1.TryGetValue((("", CultureInfo.InvariantCulture), key), out localizedText)) { }
            // Acquire lines for invariant
            else if (textProvider2 != null && textProvider2.TryGetValue(("", key), out localizedText)) { }
            // Fallback
            else { defaultText = new ValueResult<ILocalizedText> { Value = LocalizedText.CreateKeyNotFound("", key), Status = ResultStatus.Ok }; return defaultText.Value; }
            // Apply emplacements
            if (this.emplacementedProvider != null) localizedText = (ILocalizedText)this.emplacementedProvider[localizedText];
            // Wrap to result container and assign
            defaultText = new ValueResult<ILocalizedText> { Value = localizedText, Status = ResultStatus.Ok };
            // Return
            return defaultText.Value;
        }
        set { defaultText = new ValueResult<ILocalizedText> { Value = value, Status = ResultStatus.Ok }; }
    }

    ILocalized<ILocalizedText>? ILocalizable<ILocalizedText>.Default => Localized<ILocalizedText>.GetOrCreate(Key, Culture, Default);

    /// <summary></summary>
    public LocalizableText(string key, IProvider<((string culture, IFormatProvider formatProvider) culture, string key), ILocalizedText> textProvider, ICultureProvider? cultureProvider = null, ITemplateText? defaultText = null, ITemplateParameterEmplacement[][]? emplacementSequences = null)
    {
        this.key = key ?? throw new ArgumentNullException(nameof(key));
        this.textProvider1 = textProvider ?? throw new ArgumentNullException(nameof(textProvider));
        this.cultureProvider = cultureProvider;
        this.emplacementSequences = emplacementSequences;
        if (this.emplacementSequences!=null) this.emplacementedProvider = Providers.Func<ITemplateText, ITemplateText>(ApplyEmplacements).Cached(ReferenceEqualityComparer<ITemplateText>.Instance);
        if (defaultText != null) this.Default = new LocalizedText(defaultText, null!, null!, key);
    }

    /// <summary></summary>
    public LocalizableText(string key, IProvider<(string culture, string key), ILocalizedText> textProvider, ICultureProvider? cultureProvider = null, ITemplateText? defaultText = null, ITemplateParameterEmplacement[][]? emplacementSequences = null)
    {
        this.key = key ?? throw new ArgumentNullException(nameof(key));
        this.textProvider2 = textProvider ?? throw new ArgumentNullException(nameof(textProvider));
        this.cultureProvider = cultureProvider;
        this.emplacementSequences = emplacementSequences;
        if (this.emplacementSequences != null) this.emplacementedProvider = Providers.Func<ITemplateText, ITemplateText>(ApplyEmplacements).Cached(ReferenceEqualityComparer<ITemplateText>.Instance);
        if (defaultText != null) this.Default = new LocalizedText(defaultText, null!, null!, key);
    }

    /// <summary>Apply assigned emplacements to <paramref name="text"/>.</summary>
    protected virtual ITemplateText ApplyEmplacements(ITemplateText text)
    {
        // No emplacements
        if (emplacementSequences == null) return text;
        // Apply emplacements
        foreach (ITemplateParameterEmplacement[] emplacements in emplacementSequences)
            text = text.Place(emplacements);
        // Return
        return text;
    }

    /// <summary></summary>
    public virtual bool TryPlace(ITemplateParameterEmplacement[] emplacements, [NotNullWhen(true)] out ITemplateText emplaced)
    {
        // Return this
        if (emplacements.Length == 0) { emplaced = this; return true; }
        // Concat emplacements
        ITemplateParameterEmplacement[][]? _emplacementSequences = ArrayUtilities.Append(this.emplacementSequences, emplacements);
        // Get default text
        ITemplateText? _defaultText = this.defaultText.Value;
        // Add emplacements to default text
        if (_defaultText != null) _defaultText = _defaultText.Place(emplacements);
        // Create new text
        emplaced =
            this.textProvider1 != null ?
            new LocalizableText(key, textProvider1, cultureProvider, _defaultText, _emplacementSequences) :
            new LocalizableText(key, textProvider2!, cultureProvider, _defaultText, _emplacementSequences);
        // Done
        return true;
    }

    /// <summary>Choose culture</summary>
    public virtual (string culture, IFormatProvider formatProvider) GetCultureSet(IFormatProvider? formatProvider = null)
    {
        // Use culture provider (+ format provider)
        if (cultureProvider != null)
        {
            // Get active culture
            (string culture, IFormatProvider formatProvider) _culture = cultureProvider.Set();
            // Use active culture and possibly overriding format provider
            return (_culture.culture, formatProvider ?? _culture.formatProvider);
        }

        // Use format provider as culture info
        if (formatProvider is CultureInfo cultureInfo) return (cultureInfo.Name, cultureInfo);

        // Use format provider (non culture info)
        if (formatProvider != null) return ("", formatProvider);

        // Use fallback culture
        return ("", CultureInfo.InvariantCulture);        
    }

    /// <summary></summary>
    public ITemplatePrintable Pluralize(object?[]? arguments)
    {
        // Get culture
        (string culture, IFormatProvider formatProvider) = GetCultureSet();
        // Pluralize
        ITemplatePrintable pluralized = Pluralize(culture, formatProvider, arguments).WithFormat(formatProvider);
        // Return
        return pluralized;
    }

    /// <summary></summary>
    public ITemplateText Pluralize(IFormatProvider? formatProvider, object?[]? arguments)
    {
        // Get culture
        (string culture, IFormatProvider formatProvider_) = GetCultureSet(formatProvider);
        // Pluralize
        ITemplateText pluralized = Pluralize(culture, formatProvider_, arguments);
        // Return pluralized
        return pluralized;
    }

    /// <summary></summary>
    protected ITemplateText Pluralize(string culture, IFormatProvider formatProvider, object?[]? arguments)
    {
        // Place here localized text
        ILocalizedText localizedText;
        // Try get text
        if (textProvider1 != null && textProvider1.TryGetValue(((culture, formatProvider), Key), out localizedText)) { }
        // Try get text
        else if (textProvider2 != null && textProvider2.TryGetValue((culture, Key), out localizedText)) { }
        // Revert to default
        else localizedText = Default;
        // Apply emplacements
        if (emplacementedProvider != null) localizedText = (emplacementedProvider[localizedText] as ILocalizedText) ?? localizedText;
        // Pluralize
        ITemplateText pluralized = localizedText.Pluralize(formatProvider, arguments);
        // Return pluralized
        return pluralized;
    }

    /// <summary></summary>
    public ILocalized<ILocalizedText>? Localize(string culture)
    {
        // Place here text
        ILocalizedText? localizedText = null!;
        // Try get text
        if (textProvider1 != null)
        {
            // Get format provider
            IFormatProvider format = CultureInfo.GetCultureInfo(culture);
            // Try get text
            textProvider1.TryGetValue(((culture, format), Key), out localizedText);
        }
        // Try get text
        if (localizedText == null && textProvider2 != null) textProvider2.TryGetValue((culture, Key), out localizedText);
        // Revert to fallback default
        if (localizedText == null) localizedText = Default;
        // Apply emplacements
        if (emplacementedProvider != null) localizedText = (emplacementedProvider[localizedText] as ILocalizedText) ?? localizedText;
        // No text
        if (localizedText == null) return null;
        // Wrap
        ILocalized<ILocalizedText> localized = Localized<ILocalizedText>.GetOrCreate(Key, culture, localizedText);
        // Return
        return localized;
    }

    /// <summary></summary>
    public string Print(object?[]? arguments = null)
    {
        // Get culture
        (string culture, IFormatProvider formatProvider) = GetCultureSet();
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, culture, false);
        // Pluralize
        ITemplateText pluralized = Pluralize(culture, formatProvider, arguments);
        // Print
        string print = pluralized.Print(formatProvider, arguments);
        // Return
        return print;
    }

    /// <summary></summary>
    public string Print(IFormatProvider? formatProvider_, object?[]? arguments = null)
    {
        // Get culture
        (string culture, IFormatProvider formatProvider) = GetCultureSet(formatProvider_);
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, culture, false);
        // Pluralize
        ITemplateText pluralized = Pluralize(culture, formatProvider, arguments);
        // Print
        string print = pluralized.Print(formatProvider, arguments);
        // Return
        return print;
    }

    /// <summary></summary>
    public void AppendTo(StringBuilder sb, object?[]? arguments = null)
    {
        // Get culture
        (string culture, IFormatProvider formatProvider) = GetCultureSet();
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, culture, false);
        // Pluralize
        ITemplateText pluralized = Pluralize(culture, formatProvider, arguments);
        // Append
        pluralized.AppendTo(sb, formatProvider, arguments);
    } 

    /// <summary></summary>
    public void AppendTo(StringBuilder sb, IFormatProvider? formatProvider_, object?[]? arguments = null)
    {
        // Get culture
        (string culture, IFormatProvider formatProvider) = GetCultureSet(formatProvider_);
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, culture, false);
        // Pluralize
        ITemplateText pluralized = Pluralize(culture, formatProvider, arguments);
        // Append
        pluralized.AppendTo(sb, formatProvider, arguments);
    }

    /// <summary></summary>
    public void WriteTo(TextWriter textWriter, object?[]? arguments = null)
    {
        // Get culture
        (string culture, IFormatProvider formatProvider) = GetCultureSet();
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, culture, false);
        // Pluralize
        ITemplateText pluralized = Pluralize(culture, formatProvider, arguments);
        // Write
        pluralized.WriteTo(textWriter, formatProvider, arguments);
    }

    /// <summary></summary>
    public void WriteTo(TextWriter textWriter, IFormatProvider? formatProvider_, object?[]? arguments = null)
    {
        // Get culture
        (string culture, IFormatProvider formatProvider) = GetCultureSet(formatProvider_);
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, culture, false);
        // Pluralize
        ITemplateText pluralized = Pluralize(culture, formatProvider, arguments);
        // Write
        pluralized.WriteTo(textWriter, formatProvider, arguments);
    }

    /// <summary></summary>
    public bool TryPrintTo(Span<char> dst, out int length, object?[]? arguments = null)
    {
        // Get culture
        (string culture, IFormatProvider formatProvider) = GetCultureSet();
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, culture, false);
        // Pluralize
        ITemplateText pluralized = Pluralize(culture, formatProvider, arguments);
        // Try print
        bool ok = pluralized.TryPrintTo(dst, out length, formatProvider, arguments);
        // Return
        return ok;
    }

    /// <summary></summary>
    public bool TryPrintTo(Span<char> dst, out int length, IFormatProvider? formatProvider_, object?[]? arguments = null)
    {
        // Get culture
        (string culture, IFormatProvider formatProvider) = GetCultureSet(formatProvider_);
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, culture, false);
        // Pluralize
        ITemplateText pluralized = Pluralize(culture, formatProvider, arguments);
        // Try print
        bool ok = pluralized.TryPrintTo(dst, out length, formatProvider, arguments);
        // Return
        return ok;
    }

    /// <summary></summary>
    public bool TryEstimatePrintLength(out int length, object?[]? arguments = null)
    {
        // Get culture
        (string culture, IFormatProvider formatProvider) = GetCultureSet();
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, culture, false);
        // Pluralize
        ITemplateText pluralized = Pluralize(culture, formatProvider, arguments);
        // Try estimate
        bool ok = pluralized.TryEstimatePrintLength(out length, formatProvider, arguments);
        // Return
        return ok;
    }

    /// <summary></summary>
    public bool TryEstimatePrintLength(out int length, IFormatProvider? formatProvider_, object?[]? arguments = null)
    {
        // Get culture
        (string culture, IFormatProvider formatProvider) = GetCultureSet(formatProvider_);
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, culture, false);
        // Pluralize
        ITemplateText pluralized = Pluralize(culture, formatProvider, arguments);
        // Try estimate
        bool ok = pluralized.TryEstimatePrintLength(out length, formatProvider, arguments);
        // Return
        return ok;
    }

    /// <summary></summary>
    public override string ToString() => Default.ToString() ?? Key;
}

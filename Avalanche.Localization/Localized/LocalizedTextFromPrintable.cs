// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Avalanche.Template;
using Avalanche.Utilities;

/// <summary>Adapts <see cref="ITemplatePrintable"/> into <see cref="ILocalizedText"/>.</summary>
public class LocalizedTextFromPrintable : ILocalizedText, ILocalized<ILocalizedText>, ITemplateEmplacementable
{
    /// <summary>Empty text</summary>
    static ParameterlessText empty = new ParameterlessText("");
    /// <summary>Create text that carries key not found error.</summary>
    public static ILocalizedText CreateKeyNotFound(string culture, string key)
    {
        // Create error
        ILocalizationError error = new LocalizationError { Code = LocalizationMessageIds.NoKey, Culture = culture, Key = key, Message = "Key not found {Key}, culture={Culture}" };
        // Create info
        ILocalizedText info = new LocalizedTextFromPrintable(empty, culture, null, key, new ILocalizationError[] { error });
        // Return
        return info;
    }

    /// <summary></summary>
    protected IList<ILocalizationError> errors;
    /// <summary></summary>
    protected string key;
    /// <summary></summary>
    protected string culture;
    /// <summary></summary>
    protected IFormatProvider format;
    /// <summary></summary>
    protected ITemplatePrintable printable;

    /// <summary></summary>
    public IList<ILocalizationError> Errors { get => errors; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string Key { get => key; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string?[] ParameterNames { get => printable?.ParameterNames!; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string Text { get => printable is ITemplateText _text ? _text.Text : printable.ToString()!; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplateFormat? TemplateFormat { get => printable is ITemplateText _text ? _text.TemplateFormat : null; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplateBreakdown Breakdown { get => (printable as ITemplateBreakdown) ?? (printable is ITemplateText _text ? _text.Breakdown : null)!; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string Culture { get => culture; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public IFormatProvider Format { get => format; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ILocalizedText Value => this;

    /// <summary></summary>
    public LocalizedTextFromPrintable(ITemplatePrintable printable, string? culture, IFormatProvider? format, string key, IList<ILocalizationError> errors = null!) : base()
    {
        this.printable = printable ?? throw new ArgumentNullException(nameof(printable));
        this.culture = culture ?? "";;
        this.format = format ?? (culture == null ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(culture));
        this.key = key;
        this.errors = errors ?? Array.Empty<ILocalizationError>();
    }

    /// <summary></summary>
    public virtual bool TryPlace(ITemplateParameterEmplacement[] emplacements, [NotNullWhen(true)] out ITemplateText emplaced)
    {
        // Return this
        if (emplacements.Length == 0) { emplaced = this; return true; }
        // Not template text
        if (this.printable is not ITemplateText text) { emplaced = null!; return false; }
        // Concat errors
        StructList4<ILocalizationError> _errors = new();
        // Add errors
        if (this.Errors != null) _errors.AddRange(this.Errors);
        foreach (var emplacement in emplacements)
            if (emplacement is ILocalizationErrorProvider errorProvider && errorProvider.Errors != null)
                _errors.AddRange(errorProvider.Errors);
        // Localize emplacements
        if (culture != null) emplacements = LocalizationEmplacementExtensions.LocalizeParameterEmplacements(emplacements ?? throw new ArgumentNullException(nameof(emplacements)), culture);
        // Emplace
        ITemplateText _emplaced = TemplateEmplacementExtensions.Compose(text, emplacements);
        // Not emplaced
        if (_emplaced is not ITemplatePrintable emplacedPrintable) { emplaced = null!; return false; }
        // Create new localized text
        emplaced = new LocalizedTextFromPrintable(emplacedPrintable, Culture, Format, Key, _errors.ToArray());
        // Return
        return true;
    }

    /// <summary></summary>
    public void AppendTo(StringBuilder sb, object?[]? arguments = null) => printable.AppendTo(sb, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));
    /// <summary></summary>
    public void AppendTo(StringBuilder sb, IFormatProvider? formatProvider, object?[]? arguments = null) => printable.AppendTo(sb, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));

    /// <summary></summary>
    public string Print(object?[]? arguments = null) => printable.Print(LocalizedExtensions_.LocalizeArguments(arguments, culture, false));
    /// <summary></summary>
    public string Print(IFormatProvider? formatProvider, object?[]? arguments = null) => printable.Print(LocalizedExtensions_.LocalizeArguments(arguments, culture, false));

    /// <summary></summary>
    public bool TryEstimatePrintLength(out int length, object?[]? arguments = null) => printable.TryEstimatePrintLength(out length, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));
    /// <summary></summary>
    public bool TryEstimatePrintLength(out int length, IFormatProvider? formatProvider, object?[]? arguments = null) => printable.TryEstimatePrintLength(out length, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));

    /// <summary></summary>
    public bool TryPrintTo(Span<char> dst, out int length, object?[]? arguments = null) => printable.TryPrintTo(dst, out length, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));
    /// <summary></summary>
    public bool TryPrintTo(Span<char> dst, out int length, IFormatProvider? formatProvider, object?[]? arguments = null) => printable.TryPrintTo(dst, out length, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));

    /// <summary></summary>
    public void WriteTo(TextWriter textWriter, object?[]? arguments = null) => printable.WriteTo(textWriter, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));
    /// <summary></summary>
    public void WriteTo(TextWriter textWriter, IFormatProvider? formatProvider, object?[]? arguments = null) => printable.WriteTo(textWriter, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));

    /// <summary></summary>
    public ITemplatePrintable Pluralize(object?[]? arguments) => printable;
    /// <summary></summary>
    public ITemplateText Pluralize(IFormatProvider? formatProvider, object?[]? arguments) => this;

    /// <summary></summary>
    public override string ToString() => printable?.ToString() ?? key;
}

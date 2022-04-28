// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Avalanche.Template;
using Avalanche.Utilities;

/// <summary>Adapts <see cref="ITemplateFormatPrintable"/> into <see cref="ILocalizedText"/>.</summary>
public class LocalizedText : ILocalizedText, ILocalized<ILocalizedText>, ITemplateEmplacementable
{
    /// <summary>Empty text</summary>
    static ParameterlessText empty = new ParameterlessText("");
    /// <summary>Create text that carries key not found error.</summary>
    public static ILocalizedText CreateKeyNotFound(string culture, string key)
    {
        // Create error
        ILocalizationError error = new LocalizationError { Code = LocalizationMessageIds.NoKey, Culture = culture, Key = key, Message = "Key not found {Key}, culture={Culture}" };
        // Create info
        ILocalizedText info = new LocalizedText(empty, culture, null!, key, new ILocalizationError[] { error });
        // Return
        return info;
    }

    /// <summary></summary>
    protected IList<ILocalizationError> errors;
    /// <summary></summary>
    protected string key;
    /// <summary></summary>
    protected ITemplateFormatPrintable printable;
    /// <summary></summary>
    protected string culture;
    /// <summary></summary>
    protected IFormatProvider format;

    /// <summary></summary>
    public IList<ILocalizationError> Errors { get => errors; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string Key { get => key; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string?[] ParameterNames { get => printable?.ParameterNames!; set => throw new InvalidOperationException(); }

    /// <summary></summary>
    public string Text { get => AsTemplateText()?.Text ?? printable.ToString()!; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplateFormat? TemplateFormat { get => AsTemplateText()?.TemplateFormat; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplateBreakdown Breakdown { get => AsTemplateText()?.Breakdown!; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string Culture { get => culture; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public IFormatProvider Format { get => format; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ILocalizedText Value => this;

    /// <summary></summary>
    public LocalizedText(ITemplateFormatPrintable printable, string culture, IFormatProvider format, string key, IList<ILocalizationError> errors = null!) : base()
    {
        this.printable = printable ?? throw new ArgumentNullException(nameof(printable));
        this.key = key;
        this.errors = errors ?? Array.Empty<ILocalizationError>();
        this.culture = culture;
        this.format = format ?? CultureInfo.GetCultureInfo(culture ?? "");
    }

    /// <summary></summary>
    public LocalizedText(string braceText, string culture, IFormatProvider format, string key, IList<ILocalizationError> errors = null!) : base()
    {
        this.printable = new TemplateText(braceText ?? throw new ArgumentNullException(nameof(braceText)), Avalanche.Template.TemplateFormat.Brace);
        this.key = key;
        this.errors = errors ?? Array.Empty<ILocalizationError>();
        this.culture = culture;
        this.format = format ?? CultureInfo.GetCultureInfo(culture ?? "");
    }

    /// <summary></summary>
    public virtual bool TryPlace(ITemplateParameterEmplacement[] emplacements, [NotNullWhen(true)] out ITemplateText emplaced)
    {
        // Return this
        if (emplacements.Length == 0) { emplaced = this as ITemplateText; return emplaced != null; }
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
        if (culture!=null) emplacements = LocalizationEmplacementExtensions.LocalizeParameterEmplacements(emplacements ?? throw new ArgumentNullException(nameof(emplacements)), culture);
        // Emplace
        ITemplateText _emplaced = TemplateEmplacementExtensions.Compose(text, emplacements);
        // Create new localized text
        emplaced = new LocalizedText(_emplaced, Culture, Format, Key, _errors.ToArray());
        // Return
        return true;
    }

    /// <summary>Find</summary>
    protected ITemplateText? AsTemplateText()
    {
        // Visit decoration path
        for (object? o = printable; o != null; o = (o as IDecoration)?.Decoree)
            if (o is ITemplateText text) return text;
        return null;
    }

    /// <summary></summary>
    public void AppendTo(StringBuilder sb, object?[]? arguments = null) => printable.AppendTo(sb, format, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));
    /// <summary></summary>
    public void AppendTo(StringBuilder sb, IFormatProvider? formatProvider, object?[]? arguments = null) => printable.AppendTo(sb, formatProvider??format, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));

    /// <summary></summary>
    public string Print(object?[]? arguments = null) => printable.Print(format, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));
    /// <summary></summary>
    public string Print(IFormatProvider? formatProvider, object?[]? arguments = null) => printable.Print(formatProvider??format, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));

    /// <summary></summary>
    public bool TryEstimatePrintLength(out int length, object?[]? arguments = null) => printable.TryEstimatePrintLength(out length, format, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));
    /// <summary></summary>
    public bool TryEstimatePrintLength(out int length, IFormatProvider? formatProvider, object?[]? arguments = null) => printable.TryEstimatePrintLength(out length, formatProvider??format, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));

    /// <summary></summary>
    public bool TryPrintTo(Span<char> dst, out int length, object?[]? arguments = null) => printable.TryPrintTo(dst, out length, format, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));
    /// <summary></summary>
    public bool TryPrintTo(Span<char> dst, out int length, IFormatProvider? formatProvider, object?[]? arguments = null) => printable.TryPrintTo(dst, out length, formatProvider??format, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));

    /// <summary></summary>
    public void WriteTo(TextWriter textWriter, object?[]? arguments = null) => printable.WriteTo(textWriter, format, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));
    /// <summary></summary>
    public void WriteTo(TextWriter textWriter, IFormatProvider? formatProvider, object?[]? arguments = null) => printable.WriteTo(textWriter, formatProvider??format, LocalizedExtensions_.LocalizeArguments(arguments, culture, false));

    /// <summary></summary>
    public ITemplatePrintable Pluralize(object?[]? arguments) => printable.WithFormat(format);
    /// <summary></summary>
    public ITemplateText Pluralize(IFormatProvider? formatProvider, object?[]? arguments) => this;

    /// <summary></summary>
    public override string ToString() => printable?.ToString() ?? key;
}

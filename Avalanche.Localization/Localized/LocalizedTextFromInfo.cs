// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Avalanche.Template;
using Avalanche.Localization.Internal;
using System.Diagnostics.CodeAnalysis;
using Avalanche.Utilities;

/// <summary></summary>
public class LocalizedTextFromInfo : ILocalizedText, ITemplateBreakdown, ILocalized<ILocalizedText>, ITemplateEmplacementable
{
    /// <summary>Info</summary>
    public ILocalizationLinesInfo Info => info;
    /// <summary></summary>
    public IList<ILocalizationError> Errors { get => info.Errors ?? Array.Empty<ILocalizationError>(); set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string Key { get => info.Key; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string?[] ParameterNames { get => parameterNames ?? (parameterNames = info.Parameters.Select(pi => pi.Name).ToArray()); set => throw new InvalidOperationException(); }
    /// <summary>Parameter count</summary>
    public int ParameterCount => info.Parameters.Count;

    /// <summary></summary>
    public string Text { get => Default.Text; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplateFormat? TemplateFormat { get => Default.TemplateFormat; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplateBreakdown Breakdown { get => Default.Breakdown; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplatePart[] Parts { get => Default.Breakdown.Parts; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplatePlaceholderPart[] Placeholders { get => Default.Breakdown.Placeholders; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplateParameterPart?[] Parameters { get => Default.Breakdown.Parameters; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string Culture { get => Info.Culture; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public IFormatProvider Format { get => Info.Format; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ILocalizedText Value => this;

    /// <summary></summary>
    protected ITemplateText Default => info.Default.Breakdown;
    /// <summary>Info</summary>
    protected ILocalizationLinesInfo info;
    /// <summary>Assigned format provider</summary>
    protected IFormatProvider? formatProvider;
    /// <summary></summary>
    protected string?[]? parameterNames;

    /// <summary>Active format from the assigned <see cref="formatProvider"/>, and then fallback to <see cref="Culture"/>.</summary>
    public IFormatProvider ActiveFormat
    {
        get
        {
            // Get snapshot
            IFormatProvider? _format = formatProvider;
            // Got snapshot
            if (_format != null) return _format;
            // Get culture
            string? _culture = info?.Culture;
            // No culture
            if (_culture == null) return CultureInfo.InvariantCulture;
            // Get culture info
            _format = formatProvider = CultureInfo.GetCultureInfo(_culture);
            // Return
            return _format;
        }
    }

    /// <summary></summary>
    public LocalizedTextFromInfo(ILocalizationLinesInfo info, IFormatProvider? formatProvider = null)
    {
        this.info = info ?? throw new ArgumentNullException(nameof(info));
        this.formatProvider = formatProvider;
    }

    /// <summary></summary>
    public virtual bool TryPlace(ITemplateParameterEmplacement[] emplacements, [NotNullWhen(true)] out ITemplateText emplaced)
    {
        // Return this
        if (emplacements.Length == 0) { emplaced = this; return true; }
        // Create new text
        emplaced = new LocalizedTextWithEmplacements(this, emplacements);
        return true;
    }

    /// <summary></summary>
    public string Print(object?[]? arguments = null)
    {
        // Get format
        IFormatProvider _format = ActiveFormat;
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, Culture, false);
        // Choose variant
        ITemplateText text = info.Pluralize(_format, arguments);
        // Print
        string print = text.Print(_format, arguments);
        // Return
        return print;
    }

    /// <summary></summary>
    public void AppendTo(StringBuilder sb, object?[]? arguments = null)
    {
        // Get format
        IFormatProvider _format = ActiveFormat;
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, Culture, false);
        // Choose variant
        ITemplateText text = info.Pluralize(_format, arguments);
        // Append
        text.AppendTo(sb, _format, arguments);
    }

    /// <summary></summary>
    public bool TryEstimatePrintLength(out int length, object?[]? arguments = null)
    {
        // Get format
        IFormatProvider _format = ActiveFormat;
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, Culture, false);
        // Choose variant
        ITemplateText text = info.Pluralize(_format, arguments);
        // Estimate
        return text.TryEstimatePrintLength(out length, _format, arguments);
    }

    /// <summary></summary>
    public bool TryPrintTo(Span<char> dst, out int length, object?[]? arguments = null)
    {
        // Get format
        IFormatProvider _format = ActiveFormat;
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, Culture, false);
        // Choose variant
        ITemplateText text = info.Pluralize(_format, arguments);
        // Try print
        return text.TryPrintTo(dst, out length, _format, arguments);
    }

    /// <summary></summary>
    public void WriteTo(TextWriter textWriter, object?[]? arguments = null)
    {
        // Get format
        IFormatProvider _format = ActiveFormat;
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, Culture, false);
        // Choose variant
        ITemplateText text = info.Pluralize(_format, arguments);
        // Try print
        text.WriteTo(textWriter, _format, arguments);
    }

    /// <summary></summary>
    public string Print(IFormatProvider? formatProvider, object?[]? arguments = null)
    {
        // Get format
        IFormatProvider _format = formatProvider ?? ActiveFormat;
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, Culture, false);
        // Choose variant
        ITemplateText text = info.Pluralize(_format, arguments);
        // Print
        string print = text.Print(_format, arguments);
        // Return
        return print;
    }

    /// <summary></summary>
    public void AppendTo(StringBuilder sb, IFormatProvider? formatProvider, object?[]? arguments = null)
    {
        // Get format
        IFormatProvider _format = formatProvider ?? ActiveFormat;
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, Culture, false);
        // Choose variant
        ITemplateText text = info.Pluralize(_format, arguments);
        // Append
        text.AppendTo(sb, _format, arguments);
    }

    /// <summary></summary>
    public bool TryEstimatePrintLength(out int length, IFormatProvider? formatProvider, object?[]? arguments = null)
    {
        // Get format
        IFormatProvider _format = formatProvider ?? ActiveFormat;
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, Culture, false);
        // Choose variant
        ITemplateText text = info.Pluralize(_format, arguments);
        // Estimate
        return text.TryEstimatePrintLength(out length, _format, arguments);
    }

    /// <summary></summary>
    public bool TryPrintTo(Span<char> dst, out int length, IFormatProvider? formatProvider, object?[]? arguments = null)
    {
        // Get format
        IFormatProvider _format = formatProvider ?? ActiveFormat;
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, Culture, false);
        // Choose variant
        ITemplateText text = info.Pluralize(_format, arguments);
        // Try print
        return text.TryPrintTo(dst, out length, _format, arguments);
    }

    /// <summary></summary>
    public void WriteTo(TextWriter textWriter, IFormatProvider? formatProvider, object?[]? arguments = null)
    {
        // Get format
        IFormatProvider _format = formatProvider ?? ActiveFormat;
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, Culture, false);
        // Choose variant
        ITemplateText text = info.Pluralize(_format, arguments);
        // Try print
        text.WriteTo(textWriter, _format, arguments);
    }

    /// <summary></summary>
    public ITemplatePrintable Pluralize(object?[]? arguments)
    {
        // Get format
        IFormatProvider format = ActiveFormat;
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, Culture, false);
        // Get text
        ITemplateText text = info.Pluralize(format, arguments);
        // Decorate
        ITemplatePrintable printable = text.WithFormat(format);
        // Return
        return printable;
    }

    /// <summary></summary>
    public ITemplateText Pluralize(IFormatProvider? formatProvider, object?[]? arguments)
    {
        // Get format
        IFormatProvider format = formatProvider ?? ActiveFormat;
        // Localize arguments
        arguments = LocalizedExtensions_.LocalizeArguments(arguments, Culture, false);
        // Get text
        ITemplateText text = info.Pluralize(format, arguments);
        // Return
        return text;
    }

    /// <summary>Print information</summary>
    public override string ToString() => Default.Text ?? Key;
}

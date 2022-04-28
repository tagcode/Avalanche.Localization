// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;
using Avalanche.Template;
using Avalanche.Template.Internal;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

/// <summary></summary>
public class LocalizedTextWithEmplacements : ILocalizedText, ITemplateEmplacementable, ITemplateEmplaceable
{
    /// <summary></summary>
    public string Text { get => text.Text; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplateFormat? TemplateFormat { get => text.TemplateFormat; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplateBreakdown Breakdown { get => text.Breakdown; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string?[] ParameterNames { get => text.ParameterNames; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string Key { get => localizedText.Key; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public string Culture { get => culture; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public IFormatProvider Format { get => format; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public IList<ILocalizationError> Errors { get => errors; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    public ITemplateParameterEmplacement[]? Emplacements { get => emplacements; set => throw new InvalidOperationException(); }

    /// <summary>Parameter count</summary>
    protected int internalParameterCount;
    /// <summary>Highest number of emplacement parameters</summary>
    protected int highestEmplacementParameterCount;
    /// <summary></summary>
    protected string culture;
    /// <summary></summary>
    protected IFormatProvider format;
    /// <summary>Captured errors</summary>
    protected ILocalizationError[] errors;
    /// <summary></summary>
    protected ILocalizedText localizedText;
    /// <summary>Default text</summary>
    protected ITemplateText text;
    /// <summary>Assigned emplacements</summary>
    protected ITemplateParameterEmplacement[] emplacements;
    /// <summary>Parameter mappings</summary>
    protected TemplateEmplacementMapping emplacementMapping;
    /// <summary>Provider that provides pluralized+emplacement pluralized version. Key is array of emplacement texts, plus last index is pluralized version of .</summary>
    protected ConcurrentDictionaryCacheProvider<object[], ITemplateText> emplacementedProvider;

    /// <summary></summary>
    public LocalizedTextWithEmplacements(ILocalizedText localizedText, ITemplateParameterEmplacement[] emplacements)
    {
        // Assign text
        this.localizedText = localizedText ?? throw new ArgumentNullException(nameof(localizedText));
        // Get culture
        culture = localizedText.Culture;
        // Assign format
        format = localizedText.Format ?? (localizedText.Culture == null ? null : CultureInfo.GetCultureInfo(localizedText.Culture)) ?? CultureInfo.InvariantCulture;
        // Adjust emplacements to culture
        this.emplacements = LocalizationEmplacementExtensions.LocalizeParameterEmplacements(emplacements ?? throw new ArgumentNullException(nameof(emplacements)), culture);
        // Create parameter mapping
        emplacementMapping = TemplateEmplacementMapping.CreateEmplacementMapping(this.localizedText, this.emplacements);
        // Create cached provider that applies emplacements
        emplacementedProvider = Providers.Func<object[], ITemplateText>(ApplyEmplacements).Cached(new ArrayEqualityComparer<object>(ReferenceEqualityComparer<object>.Instance));
        // Create key for provider
        object[] emplacementTextsKey = this.emplacements.Select(e => e.Emplacement).Append(localizedText.Breakdown).ToArray();
        // Get default text and capture parameter map
        text = emplacementedProvider[emplacementTextsKey];
        // Capture errors
        StructList2<ILocalizationError> _errors = new(localizedText.Errors);
        foreach (var emplacement in emplacements)
            if (emplacement.Emplacement is ILocalizationErrorProvider errors)
                _errors.AddRange(errors.Errors);
        errors = _errors.ToArray();
        // Get highest emplacement argument count
        foreach (var emplacement in this.emplacements) highestEmplacementParameterCount = Math.Max(highestEmplacementParameterCount, emplacement.Emplacement.ParameterNames.Length);
        // Assign parameter count
        internalParameterCount = localizedText.ParameterNames.Length;
    }

    /// <summary>Apply <paramref name="emplacementTexts"/>.</summary>
    /// <remarks>This method is used in provider <see cref="emplacementedProvider"/></remarks>
    protected ITemplateText ApplyEmplacements(object[] emplacementTexts)
    {
        // Create new emplacement parameters
        ITemplateParameterEmplacement[] newEmplacements = new ITemplateParameterEmplacement[emplacements.Length];
        // Create text
        for (int i = 0; i < emplacements.Length; i++) newEmplacements[i] = new TemplateParameterEmplacement(emplacements[i].ParameterName, emplacementTexts[i] as ITemplateText ?? emplacements[i].Emplacement);
        // Get parent text
        ITemplateText parentText = (emplacementTexts[emplacementTexts.Length - 1] as ITemplateText) ?? this.localizedText;
        // Create new mapping
        TemplateEmplacementMapping newMapping = TemplateEmplacementMapping.CreateEmplacementMapping(parentText, newEmplacements);
        // 
        ITemplateText result = TemplateEmplacementExtensions.Compose(newMapping, parentText, newEmplacements);
        // 
        return result;
    }

    /// <summary></summary>
    public bool TryPlace(ITemplateParameterEmplacement[] emplacements, [NotNullWhen(true)] out ITemplateText emplaced)
    {
        // Return this
        if (emplacements.Length == 0) { emplaced = this; return true; }
        // Create new text
        emplaced = new LocalizedTextWithEmplacements(this, emplacements);
        return true;
    }

    /// <summary>Return pluralized and emplacement assigned text that has the same parameter signature as the default text.</summary>
    public ITemplateText Pluralize(IFormatProvider? formatProvider, object?[]? arguments)
    {
        // Allocate internal arguments
        var internalArguments = ArrayPool<object?>.Shared.Rent(internalParameterCount);
        // Allocate emplacement arguments
        var emplacementArguments = ArrayPool<object?>.Shared.Rent(highestEmplacementParameterCount);
        // Allocate external arguments
        var externalArguments = ArrayPool<object?>.Shared.Rent(arguments == null ? 0 : arguments.Length);
        // Allocate emplacement texts
        object[] emplacementTexts = new object[emplacementMapping.Emplacements.Count+1];
        //
        try
        {
            // Copy arguments
            for (int i = 0; i < externalArguments.Length; i++) externalArguments[i] = arguments == null || i >= arguments.Length ? null : arguments[i];
            // Localize arguments
            LocalizedExtensions_.LocalizeArguments(externalArguments, culture, true);
            // Print to internal arguments
            LocalizationEmplacementExtensions.PrintToInternalArguments(emplacementMapping, formatProvider, externalArguments, internalArguments, emplacementArguments, emplacementTexts, culture);
            // Pluralize with internal arguments
            var pluralized = localizedText.Pluralize(formatProvider, internalArguments);
            // Assign pluralized parent text to last index of emplacementTexts
            emplacementTexts[emplacementTexts.Length - 1] = pluralized;
            // Apply emplacements to pluralized version of text
            var pluralizedEmplacemented = emplacementedProvider[emplacementTexts];
            // Return
            return pluralizedEmplacemented;
        }
        finally
        {
            // Return arguments
            ArrayPool<object?>.Shared.Return(externalArguments);
            ArrayPool<object?>.Shared.Return(internalArguments);
            ArrayPool<object?>.Shared.Return(emplacementArguments);
        }
    }

    /// <summary></summary>
    public ITemplatePrintable Pluralize(object?[]? arguments)
    {
        // 
        var text = Pluralize(format, arguments);
        // 
        var printable = text.WithFormat(format);
        //
        return printable;
    }

    /// <summary></summary>
    public string Print(IFormatProvider? formatProvider, object?[]? arguments = null)
    {
        // Allocate internal arguments
        var internalArguments = ArrayPool<object?>.Shared.Rent(internalParameterCount);
        // Allocate emplacement arguments
        var emplacementArguments = ArrayPool<object?>.Shared.Rent(highestEmplacementParameterCount);
        // Allocate external arguments
        var externalArguments = ArrayPool<object?>.Shared.Rent(arguments == null ? 0 : arguments.Length);
        //
        try
        {
            // Copy arguments
            for (int i = 0; i < externalArguments.Length; i++) externalArguments[i] = arguments == null || i >= arguments.Length ? null : arguments[i];
            // Localize arguments
            LocalizedExtensions_.LocalizeArguments(externalArguments, culture, true);
            // Print to internal arguments
            LocalizationEmplacementExtensions.PrintToInternalArguments(emplacementMapping, formatProvider, externalArguments, internalArguments, emplacementArguments, null, culture);
            // Pluralize with internal arguments
            var pluralized = localizedText.Pluralize(formatProvider, internalArguments);
            // Print
            var print = pluralized.Print(formatProvider, internalArguments);
            // Return
            return print;
        }
        finally
        {
            // Return arguments
            ArrayPool<object?>.Shared.Return(externalArguments);
            ArrayPool<object?>.Shared.Return(internalArguments);
            ArrayPool<object?>.Shared.Return(emplacementArguments);
        }
    }

    /// <summary></summary>
    public string Print(object?[]? arguments = null)
    {
        // Print
        var print = Print(format, arguments);
        // Return
        return print;
    }

    /// <summary></summary>
    public void AppendTo(StringBuilder sb, IFormatProvider? formatProvider, object?[]? arguments = null)
    {
        // Allocate internal arguments
        var internalArguments = ArrayPool<object?>.Shared.Rent(internalParameterCount);
        // Allocate emplacement arguments
        var emplacementArguments = ArrayPool<object?>.Shared.Rent(highestEmplacementParameterCount);
        // Allocate external arguments
        var externalArguments = ArrayPool<object?>.Shared.Rent(arguments == null ? 0 : arguments.Length);
        //
        try
        {
            // Copy arguments
            for (int i = 0; i < externalArguments.Length; i++) externalArguments[i] = arguments == null || i >= arguments.Length ? null : arguments[i];
            // Localize arguments
            LocalizedExtensions_.LocalizeArguments(externalArguments, culture, true);
            // Print to internal arguments
            LocalizationEmplacementExtensions.PrintToInternalArguments(emplacementMapping, formatProvider, externalArguments, internalArguments, emplacementArguments, null, culture);
            // Pluralize with internal arguments
            var pluralized = localizedText.Pluralize(formatProvider, internalArguments);
            // Append
            pluralized.AppendTo(sb, formatProvider, internalArguments);
        }
        finally
        {
            // Return arguments
            ArrayPool<object?>.Shared.Return(externalArguments);
            ArrayPool<object?>.Shared.Return(internalArguments);
            ArrayPool<object?>.Shared.Return(emplacementArguments);
        }
    }

    /// <summary></summary>
    public void AppendTo(StringBuilder sb, object?[]? arguments = null)
    {
        // 
        AppendTo(sb, format, arguments);
    }

    /// <summary></summary>
    public bool TryEstimatePrintLength(out int length, IFormatProvider? formatProvider, object?[]? arguments = null)
    {
        // Allocate internal arguments
        var internalArguments = ArrayPool<object?>.Shared.Rent(internalParameterCount);
        // Allocate emplacement arguments
        var emplacementArguments = ArrayPool<object?>.Shared.Rent(highestEmplacementParameterCount);
        // Allocate external arguments
        var externalArguments = ArrayPool<object?>.Shared.Rent(arguments == null ? 0 : arguments.Length);
        //
        try
        {
            // Copy arguments
            for (int i = 0; i < externalArguments.Length; i++) externalArguments[i] = arguments == null || i >= arguments.Length ? null : arguments[i];
            // Localize arguments
            LocalizedExtensions_.LocalizeArguments(externalArguments, culture, true);
            // Print to internal arguments
            LocalizationEmplacementExtensions.PrintToInternalArguments(emplacementMapping, formatProvider, externalArguments, internalArguments, emplacementArguments, null, culture);
            // Pluralize with internal arguments
            var pluralized = localizedText.Pluralize(formatProvider, internalArguments);
            // Estimate
            var ok = pluralized.TryEstimatePrintLength(out length, formatProvider, internalArguments);
            // Done
            return ok;
        }
        finally
        {
            // Return arguments
            ArrayPool<object?>.Shared.Return(externalArguments);
            ArrayPool<object?>.Shared.Return(internalArguments);
            ArrayPool<object?>.Shared.Return(emplacementArguments);
        }
    }

    /// <summary></summary>
    public bool TryEstimatePrintLength(out int length, object?[]? arguments = null)
    {
        // 
        var ok = TryEstimatePrintLength(out length, format, arguments);
        // 
        return ok;
    }

    /// <summary></summary>
    public bool TryPrintTo(Span<char> dst, out int length, IFormatProvider? formatProvider, object?[]? arguments = null)
    {
        // Allocate internal arguments
        var internalArguments = ArrayPool<object?>.Shared.Rent(internalParameterCount);
        // Allocate emplacement arguments
        var emplacementArguments = ArrayPool<object?>.Shared.Rent(highestEmplacementParameterCount);
        // Allocate external arguments
        var externalArguments = ArrayPool<object?>.Shared.Rent(arguments == null ? 0 : arguments.Length);
        //
        try
        {
            // Copy arguments
            for (int i = 0; i < externalArguments.Length; i++) externalArguments[i] = arguments == null || i >= arguments.Length ? null : arguments[i];
            // Localize arguments
            LocalizedExtensions_.LocalizeArguments(externalArguments, culture, true);
            // Print to internal arguments
            LocalizationEmplacementExtensions.PrintToInternalArguments(emplacementMapping, formatProvider, externalArguments, internalArguments, emplacementArguments, null, culture);
            // Pluralize with internal arguments
            var pluralized = localizedText.Pluralize(formatProvider, internalArguments);
            // Try print
            var ok = pluralized.TryPrintTo(dst, out length, formatProvider, internalArguments);
            // Done
            return ok;
        }
        finally
        {
            // Return arguments
            ArrayPool<object?>.Shared.Return(externalArguments);
            ArrayPool<object?>.Shared.Return(internalArguments);
            ArrayPool<object?>.Shared.Return(emplacementArguments);
        }
    }

    /// <summary></summary>
    public bool TryPrintTo(Span<char> dst, out int length, object?[]? arguments = null)
    {
        // 
        var ok = TryPrintTo(dst, out length, format, arguments);
        //
        return ok;
    }

    /// <summary></summary>
    public void WriteTo(TextWriter textWriter, IFormatProvider? formatProvider, object?[]? arguments = null)
    {
        // Allocate internal arguments
        var internalArguments = ArrayPool<object?>.Shared.Rent(internalParameterCount);
        // Allocate emplacement arguments
        var emplacementArguments = ArrayPool<object?>.Shared.Rent(highestEmplacementParameterCount);
        // Allocate external arguments
        var externalArguments = ArrayPool<object?>.Shared.Rent(arguments == null ? 0 : arguments.Length);
        //
        try
        {
            // Copy arguments
            for (int i = 0; i < externalArguments.Length; i++) externalArguments[i] = arguments == null || i >= arguments.Length ? null : arguments[i];
            // Localize arguments
            LocalizedExtensions_.LocalizeArguments(externalArguments, culture, true);
            // Print to internal arguments
            LocalizationEmplacementExtensions.PrintToInternalArguments(emplacementMapping, formatProvider, externalArguments, internalArguments, emplacementArguments, null, culture);
            // Pluralize with internal arguments
            var pluralized = localizedText.Pluralize(formatProvider, internalArguments);
            // Return
            pluralized.WriteTo(textWriter, formatProvider, internalArguments);
        }
        finally
        {
            // Return arguments
            ArrayPool<object?>.Shared.Return(externalArguments);
            ArrayPool<object?>.Shared.Return(internalArguments);
            ArrayPool<object?>.Shared.Return(emplacementArguments);
        }
    }

    /// <summary></summary>
    public void WriteTo(TextWriter textWriter, object?[]? arguments = null)
    {
        // 
        WriteTo(textWriter, format, arguments);
    }

    /// <summary></summary>
    public override string ToString() => text.ToString()!;
}

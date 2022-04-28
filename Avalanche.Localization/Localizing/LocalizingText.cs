// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System;
using System.Diagnostics.CodeAnalysis;
using Avalanche.Template;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary></summary>
public class LocalizingText : LocalizableText, ILocalizingText, ITemplateEmplacementable
{
    /// <summary></summary>
    public ILocalized<ILocalizedText>? Value => Localize();
    /// <summary></summary>
    public ICultureProvider CultureProvider { get => this.cultureProvider ?? this; set => throw new InvalidOperationException(); }

    /// <summary></summary>
    public ILocalized<ILocalizedText>? Localize()
    {
        // Get culture
        (string culture, IFormatProvider format) = GetCultureSet();
        // Place here text
        ILocalizedText? text = null!;
        // Try get text
        if (textProvider1 != null) textProvider1.TryGetValue(((culture, format), Key), out text);
        // Try get text
        if (text == null && textProvider2 != null) textProvider2.TryGetValue((culture, Key), out text);
        // Revert to fallback default
        if (text == null) text = Default;
        // No text
        if (text == null) return null;
        // Wrap
        ILocalized<ILocalizedText> localized = Localized<ILocalizedText>.GetOrCreate(Key, culture, text);
        // Return
        return localized;
    }

    /// <summary></summary>
    public LocalizingText(
        string key,
        IProvider<((string culture, IFormatProvider formatProvider) culture, string key), ILocalizedText> textProvider,
        ICultureProvider cultureProvider,
        ITemplateText? defaultText = null,
        ITemplateParameterEmplacement[][]? emplacementSequences = null)
        : base(key, textProvider, cultureProvider ?? throw new ArgumentNullException(nameof(cultureProvider)), defaultText, emplacementSequences)
    {
    }

    /// <summary></summary>
    public LocalizingText(string key, IProvider<(string culture, string key),
        ILocalizedText> textProvider,
        ICultureProvider cultureProvider,
        ITemplateText? defaultText = null,
        ITemplateParameterEmplacement[][]? emplacementSequences = null)
        : base(key, textProvider, cultureProvider ?? throw new ArgumentNullException(nameof(cultureProvider)), defaultText, emplacementSequences)
    {
    }

    /// <summary></summary>
    public override bool TryPlace(ITemplateParameterEmplacement[] emplacements, [NotNullWhen(true)] out ITemplateText emplaced)
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
            new LocalizingText(key, textProvider1, cultureProvider!, _defaultText, _emplacementSequences) :
            new LocalizingText(key, textProvider2!, cultureProvider!, _defaultText, _emplacementSequences);
        // Done
        return true;
    }

}

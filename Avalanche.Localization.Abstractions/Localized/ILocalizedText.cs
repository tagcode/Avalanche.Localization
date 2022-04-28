// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;

// <docs>
/// <summary></summary>
public interface ILocalizedText : ILocalizationKeyProvider, ICultureProvider, ITemplatePrintable, ITemplateText, ILocalized
{
    /// <summary>Choose pluralized version. Each returned variation has same parameter signature.</summary>
    ITemplatePrintable Pluralize(object?[]? arguments);
    /// <summary>Choose pluralized version. Each returned variation has same parameter signature.</summary>
    ITemplateText Pluralize(IFormatProvider? formatProvider, object?[]? arguments);
}
// </docs>

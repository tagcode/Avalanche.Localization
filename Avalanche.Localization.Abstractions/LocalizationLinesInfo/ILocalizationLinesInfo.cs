// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;

// <docs>
/// <summary>Sum-up information about lines, parameters, plural cases. This is used by <see cref="ILocalizedText"/> implementations.</summary>
public interface ILocalizationLinesInfo : ILocalizationKeyProvider, ICultureProvider
{
    /// <summary>Default template text, the one where every pluralized parameter is "other" or with no pluralized assignments.</summary>
    ITemplateText Default { get; set; }
    /// <summary>Parameter infos</summary>
    IList<ILocalizationLinesParameter> Parameters { get; set; }
    /// <summary>Load and parse errors.</summary>
    IList<ILocalizationError> Errors { get; set; }

    /// <summary>Number of pluralized parameters</summary>
    int PluralizedParameterCount { get; set; }
    /// <summary>Plural permutations and texts</summary>
    IList<(PluralAssignment[], ITemplateText)>? Plurals { get; set; }
}
// </docs>

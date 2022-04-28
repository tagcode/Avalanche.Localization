// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;

// <docs>
/// <summary>Interface to list of localization errors.</summary>
public interface ILocalizationErrorProvider
{
    /// <summary>Associated Errors</summary>
    IList<ILocalizationError> Errors { get; set; }
}
// </docs>

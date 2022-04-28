// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;

/// <summary>
/// File pattern represents localization files, e.g. "Resources/{Culture}/{Key}".
/// 
/// Folllowing placeholders can be used: "Key", "Culture", "Namespace", "Name".
/// 
/// A pattern without "Culture" placeholder is applied when searching for invariant culture "".
/// </summary>
public interface ILocalizationFilePatterns
{
    /// <summary>Pattern, e.g. "Resources/{Culture}/{Key}".</summary>
    ITemplateFormatPrintable[] Patterns { get; set; }
}

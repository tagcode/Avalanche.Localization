// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;

// <docs>
/// <summary>Localization file format</summary>
public interface ILocalizationFileFormat
{
    /// <summary>Supported extensions, low case, with prefix dot. e.g. ".xml", ".yaml", ".json"</summary>
    string[] Extensions { get; }
}
// </docs>

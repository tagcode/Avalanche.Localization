// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;

// <docs>
/// <summary>Localization line file format</summary>
public interface ILocalizationLineFileFormat : ILocalizationFileFormat
{
    /// <summary>Read lines from <paramref name="stream"/>. Caller must close <paramref name="stream"/>.</summary>
    /// <returns>Returns lines which must be read completely before call returns.</returns>
    IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> Read(Stream stream);
}
// </docs>

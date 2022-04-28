// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Diagnostics.CodeAnalysis;

// <docs>
/// <summary>Localization resource, typically a file.</summary>
public interface ILocalizationFile : ILocalizationFileName, ILocalizationKeyProvider, ICultureProvider
{
    /// <summary>File format.</summary>
    ILocalizationFileFormat FileFormat { get; set; }

    /// <summary>Try open <paramref name="stream"/> to resource.</summary>
    /// <exception cref="Exception">On unexpected error.</exception>
    bool TryOpen([NotNullWhen(true)] out Stream? stream);
}
// </docs>

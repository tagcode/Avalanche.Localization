// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;

// <docs>
/// <summary>Interface for resources that have an assigned filename.</summary>
public interface ILocalizationFileName
{
    /// <summary>Resource file name, relative to file provider root, e.g. applicatin root</summary>
    string? FileName { get; set; }
}
// </docs>

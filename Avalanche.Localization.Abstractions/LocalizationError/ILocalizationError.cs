// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;

// <docs>
/// <summary>Localization error</summary>
public interface ILocalizationError
{
    /// <summary>Key</summary>
    string Key { get; set; }
    /// <summary>Culture</summary>
    string Culture { get; set; }
    /// <summary>Error message</summary>
    string Message { get; set; }
    /// <summary>The text span where the error occurs. Can have reference to source file, line, column, and position index.</summary>
    MarkedText Text { get; set; }
    /// <summary>Error code, see <see cref="LocalizationMessageIds"/>.</summary>
    int Code { get; set; }
}
// </docs>

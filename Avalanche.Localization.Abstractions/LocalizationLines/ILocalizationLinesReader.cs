// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;

// <docs>
/// <summary>Indicates that is reader of localization content. Used for Dependency Injection detection.</summary>
public interface ILocalizationLinesReader : IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> { }
// </docs>

// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;

// <docs>
/// <summary>Resource that is localized to specific culture.</summary>
public interface ILocalized : ILocalizationKeyProvider, ICultureProvider, ILocalizationErrorProvider
{
}

/// <summary>Resource that is localized to specific culture.</summary>
/// <typeparam name="T">Resource type</typeparam>
public interface ILocalized<out T> : ILocalized
{
    /// <summary>Resource value</summary>
    T Value { get; }
}
// </docs>

// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;

// <docs>
/// <summary>Indicates that implements sub-interface <see cref="ILocalizable{T}"/>.</summary>
public interface ILocalizable : ILocalizationKeyProvider { }

/// <summary>Localizable resource</summary>
/// <typeparam name="T">Resource type</typeparam>
public interface ILocalizable<out T> : ILocalizable
{
    /// <summary>Default value for invariant or fallback culture</summary>
    ILocalized<T>? Default { get; }
    /// <summary>Localize to <paramref name="culture"/> or return with fallback culture. If returns text, then each returned variation has same parameter signature.</summary>
    ILocalized<T>? Localize(string culture);
}
// </docs>

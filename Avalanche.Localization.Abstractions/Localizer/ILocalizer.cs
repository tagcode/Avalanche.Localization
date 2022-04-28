// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;

// <docs>
/// <summary>Base interface for localizer.</summary>
public interface ILocalizer
{
    /// <summary>Namespace prefix</summary>
    string? Namespace { get; set; }

    /// <summary>Resource Type</summary>
    Type ResourceType { get; }

    /// <summary>Applicable actice culture provider</summary>
    ICultureProvider CultureProvider { get; set; }

    /// <summary>
    /// Get localized resource for <paramref name="name"/>.
    /// 
    /// If <see cref="ILocalizer.Namespace"/> or <paramref name="name"/> is null, then counter part is used as key without separator '.'.
    /// </summary>
    ILocalized? this[string? name] { get; }
}

/// <summary><typeparamref name="T"/> localizer.</summary>
/// <typeparam name="T">Resource type</typeparam>
public interface ILocalizer<out T> : ILocalizer
{
    /// <summary>
    /// Get localized resource for <paramref name="name"/>. 
    /// 
    /// If <see cref="ILocalizer.Namespace"/> or <paramref name="name"/> is null, then counter part is used as key without separator '.'.
    /// </summary>
    new ILocalized<T>? this[string? name] { get; }
}

/// <summary><typeparamref name="T"/> for <typeparamref name="Namespace"/>.</summary>
/// <typeparam name="T">Resource type</typeparam>
/// <typeparam name="Namespace">Key namespace</typeparam>
public interface ILocalizer<T, Namespace> : ILocalizer<T> { }

/// <summary><typeparamref name="T"/> localizer for <typeparamref name="Namespace"/> using <typeparamref name="CultureProvider"/>.</summary>
/// <remarks>This interfaces is typically used with dependency injection.</remarks>
/// <typeparam name="T">Resource type</typeparam>
/// <typeparam name="Namespace">Key namespace</typeparam>
/// <typeparam name="CultureProvider">Culture provider type. Type must have a parameterless constructor.</typeparam>
public interface ILocalizer<T, Namespace, CultureProvider> : ILocalizer<T, Namespace> where CultureProvider : ICultureProvider { }
// </docs>

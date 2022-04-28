// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;

// <docs>
/// <summary>Resource that localizes to the current culture in an assigned <see cref="ICultureProvider"/>.</summary>
public interface ILocalizing : ILocalizationKeyProvider
{
    /// <summary>Culture and fallback cultures provider</summary>
    ICultureProvider CultureProvider { get; set; }
}

/// <summary>Resource that localizes to the current culture in an assigned <see cref="ICultureProvider"/>.</summary>
/// <typeparam name="T">Resource type</typeparam>
public interface ILocalizing<out T> : ILocalizing 
{ 
    /// <summary>Resource localized to culture from <see cref="ILocalizing.CultureProvider"/> or to default value.</summary>
    ILocalized<T>? Value { get; }
}
// </docs>

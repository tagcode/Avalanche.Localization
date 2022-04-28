// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Diagnostics.CodeAnalysis;

/// <summary>Extension methods for <see cref="ILocalizable{T}"/></summary>
public static class LocalizableExtensions
{
    /// <summary>Try to localized to <paramref name="culture"/> or its fallback value.</summary>
    public static bool TryLocalize<T>(this ILocalizable<T> localizable, string culture, [NotNullWhen(true)] out T value, out string localizedCulture)
    {
        // Get localized container
        ILocalized<T>? localized = localizable.Localize(culture);
        // No container
        if (localized == null) { value = default!; localizedCulture = null!;  return false; }
        // Get value
        value = localized.Value;
        localizedCulture = localized.Culture;
        // Return whether got value
        return value != null;
    }

    /// <summary>Try to localized to <paramref name="culture"/> but not to fallback value.</summary>
    public static bool TryLocalizeExact<T>(this ILocalizable<T> localizable, string culture, [NotNullWhen(true)] out T value)
    {
        // Get localized container
        ILocalized<T>? localized = localizable.Localize(culture);
        // No container
        if (localized == null) { value = default!; return false; }
        // Not requested culture
        if (localized.Culture != culture) { value = default!; return false; }
        // Get value
        value = localized.Value;
        // Return whether got value
        return value != null;
    }
}

// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;

/// <summary>Extension methods for <see cref="ICultureProvider"/></summary>
public static class CultureProviderExtensions
{
    /// <summary>Get as <see cref="System.Globalization.CultureInfo"/></summary>
    public static (string culture, IFormatProvider format) Set(this ICultureProvider cultureProvider) => cultureProvider == null ? (null!, null!) : (cultureProvider.Culture, cultureProvider.Format);
    /// <summary>Set culture</summary>
    public static C SetCulture<C>(this C cultureProvider, string culture) where C : ICultureProvider { cultureProvider.Culture = culture; return cultureProvider; }
    /// <summary>Set format provider</summary>
    public static C SetFormat<C>(this C cultureProvider, IFormatProvider format) where C : ICultureProvider { cultureProvider.Format = format; return cultureProvider; }

    /// <summary>Try get culture</summary>
    public static bool TryGetCulture(this ICultureProvider? cultureProvider, out string culture)
    {
        // No culture provider
        if (cultureProvider == null) { culture = null!; return false; }
        // Get language
        culture = cultureProvider.Culture;
        return culture != null;
    }

    /// <summary>Try get format</summary>
    public static bool TryGetFormat(this ICultureProvider? cultureProvider, out IFormatProvider format)
    {
        // No culture provider
        if (cultureProvider == null) { format = null!; return false; }
        // Get format
        format = cultureProvider.Format;
        return format != null;
    }
}

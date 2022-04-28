// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Diagnostics.CodeAnalysis;

/// <summary>Extension methods for <see cref="ILocalized{T}"/></summary>
public static class LocalizationErrorExtensions
{
    /// <summary>Test if list contains an error with <paramref name="code"/>.</summary>
    public static bool ContainsCode<Errors>(ref Errors errors, int code) where Errors : IList<ILocalizationError>
    {
        if (errors == null) return false;
        for (int i = 0; i < errors.Count; i++)
            if (errors[i].Code == code) return true;
        return false;
    }

    /// <summary>Test if list contains an error with <paramref name="code"/>.</summary>
    public static bool ContainsCode(this IList<ILocalizationError>? errors, int code)
    {
        if (errors == null) return false;
        for (int i = 0; i < errors.Count; i++)
            if (errors[i].Code == code) return true;
        return false;
    }

    /// <summary>Get first error by <paramref name="code"/>.</summary>
    public static bool TryGetByCode<Errors>(ref Errors errors, int code, [NotNullWhen(true)] out ILocalizationError? error) where Errors : IList<ILocalizationError>
    {
        if (errors == null) { error = null!; return false; }
        for (int i = 0; i < errors.Count; i++)
            if (errors[i].Code == code) { error = errors[i]; return true; }
        error = null;
        return false;
    }

    /// <summary>Get first error by <paramref name="code"/>.</summary>
    public static bool TryGetByCode(this IList<ILocalizationError>? errors, int code, [NotNullWhen(true)] out ILocalizationError? error)
    {
        if (errors == null) { error = null!; return false; }
        for (int i = 0; i < errors.Count; i++)
            if (errors[i].Code == code) { error = errors[i]; return true; }
        error = null;
        return false;
    }
}

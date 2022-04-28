// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;

/// <summary>Extension methods for <see cref="IPluralNumber"/>.</summary>
public static class PluralNumberExtensions
{
    /// <summary>Tests if <paramref name="number"/> exists in <paramref name="group"/>.</summary>
    public static bool InGroup(this IPluralNumber number, params IPluralNumber[] group)
    {
        //
        foreach (var value in group)
            if (number.Equals(value)) return true;
        //
        return false;
    }
}

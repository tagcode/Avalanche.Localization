// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Internal;
using System.Diagnostics.CodeAnalysis;

/// <summary>Compares by <see cref="ILocalizationError.Code"/>.</summary>
public class LocalizationErrorComparerByCode : IEqualityComparer<ILocalizationError>, IComparer<ILocalizationError>
{
    /// <summary></summary>
    static LocalizationErrorComparerByCode instance = new LocalizationErrorComparerByCode();
    /// <summary></summary>
    public static LocalizationErrorComparerByCode Instance => instance;

    /// <summary></summary>
    public int Compare(ILocalizationError? x, ILocalizationError? y) => (x == null ? 0 : x.Code) - (y == null ? 0 : y.Code);
    /// <summary></summary>
    public bool Equals(ILocalizationError? x, ILocalizationError? y) => (x == null ? 0 : x.Code) == (y == null ? 0 : y.Code);
    /// <summary></summary>
    public int GetHashCode([DisallowNull] ILocalizationError obj) => obj.Code;
}

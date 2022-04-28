// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;

/// <summary><see cref="IPluralRule"/> comparer</summary>
public class PluralRuleComparer : IEqualityComparer<IPluralRule>
{
    /// <summary>Singleton</summary>
    private static readonly IEqualityComparer<IPluralRule> instance = new PluralRuleComparer(PluralRuleInfo.Comparer.Default);
    /// <summary>Singleton</summary>
    public static IEqualityComparer<IPluralRule> Default => instance;

    /// <summary>Plural rule comparer</summary>
    public readonly IEqualityComparer<PluralRuleInfo> InfoComparer;

    /// <summary>Create comparer</summary>
    public PluralRuleComparer(IEqualityComparer<PluralRuleInfo> comparer)
    {
        InfoComparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
    }

    /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
    public bool Equals(IPluralRule? x, IPluralRule? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        // Forward to component
        return InfoComparer.Equals(x.Info, y.Info);
    }

    /// <summary>Calculate hashcode.</summary>
    public int GetHashCode(IPluralRule? obj) => obj == null ? 0 : InfoComparer.GetHashCode(obj.Info);
}

// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;

// <docs>
/// <summary>Plurality rule, evaluatable.</summary>
public interface IPluralRule
{
    /// <summary>Metadata info record.</summary>
    PluralRuleInfo Info { get; }

    /// <summary>Evaluate whether any of the associated rules apply to <paramref name="number"/>.</summary>
    /// <param name="number">numeric or text representation of numeric value</param>
    bool Evaluate<N>(N number) where N : IPluralNumber;
}
// </docs>

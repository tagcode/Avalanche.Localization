// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;

// <docs>
/// <summary>Evaluates numbers against rules.</summary>
public interface IPluralRulesEvaluator
{
    /// <summary>
    /// Evaluates <paramref name="number"/> against sub-set of rules.
    /// 
    /// First array elements are optional rules, and the last array element is mandatory rule.
    /// </summary>
    /// <param name="number">(optional) numeric and text representation of numberic value</param>
    /// <returns>Matching cases, first ones are optional, the last one is always mandatory (and only mandatory). Or null if evaluate failed.</returns>
    IPluralRule[]? Evaluate<N>(N number) where N : IPluralNumber;
}
// </docs>

// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using Avalanche.Utilities.Provider;

// <docs>
/// <summary>Table containing providers for plural rules. May contain one or multiple rulesets.</summary>
/// <example>
/// category=cardinal
///      case="zero":     "no cars"
///      case="one":      "a car"
///      case="other":    "{0} cars"
///      
/// category=ordinal
///      case="one":      "Take first exit out."
///      case="two":      "Take second exit out."
///      case="few":      "Take third exit out."
///      case="other":    "Take {0}th exit out."
/// </example>
public interface IPluralRules
{
    /// <summary>All rules</summary>
    IPluralRule[] AllRules { get; set; }

    /// <summary>Queries rules</summary>
    IProvider<PluralRuleInfo, IPluralRule[]> Rules { get; set; }
    /// <summary>Queries and caches rules</summary>
    IProvider<PluralRuleInfo, IPluralRule[]> RulesCached { get; set; }
    /// <summary>Creates rule evaluator</summary>
    IProvider<PluralRuleInfo, IPluralRulesEvaluator> Evaluator { get; set; }
    /// <summary>Creates and caches rule evaluator</summary>
    IProvider<PluralRuleInfo, IPluralRulesEvaluator> EvaluatorCached { get; set; }
}
// </docs>

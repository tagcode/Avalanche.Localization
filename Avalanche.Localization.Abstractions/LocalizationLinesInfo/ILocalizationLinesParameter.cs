// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Localization.Pluralization;

// <docs>
/// <summary>Information about localization text parameter.</summary>
public interface ILocalizationLinesParameter
{
    /// <summary>Parameter name</summary>
    string Name { get; set; }
    /// <summary>Parameter index</summary>
    int Index { get; set; }
    /// <summary>Parameter format</summary>
    string? Format { get; set; }
    /// <summary>Plural assignments</summary>
    IList<PluralRuleInfo> PluralRuleInfos { get; set; }
    /// <summary>Plural rule evaluators, should be 0 or 1 evaluator. May contain more if localization content is faulty, supplies to multiple categories, uses custom rules, e.g. "cardinal", "ordinal". All will be evaluated until matching rule is detected.</summary>
    IList<IPluralRulesEvaluator> PluralRuleEvaluators { get; set; }
}
// </docs>

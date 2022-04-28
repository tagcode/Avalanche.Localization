// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using System;
using System.Collections.Generic;
using System.Linq;
using Avalanche.Utilities;

/// <summary>
/// Indexed rules and evaluator for cases of one combination of { RuleSet,Culture,Category }.
/// 
/// This class evaluates an array of <see cref="IPluralRule"/> as a whole, and returns
/// all the cases - optional and required - that match the requested <see cref="IPluralNumber" />.
/// </summary>
public class PluralRulesEvaluator : ReadOnlyAssignableClass, IPluralRulesEvaluator
{
    /// <summary>Assigned rules</summary>
    protected IPluralRule[] allRules = null!;
    /// <summary>Assigned rules</summary>
    public IPluralRule[] AllRules { get => allRules; set => this.AssertWritable().setRules(value); }

    /// <summary>List of evaluatable cases in order of: 1. optional, 2. required.</summary>
    public IPluralRule[] EvaluatableCases = null!;
    /// <summary>Number of cases that are optional.</summary>
    public int OptionalCaseCount = 0;
    /// <summary>Number of permutations of optional cases: 2 ^ OptionalCaseCount - 1</summary>
    public int OptionalCasePermutationCount = 0;

    /// <summary>List of cases organized so that each required case forms a <see cref="Entry"/>. And each line has a preconfigured result array for each permutation of optional cases.</summary>
    Entry[] lines = null!;

    /// <summary>Reorders optional cases first then followed by non-optional. Also filters out non-<see cref="IPluralRulesEvaluator"/> rules.</summary>
    /// <exception cref="ArgumentException">If one of the cases doesn't implement <see cref="IPluralRule"></see></exception>
    public static IEnumerable<IPluralRule> ReorderAndFilter(IEnumerable<IPluralRule> rules)
    {
        // Add optional cases
        foreach (IPluralRule rule in rules)
        {
            if (rule.Info.Required.HasValue && !rule.Info.Required.Value && rule is IPluralRule)
                yield return rule;
        }

        // Add required cases
        foreach (IPluralRule rule in rules)
        {
            if (rule.Info.Required.HasValue && rule.Info.Required.Value && rule is IPluralRule) yield return rule;
        }
    }

    /// <summary></summary>
    protected virtual void setRules(IEnumerable<IPluralRule> rules)
    {
        var _rules = this.allRules = ReorderAndFilter(rules).ToArray();
        StructList12<IPluralRule> evaluatables = new StructList12<IPluralRule>();
        int firstNonOptionalCase = -1;
        for (int i = 0; i < _rules.Length; i++)
        {
            IPluralRule rule = _rules[i];
            if (rule is IPluralRule ce) evaluatables.Add(ce);
            bool isOptional = rule.Info.Required.HasValue && !rule.Info.Required.Value;
            if (!isOptional && firstNonOptionalCase < 0) firstNonOptionalCase = i;
        }
        this.EvaluatableCases = evaluatables.ToArray();
        this.OptionalCaseCount = firstNonOptionalCase<0 ? 0 : firstNonOptionalCase;
        if (OptionalCaseCount > 10) throw new ArgumentException($"Maximum number of optional cases is 10, got {OptionalCaseCount}");
        OptionalCasePermutationCount = (1 << OptionalCaseCount);

        // Add non-optional
        StructList12<IPluralRule> list = new StructList12<IPluralRule>();
        StructList12<Entry> lines = new StructList12<Entry>();
        for (int l = OptionalCaseCount; l < EvaluatableCases.Length; l++)
        {
            IPluralRule c = _rules[l];
            IPluralRule ce = EvaluatableCases[l];
            Entry line = new Entry { Evaluatable = ce };
            line.OptionalRulePermutations = (IPluralRule[][])Array.CreateInstance(typeof(IPluralRule[]), OptionalCasePermutationCount);
            for (int i = 0; i < OptionalCasePermutationCount; i++)
            {
                list.Clear();
                for (int j = 0; j < OptionalCaseCount; j++)
                    if ((i & (1 << j)) != 0) list.Add(_rules[j]);
                list.Add(c);
                line.OptionalRulePermutations[i] = list.ToArray();
            }
            lines.Add(line);
        }
        this.lines = lines.ToArray();
    }

    /// <summary>
    /// Create evaluatable rules from a list of cases.
    /// 
    /// Last case can be non-evaluatable (e.g. "other"). 
    /// It will be used as fallback result, if no evaluatable cases match.
    /// </summary>
    /// <param name="evaluatableRule">cases that implement <see cref="IPluralRule"></see></param>
    public PluralRulesEvaluator(params IPluralRule[] evaluatableRule) : base()
    {
        setRules(evaluatableRule);
    }

    /// <summary>
    /// Create evaluatable rules from a list of cases.
    /// 
    /// Last case can be non-evaluatable (e.g. "other"). 
    /// It will be used as fallback result, if no evaluatable cases match.
    /// </summary>
    /// <param name="evaluatableCases">cases that implement <see cref="IPluralRule"></see></param>
    public PluralRulesEvaluator(IEnumerable<IPluralRule> evaluatableCases) : base()
    {
        setRules(evaluatableCases);
    }

    /// <summary>Evaluate cases</summary>
    /// <param name="number"></param>
    /// <returns>matching cases. First ones are optional, last one is non-optional. Or null if none matched.</returns>
    public IPluralRule[]? Evaluate<N>(N number) where N : IPluralNumber
    {
        // Evaluate each optional cases
        int optionalCaseBits = 0;
        for (int i = 0; i < OptionalCaseCount; i++)
            if (EvaluatableCases[i].Evaluate(number)) optionalCaseBits |= 1 << i;

        // Evaluate required cases
        for (int i = 0; i < lines.Length; i++)
            // Evaluate required case
            if (lines[i].Evaluatable.Evaluate(number))
                // Return precalculated array
                return lines[i].OptionalRulePermutations[optionalCaseBits];

        // None matched
        return null;
    }

    /// <summary>
    /// List of cases organized so that each non-optional case forms a <see cref="Entry"/>.
    /// 
    /// And each line has a preconfigured result array for each permutation of optional cases.
    /// </summary>
    class Entry
    {
        /// <summary>Evaluatable, non-optional, rule.</summary>
        public IPluralRule Evaluatable = null!;

        /// <summary>
        /// List of case-result arrays for the result of <see cref="IPluralRulesEvaluator.Evaluate{N}(N)"/>.
        /// One result array for every permutation of optional cases.
        /// 
        /// The last element of the array is the required case.
        /// </summary>
        public IPluralRule[][] OptionalRulePermutations = null!;
    }
}



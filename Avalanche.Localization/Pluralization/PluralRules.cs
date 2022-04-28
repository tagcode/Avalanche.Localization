// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary></summary>
public class PluralRules : PluralRulesBase
{
    /// <summary>Provider that parses rule text, e.g. "[Category=cardinal,Culture=,Case=one,Required=True] i=1 and v=0"</summary>
    static readonly IProvider<ReadOnlyMemory<char>, IPluralRules> ruleParser = Providers.Func<ReadOnlyMemory<char>, IPluralRules>(TryConvertToRules);
    /// <summary>Provider that parses rule text, e.g. "[Category=cardinal,Culture=,Case=one,Required=True] i=1 and v=0"</summary>
    static readonly IProvider<ReadOnlyMemory<char>, IPluralRules> ruleParserCached = ruleParser.Cached();
    /// <summary>Provider that parses rule text, e.g. "[Category=cardinal,Culture=,Case=one,Required=True] i=1 and v=0"</summary>
    public static IProvider<ReadOnlyMemory<char>, IPluralRules> RuleParser => ruleParser;
    /// <summary>Provider that parses rule text, e.g. "[Category=cardinal,Culture=,Case=one,Required=True] i=1 and v=0"</summary>
    public static IProvider<ReadOnlyMemory<char>, IPluralRules> RuleParserCached => ruleParserCached;

    /// <summary>Rule query cache</summary>
    protected ConcurrentDictionary<PluralRuleInfo, ValueResult<IPluralRule[]>> ruleQueryCache = new(PluralRuleInfo.Comparer.Default);
    /// <summary>Rule evaluator cache</summary>
    protected ConcurrentDictionary<PluralRuleInfo, IPluralRulesEvaluator> ruleEvaluatorCache = new(PluralRuleInfo.Comparer.Default);
     
    /// <summary>Create rules</summary>
    public PluralRules() : base()
    {
        this.allRules = Array.Empty<IPluralRule>();
        this.rules = Providers.Func<PluralRuleInfo, IPluralRule[]>(TryQueryRules);
        this.rulesCached = this.rules.ValueResultCaptured().Cached(ruleQueryCache).ValueResultOpened();
        this.evaluator = Providers.Func<PluralRuleInfo, IPluralRulesEvaluator>(TryCreateEvaluator);
        this.evaluatorCached = Providers.Func<PluralRuleInfo, IPluralRulesEvaluator>(TryCreateEvaluatorCached).Cached(ruleEvaluatorCache);
    }

    /// <summary>Create rules</summary>
    /// <param name="rules">Rules to copy from</param>
    public PluralRules(IEnumerable<IPluralRule> rules) : this()
    {
        AllRules = rules.ToArray();
    }

    /// <summary>Create rules</summary>
    /// <param name="ruleSets">enumerable of rule sets to copy reference from</param>
    public PluralRules(params IEnumerable<IPluralRule>[] ruleSets) : this()
    {
        this.SetAllRules(ruleSets);
    }

    /// <summary>Handle cache invalidation.</summary>
    protected override void InvalidateCaches()
    {
        ruleEvaluatorCache.Clear();
        ruleQueryCache.Clear();
    }

    /// <summary>Try to find <paramref name="rules"/> that match <paramref name="query"/>.</summary>
    protected virtual bool TryQueryRules(PluralRuleInfo query, out IPluralRule[] rules)
    {
        // Get snapshot
        var _rules = this.AllRules;
        // Query rules
        if (_rules == null || !_rules.TryQuery(query, out rules)) { rules = null!; return false; }
        // 
        return true;
    }

    /// <summary>Try to create evaluator for <paramref name="info"/>.</summary>
    protected virtual bool TryCreateEvaluator(PluralRuleInfo info, out IPluralRulesEvaluator pluralRulesEvaluator)
    {
        // Get snapshot
        var _rules = this.Rules;
        // Query rules
        if (_rules == null || !_rules.TryGetValue(info, out IPluralRule[] rules)) { pluralRulesEvaluator = null!; return false; }
        // Create evaluator
        pluralRulesEvaluator = new PluralRulesEvaluator(rules).SetReadOnly();
        return true;
    }

    /// <summary>Try to create evaluator for <paramref name="info"/>. Uses cached rules.</summary>
    protected virtual bool TryCreateEvaluatorCached(PluralRuleInfo info, out IPluralRulesEvaluator pluralRulesEvaluator)
    {
        // Get snapshot
        var _rules = this.RulesCached;
        // Query rules
        if (_rules == null || !_rules.TryGetValue(info, out IPluralRule[] rules)) { pluralRulesEvaluator = null!; return false; }
        // Create evaluator
        pluralRulesEvaluator = new PluralRulesEvaluator(rules).SetReadOnly();
        return true;
    }

    /// <summary>Converts rule line. </summary>
    /// <param name="text">
    /// Rule line.
    /// 
    /// E.g. "[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one] one v = 0 and i % 10 = 1 @integer 0, 1, 2, 3, … @decimal 0.0~1.5, 10.0, …".
    /// 
    /// RuleInfo, expression and samples parts are all optional.
    /// </param>
    /// <returns>Expression</returns>
    /// <exception cref="InvalidOperationException">If could not tokenize all </exception>
    public static bool TryConvertToRules(ReadOnlyMemory<char> text, out IPluralRules rules)
    {
        //
        if (!PluralRuleExpressionParsers.TryConvertToRuleExpressions(text, out IPluralRuleExpression[] ruleExpressions)) { rules = null!; return false; }
        // 
        StructList6<IPluralRule> list = new();
        // 
        foreach (IPluralRuleExpression ruleExpression in ruleExpressions)
        {
            list.Add(new PluralRule.Expression(ruleExpression.Info, ruleExpression.Rule, ruleExpression.Samples));
        }
        // Return as rules
        rules = new PluralRules(list.ToArray());
        return true;
    }

}

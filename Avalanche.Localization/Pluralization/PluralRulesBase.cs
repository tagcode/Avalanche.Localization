// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using System.Collections;
using System.Linq;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Base implementation plural rules table. All properties are initialized as null.</summary>
public class PluralRulesBase : ReadOnlyAssignableClass, IPluralRules, IEnumerable<IPluralRule>
{
    /// <summary>Assigned rules</summary>
    protected IPluralRule[] allRules = null!;
    /// <summary>Assigned rules</summary>
    public virtual IPluralRule[] AllRules { get => allRules; set => this.AssertWritable().setAllRules(value); }

    /// <summary>Queries rules</summary>
    protected IProvider<PluralRuleInfo, IPluralRule[]> rules = null!;
    /// <summary>Queries and caches rules</summary>
    protected IProvider<PluralRuleInfo, IPluralRule[]> rulesCached = null!;
    /// <summary>Creates rule evaluator</summary>
    protected IProvider<PluralRuleInfo, IPluralRulesEvaluator> evaluator = null!;
    /// <summary>Creates and caches rule evaluator</summary>
    protected IProvider<PluralRuleInfo, IPluralRulesEvaluator> evaluatorCached = null!;

    /// <summary>Queries rules</summary>
    public virtual IProvider<PluralRuleInfo, IPluralRule[]> Rules { get => rules; set => this.AssertWritable().rules = value; }
    /// <summary>Queries and caches rules</summary>
    public virtual IProvider<PluralRuleInfo, IPluralRule[]> RulesCached { get => rulesCached; set => this.AssertWritable().rulesCached = value; }
    /// <summary>Creates rule evaluator</summary>
    public virtual IProvider<PluralRuleInfo, IPluralRulesEvaluator> Evaluator { get => evaluator; set => this.AssertWritable().evaluator = value; }
    /// <summary>Creates and caches rule evaluator</summary>
    public virtual IProvider<PluralRuleInfo, IPluralRulesEvaluator> EvaluatorCached { get => evaluatorCached; set => this.AssertWritable().evaluatorCached = value; }

    /// <summary>Create rules</summary>
    public PluralRulesBase() { }

    /// <summary>Assign <paramref name="ruleArray"/></summary>
    protected virtual void setAllRules(IPluralRule[] ruleArray)
    {
        this.allRules = ruleArray.ToArray();
        InvalidateCaches();
    }

    /// <summary>Handle cache invalidation.</summary>
    protected virtual void InvalidateCaches()
    {
    }

    /// <summary>Enumerator of rules</summary>
    public IEnumerator<IPluralRule> GetEnumerator() => ((IEnumerable<IPluralRule>)AllRules).GetEnumerator();
    /// <summary>Enumerator of rules</summary>
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)AllRules).GetEnumerator();
}

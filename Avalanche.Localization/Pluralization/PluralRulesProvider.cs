namespace Avalanche.Localization.Pluralization;
using System;
using Avalanche.Utilities.Provider;

/// <summary>Plural rules class loader</summary>
public class PluralRulesProvider : ProviderBase<PluralRuleInfo, IPluralRule[]>
{
    /// <summary>Singleton</summary>
    static readonly IProvider<PluralRuleInfo, IPluralRule[]> instance = new PluralRulesProvider(RuleSetProvider.Instance);
    /// <summary>Singleton</summary>
    static readonly IProvider<PluralRuleInfo, IPluralRule[]> cached = new PluralRulesProvider(RuleSetProvider.Cached);
    /// <summary>Singleton</summary>
    public static IProvider<PluralRuleInfo, IPluralRule[]> Instance => instance;
    /// <summary>Singleton</summary>
    public static IProvider<PluralRuleInfo, IPluralRule[]> Cached => cached;

    /// <summary></summary>
    protected IProvider<string, IPluralRules> rulesetProvider;

    /// <summary></summary>
    public PluralRulesProvider(IProvider<string, IPluralRules> rulesetProvider)
    {
        this.rulesetProvider = rulesetProvider ?? throw new ArgumentNullException(nameof(rulesetProvider));
    }

    /// <summary></summary>
    public override bool TryGetValue(PluralRuleInfo info, out IPluralRule[] rules)
    {
        //
        if (info.RuleSet == null || !rulesetProvider.TryGetValue(info.RuleSet, out IPluralRules pluralRules)) { rules = null!; return false; }
        // Get rules
        IProvider<PluralRuleInfo, IPluralRule[]>? provider = pluralRules.RulesCached ?? pluralRules.Rules;
        // No provider
        if (provider == null) { rules = null!; return false; }
        // Get rules
        if (provider.TryGetValue(info, out rules)) return true;
        // No rules
        rules = null!;
        return false;
    }
}

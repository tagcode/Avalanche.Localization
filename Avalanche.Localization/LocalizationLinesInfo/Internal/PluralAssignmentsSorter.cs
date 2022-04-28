// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Internal;
using Avalanche.Localization.Pluralization;
using Avalanche.Utilities.Provider;

/// <summary>Sorts plurals so that optional cases are first, "other" cases last and other cases in the middle.</summary>
public class PluralsSorter<T> : IComparer<T>
{
    /// <summary></summary>
    protected string? ruleSet;
    /// <summary></summary>
    protected string? culture;
    /// <summary></summary>
    protected IProvider<PluralRuleInfo, IPluralRule[]>? pluralRuleProvider;

    /// <summary>Selects plural assignments</summary>
    protected Func<T, PluralAssignment[]> pluralsSelector;

    /// <summary></summary>
    /// <param name="pluralsSelector">Selects plural assignments</param>
    public PluralsSorter(string? ruleSet, string? culture, IProvider<PluralRuleInfo, IPluralRule[]>? pluralRuleProvider, Func<T, PluralAssignment[]> pluralsSelector)
    {
        this.ruleSet = ruleSet;
        this.culture = culture;
        this.pluralRuleProvider = pluralRuleProvider;
        this.pluralsSelector = pluralsSelector ?? throw new ArgumentNullException(nameof(pluralsSelector));
    }

    /// <summary></summary>
    public int Compare(T? x_, T? y_)
    {
        // 
        long xscore = 0, yscore = 0;
        // 
        if (x_ == null && y_ == null) return 0;
        if (x_ == null) return -1;
        if (y_ == null) return 1;
        //
        PluralAssignment[] x = pluralsSelector(x_), y = pluralsSelector(y_);
        // 
        for (int i = 0; i < x.Length; i++)
        {
            // Get parameter assignment
            var pa = x[i];
            // Is required
            bool isRequired = true;
            //
            string? _culture = pa.culture ?? culture;
            // Estimate optionality
            if (pluralRuleProvider != null && ruleSet != null && _culture != null && pluralRuleProvider.TryGetValue(new PluralRuleInfo(ruleSet, pa.category, _culture, pa.@case, null), out IPluralRule[] rules))
            {
                foreach (IPluralRule rule in rules) if (rule.Info.Required.HasValue && !rule.Info.Required.Value) { isRequired = false; break; }
            }
            // 
            xscore += int.MaxValue;
            xscore += i;
            if (pa.@case == "other") xscore += 0x4000;
            if (isRequired) xscore += 0x8000;
        }
        // 
        for (int i = 0; i < y.Length; i++)
        {
            // Get parameter assignment
            var pa = y[i];
            // Is required
            bool isRequired = true;
            //
            string? _culture = pa.culture ?? culture;
            // Estimate optionality
            if (pluralRuleProvider != null && ruleSet != null && _culture != null && pluralRuleProvider.TryGetValue(new PluralRuleInfo(ruleSet, pa.category, _culture, pa.@case, null), out IPluralRule[] rules))
            {
                foreach (IPluralRule rule in rules) if (rule.Info.Required.HasValue && !rule.Info.Required.Value) { isRequired = false; break; }
            }
            // 
            yscore += int.MaxValue;
            yscore += i;
            if (pa.@case == "other") yscore += 0x4000;
            if (isRequired) yscore += 0x8000;
        }
        // 
        return (int)Math.Sign(xscore - yscore);
    }
}

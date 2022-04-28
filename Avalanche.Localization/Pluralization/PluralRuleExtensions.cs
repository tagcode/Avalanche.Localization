// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using System.Runtime.CompilerServices;
using Avalanche.Utilities;

/// <summary>Extension methods for <see cref="IPluralRule"/>.</summary>
public static partial class PluralRuleExtensions_
{
    /// <summary>Try query for rules</summary>
    /// <param name="filterCriteria">Filter for rules that must pass.</param>
    /// <param name="pluralRules">Rules. Enumerable will be enumerated multiple times.</param>
    /// <returns>Filtered rules</returns>
    public static bool TryQuery(this IEnumerable<IPluralRule> pluralRules, PluralRuleInfo filterCriteria, out IPluralRule[] rules)
    {
        // Return as is.
        if (filterCriteria.Equals(PluralRuleInfo.NoConstraints)) { rules = pluralRules is IPluralRule[] array ? array : pluralRules.ToArray(); return true; }
        // Search newest ruleset
        if (filterCriteria.RuleSet == PluralRuleInfo.NEWEST) filterCriteria = filterCriteria.ChangeRuleSet(pluralRules.NewestRuleSet());
        // Count matches
        int matchingRuleCount = 0;
        foreach (var rule in pluralRules)
            if (filterCriteria.FilterMatch(rule.Info)) matchingRuleCount++;
        // No matches
        if (matchingRuleCount == 0) { rules = Array.Empty<IPluralRule>(); return false; }
        // Allocate
        rules = new IPluralRule[matchingRuleCount];
        // Assign
        int i = 0;
        foreach (var rule in pluralRules)
            if (filterCriteria.FilterMatch(rule.Info))
                rules[i++] = rule;
        // Return
        return true;
    }

    /// <summary>Query for rules</summary>
    /// <param name="filterCriteria">Filter for rules that must pass.</param>
    /// <param name="pluralRules">Rules. Enumerable will be enumerated multiple times.</param>
    /// <returns>Filtered rules</returns>
    public static IPluralRule[] Query(this IEnumerable<IPluralRule> pluralRules, PluralRuleInfo filterCriteria)
    {
        // Return as is.
        if (filterCriteria.Equals(PluralRuleInfo.NoConstraints)) return pluralRules is IPluralRule[] array ? array : pluralRules.ToArray();
        // Search newest ruleset
        if (filterCriteria.RuleSet == PluralRuleInfo.NEWEST) filterCriteria = filterCriteria.ChangeRuleSet(pluralRules.NewestRuleSet());
        // Count matches
        int matchingRuleCount = 0;
        foreach (var rule in pluralRules)
            if (filterCriteria.FilterMatch(rule.Info)) matchingRuleCount++;
        // No matches
        if (matchingRuleCount == 0) return Array.Empty<IPluralRule>();
        // Allocate
        IPluralRule[] rules = new IPluralRule[matchingRuleCount];
        // Assign
        int i = 0;
        foreach (var rule in pluralRules)
            if (filterCriteria.FilterMatch(rule.Info))
                rules[i++] = rule;
        // Return
        return rules;
    }

    /// <summary>Create query filter</summary>
    /// <param name="ruleset">Rule set, e.g. "Unicode.CLDR35", "newest", or null (all)</param>
    /// <param name="category">Category, one of: "cardinal", "ordinal", "optional", null (all)</param>
    /// <param name="culture">"Culture, e.g. "en", "fi"</param>
    /// <param name="case">Case, e.g. "zero", "one", "few", "many", "other", null (all)</param>
    /// <param name="required">Optionality, 0=no, 1=yes, -1=unspecified</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IPluralRule[] Query(this IEnumerable<IPluralRule> pluralRules, string? ruleset, string? category, string? culture, string? @case, bool? required) 
        => Query(pluralRules, new PluralRuleInfo(ruleset, category, culture, @case, required));

    /// <summary>
    /// Evaluates <paramref name="number"/> against subset of <paramref name="rules"/>.
    /// 
    /// First results are optional, last entry is mandatory.
    /// </summary>
    /// <param name="rules"></param>
    /// <param name="subset">RuleSet, Culture and Category must have non-null value. "" is valid. Case must be null and optional must be -1.</param>
    /// <param name="number">(optional) numeric and text representation of numberic value</param>
    /// <returns>Matching cases, first ones are optional, the last one is always mandatory (and only mandatory). Or null if evaluate failed.</returns>
    public static IPluralRule[]? Evaluate(this IEnumerable<IPluralRule> rules, PluralRuleInfo subset, IPluralNumber number)
        => new PluralRulesEvaluator(rules.Query(subset)).Evaluate(number);

    /// <summary>Get name of newest rule set.</summary>
    /// <returns>Newest ruleset or null</returns>
    /// <remarks>
    /// Rule-set newness is compared with <see cref="AlphaNumericComparer.InvariantCultureIgnoreCase"/>, which compares numbers in one segment.
    /// Therefore string "Unicode.CLDR100" is newer than "Unicode.CLDR20". However ruleset "Zzz" is newer than "Unicode.CLDR"
    /// </remarks>
    public static string? NewestRuleSet(this IEnumerable<IPluralRule> pluralRules)
    {
        // Name of rulesets
        string? newestRuleSet = null;
        // Gather ruleset names
        foreach (var ruleset in pluralRules.RuleSets())
        {
            // Null
            if (string.IsNullOrEmpty(ruleset)) continue;
            // First
            if (newestRuleSet == null) { newestRuleSet = ruleset; continue; }
            // This ruleset is older than the assigned
            if (AlphaNumericComparer.InvariantCultureIgnoreCase.Compare(ruleset, newestRuleSet) < 0) continue;
            // Assign
            newestRuleSet = ruleset;
        }
        //
        return newestRuleSet;
    }

    /// <summary>Get all rulesets</summary>
    public static string[] RuleSets(this IEnumerable<IPluralRule> rules)
    {
        // No rules
        if (rules == null) return Array.Empty<string>();
        // Gather here rulesets
        StructList4<string> list = new();
        // Gather
        foreach (var rule in rules)
        {
            // Get value
            string? value = rule.Info.RuleSet;
            // Add if new
            if (!string.IsNullOrEmpty(value)) list.AddIfNew(value);
        }
        // Return
        return list.ToArray();
    }

    /// <summary>Get all categories</summary>
    public static string[] Categories(this IEnumerable<IPluralRule> rules)
    {
        // No rules
        if (rules == null) return Array.Empty<string>();
        // Gather here rulesets
        StructList4<string> list = new();
        // Gather
        foreach (var rule in rules)
        {
            // Get value
            string? value = rule.Info.Category;
            // Add if new
            if (!string.IsNullOrEmpty(value)) list.AddIfNew(value);
        }
        // Return
        return list.ToArray();
    }

    /// <summary>Get all cultures</summary>
    public static string[] Cultures(this IEnumerable<IPluralRule> rules)
    {
        // No rules
        if (rules == null) return Array.Empty<string>();
        // Gather here rulesets
        StructList4<string> list = new();
        // Gather
        foreach (var rule in rules)
        {
            // Get value
            string? value = rule.Info.Culture;
            // Add if new
            if (!string.IsNullOrEmpty(value)) list.AddIfNew(value);
        }
        // Return
        return list.ToArray();
    }

    /// <summary>Get all cases</summary>
    public static string[] Cases(this IEnumerable<IPluralRule> rules)
    {
        // No rules
        if (rules == null) return Array.Empty<string>();
        // Gather here rulesets
        StructList4<string> list = new();
        // Gather
        foreach (var rule in rules)
        {
            // Get value
            string? value = rule.Info.Case;
            // Add if new
            if (!string.IsNullOrEmpty(value)) list.AddIfNew(value);
        }
        // Return
        return list.ToArray();
    }

}


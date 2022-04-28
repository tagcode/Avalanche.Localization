// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;

/// <summary>Extension methods for <see cref="IPluralRules"/>.</summary>
public static partial class PluralRuleExtensions
{
    /// <summary>Copy from <paramref name="ruleSource"/> and assign into <paramref name="rules"/>.</summary>
    public static T SetAllRules<T>(this T rules, IEnumerable<IPluralRule> ruleSource) where T : IPluralRules
    {
        // Assign rules
        rules.AllRules = ruleSource.ToArray();
        // Return
        return rules;
    }

    /// <summary>Copy from <paramref name="ruleSets"/> and assign into <paramref name="rules"/>.</summary>
    public static T SetAllRules<T>(this T rules, params IEnumerable<IPluralRule>[] ruleSets) where T : IPluralRules
    {
        // List
        List<IPluralRule> list = new();
        // Add all rules
        foreach (IEnumerable<IPluralRule> ruleSet in ruleSets) list.AddRange(ruleSet);
        // Assign rules
        rules.AllRules = list.ToArray();
        // Return
        return rules;
    }

}


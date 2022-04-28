// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using Avalanche.Utilities;

/// <summary>Extension methods for <see cref="PluralRuleInfo"/>.</summary>
public static partial class PluralRuleInfoExtensions_
{
    /// <summary>List cultures</summary>
    public static string[] Culture(this IEnumerable<PluralRuleInfo> infos)
    {
        // Place here categories
        StructList4<string> list = new();
        // Add categories
        foreach(PluralRuleInfo info in infos) if (info.Culture != null) list.AddIfNew(info.Culture);
        // Return
        return list.ToArray();
    }

    /// <summary>List categories</summary>
    public static string[] Categories(this IEnumerable<PluralRuleInfo> infos)
    {
        // Place here categories
        StructList4<string> list = new();
        // Add categories
        foreach(PluralRuleInfo info in infos) if (info.Category != null) list.AddIfNew(info.Category);
        // Return
        return list.ToArray();
    }

    /// <summary>List categories</summary>
    public static string[] RuleSets(this IEnumerable<PluralRuleInfo> infos)
    {
        // Place here 
        StructList4<string> list = new();
        // Add 
        foreach(PluralRuleInfo info in infos) if (info.RuleSet != null) list.AddIfNew(info.RuleSet);
        // Return
        return list.ToArray();
    }
}


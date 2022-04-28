// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;

/// <summary>CLDR based plural rules</summary>
public interface ICLDRPluralRules : IPluralRules
{
    /// <summary>RuleSet name</summary>
    string RuleSet { get; set; }
    /// <summary>CLDR Version</summary>
    int Version { get; set; }
    /// <summary>Rule count</summary>
    int RuleCount { get; set; }
}

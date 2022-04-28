// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using Avalanche.Utilities;

/// <summary>Plural rules record</summary>
public class CLDRPluralRules : PluralRules, ICLDRPluralRules
{
    /// <summary>Latest rules</summary>
    public static IPluralRules Instance => CLDR41PluralRules.Instance;

    /// <summary></summary>
    protected string ruleSet = null!;
    /// <summary></summary>
    protected int version;
    /// <summary></summary>
    protected int ruleCount;

    /// <summary></summary>
    public virtual string RuleSet { get => ruleSet; set => this.AssertWritable().ruleSet = value; }
    /// <summary></summary>
    public virtual int Version { get => version; set => this.AssertWritable().version = value; }
    /// <summary></summary>
    public virtual int RuleCount { get => ruleCount; set => this.AssertWritable().ruleCount = value; }

    /// <summary></summary>
    public CLDRPluralRules() : base() { }
    /// <summary></summary>
    public CLDRPluralRules(string ruleSet, int version, int ruleCount) : base() 
    {
        this.RuleSet = ruleSet;
        this.Version = version;
        this.RuleCount = ruleCount;
    }
}


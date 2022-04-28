// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using System;
using System.Linq;
using System.Text;

/// <summary>Plural rule</summary>
public class PluralRuleExpression : Avalanche.Localization.Pluralization.Expression, IPluralRuleExpression
{
    /// <summary>Rule infos. E.g. "[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one]"</summary>
    public PluralRuleInfo Info { get; set; }
    /// <summary></summary>
    public IExpression? Rule { get; set; }
    /// <summary></summary>
    public ISamplesExpression[]? Samples { get; set; }
    /// <summary></summary>
    public int ComponentCount => (Samples == null ? 0 : Samples.Length);
    /// <summary></summary>
    public IExpression GetComponent(int ix) => Samples![ix];

    /// <summary></summary>
    public PluralRuleExpression(PluralRuleInfo info, IExpression rule, params ISamplesExpression[] samples)
    {
        Info = info;
        Rule = rule;
        Samples = samples;
    }

    /// <summary></summary>
    public override void Append(StringBuilder sb) => new PluralRuleExpressionStringPrinter(sb).Append(this);
}

/// <summary>List of plural rule samples.</summary>
public class SamplesExpression : Avalanche.Localization.Pluralization.Expression, ISamplesExpression
{
    /// <summary></summary>
    public string Name { get; set; }
    /// <summary></summary>
    public IExpression[] Samples { get; set; }
    /// <summary></summary>
    public int ComponentCount => Samples == null ? 0 : Samples.Length;
    /// <summary></summary>
    public IExpression GetComponent(int ix) => Samples[ix];

    /// <summary></summary>
    public static SamplesExpression Create(string name, params Object[] samples) => new SamplesExpression(name, samples.Select(s => new ConstantExpression(s)).ToArray());

    /// <summary></summary>
    public SamplesExpression(string name, params IExpression[] samples)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Samples = samples ?? throw new ArgumentNullException(nameof(samples));
    }

    /// <summary></summary>
    public override void Append(StringBuilder sb) => new PluralRuleExpressionStringPrinter(sb).Append(this);
}

/// <summary>Marks sample sequence as infinite. "…" or "..."</summary>
public class InfiniteExpression : Avalanche.Localization.Pluralization.Expression, IInfiniteExpression
{
    /// <summary></summary>
    public override void Append(StringBuilder sb)
    {
        sb.Append('…');
    }
}

/// <summary>Range expression.</summary>
public class RangeExpression : Avalanche.Localization.Pluralization.Expression, IRangeExpression
{
    /// <summary></summary>
    public int ComponentCount => 2;
    /// <summary></summary>
    public IExpression GetComponent(int ix) => ix == 0 ? MinValue : ix == 1 ? MaxValue : null!;
    /// <summary></summary>
    public IExpression MinValue { get; internal set; }
    /// <summary></summary>
    public IExpression MaxValue { get; internal set; }

    /// <summary>Create range expression</summary>
    public RangeExpression(IExpression minValue, IExpression maxValue)
    {
        MinValue = minValue ?? throw new ArgumentNullException(nameof(minValue));
        MaxValue = maxValue ?? throw new ArgumentNullException(nameof(maxValue));
    }

    /// <summary></summary>
    public override void Append(StringBuilder sb) => new PluralRuleExpressionStringPrinter(sb).Append(this);
}

/// <summary>Group expression</summary>
public class GroupExpression : Avalanche.Localization.Pluralization.Expression, IGroupExpression
{
    /// <summary></summary>
    public IExpression[] Values { get; internal set; }
    /// <summary></summary>
    public int ComponentCount => Values == null ? 0 : Values.Length;
    /// <summary></summary>
    public IExpression GetComponent(int ix) => Values[ix];

    /// <summary>Create group</summary>
    public GroupExpression(params IExpression[] values)
    {
        Values = values ?? throw new ArgumentNullException(nameof(values));
    }

    /// <summary></summary>
    public override void Append(StringBuilder sb) => new PluralRuleExpressionStringPrinter(sb).Append(this);
}



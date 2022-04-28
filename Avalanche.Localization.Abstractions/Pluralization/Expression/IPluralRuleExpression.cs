// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;

/// <summary>
/// Plural rule expression.
/// 
/// e.g. "[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one] v = 0 and i % 10 = 1 @integer 0, 1, 2, 3, … @decimal 0.0~1.5, 10.0, …".
/// </summary>
public interface IPluralRuleExpression : ICompositeExpression
{
    /// <summary>Rule infos. E.g. "[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one]"</summary>
    PluralRuleInfo Info { get; set; }
    /// <summary>Rule expression that can evaluate a number</summary>
    IExpression? Rule { get; set; }
    /// <summary>Samples</summary>
    ISamplesExpression[]? Samples { get; set; }
}

/// <summary>Samples e.g. "@decimal 1.0, 1.00, 1.000, 1.0000"</summary>
public interface ISamplesExpression : ICompositeExpression
{
    /// <summary>Name of sample group, e.g. "integer" and "decimal". </summary>
    String Name { get; set; }

    /// <summary>Each value is one of: <see cref="IConstantExpression"/>, <see cref="IRangeExpression"/>, <see cref="IInfiniteExpression"/>
    /// 
    /// If list ends with <see cref="IInfiniteExpression"/> then there are infinite possible values.
    /// If not, then all the possible samples are listed in the samples list.
    /// </summary>
    IExpression[] Samples { get; set; }
}

/// <summary>Indicates that list is infinite. "…" or "..."</summary>
public interface IInfiniteExpression : IExpression { }

/// <summary>Expression for multiple values, <see cref="IRangeExpression"/> and <see cref="IGroupExpression"/>.</summary>
public interface IValuesExpression : IExpression { }

/// <summary>Range of interger values.</summary>
public interface IRangeExpression : IValuesExpression, ICompositeExpression
{
    /// <summary>Start of range (inclusive)</summary>
    IExpression MinValue { get; }
    /// <summary>End of range (inclusive)</summary>
    IExpression MaxValue { get; }
}

/// <summary>Group of values.</summary>
public interface IGroupExpression : IValuesExpression, ICompositeExpression
{
    /// <summary>Values.</summary>
    IExpression[] Values { get; }
}


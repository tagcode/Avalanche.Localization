// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using System.Collections;
using System.Text;

/// <summary>Prints <see cref="IPluralRuleExpression"/> as string.</summary>
public abstract class PluralRuleExpressionPrinter
{
    /// <summary>String builder</summary>
    public readonly StringBuilder sb;

    /// <summary>Create printer</summary>
    public PluralRuleExpressionPrinter(StringBuilder? sb = default)
    {
        this.sb = sb ?? new StringBuilder();
    }

    /// <summary>Print as string</summary>
    public override string ToString() => sb.ToString();

    /// <summary>Append string</summary>
    public PluralRuleExpressionPrinter Append(string? str)
    {
        if (str != null) sb.Append(str);
        return this;
    }

    /// <summary>Append char</summary>
    public PluralRuleExpressionPrinter Append(char ch)
    {
        sb.Append(ch);
        return this;
    }

    /// <summary>Append List</summary>
    public PluralRuleExpressionPrinter Append(IEnumerable? enumr, string separator)
    {
        if (enumr == null) return this;
        int c = 0;
        IEnumerator etor = enumr.GetEnumerator();
        while (etor.MoveNext())
        {
            if (c++ > 0) sb.Append(separator);
            if (etor.Current is IExpression exp) Append(exp);
            else Append(etor.Current?.ToString());
        }
        return this;
    }

    /// <summary>Append expression</summary>
    public abstract PluralRuleExpressionPrinter Append(IExpression? exp, string? postSeparator = null);

}

/// <summary>
/// Prints <see cref="IPluralRuleExpression"/> as expression string.
/// 
/// Uses unicode <see href="https://www.unicode.org/reports/tr35/tr35-numbers.html#Plural_rules_syntax"/> notation,
/// with some additional features. 
/// 
/// The rule info is added in brackets "[RuleSet=Unicode.CLDR35,Category=cardinal,Case=zero,Culture=fi,Optional=1]".
/// </summary>
public class PluralRuleExpressionStringPrinter : PluralRuleExpressionPrinter
{
    /// <summary>Create printer</summary>
    public PluralRuleExpressionStringPrinter(StringBuilder? sb = default) : base(sb) { }

    /// <summary>Append expression</summary>
    public override PluralRuleExpressionPrinter Append(IExpression? exp, string? postSeparator = null)
    {
        if (exp == null) return this;
        var x = exp switch
        {
            IPluralRuleExpression pre => Append("[").Append(pre.Info.ToString()).Append("] ").Append(pre.Rule, " ").Append(pre.Samples, " "),
            ISamplesExpression samples => (Append("@" + samples.Name + " ") as PluralRuleExpressionStringPrinter)!.AppendSamples(samples.Samples, ", "),
            IRangeExpression range => Append(range.MinValue).Append("..").Append(range.MaxValue),
            IGroupExpression group => Append(group.Values, ","),
            IInfiniteExpression inf => Append('…'),
            IConstantExpression c => Append(c.Value?.ToString()),
            IArgumentNameExpression arg => Append(arg.Name),
            IParenthesisExpression par => Append('(').Append(par.Element).Append(')'),
            IUnaryOpExpression uop => Append(uop.Op switch { UnaryOp.Plus => "+", UnaryOp.Not => "not ", UnaryOp.OnesComplement => "~", UnaryOp.Negate => "-", _ => "¤" }).Append(uop.Element),
            IBinaryOpExpression bop =>
                Append(bop.Left).Append(bop.Op switch
                {
                    BinaryOp.Add => "+",
                    BinaryOp.And => " and ",
                    BinaryOp.Divide => "/",
                    BinaryOp.Equal => "=",
                    BinaryOp.GreaterThan => ">",
                    BinaryOp.GreaterThanOrEqual => ">=",
                    BinaryOp.LeftShift => "<<",
                    BinaryOp.RightShift => ">>",
                    BinaryOp.LessThan => "<",
                    BinaryOp.LessThanOrEqual => "<=",
                    BinaryOp.LogicalAnd => " and ",
                    BinaryOp.LogicalOr => " or ",
                    BinaryOp.Modulo => " % ",
                    BinaryOp.Multiply => "*",
                    BinaryOp.NotEqual => "!=",
                    BinaryOp.Or => " or ",
                    BinaryOp.Power => " pow ",
                    BinaryOp.Xor => "^",
                    BinaryOp.Subtract => "-",
                    BinaryOp.Coalesce => "??",
                    _ => "¤"
                }).Append(bop.Right),
            _ => this
        };
        if (postSeparator != null) sb.Append(postSeparator);
        return this;
    }

    /// <summary>Append List</summary>
    public PluralRuleExpressionPrinter AppendSamples(IEnumerable enumr, string separator)
    {
        if (enumr == null) return this;
        int c = 0;
        IEnumerator etor = enumr.GetEnumerator();
        while (etor.MoveNext())
        {
            if (c++ > 0) sb.Append(separator);
            if (etor.Current is IExpression exp) AppendSample(exp);
            else Append(etor.Current.ToString());
        }
        return this;
    }

    /// <summary>Append sample</summary>
    public PluralRuleExpressionPrinter AppendSample(IExpression exp)
    {
        if (exp == null) return this;
        var x = exp switch
        {
            ISamplesExpression samples => Append("@" + samples.Name + " ").Append(samples.Samples, ", "),
            IRangeExpression range => Append(range.MinValue).Append("~").Append(range.MaxValue),
            IGroupExpression group => Append(group.Values, ", "),
            IInfiniteExpression inf => Append('…'),
            IConstantExpression c => Append(c.Value?.ToString() ?? ""),
            _ => this
        };
        return this;
    }
}


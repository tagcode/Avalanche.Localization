// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using System;
using System.Collections;
using Avalanche.Utilities;

/// <summary>Calculates <see cref="IPluralRuleExpression"/> hashcode so that strings produce consistent values across sessions.</summary>
public static class PluralRuleExpressionHashCode
{
    /// <summary></summary>
    public static ulong Code(IExpression e)
    {
        FNVHash64 hash = new FNVHash64();
        hash = PluralRuleExpressionHashCode.HashIn(hash, e);
        return hash.Hash;
    }

    /// <summary>Hash in</summary>
    public static FNVHash64 HashIn(this FNVHash64 hashcode, PluralRuleInfo info)
    {
        hashcode.HashIn(info.GetHashCode());
        return hashcode;
    }

    /// <summary>Hash in List</summary>
    public static FNVHash64 HashIn(this FNVHash64 hashcode, IEnumerable? enumr)
    {
        if (enumr == null) return hashcode;
        IEnumerator etor = enumr.GetEnumerator();
        while (etor.MoveNext())
        {
            object o = etor.Current;
            if (o == null) hashcode.HashIn(0);
            else if (o is PluralRuleInfo info) hashcode.HashIn(info.GetHashCode());
            else if (o is IExpression exp) hashcode = HashIn(hashcode, exp);
            else if (o is String str) hashcode.HashIn(str);
            else if (o is int _int) hashcode.HashIn(_int);
            else hashcode.HashIn(o.GetHashCode());
        }
        return hashcode;
    }

    /// <summary>Hash in expression</summary>
    public static FNVHash64 HashIn(this FNVHash64 hashcode, IExpression? exp)
    {
        if (exp == null) return hashcode;
        if (exp is IConstantExpression c)
        {
            hashcode.HashIn(nameof(IConstantExpression));
            hashcode.HashIn(c.Value.ToString());
        }
        else if (exp is IPluralRuleExpression pre)
        {
            hashcode.HashIn(nameof(IConstantExpression));
            hashcode = hashcode.HashIn(pre.Rule);
            if (pre.Samples != null) hashcode = hashcode.HashIn(pre.Samples);
        }
        else if (exp is ISamplesExpression samples)
        {
            hashcode.HashIn(nameof(ISamplesExpression));
            hashcode.HashIn(samples.Name);
            if (samples.Samples != null) hashcode = HashIn(hashcode, samples.Samples);
            return hashcode;
        }
        else if (exp is IRangeExpression range)
        {
            hashcode.HashIn(nameof(IRangeExpression));
            hashcode = HashIn(hashcode, range.MinValue);
            hashcode = HashIn(hashcode, range.MaxValue);
        }
        else if (exp is IGroupExpression group)
        {
            hashcode.HashIn(nameof(IGroupExpression));
            hashcode = HashIn(hashcode, group.Values);
        }
        else if (exp is IInfiniteExpression inf)
        {
            hashcode.HashIn('…');
        }
        else if (exp is IArgumentNameExpression arg)
        {
            hashcode.HashIn(nameof(IArgumentNameExpression));
            hashcode.HashIn(arg.Name);
        }
        else if (exp is IParenthesisExpression par)
        {
            hashcode.HashIn(nameof(IParenthesisExpression));
            hashcode = HashIn(hashcode, par.Element);
        }
        else if (exp is IUnaryOpExpression uop)
        {
            hashcode.HashIn(nameof(IUnaryOpExpression));
            hashcode.HashIn((int)uop.Op);
            hashcode = HashIn(hashcode, uop.Element);
        }
        else if (exp is IBinaryOpExpression bop)
        {
            hashcode.HashIn(nameof(IBinaryOpExpression));
            hashcode.HashIn((int)bop.Op);
            hashcode = HashIn(hashcode, bop.Left);
            hashcode = HashIn(hashcode, bop.Right);
        }
        else if (exp is ITrinaryOpExpression top)
        {
            hashcode.HashIn(nameof(ITrinaryOpExpression));
            hashcode.HashIn((int)top.Op);
            hashcode = HashIn(hashcode, top.A);
            hashcode = HashIn(hashcode, top.B);
            hashcode = HashIn(hashcode, top.C);
        }
        else throw new NotSupportedException(exp.ToString());
        return hashcode;
    }
}



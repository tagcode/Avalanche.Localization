﻿// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using System.Text;

/// <summary>Expression base class</summary>
public abstract class Expression : IExpression
{
    /// <summary>Print information</summary>
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        Append(sb);
        return sb.ToString();
    }

    /// <summary>Append information to <paramref name="sb"/> for debugging. Does not produce parseable expression. </summary>
    public abstract void Append(StringBuilder sb);

    /// <summary>Append <paramref name="exp"/> to <paramref name="sb"/>.</summary>
    public static void AppendExp(StringBuilder sb, IExpression exp)
    {
        if (exp is Expression _exp) _exp.Append(sb); else sb.Append(exp.ToString());
    }
}

/// <summary>Unary operation expression</summary>
public class UnaryOpExpression : Expression, IUnaryOpExpression
{
    /// <summary></summary>
    public UnaryOp Op { get; internal set; }
    /// <summary> </summary>
    public IExpression Element { get; internal set; }
    /// <summary></summary>
    public int ComponentCount => 1;
    /// <summary></summary>
    public IExpression GetComponent(int ix) => ix == 0 ? Element : throw new IndexOutOfRangeException();

    /// <summary>Create unary operator expression</summary>
    public UnaryOpExpression(Avalanche.Localization.Pluralization.UnaryOp op, IExpression component)
    {
        Op = op;
        Element = component ?? throw new ArgumentNullException(nameof(component));
    }

    /// <summary>Print information</summary>
    public override void Append(StringBuilder sb)
    {
        sb.Append(Op switch { UnaryOp.Negate => "-", UnaryOp.Not => "!", UnaryOp.OnesComplement => "~", UnaryOp.Plus => "+", _ => "??" });
        AppendExp(sb, Element);
    }
}

/// <summary>Binary operation expression</summary>
public class BinaryOpExpression : Expression, IBinaryOpExpression
{
    /// <summary></summary>
    public BinaryOp Op { get; internal set; }
    /// <summary></summary>
    public IExpression Left { get; internal set; }
    /// <summary></summary>
    public IExpression Right { get; internal set; }
    /// <summary></summary>
    public int ComponentCount => 2;
    /// <summary></summary>
    public IExpression GetComponent(int ix) => ix switch { 0 => Left, 1 => Right, _ => throw new IndexOutOfRangeException() };

    /// <summary>Create expression</summary>
    public BinaryOpExpression(Avalanche.Localization.Pluralization.BinaryOp op, IExpression left, IExpression right)
    {
        Op = op;
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    /// <summary>Print information</summary>
    public override void Append(StringBuilder sb)
    {
        AppendExp(sb, Left);
        sb.Append(' ');
        sb.Append(Op switch
        {
            BinaryOp.And => "&",
            BinaryOp.Or => "|",
            BinaryOp.LogicalAnd => "&&",
            BinaryOp.LogicalOr => "||",
            BinaryOp.Divide => "/",
            BinaryOp.Equal => "=",
            BinaryOp.Xor => "^",
            BinaryOp.Add => "+",
            BinaryOp.GreaterThan => ">",
            BinaryOp.GreaterThanOrEqual => ">=",
            BinaryOp.In => "=",
            BinaryOp.LeftShift => "<<",
            BinaryOp.LessThan => "<",
            BinaryOp.LessThanOrEqual => "<=",
            BinaryOp.Modulo => "%",
            BinaryOp.Multiply => "*",
            BinaryOp.NotEqual => "!=",
            BinaryOp.Power => "^",
            BinaryOp.RightShift => ">>",
            BinaryOp.Subtract => "-",
            BinaryOp.Coalesce => "??",
            _ => throw new NotImplementedException($"{nameof(BinaryOpExpression)}: {Op} is not implemented")
        });
        sb.Append(' ');
        AppendExp(sb, Right);
    }
}

/// <summary>Trinary operation expression</summary>
public class TrinaryOpExpression : Expression, ITrinaryOpExpression
{
    /// <summary></summary>
    public TrinaryOp Op { get; internal set; }
    /// <summary></summary>
    public IExpression A { get; internal set; }
    /// <summary></summary>
    public IExpression B { get; internal set; }
    /// <summary></summary>
    public IExpression C { get; internal set; }
    /// <summary></summary>
    public int ComponentCount => 3;
    /// <summary></summary>
    public IExpression GetComponent(int ix) => ix switch { 0 => A, 1 => B, 2 => C, _ => throw new IndexOutOfRangeException() };

    /// <summary>Create expression</summary>
    public TrinaryOpExpression(TrinaryOp op, IExpression a, IExpression b, IExpression c)
    {
        Op = op;
        A = a ?? throw new ArgumentNullException(nameof(a));
        B = b ?? throw new ArgumentNullException(nameof(b));
        C = c ?? throw new ArgumentNullException(nameof(c));
    }

    /// <summary>Print information</summary>
    public override void Append(StringBuilder sb)
    {
        AppendExp(sb, A);
        sb.Append(' ');
        sb.Append(Op switch
        {
            TrinaryOp.Condition => "?",
            _ => throw new NotImplementedException($"{nameof(TrinaryOpExpression)}: {Op} is not implemented")
        });
        sb.Append(' ');
        AppendExp(sb, B);
        sb.Append(' ');
        sb.Append(Op switch
        {
            TrinaryOp.Condition => ":",
            _ => throw new NotImplementedException($"{nameof(TrinaryOpExpression)}: {Op} is not implemented")
        });
        sb.Append(' ');
        AppendExp(sb, C);
    }
}

/// <summary>Argument name</summary>
public class ArgumentNameExpression : Expression, IArgumentNameExpression
{
    /// <summary></summary>
    public string Name { get; internal set; }

    /// <summary>Create expression</summary>
    public ArgumentNameExpression(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <summary>Print information</summary>
    public override void Append(StringBuilder sb)
    {
        sb.Append(Name);
    }
}

/// <summary>Argument index reference expression</summary>
public class ArgumentIndexExpression : Expression, IArgumentIndexExpression
{
    /// <summary></summary>
    public int Index { get; internal set; }

    /// <summary>Create expression</summary>
    public ArgumentIndexExpression(int index)
    {
        Index = index;
    }

    /// <summary>Print information</summary>
    public override void Append(StringBuilder sb)
    {
        sb.Append("#");
        sb.Append(Index);
    }
}

/// <summary>Function call</summary>
public class CallExpression : Expression, ICallExpression
{
    /// <summary>Function Name</summary>
    public String Name { get; internal set; }
    /// <summary>Function arguments</summary>
    public IExpression[] Args { get; internal set; }
    /// <summary></summary>
    public int ComponentCount => Args == null ? 0 : Args.Length;
    /// <summary></summary>
    public IExpression GetComponent(int ix) => Args[ix];

    /// <summary>Create function call expression</summary>
    public CallExpression(string name, params IExpression[] args)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Args = args ?? throw new ArgumentNullException(nameof(args));
    }

    /// <summary>Print information</summary>
    public override void Append(StringBuilder sb)
    {
        sb.Append(Name);
        sb.Append('(');
        for (int i = 0; i < Args.Length; i++)
        {
            if (i > 0) sb.Append(", ");
            AppendExp(sb, Args[i]);
        }
        sb.Append(')');
    }
}

/// <summary>Constant value</summary>
public class ConstantExpression : Expression, IConstantExpression
{
    /// <summary></summary>
    public object Value { get; internal set; }

    /// <summary></summary>
    public ConstantExpression(object value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>Print information</summary>
    public override void Append(StringBuilder sb)
    {
        sb.Append(Value);
    }
}

/// <summary>Parenthesis expression.</summary>
public class ParenthesisExpression : Expression, IParenthesisExpression
{
    /// <summary></summary>
    public IExpression Element { get; internal set; }
    /// <summary></summary>
    public int ComponentCount => 1;
    /// <summary></summary>
    public IExpression GetComponent(int ix) => ix == 0 ? Element : throw new IndexOutOfRangeException();

    /// <summary>Create parenthesis expression</summary>
    public ParenthesisExpression(IExpression element)
    {
        this.Element = element ?? throw new ArgumentNullException(nameof(element));
    }

    /// <summary>Print information</summary>
    public override void Append(StringBuilder sb)
    {
        sb.Append('(');
        AppendExp(sb, Element);
        sb.Append(')');
    }
}


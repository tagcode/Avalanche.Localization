// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;

/// <summary>Root expression</summary>
public interface IExpression { }

/// <summary>Interface that exposes component expressions.</summary>
public interface ICompositeExpression : IExpression
{
    /// <summary>Number of component expressions.</summary>
    int ComponentCount { get; }
    /// <summary>Get component expression.</summary>
    IExpression? GetComponent(int ix);
}

/// <summary>Parenthesis expression "(exp)"</summary>
public interface IParenthesisExpression : ICompositeExpression
{
    /// <summary>Inner expression</summary>
    IExpression Element { get; }
}

/// <summary>Unary +, -, ~, !</summary>
public enum UnaryOp
{
    /// <summary>+</summary>
    Plus,
    /// <summary>-</summary>
    Negate,
    /// <summary>~</summary>
    OnesComplement,
    /// <summary>!</summary>
    Not
};

/// <summary>Binary A o B</summary>
public enum BinaryOp
{
    // Arithmetic operands
    /// <summary>+</summary>
    Add,
    /// <summary>-</summary>
    Subtract,
    /// <summary>*</summary>
    Multiply,
    /// <summary>/</summary>
    Divide,
    /// <summary>%</summary>
    Modulo,
    /// <summary>pow</summary>
    Power,
    /// <summary>&amp;</summary>
    And,
    /// <summary>|</summary>
    Or,
    /// <summary>^</summary>
    Xor,

    // Logical operands
    /// <summary>&amp;&amp;</summary>
    LogicalAnd,
    /// <summary>||</summary>
    LogicalOr,

    // Other operands
    /// <summary>&lt;&lt;</summary>
    LeftShift,
    /// <summary>&gt;&gt;</summary>
    RightShift,

    // Comparison operands
    /// <summary>&lt;</summary>
    LessThan,
    /// <summary>&gt;</summary>
    GreaterThan,
    /// <summary>&lt;=</summary>
    LessThanOrEqual,
    /// <summary>&gt;=</summary>
    GreaterThanOrEqual,
    /// <summary>!=</summary>
    NotEqual,
    /// <summary>==</summary>
    Equal,
    /// <summary>??</summary>
    Coalesce,

    /// <summary>=, in group (right side) comparer</summary>
    In,
};

/// <summary>A o B o C</summary>
public enum TrinaryOp
{
    /// <summary>?: condition operator, e.g. "condition_exp ? true_exp : false_exp"</summary>
    Condition
}

/// <summary>Unary expression: <see cref="Op"/> <see cref="Element"/>, e.g. "!false"</summary>
public interface IUnaryOpExpression : ICompositeExpression
{
    /// <summary>Operand</summary>
    UnaryOp Op { get; }
    /// <summary>Element expression</summary>
    IExpression Element { get; }
}

/// <summary>Binary expression: <see cref="Left"/> <see cref="Op"/> <see cref="Right"/>, e.g. "1+2"</summary>
public interface IBinaryOpExpression : ICompositeExpression
{
    /// <summary>Operand</summary>
    BinaryOp Op { get; }
    /// <summary>Left side</summary>
    IExpression Left { get; }
    /// <summary>Right side</summary>
    IExpression Right { get; }
}

/// <summary>Trinary expression: <see cref="A"/> <see cref="Op"/> <see cref="B"/> <see cref="C"/>, e.g. ""b ? x : y"</summary>
public interface ITrinaryOpExpression : ICompositeExpression
{
    /// <summary>Operand</summary>
    TrinaryOp Op { get; }
    /// <summary>A element</summary>
    IExpression A { get; }
    /// <summary>B element</summary>
    IExpression B { get; }
    /// <summary>C element</summary>
    IExpression C { get; }
}

/// <summary>Argument by name, e.g. "arg0"</summary>
public interface IArgumentNameExpression : IExpression
{
    /// <summary>Name of the argument</summary>
    string Name { get; }
}

/// <summary>Argument by index, e.g. "0"</summary>
public interface IArgumentIndexExpression : IExpression
{
    /// <summary>Index of the argument</summary>
    int Index { get; }
}

/// <summary>Function call expression</summary>
public interface ICallExpression : ICompositeExpression
{
    /// <summary>Function name</summary>
    String Name { get; }
    /// <summary>Function arguments</summary>
    IExpression[] Args { get; }
}

/// <summary>Literal constant, e.g. "Hello"</summary>
public interface IConstantExpression : IExpression
{
    /// <summary>Literal as <see cref="String"/>, <see cref="int"/>, etc.</summary>
    object Value { get; }
}


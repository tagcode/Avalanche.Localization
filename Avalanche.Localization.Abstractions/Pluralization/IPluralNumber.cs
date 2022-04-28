// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;

// <docs>
/// <summary>
/// Interface for numbers that have decimal string representation or truncated approximation. 
/// Base 2, 8 and 16 numbers are used aswell.
/// 
/// This interface is for extracting features for the purpose of language string declinations, not mathematical operations.
/// For example numbers "1.1" and "1.10" are not equal in linquistic sense.
/// 
/// Following features are extracted:
/// <list type="bullet">
///   <item>sign</item>
///   <item>integer digits</item>
///   <item>fraction digits</item>
///   <item>exponent sign and digits</item>
///   <item>exponent sign and digits</item>
/// </list>
/// /// 
/// The IEnumerable&lt;char&gt; returns characters in canonicalized form with the following rules:
/// <list type="bullet">
///   <item>Starts with '-' if is negative</item>
///   <item>Decimal separator is '.'</item>
///   <item>There is no whitespaces</item>
///   <item>Exponent is unwrapped so that is adjusts the location of decimal separator.</item>
///   <item>Hexdecimals are in capital 'A' to 'F'</item>
/// </list>
/// ToString() also returns the same canonical characters.
/// </summary>
public interface IPluralNumber : IEnumerable<char>
{
    /// <summary>Has a value. If value is NaN, returns false.</summary>
    bool HasValue { get; }
    /// <summary>Has decimal fractions. For example "0.10" is true, but "0.00" is false</summary>
    bool IsFloat { get; }
    /// <summary>Sign of the value, -1, 0 or 1.</summary>
    /// <remarks>If sign is 0, then text represents zero value.</remarks>
    int Sign { get; }
    /// <summary>Base of digits, 2, 8, 10 or 16.</summary>
    int Base { get; }

    /// <summary>Absolute positive value characters. Excludes '-' and preceding and trailing zeros '000'.</summary>
    IPluralNumber N { get; }
    /// <summary>Integer digits.</summary>
    IPluralNumber I { get; }
    /// <summary>Exponent digits.</summary>
    IPluralNumber E { get; }
    /// <summary>Fraction digits including trailing zeros in the open format (exponent removed).</summary>
    IPluralNumber F { get; }
    /// <summary>Fraction digits excluding trailing zeros in the open format (exponent removed).</summary>
    IPluralNumber T { get; }
    /// <summary>Fraction digit count including trailing zeroes  in the open format (exponent removed).</summary>
    IPluralNumber V { get; }
    /// <summary>Fraction digit count excluding trailing zeros in the open format (exponent removed).</summary>
    IPluralNumber W { get; }

    /// <summary>Integer digits.</summary>
    int I_Digits { get; }
    /// <summary>Exponent digits</summary>
    int E_Digits { get; }
    /// <summary>Fraction digits including trailing zeroes.</summary>
    int F_Digits { get; }
    /// <summary>Fraction digits excluding trailing zeros.</summary>
    int T_Digits { get; }

    /// <summary>Calculate modulo.</summary>
    /// <returns>modulo</returns>
    IPluralNumber Modulo(int modulo);

    /// <summary>
    /// Try to read as long.
    /// 
    /// If value doesn't fit or has floating-point fractions, then false.
    /// If value is floating-point but fractions are zero "1.00", then returns true provided it fits long.
    /// </summary>
    bool TryGet(out long value);

    /// <summary>
    /// Try to read as decimal.
    /// 
    /// If value doesn't fit decimal, then returns false.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGet(out decimal value);

    /// <summary>
    /// Try to read as double.
    /// 
    /// If value doesn't fit double, then returns false.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGet(out double value);

    /// <summary>
    /// Try to read as big integer.
    /// 
    /// If value doesn't fit or has floating-point fractions, then 
    /// If value is floating-point but fractions are zero "1.00", then returns true provided it fits long.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGet(out System.Numerics.BigInteger value);
}
// </docs>

// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

/// <summary>Double number</summary>
public struct DoubleNumber : IDecimalNumber
{
    /// <summary>Double value.</summary>
    public readonly double Value;
    /// <summary></summary>
    double truncate;

    /// <summary>Value as text</summary>
    TextNumber? text;
    /// <summary>Text representation of the value.</summary>
    public TextNumber AsText => text.HasValue ? text.Value : (text = new TextNumber(Value.ToString(CultureInfo.InvariantCulture).AsMemory(), CultureInfo.InvariantCulture, NumberStyles.Integer)).Value;

    /// <summary>Create double number</summary>
    public DoubleNumber(double value, TextNumber? text = default!)
    {
        this.Value = value;
        this.text = text;
        this.truncate = Math.Truncate(Value);
    }

    /// <summary>Has a value. If value is NaN, returns false.</summary>
    public bool HasValue => true;
    /// <summary>Has decimal fractions. For example "0.10" is true, but "0.00" is false</summary>
    public bool IsFloat => Value != truncate;
    /// <summary>Sign of the value, -1, 0 or 1.</summary>
    public int Sign => Value < 0d ? -1 : Value == 0d ? 0 : 1;
    /// <summary>Base of digits, 2, 8, 10 or 16.</summary>
    public int Base => 10;
    /// <summary>Absolute (positive) value of the source number.</summary>
    public IPluralNumber N { get { if (Value < 0d) { TextNumber _text = AsText; return new DoubleNumber(-Value, new TextNumber(_text.N_, _text.formatProvider, _text.numberStyle, _text.NumberBase)); } return this; } }
    /// <summary>Integer digits.</summary>
    public IPluralNumber I => AsText.I;
    /// <summary>Exponent digits.</summary>
    public IPluralNumber E => AsText.E;
    /// <summary>Fraction digits including trailing zeros in the open format (exponent removed).</summary>
    public IPluralNumber F => AsText.F;
    /// <summary>Fraction digits excluding trailing zeros in the open format (exponent removed).</summary>
    public IPluralNumber T => AsText.T;
    /// <summary>Fraction digit count including trailing zeroes  in the open format (exponent removed).</summary>
    public IPluralNumber V => AsText.V;
    /// <summary>Fraction digit count excluding trailing zeros in the open format (exponent removed).</summary>
    public IPluralNumber W => AsText.W;

    /// <summary>Number of integer digits.</summary>
    public int I_Digits => AsText.I_Digits;
    /// <summary>Number of exponent digits</summary>
    public int E_Digits => AsText.E_Digits;
    /// <summary>Fraction digits including trailing zeroes. Corresponds to 'v' attribute in Unicode CLDR plural.xml if exponent is removed.</summary>
    public int F_Digits => AsText.F_Digits;
    /// <summary>Fraction digits excluding trailing zeros. Corresponds to 'w' attribute in Unicode CLDR plural.xml if exponent is removed.</summary>
    public int T_Digits => AsText.T_Digits;

    /// <summary>Calculate modulo.</summary>
    /// <returns>modulo or null if failed to calculate modulo</returns>
    public IPluralNumber Modulo(int modulo) => new DoubleNumber(Value % modulo);

    /// <summary></summary>
    public bool TryGet(out long value)
    {
        long _value = unchecked((long)Value);
        double _backTo = unchecked((double)_value);
        if (_backTo != Value) { value = default; return false; }
        value = _value;
        return true;
    }
    /// <summary></summary>
    public bool TryGet(out decimal value)
    {
        decimal _value = unchecked((decimal)Value);
        double _backTo = unchecked((double)_value);
        if (_backTo != Value) { value = default; return false; }
        value = _value;
        return true;
    }
    /// <summary></summary>
    public bool TryGet(out double value)
    {
        value = Value;
        return true;
    }
    /// <summary></summary>
    public bool TryGet(out System.Numerics.BigInteger value)
    {
        if (IsFloat) { value = default; return false; }
        value = (System.Numerics.BigInteger)Value;
        return true;
    }
    /// <summary></summary>
    public IEnumerator<char> GetEnumerator() => AsText.GetEnumerator();
    /// <summary></summary>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    /// <summary></summary>
    public override string ToString() => AsText.ToString();
    /// <summary>Cached hash-code</summary>
    int? hashcode = default;
    /// <summary></summary>
    public override int GetHashCode() => hashcode.HasValue ? hashcode.Value : (hashcode = PluralNumberComparer.Default.GetHashCode(this)).Value;
    /// <summary></summary>
    public override bool Equals(object? obj)
    {
        if (obj is IPluralNumber number) return PluralNumberComparer.Default.Equals(this, number);
        return false;
    }
}

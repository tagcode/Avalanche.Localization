// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

/// <summary>ULong value</summary>
public struct ULongNumber : IDecimalNumber
{
    /// <summary>Zero singleton</summary>
    static readonly IPluralNumber _zero = new ULongNumber(0L, new TextNumber("0".AsMemory(), numberStyles: NumberStyles.Integer));
    /// <summary>Zero constant</summary>
    public static IPluralNumber Zero => _zero;

    /// <summary>ULong value.</summary>
    public readonly ulong Value;
    /// <summary>Value as text</summary>
    TextNumber? text;
    /// <summary>Text representation of the value.</summary>
    public TextNumber AsText => text.HasValue ? text.Value : (text = new TextNumber(Value.ToString(CultureInfo.InvariantCulture).AsMemory(), CultureInfo.InvariantCulture, NumberStyles.Integer)).Value;

    /// <summary>Has a value. If value is NaN, returns false.</summary>
    public bool HasValue => true;
    /// <summary>Has decimal fractions. For example "0.10" is true, but "0.00" is false</summary>
    public bool IsFloat => false;
    /// <summary>Sign of the value, -1, 0 or 1.</summary>
    public int Sign => Value < 0L ? -1 : Value == 0L ? 0 : 1;
    /// <summary>Base of digits, 2, 8, 10 or 16.</summary>
    public int Base => text.HasValue ? text.Value.Base : 10;

    /// <summary>Absolute (positive) value of the source number.</summary>
    public IPluralNumber N => this;
    /// <summary>Integer digits.</summary>
    public IPluralNumber I => AsText.I;
    /// <summary>Exponent digits.</summary>
    public IPluralNumber E => AsText.E;
    /// <summary>Visible fractional digits, with trailing zeros.</summary>
    public IPluralNumber F => TextNumber.Zero;
    /// <summary>Visible fractional digits, without trailing zeros.</summary>
    public IPluralNumber T => TextNumber.Zero;
    /// <summary>Fraction digit count including trailing zeroes.</summary>
    public IPluralNumber V => new LongNumber(F_Digits);
    /// <summary>Fraction digit count excluding trailing zeros.</summary>
    public IPluralNumber W => new LongNumber(T_Digits);

    /// <summary>Number of integer digits.</summary>
    public int I_Digits => AsText.I_Digits;
    /// <summary>Number of exponent digits</summary>
    public int E_Digits => AsText.E_Digits;
    /// <summary>Number of visible fraction digits, with trailing zeroes. Corresponds to 'v' attribute in Unicode CLDR plural.xml.</summary>
    public int F_Digits => 0;
    /// <summary>Number of visible fraction digits, without trailing zeros. Corresponds to 'w' attribute in Unicode CLDR plural.xml.</summary>
    public int T_Digits => 0;

    /// <summary>Create ulong value</summary>
    /// <param name="value"></param>
    /// <param name="text">(optional) value as text</param>
    public ULongNumber(ulong value, TextNumber? text = default!)
    {
        this.Value = value;
        this.text = text;
    }
    /// <summary></summary>
    public IPluralNumber Modulo(int modulo) => new ULongNumber(Value % (ulong)modulo);

    /// <summary></summary>
    public override string ToString() => AsText.ToString();
    /// <summary></summary>
    public bool TryGet(out long value)
    {
        if (this.Value > long.MaxValue) { value = long.MaxValue; return false; }
        value = unchecked((long)Value);
        return true;
    }
    /// <summary></summary>
    public bool TryGet(out decimal value)
    {
        value = Value;
        return true;
    }
    /// <summary></summary>
    public bool TryGet(out double value)
    {
        double _doubleValue = unchecked(Value);
        ulong _backTo = unchecked((ulong)_doubleValue);
        if (_backTo != Value) { value = default; return false; }
        value = _doubleValue;
        return true;
    }
    /// <summary></summary>
    public bool TryGet(out System.Numerics.BigInteger value)
    {
        value = Value;
        return true;
    }
    /// <summary></summary>
    public IEnumerator<char> GetEnumerator() => AsText.GetEnumerator();
    /// <summary></summary>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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

// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using System;
using System.Collections;
using System.Globalization;

/// <summary>Big integer number</summary>
public class BigIntegerNumber : IDecimalNumber
{
    /// <summary>Big integer value</summary>
    public readonly System.Numerics.BigInteger Value;
    /// <summary>Absolute version.</summary>
    IPluralNumber? absolute = null;

    /// <summary>Value as text</summary>
    TextNumber? text;
    /// <summary>Text representation of the value.</summary>
    public TextNumber AsText => text.HasValue ? text.Value : (text = new TextNumber(Value.ToString(CultureInfo.InvariantCulture).AsMemory(), CultureInfo.InvariantCulture, NumberStyles.Integer)).Value;

    /// <summary>Create big integer number</summary>
    public BigIntegerNumber(System.Numerics.BigInteger bigInteger, TextNumber? text = default!)
    {
        this.Value = bigInteger;
        this.text = text;
    }

    /// <summary>Absolute value</summary>
    public IPluralNumber Absolute => absolute ?? (absolute = Value.Sign < 0 ? new BigIntegerNumber(-Value) : this);
    /// <summary></summary>
    public bool HasValue => true;
    /// <summary></summary>
    public bool IsFloat => false;
    /// <summary></summary>
    public int Sign => Value.Sign;
    /// <summary>Absolute (positive) value of the source number.</summary>
    public IPluralNumber N => Absolute;
    /// <summary>Integer digits.</summary>
    public IPluralNumber I => Absolute;
    /// <summary>Exponent digits.</summary>
    public IPluralNumber E => LongNumber.Zero;
    /// <summary>Visible fractional digits, with trailing zeros.</summary>
    public IPluralNumber F => TextNumber.Zero;
    /// <summary>Visible fractional digits, without trailing zeros.</summary>
    public IPluralNumber T => TextNumber.Zero;
    /// <summary>Fraction digit count including trailing zeroes.</summary>
    public IPluralNumber V => new LongNumber(F_Digits);
    /// <summary>Fraction digit count excluding trailing zeros.</summary>
    public IPluralNumber W => new LongNumber(T_Digits);

    /// <summary>Number of integer digits.</summary>
    public int I_Digits => throw new NotImplementedException();
    /// <summary>Number of exponent digits</summary>
    public int E_Digits => 0;
    /// <summary>Number of visible fraction digits, with trailing zeroes. Corresponds to 'v' attribute in Unicode CLDR plural.xml.</summary>
    public int F_Digits => 0;
    /// <summary>Number of visible fraction digits, without trailing zeros. Corresponds to 'w' attribute in Unicode CLDR plural.xml.</summary>
    public int T_Digits => 0;
    /// <summary>Base of digits, 2, 8, 10 or 16.</summary>
    public int Base => 10;

    /// <summary>Calculate modulo.</summary>
    /// <returns>modulo or null if failed to calculate modulo</returns>
    public IPluralNumber Modulo(int modulo) => new BigIntegerNumber(Value % modulo);
    /// <summary></summary>
    public bool TryGet(out long value)
    {
        if (Value >= long.MinValue && Value <= long.MaxValue) { value = (long)Value; return true; }
        value = default;
        return false;
    }
    /// <summary></summary>
    public bool TryGet(out decimal value)
    {
        if (Value >= MinDecimal && Value <= MaxDecimal) { value = (decimal)Value; return true; }
        value = default;
        return false;
    }
    /// <summary></summary>
    static readonly System.Numerics.BigInteger MinDecimal = new System.Numerics.BigInteger(-79228162514264337593543950335M);
    /// <summary></summary>
    static readonly System.Numerics.BigInteger MaxDecimal = new System.Numerics.BigInteger(79228162514264337593543950335M);

    /// <summary></summary>
    public bool TryGet(out double value)
    {
        double _value = unchecked((double)Value);
        System.Numerics.BigInteger _backTo = unchecked((System.Numerics.BigInteger)_value);
        if (_backTo != Value) { value = default; return false; }
        value = _value;
        return true;
    }

    /// <summary></summary>
    public bool TryGet(out System.Numerics.BigInteger value)
    {
        value = Value;
        return true;
    }

    /// <summary></summary>
    public virtual IEnumerator<char> GetEnumerator() => AsText.GetEnumerator();
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

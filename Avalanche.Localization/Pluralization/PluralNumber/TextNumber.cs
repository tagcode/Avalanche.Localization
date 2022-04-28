// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Avalanche.Utilities;

/// <summary>Text based number</summary>
public struct TextNumber : IDecimalNumber
{
    /// <summary>Zero value constant</summary>
    static readonly TextNumber zero = new TextNumber("0".AsMemory(), CultureInfo.InvariantCulture, numberStyles: NumberStyles.Integer);
    /// <summary>Empty value constant</summary>
    static readonly TextNumber empty = new TextNumber("".AsMemory(), CultureInfo.InvariantCulture, numberStyles: NumberStyles.Integer);
    /// <summary>Zero value constant (Boxed)</summary>
    static readonly IPluralNumber zeroBox = zero;
    /// <summary>Empty value constant (Boxed)</summary>
    static readonly IPluralNumber emptyBox = empty;
    /// <summary>Empty value constant</summary>
    public static TextNumber Empty => empty;
    /// <summary>Empty value constant (Boxed)</summary>
    public static IPluralNumber EmptyBox => emptyBox;
    /// <summary>Zero value constant</summary>
    public static TextNumber Zero => zero;
    /// <summary>Zero value constant (Boxed)</summary>
    public static IPluralNumber ZeroBox => zeroBox;

    /// <summary>The number in text format</summary>
    public readonly ReadOnlyMemory<char> String;
    /// <summary>Number base</summary>
    public readonly int NumberBase;

    /// <summary>Sign: -1, 0, 1</summary>
    /// <remarks>If sign is 0, then text represents zero value.</remarks>
    int sign = 0;

    /// <summary>Caches for component numbers as <see cref="TextNumber"/>.</summary>
    IPluralNumber? n = null, i = null, f = null, t = null, e = null, v = null, w = null;
    /// <summary>Exponents printed open</summary>
    IPluralNumber? open = null;

    /// <summary>There is compact exponent, e.g. "1.2c6".</summary>
    bool compactDecimalExponent = false;

    /// <summary>Character at <paramref name="ix"/> in <see cref="String"/></summary>
    public char this[int ix] => String.Span[ix];

    /// <summary>Positive value characters. Excludes '-' and preceding zeros '000'.</summary>
    /// <example>Value="-001.230", n_start = 3, n_end = 8.</example>
    /// <remarks>Both values are -1, if integer digits are not detected. </remarks>
    public ReadOnlyMemory<char> N_;

    /// <summary>integer digits. Preceding zeros are excluded.</summary>
    /// <example>Value="10.230", start=0, length=2</example>
    /// <remarks>Length = 0, if integer digits are not detected.</remarks>
    public ReadOnlyMemory<char> I_;

    /// <summary>The number of integer digits, preceding zeroes are excluded. (Not same as <see cref="I_"/>, which the character range including whitespace.)</summary>
    public readonly int i_digits;

    /// <summary>Exponent digits. Preceding zeros are excluded.</summary>
    /// <remarks>Length = 0, if integer digits are not detected.</remarks>
    /// <example>"-10.00e010", start=7, length=3</example>
    public ReadOnlyMemory<char> E_;

    /// <summary>Number of e digits. (Not same as i_end-i_start, which is the length of the character range including whitespace.)</summary>
    public readonly int e_digits;

    /// <summary>Fraction digits, including trailing zeros.</summary>
    /// <example>n=1.230, f=230</example>
    /// <remarks>Length = 0, if fraction digits are not detected.</remarks>
    public ReadOnlyMemory<char> F_;

    /// <summary>Number of fraction digits, including trailing zeroes. Same as 'v' in unicode CLDR plural.xml.</summary>
    /// <example>n=1.230, v=3</example>
    public readonly int f_digits;

    /// <summary>Indices of visible fraction digits, without trailing zeroes.</summary>
    /// <example>n=1.230, t=23</example>
    /// <remarks>Both values are -1, if integer digits are not detected.</remarks>
    /// 
    /// <summary>Visible fraction digits, without trailing zeroes.</summary>
    /// <example>"n=1.230", t=23</example>
    /// <remarks>Length = 0, if visible fraction digits are not detected.</remarks>
    public ReadOnlyMemory<char> T_;

    /// <summary>Number of visible fraction digits, without trailing zeros. Same as 'w' in unicode CLDR plural.xml.</summary>
    /// <example>"n=1.230", w=2</example>
    public readonly int t_digits;

    /// <summary>Culture info</summary>
    public readonly IFormatProvider? formatProvider;

    /// <summary>Number format</summary>
    public NumberFormatInfo numberFormat;

    /// <summary>Number style</summary>
    public readonly NumberStyles numberStyle;

    /// <summary>Get as text.</summary>
    public TextNumber AsText => this;

    /// <summary>Has a value. If value is NaN, returns false.</summary>
    public bool HasValue => !String.IsEmpty && String.Length > 0 && (i_digits > 0 || f_digits > 0 || t_digits > 0);
    /// <summary>Has decimal fractions. For example "0.10" is true, but "0.00" is false</summary>
    public bool IsFloat => t_digits > 0;
    /// <summary>Sign of the value, -1, 0 or 1.</summary>
    public int Sign => sign;
    /// <summary>Number base: 2, 8, 10 or 16.</summary>
    public int Base => NumberBase;

    /// <summary>Absolute positive value characters. Excludes '-' and preceding and trailing zeros '000'.</summary>
    public IPluralNumber N
    {
        get
        {
            if (n != null) return n;
            TextNumber _ = compactDecimalExponent ? Open : this;
            n = new TextNumber(_.N_, 1, _.formatProvider, NumberStyles.Integer, _.N_, _.I_, _.i_digits, _.T_, _.t_digits, _.T_, _.t_digits, _.E_, _.e_digits, _.NumberBase);
            return n;
        }
    }
    /// <summary>Integer digits.</summary>
    public IPluralNumber I
    {
        get
        {
            if (i != null) return i;
            TextNumber _ = compactDecimalExponent ? Open : this;
            i = new TextNumber(_.I_, Math.Abs(_.sign)/*0/1*/, _.formatProvider, NumberStyles.Integer, _.I_, _.I_, _.i_digits, default, 0, default, 0, default, 0, _.NumberBase);
            return i;
        }
    }
    /// <summary>Exponent digits.</summary>
    public IPluralNumber E
    {
        get
        {
            if (e != null) return e;
            //TextNumber _ = compactDecimalExponent ? Open : this;
            //e = _.e_digits == 0 ? zero : new TextNumber(_.E_, _.formatProvider, NumberStyles.Integer, _.NumberBase);
            e = e_digits == 0 ? zero : new TextNumber(E_, formatProvider, NumberStyles.Integer, NumberBase);
            return e;
        }
    }
    /// <summary>Fraction digits, including trailing zeros.</summary>
    public IPluralNumber F {
        get {
            if (f != null) return f;
            TextNumber _ = compactDecimalExponent ? Open : this;
            f = _.f_digits == 0 ? zero : new TextNumber(_.F_, _.formatProvider, NumberStyles.Integer, _.NumberBase);
            return f;
        }
    }
    /// <summary>Fraction digits, excluding trailing zeros.</summary>
    public IPluralNumber T
    {
        get
        {
            if (t != null) return t;
            TextNumber _ = compactDecimalExponent ? Open : this;
            t = _.t_digits == 0 ? zero : new TextNumber(_.T_, _.formatProvider, NumberStyles.Integer, _.NumberBase);
            return t;
        }
    }
    /// <summary>Fraction digit count including trailing zeroes.</summary>
    public IPluralNumber V => v ?? (v = new LongNumber(compactDecimalExponent ? Open.F_Digits : F_Digits));
    /// <summary>Fraction digit count excluding trailing zeros.</summary>
    public IPluralNumber W => w ?? (w = new LongNumber(compactDecimalExponent ? Open.T_Digits : T_Digits));
    /// <summary>Fraction digit count excluding trailing zeros.</summary>
    public TextNumber Open { get { if (open != null) return (TextNumber)open; TextNumber result = new TextNumber(PrintOpen(), formatProvider, numberStyle, NumberBase); this.open = result; return result; } }

    /// <summary>Number of integer digits.</summary>
    public int I_Digits => i_digits;
    /// <summary>Number of exponent digits</summary>
    public int E_Digits => e_digits;
    /// <summary>Fraction digits including trailing zeroes. Corresponds to 'v' attribute in Unicode CLDR plural.xml if exponent is removed.</summary>
    public int F_Digits => f_digits;
    /// <summary>Fraction digits excluding trailing zeros. Corresponds to 'w' attribute in Unicode CLDR plural.xml if exponent is removed.</summary>
    public int T_Digits => t_digits;

    /// <summary>Create with specific parameters.</summary>
    /// <param name="formatProvider">format provider (culture)</param>
    /// <param name="numberStyles"><see cref="NumberStyles.Integer"/>, <see cref="NumberStyles.HexNumber"/> or <see cref="NumberStyles.Float"/></param>
    public TextNumber(
        ReadOnlyMemory<char> @string, 
        int sign, 
        IFormatProvider? formatProvider, 
        NumberStyles numberStyles,
        ReadOnlyMemory<char> n_,
        ReadOnlyMemory<char> i_, int i_digits,
        ReadOnlyMemory<char> f_, int f_digits,
        ReadOnlyMemory<char> t_, int t_digits,
        ReadOnlyMemory<char> e_, int e_digits,
        int numberBase)
    {
        this.String = @string;
        this.numberStyle = numberStyles;
        this.NumberBase = numberBase >= 0 ? numberBase : ((numberStyles & NumberStyles.AllowHexSpecifier) != 0) ? 16 : 10;
        this.formatProvider = formatProvider;
        this.numberFormat = this.formatProvider?.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo ?? CultureInfo.InvariantCulture.NumberFormat;
        this.sign = sign;
        this.N_ = n_;
        this.I_ = i_; 
        this.i_digits = i_digits;
        this.F_ = f_;
        this.f_digits = f_digits;
        this.T_ = t_;
        this.t_digits = t_digits;
        this.E_ = e_;
        this.e_digits = e_digits;
    }

    /// <summary>Create text and scan <paramref name="string"/> for parameters.</summary>
    public TextNumber(string @string, IFormatProvider? formatProvider = default, NumberStyles numberStyles = NumberStyles.Float, int numberBase = -1) : this(@string.AsMemory(), formatProvider, numberStyles, numberBase) { }

    /// <summary>Create text and scan <paramref name="string"/> for parameters.</summary>
    public TextNumber(ReadOnlyMemory<char> @string, IFormatProvider? formatProvider = default, NumberStyles numberStyles = NumberStyles.Float, int numberBase = -1)
    {
        this.String = @string;
        this.numberStyle = numberStyles;
        this.NumberBase = numberBase >= 0 ? numberBase : ((numberStyles & NumberStyles.AllowHexSpecifier) != 0) ? 16 : 10;
        this.formatProvider = formatProvider;
        this.numberFormat = this.formatProvider?.GetFormat(typeof(NumberFormatInfo)) as NumberFormatInfo ?? CultureInfo.InvariantCulture.NumberFormat;
        N_ = default;
        I_ = default;
        F_ = default;
        T_ = default;
        E_ = default;
        t_digits = 0;
        f_digits = 0;
        e_digits = 0;
        i_digits = 0;

        // Empty string
        if (@string.IsEmpty) return;

        int n_start = -1, n_end = -1, i_start = -1, i_end = -1, f_start = -1, f_end = -1, t_start = -1, t_end = -1, e_start = -1, e_end = -1;

        bool zero = true;
        bool negative = false;
        ScanState state = ScanState.Zero;
        ReadOnlySpan<char> span = String.Span;
        for (int i = 0; i < span.Length; i++)
        {
            char ch = span[i];

            if (ch == ' ') continue;

            if (ch == '-' && state == ScanState.Zero)
            {
                negative = true;
                continue;
            }

            if (numberBase <= 10 && ((this.numberStyle & NumberStyles.AllowExponent) != 0) && (ch == 'e' || ch == 'E' || ch == 'c' || ch == 'C' || ch == '⏨'))
            {
                compactDecimalExponent |= ch == 'c' || ch == 'C';
                state = ScanState.Exponent;
                continue;
            }

            if (state == ScanState.Zero)
            {
                if (ch == '0')
                {
                    i_start = i;
                    if (i_end < i_start) i_end = i + 1;
                    continue;
                }
                else if (IsNonZeroDigit(ch))
                {
                    zero = false;
                    if (i_start < 0) i_start = i;
                    i_end = i + 1;
                    i_digits++;
                    state = ScanState.Integer;
                    continue;
                }
            }
            else if (state == ScanState.Integer)
            {
                if (IsDigit(ch))
                {
                    zero = false;
                    if (i_start < 0) i_start = i;
                    i_end = i + 1;
                    i_digits++;
                    continue;
                }
            }

            if (state == ScanState.Zero || state == ScanState.Integer)
            {
                // ',' or '.' decimal separator
                if ((MatchString(i, numberFormat.NumberDecimalSeparator) || MatchString(i, numberFormat.PercentDecimalSeparator) || MatchString(i, numberFormat.CurrencyDecimalSeparator))) // <- 3 separators this may cause problems, if they differ
                {
                    state = ScanState.Fraction;
                    f_start = i + 1; f_end = i + 1;
                    //t_start = i + 1; t_end = i + 1;
                    continue;
                }
                continue;
            }

            if (state == ScanState.Exponent)
            {
                if (IsDigit(ch) || ch == '-' || ch == '+')
                {
                    if (e_start < 0) e_start = i;
                    e_end = i + 1;
                    e_digits++;
                    continue;
                }
            }

            if (state == ScanState.Fraction)
            {
                if (IsDigit(ch))
                {
                    if (f_start < 0) f_start = i;
                    f_end = i + 1;
                    f_digits++;
                }

                if (IsNonZeroDigit(ch))
                {
                    zero = false;
                    if (t_start < 0) t_start = i;
                    t_end = i + 1;
                }
                continue;
            }
        }

        // Count t_digits 'w', number of fraction digits without trailing zeroes
        if (t_start >= 0 && t_end >= 0)
            for (int i = t_start; i < t_end; i++)
            {
                char ch = span[i];
                if (IsDigit(ch)) t_digits++;
            }

        // Make n positive number region
        if (i_start >= 0 && (n_start < 0 || i_start < n_start)) n_start = i_start;
        //if (f_start >= 0 && (n_start < 0 || f_start < n_start)) n_start = f_start;
        if (e_start >= 0 && (n_start < 0 || e_start < n_start)) n_start = e_start;
        if (t_digits > 0 && t_start >= 0 && (n_start < 0 || t_start < n_start)) n_start = t_start;
        if (i_end >= 0 && (n_end < 0 || i_end > n_end)) n_end = i_end;
        //if (f_end >= 0 && (n_end < 0 || f_end > n_end)) n_end = f_end;
        if (e_end >= 0 && (n_end < 0 || e_end > n_end)) n_end = e_end;
        if (t_digits > 0 && t_end >= 0 && (n_end < 0 || t_end > n_end)) n_end = t_end;

        // Assign slices
        N_ = n_start < 0 ? default : @string.Slice(n_start, n_end - n_start);
        I_ = i_start < 0 ? default : @string.Slice(i_start, i_end - i_start);
        F_ = f_start < 0 ? default : @string.Slice(f_start, f_end - f_start);
        T_ = t_start < 0 ? default : @string.Slice(t_start, t_end - t_start);
        E_ = e_start < 0 ? default : @string.Slice(e_start, e_end - e_start);


        // Set sign
        this.sign = zero ? 0 : negative ? -1 : 1;
    }

    /// <summary>Test whether <paramref name="ch"/> is a digit of the <see cref="NumberBase"/>.</summary>
    bool IsDigit(char ch) => ch >= '0' && ch <= '9' || ((NumberBase == 16) && ((ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F')));
    /// <summary>Test whether <paramref name="ch"/> is a digit of the <see cref="NumberBase"/>.</summary>
    bool IsNonZeroDigit(char ch) => ch >= '1' && ch <= '9' || ((NumberBase == 16) && ((ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F')));

    /// <summary>Get digit value based on <see cref="NumberBase"/>.</summary>
    /// <returns>value, or -1 if was not digit</returns>
    int DigitValue(char ch)
    {
        if (ch >= '0' && ch <= '9') return (int)(ch - '0');
        if (NumberBase == 16 && (ch >= 'a' && ch <= 'f')) return (int)(ch - 'a') + 10;
        if (NumberBase == 16 && (ch >= 'A' && ch <= 'F')) return (int)(ch - 'A') + 10;
        return -1;
    }

    /// <summary></summary>
    public IEnumerator<char> GetEnumerator() => ToString().GetEnumerator();
    /// <summary></summary>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Cached ToString</summary>
    string? canonicalizedString = null;

    /// <summary>Print information</summary>
    public override string ToString()
    {
        // Return cached
        if (canonicalizedString != null) return canonicalizedString;
        // Open exponent "12.3e-2" -> "0.123"
        ReadOnlyMemory<char> mem = String;
        // Return opened
        if (MemoryMarshal.TryGetString(mem, out string? str, out int start, out int length) && str != null)
        {            
            canonicalizedString = start==0&&length==str.Length ? str : str.Substring(start, length);
        }
        // Create string
        else canonicalizedString = new string(mem.Span);
        // Return 
        return canonicalizedString;
    }

    /// <summary>Compares whether a substring of <see cref="TextNumber"/> matches to <paramref name="matchTo"/>, when compare starts at end of <paramref name="endIx"/> (including <paramref name="endIx"/>).</summary>
    bool MatchString(int endIx, string matchTo)
    {
        if (matchTo.Length > endIx + 1) return false;
        ReadOnlySpan<char> span = String.Span;
        for (int i = matchTo.Length - 1; i >= 0 && endIx >= 0; i--, endIx--)
            if (span[endIx] != matchTo[i]) return false;
        return true;
    }

    /// <summary>Try to calculate modulo.</summary>
    public IPluralNumber Modulo(int modulo)
    {
        // No string
        if (String.IsEmpty) return TextNumber.Empty;
        // Zero
        if (sign == 0) return TextNumber.Zero;
        // Negative divider
        if (modulo < 0) modulo = -modulo;
        // As is
        if (modulo == 1) return this;
        // Division by zero
        if (modulo == 0) throw new DivideByZeroException();
        // Modulo by BaseNumber - so get substring digits
        if (!String.IsEmpty && I_.Length>0)
        {
            int numberOfDigitsToGet = -1;
            if (NumberBase == 10)
            {
                switch (modulo)
                {
                    case 10: numberOfDigitsToGet = 1; break;
                    case 100: numberOfDigitsToGet = 2; break;
                    case 1000: numberOfDigitsToGet = 3; break;
                    case 10000: numberOfDigitsToGet = 4; break;
                    case 100000: numberOfDigitsToGet = 5; break;
                    case 1000000: numberOfDigitsToGet = 6; break;
                    case 10000000: numberOfDigitsToGet = 7; break;
                    case 100000000: numberOfDigitsToGet = 8; break;
                    case 1000000000: numberOfDigitsToGet = 9; break;
                }
            }
            else if (NumberBase == 16)
            {
                switch (modulo)
                {
                    case 0x10: numberOfDigitsToGet = 1; break;
                    case 0x100: numberOfDigitsToGet = 2; break;
                    case 0x1000: numberOfDigitsToGet = 3; break;
                    case 0x10000: numberOfDigitsToGet = 4; break;
                    case 0x100000: numberOfDigitsToGet = 5; break;
                    case 0x1000000: numberOfDigitsToGet = 6; break;
                    case 0x10000000: numberOfDigitsToGet = 7; break;
                }
            }
            if (numberOfDigitsToGet <= 0) goto noDigitExtraction;

            // Exponent value
            long e = 0L;
            // If exponent digit read fails, use other methods
            if (e_digits > 0 && !TryGetExponent(out e)) goto noDigitExtraction;

            // Exponent == 0
            if (e == 0L)
            {
                // Get integer digits
                if (!TryGetLastIntegerDigits(0, numberOfDigitsToGet, out ReadOnlyMemory<char> digits)) goto noDigitExtraction;

                // Add fraction digits
                if (!digits.IsEmpty && !F_.IsEmpty && f_digits > 0) digits = digits.UnifyStringWith(F_);

                // Is it zero by value?
                bool isZero = true;
                // Scan digits for non-zero values
                ReadOnlySpan<char> span = digits.Span;
                for (int i = 0; i < span.Length; i++)
                {
                    // Got non-zero
                    if (IsNonZeroDigit(span[i])) { isZero = false; break; }
                }
                // All digits are zero, value is zero
                if (isZero) return TextNumber.Zero;

                // Move range to include minus '-'
                if (Sign < 0)
                {
                    if (MemoryMarshal.TryGetString(digits, out string? text, out int start, out int length) && start>0 && text[start-1] == '-')
                    {
                        ReadOnlyMemory<char> slice3 = text.AsMemory().Slice(start - 1, start + length + 1);
                        return new TextNumber(slice3, formatProvider, numberStyle, NumberBase);
                    }
                    else
                    {
                        char[] arr = ArrayPool<char>.Shared.Rent(1 + digits.Length);
                        arr[0] = '-';
                        digits.CopyTo(arr.AsMemory(1));
                        string str = new string(arr, 0, digits.Length + 1);
                        ArrayPool<char>.Shared.Return(arr);
                        return new TextNumber(str.AsMemory(), formatProvider, numberStyle, NumberBase);
                    }
                }

                // Return as is
                if (System.MemoryExtensions.SequenceEqual(digits.Span, String.Span)) return this;

                // Return digits
                return new TextNumber(digits, formatProvider, numberStyle, NumberBase);
            }
            // There are exponents:
            //     "You have 100e-2 apple."
            //     "You have 0.02e2 apples."
            // 
            // It is up to the plural rules to choose how to handle exponent, not this class.
            // 
            else
            {
                // Get n integer digits + fraction in opened format
                ReadOnlyMemory<char> buf = PrintOpen(numberOfDigitsToGet);
                // Return digits
                return new TextNumber(buf, formatProvider, numberStyle, NumberBase);
            }
        }
noDigitExtraction:

        if (IsFloat)
        {
            double _d;
            if (TryGet(out _d)) return new DoubleNumber(_d % modulo);

            // ?
        }
        else
        {
            long _l;
            if (TryGet(out _l)) return new LongNumber(_l % modulo);
            System.Numerics.BigInteger _b;
            if (TryGet(out _b)) return new BigIntegerNumber(_b % modulo);

            // ?    
        }

        // Not Implemented
        return new TextNumber("");
    }

    /// <summary>Count the number of preceeding '0' digits.</summary>
    ReadOnlyMemory<char> RemovePreceedingZeroes(ReadOnlyMemory<char> memory)
    {
        //
        ReadOnlySpan<char> span = memory.Span;
        //
        ReadOnlyMemory<char> result = memory;
        // 
        for (int i=0; i<span.Length; i++)
        {
            // Get char
            char ch = span[i];
            // Not digit
            if (!IsDigit(ch)) continue;
            // Not zero
            if (ch != '0') break;
            // Cut zero
            result = memory.Slice(i + 1);
        }
        //
        return result;
    }

    /// <summary>Print text open. Exponents are opened</summary>
    /// <param name="moduloNumberOfDigits">If given, then limits the number of integer digits in result.</param>
    /// <example>"1.2e4" -> 1200, "-1.2e-4" -> -0.00012</example>
    /// <returns></returns>
    public ReadOnlyMemory<char> PrintOpen(int? moduloNumberOfDigits = null)
    {
        // Zero
        if (sign == 0) return String;
        // Get exponent value
        TryGetExponent(out long _exp);
        //if (e_digits == 0 && _exp == 0) return String;
        //
        int exp = (int)_exp;
        // 
        int count = Math.Max(1024, exp + I_Digits + F_Digits + 3);
        // Rent buffer
        char[] buf = ArrayPool<char>.Shared.Rent(count);
        // Span to work with
        Span<char> span = buf.AsSpan();
        //
        try
        {
            // Add '-'
            if (sign < 0) { numberFormat.NegativeSign.CopyTo(span); span = span.Slice(numberFormat.NegativeSign.Length); }
            // Get digits
            ReadOnlySpan<char> meagningful_digits = RemovePreceedingZeroes(I_).UnifyStringWith(F_).Span;
            // Number of meaningful digits
            int meaningful_digits = i_digits + f_digits;
            // Integer digits to print. "1.2345E1" -> "12.345" has 2 integer digits
            int integer_digits = i_digits + exp;
            // Number of digits to omit from integer printing
            int digits_to_omit = 0;
            // Omit more integer
            if (moduloNumberOfDigits.HasValue) digits_to_omit += integer_digits - moduloNumberOfDigits.Value;
            // Flag whether any integer digits were appended
            bool integer_added = false;
            // Append integer side digits
            while (integer_digits>0 && span.Length>0)
            {
                // Next digit
                char digit = '0';
                // Find next meaningful digit, or keep fallback '0'
                while (meagningful_digits.Length>0)
                {
                    // Get char
                    char ch = meagningful_digits[0];
                    // Slice off
                    meagningful_digits = meagningful_digits.Slice(1);
                    // Is not digit
                    if (!IsDigit(ch)) continue;
                    // Got the next meagningful digit
                    digit = ch;
                    meaningful_digits--;
                    break;
                }
                // Count down
                integer_digits--;
                // Omit digit
                if (digits_to_omit-- > 0) continue;
                // Append to buffer
                span[0] = digit;
                span = span.Slice(1);
                integer_added = true;
            }
            // Fractions digits to print. "1.2345E1" -> "12.345" has 3 fraction digits
            int fraction_digits = f_digits - exp;
            //
            if (fraction_digits > 0)
            {
                // Append '0'
                if (!integer_added && span.Length>0) { span[0] = '0'; span = span.Slice(1); }
                // Append '.'
                numberFormat.NumberDecimalSeparator.CopyTo(span); span = span.Slice(numberFormat.NumberDecimalSeparator.Length);
                //
                while (fraction_digits>0)
                {
                    // Next digit
                    char digit = '0';
                    // Find next meaningful digit, or keep fallback '0'
                    while (fraction_digits<=meaningful_digits && meaningful_digits>0 && span.Length>0)
                    {
                        // Get char
                        char ch = meagningful_digits[0];
                        // Slice off
                        meagningful_digits = meagningful_digits.Slice(1);
                        // Is not digit
                        if (!IsDigit(ch)) continue;
                        // Got the next meagningful digit
                        digit = ch;
                        meaningful_digits--;
                        break;
                    }

                    // Append to buffer
                    span[0] = digit;
                    span = span.Slice(1);
                    fraction_digits--;
                }
            }

            // Create string
            string str = new String(buf.AsSpan(0, buf.Length - span.Length));
            // Return string
            return str.AsMemory();
        } finally
        {
            // Return
            ArrayPool<char>.Shared.Return(buf);
        }
    }

    /// <summary>Get <paramref name="count"/> integer from the end of <see cref="I_"/>.</summary>
    /// <param name="index">Starting index from right. e.g. 0=starts from the first digit from the right (least significant). 1=the next from the right.</param>
    /// <param name="count">Number of integer digits. Encountered white spaces are not counted.</param>
    /// <param name="digits">Span that contains <paramref name="count"/> digits</param>
    /// <returns></returns>
    bool TryGetLastIntegerDigits(int index, int count, out ReadOnlyMemory<char> digits)
    {
        // No digits
        if (I_.IsEmpty) { digits = default; return false; }
        // Return all integer digits
        if (index == 0 && count >= i_digits) { digits = I_; return true; }
        // Scan range
        ReadOnlySpan<char> span = I_.Span;
        // Return count digits
        while (count > 0 && span.Length>0)
        {
            // Get last char
            char ch = span[span.Length-1];
            // Got integer digit
            if (IsDigit(ch)) count--;
            // Slice off last digit from scan
            span = span.Slice(0, span.Length - 1);
        }
        // Get slice of integer digits (and possibly white space in between.)
        digits = I_.Slice(span.Length, I_.Length - span.Length);
        return true;
    }

    /// <inheritdoc />
    public bool TryGet(out long value)
    {
        // No string
        if (String.IsEmpty) { value = 0; return true; }
        // Float 
        if (IsFloat) { value = 0; return false; }
        // Parse integer
        long result = 0L;
        long max = long.MaxValue / NumberBase;
        long _base = NumberBase;
        if (!I_.IsEmpty)
        {
            // Get span
            ReadOnlySpan<char> span = I_.Span;
            // Scan and multiply digits from left to right
            for (int i = 0; i < span.Length; i++)
            {
                char ch = span[i];
                int digit = DigitValue(ch);
                if (digit < 0) continue;
                if (result >= max) { value = 0L; return false; }
                result = result * _base + (long)digit;
            }
        }

        // Parse exponent
        if (TryGetExponent(out long exp) && exp != 0L)
        {
            // Positive exponent
            if (exp > 0L)
            {
                for (int i = 0; i < exp; i++)
                {
                    if (result >= max) { value = 0L; return false; }
                    result *= _base;
                }
            }
            else
            // Negative exponent
            if (exp < 0L)
            {
                exp *= -1;
                for (int i = 0; i < exp; i++)
                {
                    // Check if there is non-0 reminder
                    long reminder = result % _base;
                    // Cannot fit fractions into result, -> fail
                    if (reminder != 0L) { value = 0L; return false; }
                    // Divide by base
                    result /= _base;
                }
            }
        }

        value = result;
        if (sign < 0) value = -value;
        return true;
    }

    /// <summary>Get exponent value as long, if exponent exists and can fit long.</summary>
    public bool TryGetExponent(out long value)
    {
        // Parse exponent
        if (E_.IsEmpty) { value = 0L; return true; }
        // Get span
        ReadOnlySpan<char> span = E_.Span;
        // Scan exponent digits
        long exp = 0;
        long sign = 1;
        long max = long.MaxValue / NumberBase;
        for (int i = 0; i < span.Length; i++)
        {
            char ch = span[i];
            if (ch == '-') { sign *= -1; continue; }
            int digit = DigitValue(ch);
            if (digit < 0) continue;
            if (exp >= max) { value = 0L; return false; }
            exp = exp * NumberBase + (long)digit;
        }

        value = exp * sign;
        return true;
    }

    /// <summary>Get as decimal value</summary>
    public bool TryGet(out decimal value)
    {
        // No string
        if (String.IsEmpty) { value = 0; return true; }
        // Get span
        ReadOnlySpan<char> span = String.Span;
        // Try parse
        return decimal.TryParse(span, out value);
    }

    NumberStyles NumberStyle => NumberBase == 16 ? NumberStyles.HexNumber : IsFloat ? NumberStyles.Float : NumberStyles.Integer;

    /// <summary>Get double value</summary>
    public bool TryGet(out double value)
    {
        // No string
        if (String.IsEmpty) { value = 0; return true; }
        // Get span
        ReadOnlySpan<char> span = String.Span;
        // Try parse
        return double.TryParse(span, NumberStyle, formatProvider, out value);
    }

    /// <summary>Get big inteteger value</summary>
    public bool TryGet(out System.Numerics.BigInteger value)
    {
        // No string
        if (String.IsEmpty) { value = 0; return true; }
        // Get span
        ReadOnlySpan<char> span = String.Span;
        // Try parse
        return System.Numerics.BigInteger.TryParse(span, NumberStyle, formatProvider, out value);
    }

    /// <summary></summary>
    enum ScanState
    {
        Zero,
        Integer,
        Fraction,
        Exponent
    }

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



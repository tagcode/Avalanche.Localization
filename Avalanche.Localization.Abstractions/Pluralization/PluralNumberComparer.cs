// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;

/// <summary>This comparer compares <see cref="IPluralNumber"/>s by value. For example "1.1e1" is equal to "11".</summary>
public class PluralNumberComparer : IEqualityComparer<IPluralNumber>, IComparer<IPluralNumber>
{
    /// <summary>Singleton</summary>
    private static readonly PluralNumberComparer instance = new PluralNumberComparer();
    /// <summary>Singleton</summary>
    public static PluralNumberComparer Default => instance;

    /// <summary>Compare <paramref name="x"/> for equality of <paramref name="y"/>.</summary>
    public bool Equals(IPluralNumber? x, IPluralNumber? y) => x?.ToString() == y?.ToString();

    /// <summary>Calculate hashcode of <paramref name="x"/>.</summary>
    public int GetHashCode(IPluralNumber x)
    {
        int result = FNVHashBasis;
        foreach (char ch in x)
        {
            result ^= DigitValue(ch);
            result *= FNVHashPrime;
        }
        return result;
    }

    /// <summary>Compare two numbers for order.</summary>
    public int Compare(IPluralNumber? x, IPluralNumber? y)
    {
        // Nulls
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return -1;

        // Successful TryGets
        bool _lx = false, _ly = false, _dx = false, _dy = false;

        // Compare doubles
        long lx = default, ly = default;
        if ((_lx = x.TryGet(out lx)) && (_ly = y.TryGet(out ly))) return lx < ly ? -1 : lx > ly ? 1 : 0;

        // Compare doubles
        double dx = default, dy = default;
        if ((_dx = x.TryGet(out dx)) && (_dy = y.TryGet(out dy))) return dx < dy ? -1 : dx > dy ? 1 : 0;

        // Long and doubles can be compared
        if (_lx && _dy) return lx < dy ? -1 : lx > dy ? 1 : 0;
        if (_dx && _ly) return dx < ly ? -1 : dx > ly ? 1 : 0;

        // Compare other features
        int sign = x.Sign;
        if (sign != y.Sign) return sign - y.Sign;

        // Get exponent
        bool _ex = false, _ey = false;
        long ex = 0L, ey = 0L;
        if (x.E_Digits == 0) { _ex = true; ex = 0L; } else _ex = x.E.TryGet(out ex);
        if (y.E_Digits == 0) { _ey = true; ey = 0L; } else _ey = x.E.TryGet(out ey);

        // Compare number of digits before decimal separator
        long x_digit_count = x.I_Digits + ex, y_digit_count = y.I_Digits + ey;
        if (x_digit_count < y_digit_count) return -1 * sign;
        if (y_digit_count < x_digit_count) return 1 * sign;

        // Compare digit sequences
        var x_digits = x.GetEnumerator(); var y_digits = y.GetEnumerator();
        while (true)
        {
            bool x_has_value = x_digits.MoveNext(), y_has_value = y_digits.MoveNext();

            if (!x_has_value && !y_has_value) return 0;
            if (!x_has_value && y_has_value) return -1;
            if (x_has_value && !y_has_value) return 1;

            int x_digit = DigitValue(x_digits.Current);
            int y_digit = DigitValue(y_digits.Current);
            if (x_digit < y_digit) return -1;
            if (y_digit < x_digit) return 1;
        }
    }

    const int FNVHashBasis = unchecked((int)0x811C9DC5);
    const int FNVHashPrime = 0x1000193;

    /// <summary>Get digit value.</summary>
    /// <returns>value, -1 for decimal separator and -2 if was not digit</returns>
    int DigitValue(char ch)
    {
        if (ch == '.') return -1;
        if (ch >= '0' && ch <= '9') return (int)(ch - '0');
        if (ch >= 'a' && ch <= 'f') return (int)(ch - 'a') + 10;
        if (ch >= 'A' && ch <= 'F') return (int)(ch - 'A') + 10;
        return -2;
    }

}

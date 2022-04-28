using System.Globalization;
using System.Numerics;
using Avalanche.Localization;
using Avalanche.Localization.Pluralization;
using static System.Console;

class pluralnumber
{
    public static void Run()
    {
        {
            IFormatProvider culture = CultureInfo.InvariantCulture;
            TextNumber number = new TextNumber("1,000,012.000678", culture);
            WriteLine($"Integer digit count = {number.I_Digits}");  // 7
            WriteLine($"Fraction digit count = {number.F_Digits}"); // 6
        }
        {
            IFormatProvider culture = CultureInfo.GetCultureInfo("fi");
            TextNumber number = new TextNumber("1.000.012,000678", culture);
            WriteLine($"Integer digit count = {number.I_Digits}");  // 7
            WriteLine($"Fraction digit count = {number.F_Digits}"); // 6
        }
        {
            IFormatProvider culture = CultureInfo.GetCultureInfo("fi");
            double value = 1_000_012.000678;
            string text = value.ToString(culture);
            TextNumber textNumber = new TextNumber(text, culture);
            DoubleNumber number = new DoubleNumber(value, textNumber);
            WriteLine($"Integer digit count = {number.I_Digits}");  // 7
            WriteLine($"Fraction digit count = {number.F_Digits}"); // 6
        }
        {
            IFormatProvider culture = CultureInfo.GetCultureInfo("fi");
            long value = 1_000_012;
            string text = value.ToString(culture);
            TextNumber textNumber = new TextNumber(text, culture);
            LongNumber number = new LongNumber(value, textNumber);
            WriteLine($"Integer digit count = {number.I_Digits}");  // 7
            WriteLine($"Fraction digit count = {number.F_Digits}"); // 0
        }
        {
            IFormatProvider culture = CultureInfo.GetCultureInfo("fi");
            ulong value = 1_000_012;
            string text = value.ToString(culture);
            TextNumber textNumber = new TextNumber(text, culture);
            ULongNumber number = new ULongNumber(value, textNumber);
            WriteLine($"Integer digit count = {number.I_Digits}");  // 7
            WriteLine($"Fraction digit count = {number.F_Digits}"); // 0
        }
        {
            IFormatProvider culture = CultureInfo.GetCultureInfo("fi");
            BigInteger value = 1_000_012;
            string text = value.ToString(culture);
            TextNumber textNumber = new TextNumber(text, culture);
            IPluralNumber number = new BigIntegerNumber(value, textNumber);
            //WriteLine($"Integer digit count = {number.I_Digits}");  // 0
            WriteLine($"Fraction digit count = {number.F_Digits}"); // 6
        }
    }
}

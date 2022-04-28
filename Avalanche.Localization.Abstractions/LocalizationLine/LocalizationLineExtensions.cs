// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Runtime.CompilerServices;
using Avalanche.Utilities;

/// <summary>Extension methods for <![CDATA[IEnumerable<KeyValuePair<string, MarkedText>>]]></summary>
public static class LocalizationLineExtensions
{
    /// <summary>Reads values from <paramref name="line"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReadValues(this IEnumerable<KeyValuePair<string, MarkedText>> line, out MarkedText pluralRules, out MarkedText culture, out MarkedText key, out MarkedText plurals, out MarkedText templateFormat, out MarkedText text)
    {
        // Init
        pluralRules = default; key = default; culture = default; plurals = default; text = default; templateFormat = default;
        // Heap-free enumeration
        if (line is IList<KeyValuePair<string, MarkedText>> list)
        {
            // Get count
            int count = list.Count;
            // Visit key-values. Last stated value overrides preceding values.
            for (int i = 0; i < count; i++)
            {
                var kv = list[i];
                if (kv.Key == "Key") key = kv.Value;
                else if (kv.Key == "PluralRules") pluralRules = kv.Value;
                else if (kv.Key == "Culture") culture = kv.Value;
                else if (kv.Key == "Plurals") plurals = kv.Value;
                else if (kv.Key == "TemplateFormat") templateFormat = kv.Value;
                else if (kv.Key == "Text") text = kv.Value;
            }
        }
        // Enumerator
        else
        {
            // Visit key-values. Last stated value overrides preceding values.
            foreach (var kv in line)
            {
                if (kv.Key == "Key") key = kv.Value;
                else if (kv.Key == "PluralRules") pluralRules = kv.Value;
                else if (kv.Key == "Culture") culture = kv.Value;
                else if (kv.Key == "Plurals") plurals = kv.Value;
                else if (kv.Key == "TemplateFormat") templateFormat = kv.Value;
                else if (kv.Key == "Text") text = kv.Value;
            }
        }
    }

    /// <summary>Reads values from <paramref name="line"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReadTemplateFormatText(this IEnumerable<KeyValuePair<string, MarkedText>> line, out MarkedText templateFormat, out MarkedText text)
    {
        // Init
        text = default; templateFormat = default;
        // Heap-free enumeration
        if (line is IList<KeyValuePair<string, MarkedText>> list)
        {
            // Get count
            int count = list.Count;
            // Visit key-values. Last stated value overrides preceding values.
            for (int i = 0; i < count; i++)
            {
                var kv = list[i];
                if (kv.Key == "TemplateFormat") templateFormat = kv.Value;
                else if (kv.Key == "Text") text = kv.Value;
            }
        }
        // Enumerator
        else
        {
            // Visit key-values. Last stated value overrides preceding values.
            foreach (var kv in line)
            {
                if (kv.Key == "TemplateFormat") templateFormat = kv.Value;
                else if (kv.Key == "Text") text = kv.Value;
            }
        }
    }

    /// <summary>Reads values from <paramref name="line"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReadCultureKey(this IEnumerable<KeyValuePair<string, MarkedText>> line, out MarkedText culture, out MarkedText key)
    {
        // Init
        culture = default; key = default;
        // Heap-free enumeration
        if (line is IList<KeyValuePair<string, MarkedText>> list)
        {
            // Get count
            int count = list.Count;
            // Visit key-values. Last stated value overrides preceding values.
            for (int i = 0; i < count; i++)
            {
                var kv = list[i];
                if (kv.Key == "Culture") culture = kv.Value;
                else if (kv.Key == "Key") key = kv.Value;
            }
        }
        // Enumerator
        else
        {
            // Visit key-values. Last stated value overrides preceding values.
            foreach (var kv in line)
            {
                if (kv.Key == "Culture") culture = kv.Value;
                else if (kv.Key == "Key") key = kv.Value;
            }
        }
    }

}


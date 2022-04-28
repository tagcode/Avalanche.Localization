// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Diagnostics.CodeAnalysis;
using Avalanche.Utilities;

/// <summary>
/// Compares enumerables of key-value pairs. 
/// 
/// If key re-occurs, the last occuring value is regarded effective.
/// 
/// Null values are not regarded and are considered compare-equals for not having key in the counter-part.
/// </summary>
public class LocalizationLineEqualityComparer : IEqualityComparer<IEnumerable<KeyValuePair<string, MarkedText>>>
{
    /// <summary>Compares all key-values</summary>
    static LocalizationLineEqualityComparer all = new LocalizationLineEqualityComparer();
    /// <summary>Compares keys "Key"</summary>
    static LocalizationLineEqualityComparer key = new LocalizationLineEqualityComparer("Key");
    /// <summary>Compares keys "Culture", "Key"</summary>
    static LocalizationLineEqualityComparer cultureKey = new LocalizationLineEqualityComparer("Culture", "Key");

    /// <summary>Compares all key-values</summary>
    public static LocalizationLineEqualityComparer All => all;
    /// <summary>Compares keys "Key"</summary>
    public static LocalizationLineEqualityComparer Key => key;
    /// <summary>Compares keys "Culture", "Key"</summary>
    public static LocalizationLineEqualityComparer CultureKey => cultureKey;

    /// <summary>Keys to compare. If null, then all keys are compared.</summary>
    protected string[]? keysToCompare;
    /// <summary>Keys to compare. If null, then all keys are compared.</summary>
    protected Dictionary<string, int>? keyToIndice;
    /// <summary>Keys to compare. If null, then all keys are compared.</summary>
    public string[]? KeysToCompare => keysToCompare;

    /// <summary>Create line comparer</summary>
    /// <param name="keysToCompare">keys to compare, if empty all keys are compared.</param>
    public LocalizationLineEqualityComparer(params string[]? keysToCompare)
    {
        this.keysToCompare = keysToCompare == null ? null : keysToCompare.Length == 0 ? null : keysToCompare;
        this.keyToIndice = null;
        if (this.keysToCompare != null)
        {
            // Create map
            this.keyToIndice = new Dictionary<string, int>(this.KeysToCompare!.Length);
            // Assign key->index
            for (int i=0; i<this.keysToCompare.Length; i++) keyToIndice[this.keysToCompare[i]] = i;
        }
    }

    /// <summary>empty line</summary>
    static KeyValuePair<string, MarkedText>[] empty => Array.Empty<KeyValuePair<string, MarkedText>>();

    /// <summary>Compare <paramref name="x"/> to <paramref name="y"/></summary>
    public bool Equals(IEnumerable<KeyValuePair<string, MarkedText>>? x, IEnumerable<KeyValuePair<string, MarkedText>>? y)
    {
        // Handle nulls
        if (x == null && y == null) return true;
        if (x == null) x = empty;
        if (y == null) y = empty;
        // Put here values (recurring value is overwritten)
        StructList6<KeyValuePair<string, string>> x_ = new(), y_ = new();
        // Add all key-values
        if (keyToIndice == null)
        {
            // Add x values
            foreach (var kv in x) 
            {
                // Nulls are not applied
                if (kv.Key == null || kv.Value == default) continue;
                // Find prev ix
                int ix = -1;
                for (int j = 0; j < x_.Count; j++) if (x_[j].Key == kv.Key) { ix = j; break; }
                // Add or replace
                if (ix < 0) x_.Add(new KeyValuePair<string, string>(kv.Key, kv.Value));
                else x_[ix] = new KeyValuePair<string, string>(kv.Key, kv.Value);
            }
            // Add y values
            foreach (var kv in y)
            {
                // Nulls are not applied
                if (kv.Key == null || kv.Value == default) continue;
                // Find prev ix
                int ix = -1;
                for (int j = 0; j < y_.Count; j++) if (y_[j].Key == kv.Key) { ix = j; break; }
                // Add or replace
                if (ix < 0) y_.Add(new KeyValuePair<string, string>(kv.Key, kv.Value));
                else y_[ix] = new KeyValuePair<string, string>(kv.Key, kv.Value);
            }
        }
        // Add specific all key-values
        else
        {
            // Add x values
            foreach (var kv in x)
            {
                // Nulls are not applied
                if (kv.Key == null || kv.Value == default) continue;
                // Key is not regarded
                if (!keyToIndice.ContainsKey(kv.Key)) continue;
                // Find prev ix
                int ix = -1;
                for (int j = 0; j < x_.Count; j++) if (x_[j].Key == kv.Key) { ix = j; break; }
                // Add or replace
                if (ix < 0) x_.Add(new KeyValuePair<string, string>(kv.Key, kv.Value));
                else x_[ix] = new KeyValuePair<string, string>(kv.Key, kv.Value);
            }
            // Add y values
            foreach (var kv in y)
            {
                // Nulls are not applied
                if (kv.Key == null || kv.Value == default) continue;
                // Key is not regarded
                if (!keyToIndice.ContainsKey(kv.Key)) continue;
                // Find prev ix
                int ix = -1;
                for (int j = 0; j < y_.Count; j++) if (y_[j].Key == kv.Key) { ix = j; break; }
                // Add or replace
                if (ix < 0) y_.Add(new KeyValuePair<string, string>(kv.Key, kv.Value));
                else y_[ix] = new KeyValuePair<string, string>(kv.Key, kv.Value);
            }
        }
        // Get counts
        int xc = x_.Count, yc = y_.Count;
        // Empty
        if (xc == 0 && yc == 0) return true;
        // Different item count
        if (xc != yc) return false;
        //
        for (int i = 0; i < xc; i++)
        {
            var xkv = x_[i];
            bool matched = false;
            for (int j = 0; j < yc; j++)
            {
                var ykv = y_[j];
                if (xkv.Key == ykv.Key)
                {
                    // Value mismatch
                    if (xkv.Value != ykv.Value) return false;
                    // Value match
                    matched = true;
                    break;
                }
            }
            // Key-value was not matched
            if (!matched) return false;
        }
        // Match
        return true;
    }

    /// <summary>Calculates <paramref name="line"/> hashcode.</summary>
    public int GetHashCode([DisallowNull] IEnumerable<KeyValuePair<string, MarkedText>>? line)
    {
        // Use empty
        if (line == null) line = empty;
        // Init hash
        FNVHash32 hash = new FNVHash32();
        // Hash all key-values
        if (keyToIndice == null)
        {
            // Hash all key-values
            foreach (var kv in line)
            {
                if (kv.Key != null) hash.HashIn(kv.Key.GetHashCode());
                if (kv.Value != default) hash.HashIn(kv.Value.GetHashCode());
            }
        }
        else
        {
            // visit key-values
            foreach (var kv in line)
            {
                // Not compared
                if (!keyToIndice.TryGetValue(kv.Key, out int index)) continue;
                // Hash index
                hash.HashIn(index);
                // Hash in value
                if (kv.Value != default) hash.HashIn(kv.Value.GetHashCode());
            }
        }
        // Return hash
        return hash.Hash;
    }
}


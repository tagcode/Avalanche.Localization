// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Decoration that removes overlaps. Use <see cref="ProviderExtensions_.Concat"/> to append to another provider.</summary>
public class LocalizationLineProviderDistinct : ProviderBase<IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>, IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>
{
    /// <summary></summary>
    static LocalizationLineProviderDistinct instance = new LocalizationLineProviderDistinct();
    /// <summary></summary>
    public static LocalizationLineProviderDistinct Instance => instance;

    /// <summary>Query and concatenate results</summary>
    public override bool TryGetValue(IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> input, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> output)
    {
        // Place results here, maintain order
        StructList10<IEnumerable<KeyValuePair<string, MarkedText>>> result = new(LocalizationLineEqualityComparer.All);
        // Lazy initialized table for detecting duplicates
        HashSet<IEnumerable<KeyValuePair<string, MarkedText>>>? set = null!;
        // Add file
        foreach (IEnumerable<KeyValuePair<string, MarkedText>> _line in input)
        {
            // Create hash set for detecting duplicates
            if (result.Count >= result.StackCount && set == null) { set = new HashSet<IEnumerable<KeyValuePair<string, MarkedText>>>(LocalizationLineEqualityComparer.All); for (int i = 0; i < result.Count; i++) set.Add(result[i]); }
            // Detected duplicate
            if (set == null ? result.Contains(_line) : set.Contains(_line)) continue;
            // Add
            result.Add(_line);
            if (set != null) set.Add(_line);
        }
        // All or some returned results (concat results)
        output = result.ToArray();
        return true;
    }

    /// <summary>Print information</summary>
    public override string ToString() => GetType().Name;
}

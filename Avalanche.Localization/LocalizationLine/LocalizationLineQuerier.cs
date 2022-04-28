// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Executes queries to a source of lines</summary>
public class LocalizationLineQuerier : ProviderBase<(string? culture, string? key), IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>>
{
    /// <summary>Source of lines</summary>
    protected IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines;
    /// <summary>Source of lines</summary>
    public virtual IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> Lines => lines;

    /// <summary></summary>
    public LocalizationLineQuerier(IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines)
    {
        this.lines = lines ?? throw new ArgumentNullException(nameof(lines));
    }

    /// <summary>Query lines</summary>
    /// <param name="query">Query with culture and key constraints. If culture and/or key is null, then constricts less, and returns more lines.</param>
    public override bool TryGetValue((string? culture, string? key) query, out IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines)
    {
        // 
        StructList16<IEnumerable<KeyValuePair<string, MarkedText>>> result = new();
        // Visit lines
        foreach(IEnumerable<KeyValuePair<string, MarkedText>> line in this.lines)
        {
            // Read values
            line.ReadCultureKey(out MarkedText culture, out MarkedText key);
            // Disqualify with culture criteria
            if (query.culture != null && query.culture != culture.AsString) continue;
            // Disqualify with key criteria
            if (query.key != null && query.key != key.AsString) continue;
            // Add to result
            result.Add(line);
        }
        // Return
        lines = result.ToArray();
        return result.Count>0;
    }

    /// <summary>Print information</summary>
    public override string ToString() => $"{GetType().Name}({lines})";
}

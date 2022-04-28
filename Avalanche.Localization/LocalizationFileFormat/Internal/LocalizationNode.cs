// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Internal;
using System.Text;
using Avalanche.Utilities;

/// <summary>Trie node that describes structural localization key-value tree.</summary>
/// <remarks>This class is used in localization file readers.</remarks>
public record LocalizationNode(string? key, MarkedText value, LocalizationNode? parent = null)
{
    /// <summary>Create new node with <paramref name="newValue"/>.</summary>
    public LocalizationNode WithNewValue(MarkedText newValue) => new LocalizationNode(key, newValue, parent);

    /// <summary>Print array</summary>
    public KeyValuePair<string, MarkedText>[] ToArray()
    {
        // Put here
        StructList10<KeyValuePair<string, MarkedText>> list = new();
        // Add levels to list
        for (LocalizationNode? l = this; l != null; l = l.parent)
        {
            // No key
            if (l.key == null) continue;
            // Add
            list.Add(new KeyValuePair<string, MarkedText>(l.key!, l.value));
        }
        // Create array
        return list.ToReverseArray();
    }

    /// <summary>Print information</summary>
    public override string ToString()
    {
        // Put here
        StructList10<KeyValuePair<string, MarkedText>> list = new();
        // Add levels to list
        for (LocalizationNode? l = this; l != null; l = l.parent)
        {
            // No key
            if (l.key == null) continue;
            // Add
            list.Add(new KeyValuePair<string, MarkedText>(l.key, l.value));
        }
        //
        StringBuilder sb = new StringBuilder();
        // Append levels
        for (int i = list.Count - 1; i >= 0; i--)
        {
            // Append
            var kv = list[i];
            sb.Append(kv.Key == null ? "null" : Escaper.Backslash.Escape(kv.Key));
            sb.Append("=");
            sb.Append(Escaper.Backslash.Escape(kv.Value.Text));
            if (i > 0) sb.Append(",");
        }
        // Return 
        return sb.ToString();
    }
}



// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Internal;
using System.Collections;
using Avalanche.Utilities;

/// <summary>Annotates every <see cref="MarkedText"/> to specific filename.</summary>
/// /// <remarks>See <see cref="LocalizationLineExtensions"/> for constructing annotator.</remarks>
public class LocalizationLineFilenameAnnotator : IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>, IDecoration
{
    /// <summary></summary>
    protected IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader;
    /// <summary>Filename to assign</summary>
    protected string? filename;

    /// <summary></summary>
    bool IDecoration.IsDecoration { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    object? IDecoration.Decoree { get => reader; set => throw new InvalidOperationException(); }

    /// <summary></summary>
    /// <param name="reader">Reader to decorate</param>
    /// <param name="filename">FileName to assign</param>
    public LocalizationLineFilenameAnnotator(IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader, string? filename)
    {
        this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        this.filename = filename;
    }

    /// <summary></summary>
    public IEnumerator<IEnumerable<KeyValuePair<string, MarkedText>>> GetEnumerator()
    {
        // List used for working with elements
        List<KeyValuePair<string, MarkedText>> list = new(10);

        foreach (IEnumerable<KeyValuePair<string, MarkedText>> line in reader)
        {
            // Reset list
            list.Clear();
            // Add each element decorated
            foreach (var kv in line)            
                list.Add(new KeyValuePair<string, MarkedText>(kv.Key, kv.Value.SetFileName(filename)));
            // Yield decorated line
            yield return list.ToArray();
        }
    }

    /// <summary>Read lines</summary>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}


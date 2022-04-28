// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Internal;
using System.Collections;
using Avalanche.Utilities;

/// <summary>Adds key-values to intermediate lines.</summary>
/// <remarks>See <see cref="LocalizationLineExtensions"/> for constructing prefixer.</remarks>
public class LocalizationLinePrefixer : IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>, IDecoration
{
    /// <summary></summary>
    protected IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader;
    /// <summary>Key-Values to prefix to each line</summary>
    protected IEnumerable<KeyValuePair<string, MarkedText>>? prefix;

    /// <summary></summary>
    bool IDecoration.IsDecoration { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    object? IDecoration.Decoree { get => reader; set => throw new InvalidOperationException(); }

    /// <summary></summary>
    /// <param name="reader">Reader to decorate</param>
    /// <param name="prefix">Key-Values to prefix to each line</param>
    public LocalizationLinePrefixer(IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> reader, IEnumerable<KeyValuePair<string, MarkedText>>? prefix)
    {
        this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
        this.prefix = prefix;
    }

    /// <summary></summary>
    public IEnumerator<IEnumerable<KeyValuePair<string, MarkedText>>> GetEnumerator()
    {
        foreach (IEnumerable<KeyValuePair<string, MarkedText>> line in reader)
        {
            // Decorate line
            KeyValuePair<string, MarkedText>[] decoratedLine = Avalanche.Utilities.EnumerableExtensions.ConcatToArray(prefix, line);
            // Yield decorated line
            yield return decoratedLine;
        }
    }

    /// <summary>Read lines</summary>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}


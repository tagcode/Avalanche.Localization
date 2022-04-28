// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Internal;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Decorates query provider of <typeparamref name="T"/> to search with fallback cultures.</summary>
public class CultureFallbackDecoration<T> : ProviderBase<(string? culture, string? key), T>, IDecoration
{
    /// <summary></summary>
    protected IProvider<(string? culture, string? key), T> linesProvider;
    /// <summary></summary>
    protected IProvider<string, string[]> source;
    /// <summary></summary>
    bool IDecoration.IsDecoration { get => true; set => throw new InvalidOperationException(); }
    /// <summary></summary>
    object? IDecoration.Decoree { get => source; set => throw new InvalidOperationException(); }

    /// <summary></summary>
    public CultureFallbackDecoration(IProvider<(string? culture, string? key), T> linesProvider, IProvider<string, string[]> fallbackCultureProvider)
    {
        this.linesProvider = linesProvider ?? throw new ArgumentNullException(nameof(linesProvider));
        this.source = fallbackCultureProvider ?? throw new ArgumentNullException(nameof(fallbackCultureProvider));
    }

    /// <summary>Try get lines with any of the fallback cultures</summary>
    public override bool TryGetValue((string? culture, string? key) query, out T lines)
    {
        // Get fallback cultures
        if (query.culture == null || !source.TryGetValue(query.culture, out string[] cultures)) { lines = default!; return false; }
        // Try with each cultures
        foreach (string culture in cultures)
        {
            // Found lines
            if (linesProvider.TryGetValue((culture, query.key), out lines)) return true;
        }
        // No lines
        lines = default!;
        return false;
    }
}

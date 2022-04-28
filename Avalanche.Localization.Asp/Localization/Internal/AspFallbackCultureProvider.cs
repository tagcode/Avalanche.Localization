// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Internal;
using Avalanche.Utilities.Provider;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

/// <summary>Adapts <![CDATA[IOptions<RequestLocalizationOptions>]]> into fallback culture provider</summary>
public class AspFallbackCultureProvider : ProviderBase<string, string[]>
{
    /// <summary>Asp options</summary>
    protected IOptions<RequestLocalizationOptions>? aspOptions;

    /// <summary>Create without asp options.</summary>
    public AspFallbackCultureProvider()
    {
        this.aspOptions = null;
    }

    /// <summary>Create with <paramref name="aspOptions"/>.</summary>
    public AspFallbackCultureProvider(IOptions<RequestLocalizationOptions> aspOptions)
    {
        this.aspOptions = aspOptions;
    }

    /// <summary>Get fallback cultures</summary>
    public override bool TryGetValue(string culture, out string[] fallbackCultures)
    {
        // Get options
        RequestLocalizationOptions? options = aspOptions?.Value;
        // No options - Use default
        if (options == null) return FallbackCultureProvider.Default.TryGetValue(culture, out fallbackCultures);
        // Fallback is disabled - Use no-fallback provider
        if (!options.FallBackToParentUICultures) return FallbackCultureProvider.NoFallback.TryGetValue(culture, out fallbackCultures);
        // Get fallback culture
        string? fallbackCulture = options.DefaultRequestCulture?.UICulture?.Name;
        // Fallback to invariant culture
        if (fallbackCulture == null || fallbackCulture == "") return FallbackCultureProvider.Invariant.TryGetValue(culture, out fallbackCultures);
        // Get-or-create fallback culture provider
        IProvider<string, string[]> provider = FallbackCultureProvider.Get(fallbackCulture);
        // Use culture provider
        return provider.TryGetValue(culture, out fallbackCultures);
    }
}

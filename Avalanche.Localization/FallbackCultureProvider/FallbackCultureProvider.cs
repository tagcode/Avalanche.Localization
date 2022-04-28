// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Provides fallback cultures: culture, parent cultures, fallback cultures, invariant culture</summary>
/// <remarks>The instantiated provider should be decorated with cache, and this must be done by the initializer.</remarks>
public class FallbackCultureProvider : ProviderBase<string, string[]>
{
    /// <summary>The netural language from application's <![CDATA[[assembly: System.Resources.NeutralResourcesLanguage("")]]]></summary>
    public static string? ApplicationNeutralResourcesLanguage => (Assembly.GetEntryAssembly()?.GetCustomAttribute(typeof(NeutralResourcesLanguageAttribute)) as NeutralResourcesLanguageAttribute)?.CultureName;
    /// <summary>Fallback culture provider factory</summary>
    static readonly IProvider<string?, IProvider<string, string[]>> fallbackCultureProviderFactory = Providers.Func<string?, IProvider<string, string[]>>((string? fallbackCulture) => new FallbackCultureProvider(fallbackCulture).CachedNullableKey());
    /// <summary>Provider returns: culture, parent cultures, application assembly's <![CDATA[[assembly: System.Resources.NeutralResourcesLanguage()]]]>, invariant culture</summary>
    static readonly IProvider<string, string[]> instance = new FallbackCultureProvider(ApplicationNeutralResourcesLanguage).Cached();
    /// <summary>Provider returns: culture, parent cultures, "en", invariant culture</summary>
    static readonly IProvider<string, string[]> en = Get("en");
    /// <summary>Provider returns: culture, parent cultures, "en", invariant culture</summary>
    static readonly IProvider<string, string[]> invariant = Get("");
    /// <summary>Provider returns: culture</summary>
    static readonly IProvider<string, string[]> noFallbackCulture = Providers.Func((string culture) => new string[] { culture }).Cached();

    /// <summary>Fallback culture provider factory</summary>
    public static IProvider<string, string[]> Get(string? fallbackCulture) => fallbackCultureProviderFactory[fallbackCulture];
    /// <summary>Provider returns: culture, parent cultures, application assembly's <![CDATA[[assembly: System.Resources.NeutralResourcesLanguage()]]]>, invariant culture</summary>
    public static IProvider<string, string[]> Default => instance;
    /// <summary>Provider returns: culture, parent cultures, "en", invariant culture</summary>
    public static IProvider<string, string[]> Invariant => invariant;
    /// <summary>Provider returns: culture, parent cultures, "en", invariant culture</summary>
    public static IProvider<string, string[]> En => en;
    /// <summary>Provider returns: culture</summary>
    public static IProvider<string, string[]> NoFallback => noFallbackCulture;

    /// <summary>Optional fallback culture, e.g. "en"</summary>
    protected string? fallbackCulture;

    /// <summary>Create culture provider that returns parent cultures and fallback cultures</summary>
    /// <param name="fallbackCulture">Optional fallback culture, e.g. "en"</param>
    public FallbackCultureProvider(string? fallbackCulture = default)
    {
        this.fallbackCulture = fallbackCulture;
    }

    /// <summary>Returns: culture, parent cultures, invariant culture, fallback culture</summary>
    public override bool TryGetValue(string culture, out string[] fallbackCultures)
    {
        // Null
        if (culture == null) { fallbackCultures = null!; return false; }
        // Place here cultures
        StructList8<string> cultures = new StructList8<string>();

        if (culture != null)
            try
            {
                // Move towards root culture (ignore invariant culture, for now)
                for (CultureInfo? c = CultureInfo.GetCultureInfo(culture); !string.IsNullOrEmpty(c?.Name); c = c.Parent)
                {
                    if (cultures.Contains(c.Name)) break;
                    cultures.Add(c.Name);
                }
            }
            catch (CultureNotFoundException)
            {
                cultures.AddIfNew(culture);
            }

        // Add fallback culture
        if (fallbackCulture != null)
            try
            {
                // Move towards root culture (ignore invariant culture, for now)
                for (CultureInfo? c = CultureInfo.GetCultureInfo(fallbackCulture); !string.IsNullOrEmpty(c?.Name); c = c.Parent)
                {
                    if (cultures.Contains(c.Name)) break;
                    cultures.Add(c.Name);
                }
            }
            catch (CultureNotFoundException)
            {
                cultures.AddIfNew(fallbackCulture);
            }

        // Add invariant culture
        cultures.AddIfNew("");

        // Return cultures
        fallbackCultures = cultures.ToArray();
        return true;
    }
}

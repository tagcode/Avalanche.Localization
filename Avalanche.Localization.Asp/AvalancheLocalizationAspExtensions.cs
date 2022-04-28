// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Avalanche.Utilities;

/// <summary>Extension methods for Asp.</summary>
public static class AvalancheLocalizationAspExtensions
{
    /// <summary>Adds service descriptors that use <see cref="Microsoft.AspNetCore.Builder.RequestLocalizationOptions"/></summary>
    public static IServiceCollection AddAvalancheLocalizationAspSupport(this IServiceCollection serviceCollection)
    {
        // Replace service
        serviceCollection.AddIfNew(AspServiceDescriptors.Instance.AspFallbackCultureProvider);
        // Return service collection
        return serviceCollection;
    }

    /// <summary>Print as html</summary>
    public static LocalizedString Localize(this ILocalizedText? text)
    {
        // No text
        if (text == null) return new LocalizedString("", "", true);
        // Print
        string print = text.Print(null);
        // No print
        if (print == null) return new LocalizedString(text.Key, "", true);
        // Adapt to html
        LocalizedString html = new LocalizedString(text.Key, print, false);
        // Return
        return html;
    }

    /// <summary>Print as html</summary>
    /// <param name="arguments"></param>
    public static LocalizedString Localize(this ILocalizedText? text, params object[]? arguments)
    {
        // No text
        if (text == null) return new LocalizedString("", "", true);
        // Print
        string print = text.Print(arguments);
        // No print
        if (print == null) return new LocalizedString(text.Key, "", true);
        // Adapt to html
        LocalizedString html = new LocalizedString(text.Key, print, false);
        // Return
        return html;
    }

    /// <summary>Print as html</summary>
    public static LocalizedHtmlString LocalizeHtml(this ILocalizedText? text)
    {
        // No text
        if (text == null) return new LocalizedHtmlString("", "", true);
        // Print
        string print = text.Print(null);
        // No print
        if (print == null) return new LocalizedHtmlString(text.Key, "", true);
        // Adapt to html
        LocalizedHtmlString html = new LocalizedHtmlString(text.Key, print, false);
        // Return
        return html;
    }

    /// <summary>Print as html</summary>
    /// <param name="arguments"></param>
    public static LocalizedHtmlString LocalizeHtml(this ILocalizedText? text, params object[]? arguments)
    {
        // No text
        if (text == null) return new LocalizedHtmlString("", "", true, arguments);
        // Print
        string print = text.Print(arguments);
        // No print
        if (print == null) return new LocalizedHtmlString(text.Key, "", true, arguments);
        // Adapt to html
        LocalizedHtmlString html = new LocalizedHtmlString(text.Key, print, false, arguments);
        // Return
        return html;
    }
}

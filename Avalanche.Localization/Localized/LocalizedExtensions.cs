// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;

/// <summary>Extension methods for <see cref="ILocalized{T}"/></summary>
public static class LocalizedExtensions_
{
    /// <summary>If any argument implements <see cref="ILocalizable{T}"/> then localize to <paramref name="culture"/>.</summary>
    /// <param name="mutateArgumentsReference">If true then <paramref name="arguments"/> can be modified, if false, then a new array is created</param>
    /// <returns>Either <paramref name="arguments"/> a new array with localized versions of arguments.</returns>
    public static object?[]? LocalizeArguments(object?[]? arguments, string? culture, bool mutateArgumentsReference)
    {
        // No arguments
        if (arguments == null) return arguments;
        // No assigned culture
        if (culture == null) return arguments;
        // Place here new array
        object?[]? result = null;
        // Test each argument
        for (int i=0; i<arguments.Length; i++)
        {
            // Get argument
            object? argument = arguments[i];
            // No argument
            if (argument == null) continue;
            // Get current culture of the argument
            string? currentCulture = (argument as ICultureProvider)?.Culture;
            // Already same culture
            if (currentCulture != null && currentCulture == culture) continue;
            // Place here localized
            ITemplatePrintableBase? localized = null;
            string localizedCulture = null!;
            // Try localize
            if (argument is ILocalizable<ILocalizedText> localizable1 && localizable1.TryLocalize(culture, out ILocalizedText localizedText, out localizedCulture)) localized = localizedText;
            else if (argument is ILocalizable<ITemplateText> localizable2 && localizable2.TryLocalize(culture, out ITemplateText text2, out localizedCulture)) localized = text2;
            else if (argument is ILocalizable<ITemplateFormatPrintable> localizable3 && localizable3.TryLocalize(culture, out ITemplateFormatPrintable text3, out localizedCulture)) localized = text3;
            else if (argument is ILocalizable<ITemplatePrintable> localizable4 && localizable4.TryLocalize(culture, out ITemplatePrintable text4, out localizedCulture)) localized = text4;
            // Could not localize to 'culture'
            if (localized == null) continue;
            // The returned localized text is of same culture as argument was to begin with. No need to re-create arguments array
            if (localizedCulture != null && currentCulture != null && currentCulture == localizedCulture) continue;
            // Recreate array, and assign localized version
            if (result == null) result = mutateArgumentsReference ? arguments : (object?[]?)arguments.Clone();
            // Assign localized version
            result![i] = localized;
        }
        // Return
        return result ?? arguments;
    }
}

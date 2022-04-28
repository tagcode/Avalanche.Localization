// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;
using Avalanche.Template.Internal;
using Avalanche.Utilities;

/// <summary>Emplacement extensions</summary>
public static class LocalizationEmplacementExtensions
{

    /// <summary>Localize emplacements to <paramref name="culture"/>.</summary>
    public static ITemplateParameterEmplacement[] LocalizeParameterEmplacements(ITemplateParameterEmplacement[] emplacements, string culture)
    {
        // No culture
        if (culture == null) return emplacements;
        // Place here adapted list
        StructList8<ITemplateParameterEmplacement> list = new();
        // Was any emplacement localized
        bool readjusted = false;
        // Try ajust each emplacement
        foreach(ITemplateParameterEmplacement emplacement in emplacements)
        {
            // Localize if localizable
            ITemplateText localizedEmplacement = (emplacement.Emplacement as ILocalizable<ILocalizedText>)?.Localize(culture)?.Value ?? emplacement.Emplacement;
            // Was readjustment
            if (localizedEmplacement != emplacement.Emplacement)
            {
                // Flag modification
                readjusted = true;
                // Add to list
                list.Add(new TemplateParameterEmplacement(emplacement.ParameterName, localizedEmplacement));
            }
            else list.Add(emplacement);
        }
        // Return
        return readjusted ? list.ToArray() : emplacements;
    }

    /// <summary>Print <paramref name="externalArguments"/> into <paramref name="internalArguments"/>.</summary>
    /// <param name="emplacementMapping"></param>
    /// <param name="formatProvider">Format provider to print emplacements with</param>
    /// <param name="externalArguments">Input arguments</param>
    /// <param name="internalArguments">Output where internal arguments are printed to, allocated by caller</param>
    /// <param name="emplacementArguments">Temporary array allocated by caller</param>
    /// <param name="emplacementTexts">Array where localized+pluralized version of each emplacement text is placed</param>
    /// <param name="culture">Culture to localize emplacementables to</param>
    public static void PrintToInternalArguments(TemplateEmplacementMapping emplacementMapping, IFormatProvider? formatProvider, object?[]? externalArguments, object?[] internalArguments, object?[] emplacementArguments, object[]? emplacementTexts, string? culture)
    {
        // Assert enough internal arguments
        if (internalArguments.Length < emplacementMapping.OriginalParameters.Count) throw new ArgumentException(nameof(internalArguments));
        // externalArguments array index
        int externalArgumentIndex = 0;
        // Index in 'emplacementTexts'
        int emplacementTextsIndex = 0;
        // Get reference to internal parameters
        var internalParameters = emplacementMapping.OriginalParameters;
        // Assign arguments
        for (int i = 0; i < internalArguments.Length; i++)
        {
            // Assign 'null'
            if (i >= internalParameters.Count) { internalArguments[i] = null; continue; }
            // Get parameter info
            TemplateEmplacementMapping.ParameterMapping parameterInfo = internalParameters[i];
            // No emplacement, copy value as is
            if (parameterInfo.EmplacementAssignment == null)
            {
                internalArguments[i] = externalArguments != null && externalArguments.Length >= externalArgumentIndex ? externalArguments[externalArgumentIndex] : null;
                externalArgumentIndex++;
                continue;
            }
            // Clear rest of emplacement arguments
            for (int j = 0; j < emplacementArguments.Length; j++) emplacementArguments[j] = null;
            // Process emplacement arguments
            foreach (TemplateEmplacementMapping.ParameterMapping parameterMapping in parameterInfo.CorrespondingEmplacementParameters)
            {
                // 
                emplacementArguments[parameterMapping.EmplacementParameterIndex] = externalArguments != null && externalArguments.Length >= externalArgumentIndex ? externalArguments[externalArgumentIndex] : null;
                //
                externalArgumentIndex++;
            }
            // Get emplacement
            ITemplateText emplacementText = parameterInfo.EmplacementAssignment.Emplacement;
            // Localizable
            if (culture != null && emplacementText is ILocalizable<ILocalizedText> localizable)
            {
                // Change language
                if (emplacementText is not ICultureProvider cultureProvider || cultureProvider.Culture != culture) emplacementText = localizable.Localize(culture)?.Value ?? emplacementText;
            }
            // Pluralize emplacement
            if (emplacementText is ILocalizedText localizedText) emplacementText = localizedText.Pluralize(formatProvider, emplacementArguments);
            // Assign emplacementText
            if (emplacementTexts != null && emplacementTextsIndex<emplacementTexts.Length) emplacementTexts[emplacementTextsIndex++] = emplacementText;
            // Print emplacement
            string emplacementPrint = emplacementText.Print(formatProvider, emplacementArguments);
            // Assign value
            internalArguments[i] = emplacementPrint;
        }
    }


}

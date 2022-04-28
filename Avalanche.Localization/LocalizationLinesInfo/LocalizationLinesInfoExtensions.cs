// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Internal;
using System.Buffers;
using Avalanche.Localization.Pluralization;
using Avalanche.Template;
using Avalanche.Utilities;

/// <summary>Extension methods for <see cref="ILocalizationLinesInfo"/>.</summary>
public static class LocalizationLinesInfoExtensions
{
    /// <summary>Get-or-create errors list.</summary>
    public static IList<ILocalizationError> GetOrCreateErrors(this ILocalizationLinesInfo localizationLinesInfo) => localizationLinesInfo.Errors ?? (localizationLinesInfo.Errors = new List<ILocalizationError>(1));

    /// <summary>Get all the rulesets used in <paramref name="lineInfo"/>.</summary>
    public static string[] RuleSets(this ILocalizationLinesInfo lineInfo)
    {
        // Place here
        StructList4<string> rulesets = new();
        // Add new
        if (lineInfo.Parameters != null)
            foreach (ILocalizationLinesParameter pi in lineInfo.Parameters)
                if (pi.PluralRuleInfos != null)
                    foreach (PluralRuleInfo pri in pi.PluralRuleInfos)
                        if (!string.IsNullOrEmpty(pri.RuleSet))
                            rulesets.AddIfNew(pri.RuleSet);
        // Return
        return rulesets.ToArray();
    }

    /// <summary>Get Parameter info by name</summary>
    public static bool TryGetValue(this IEnumerable<ILocalizationLinesParameter> parameters, string parameterName, out ILocalizationLinesParameter parameterInfo)
    {
        // No parameters
        if (parameters != null)
            foreach (ILocalizationLinesParameter _parameterInfo in parameters)
                if (_parameterInfo.Name == parameterName) { parameterInfo = _parameterInfo; return true; }
        // No parameter
        parameterInfo = null!;
        return false;
    }

    /// <summary>Choose pluralized variant</summary>
    /// <param name="localizationLinesInfo"></param>
    /// <param name="formatProvider"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    public static ITemplateText Pluralize(this ILocalizationLinesInfo localizationLinesInfo, IFormatProvider? formatProvider, object?[]? arguments)
    {
        // No pluralized parameters
        if (localizationLinesInfo.PluralizedParameterCount == 0 || localizationLinesInfo.Plurals == null || localizationLinesInfo.Plurals.Count == 0) return localizationLinesInfo.Default;
        // Rent for pluralized parameters
        char[] rental = ArrayPool<char>.Shared.Rent(4096);
        // As memory
        Memory<char> buf = rental.AsMemory();
        try
        {
            // Plural options which will be excluded
            StructList12<(PluralAssignment[], ITemplateText)> pluralOptions = new(localizationLinesInfo.Plurals);
            // Print pluralized arguments
            for (int i = 0; i < localizationLinesInfo.Parameters.Count; i++)
            {
                // Get parameter
                ILocalizationLinesParameter parameterInfo = localizationLinesInfo.Parameters[i];
                // Get evaluators
                IList<IPluralRulesEvaluator> evaluators = parameterInfo.PluralRuleEvaluators;
                // Get name
                string parameterName = parameterInfo.Name;
                // Not pluralized
                if (parameterInfo.PluralRuleInfos == null || parameterInfo.PluralRuleInfos.Count == 0 || parameterInfo.Index < 0 || evaluators == null || parameterName == null) { continue; }
                // Get format
                ReadOnlyMemory<char> format = parameterInfo.Format == null ? default : parameterInfo.Format.AsMemory();
                // Get argument
                object? argument = arguments == null ? null : parameterInfo.Index >= arguments.Length ? null : arguments[parameterInfo.Index];
                // Print parameter
                Memory<char> buf2 = buf;
                ReadOnlyMemory<char> print = TemplatePrintingExtensions.PrintArgument(formatProvider, format, argument, ref buf2);
                // Evaluate case
                for (int j = 0; j < evaluators.Count; j++)
                {
                    // Create number
                    TextNumber number = new TextNumber(print, formatProvider);
                    // Get applicable rules
                    IPluralRule[]? rules = evaluators[j].Evaluate(number);
                    // No matching rule
                    if (rules == null || rules.Length == 0) continue;

                    // Visit plural cases that have not yet been excluded
                    for (int l = pluralOptions.Count - 1; l >= 0; l--)
                    {
                        // Assignment to be evaluated whether it will be excluded
                        PluralAssignment[] assignments = pluralOptions[l].Item1;
                        // Place parameter name specific assignments
                        PluralAssignment parameterSpecificAssignment = default;
                        // Find parameter name specific assignment
                        for (int n = 0; n < assignments.Length; n++) if (assignments[n].parameterName == parameterName) { parameterSpecificAssignment = assignments[n]; break; }
                        // No assignment for this parameter name
                        if (parameterSpecificAssignment.parameterName == null) continue;
                        // Place here whether case is valid
                        bool validPluralityOption = false;
                        // Visit qualifying matches
                        for (int k = 0; k < rules.Length; k++)
                        {
                            // Get info
                            PluralRuleInfo pluralRuleInfo = rules[k].Info;
                            // Match
                            if (pluralRuleInfo.Case == parameterSpecificAssignment.@case && pluralRuleInfo.Category == parameterSpecificAssignment.category && (parameterSpecificAssignment.culture == null || parameterSpecificAssignment.culture == pluralRuleInfo.Culture)) { validPluralityOption = true; break; }
                        }
                        // Remove case option
                        if (!validPluralityOption) pluralOptions.RemoveAt(l);
                    }
                }
            }
            // Choose text
            ITemplateText text = pluralOptions.Count == 0 ? localizationLinesInfo.Default : pluralOptions[0].Item2;
            // Return choise
            return text;
        }
        finally
        {
            ArrayPool<char>.Shared.Return(rental);
        }

    }
}

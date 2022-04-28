// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Internal;
using System.Collections.Generic;
using Avalanche.Template;
using Avalanche.Utilities;

/// <summary>
/// From pluralized lines, analyses parameter names and indices. 
/// 
/// If there are localization file errors, typos, inconsistensies, tries to put together parameter order and indices 
/// and hopefully produce helpful error messages.
/// </summary>
public class ParameterAnalysis
{
    //// Workarea ////
    /// <summary>Parameters by plural-case</summary>
    internal Dictionary<PluralAssignment[], Line>? lines = null!;
    /// <summary>Parameters by plural-case</summary>
    internal Dictionary<string, Parameter>? parameters = null;
    /// <summary>Parameters by plural-case</summary>
    public Dictionary<PluralAssignment[], Line> Lines => lines ?? (lines = new(ArrayEqualityComparer<PluralAssignment>.Instance!));
    /// <summary>Parameters by plural-case</summary>
    public Dictionary<string, Parameter> Parameters => parameters ?? (parameters = new());

    //// Input ////
    /// <summary></summary>
    public string Culture = null!;
    /// <summary></summary>
    public string Key = null!;
    // /// <summary></summary>
    //public IProvider<PluralRuleInfo, IPluralRule[]>? PluralRuleProvider;
    /// <summary>Map indicating source texts for each plural case</summary>
    public Dictionary<PluralAssignment[], (MarkedText pluralsText, MarkedText ruleSetText)> PluralsTexts = null!;
    /// <summary>Key source text</summary>
    public MarkedText KeyText = null!;

    //// Output ////
    /// <summary>Suggested default line</summary>
    public Line DefaultLine = null!;
    /// <summary>Parameter names</summary>
    public string?[] ParameterNames = null!;
    /// <summary>Errors</summary>
    public List<ILocalizationError> Errors = new();
    /// <summary>Plurals</summary>
    public IList<(PluralAssignment[], ITemplateText)> Plurals = null!;
    /// <summary>Number of pluralized parameters</summary>
    public int PluralizedParameterCount;

    /// <summary></summary>
    public record Line
    {
        /// <summary></summary>
        public PluralAssignment[]? PluralAssignment;
        /// <summary></summary>
        public ITemplateText Text = null!;
        /// <summary></summary>
        public int ParameterCount;
        // /// <summary></summary>
        //public long SortingScore;
    }
    /// <summary></summary>
    public record Parameter
    {
        /// <summary></summary>
        public string? ParameterName;
        /// <summary></summary>
        public int ParameterIndex = -1;
        /// <summary>Parameter is pluralized</summary>
        public bool IsPluralized;

        /// <summary>Sum of position indices in <see cref="Line.Text"/>. Return average with divided with <see cref="OccuranceCount"/></summary>
        public long PositionSum = 0L;
        /// <summary>Number of lines that have this parameter</summary>
        public long OccuranceCount = 0L;

        /// <summary>Avegate index of parameter</summary>
        public double AverageIndex => OccuranceCount <= 0 ? double.MaxValue : ((double)PositionSum) / ((double)OccuranceCount);
    }

    /// <summary>Add <paramref name="pluralAssignments"/> and <paramref name="templateText"/></summary>
    /// <param name="pluralAssignments">Parsed "Plurals", or null for fallback non-pluralized text.</param>
    public void Add(PluralAssignment[]? pluralAssignments, ITemplateText templateText)
    {
        // Find previous name
        if (Lines.TryGetValue(pluralAssignments ?? Array.Empty<PluralAssignment>(), out Line? line))
        {
            //
            (MarkedText, MarkedText) x = default;
            // 
            if (PluralsTexts != null) PluralsTexts.TryGetValue(pluralAssignments ?? Array.Empty<PluralAssignment>(), out x);
            // Duplicate plural assignment
            Errors.Add(new LocalizationError { Code = LocalizationMessageIds.PluralsDuplicateAssignment, Key = Key, Culture = Culture, Message = $"Duplicate Plurals=\"{string.Join(',', pluralAssignments!)} assignment.", Text = x.Item1.HasValue ? x.Item1 : default });
        }
        else
        {
            // Add assignment
            Lines[pluralAssignments ?? Array.Empty<PluralAssignment>()] = line = new Line { PluralAssignment = pluralAssignments, Text = templateText };
        }

        // Count parameters here for this line
        int parameterCount = 0;
        // Update parameter names
        foreach (string? parameterName in templateText.ParameterNames)
        {
            // Null
            if (parameterName == null) continue;
            // Increment parameter count for this line
            parameterCount++;
            // Add parameter
            if (!Parameters.TryGetValue(parameterName, out Parameter? parameter)) Parameters[parameterName] = parameter = new Parameter { ParameterName = parameterName };
            // Assign pluralization
            if (!parameter.IsPluralized) foreach (PluralAssignment pluralAssignment in pluralAssignments ?? Array.Empty<PluralAssignment>()) if (pluralAssignment.parameterName == parameterName) { parameter.IsPluralized = true; break; }
            // Parse parameter name if "BraceNumeric" or "Percent"
            if (templateText.TemplateFormat != null && templateText.TemplateFormat.Name == TemplateFormat.BraceNumeric.Name) if (int.TryParse(parameter.ParameterName, out int parameterIndex)) parameter.ParameterIndex = parameterIndex;
                else if (templateText.TemplateFormat != null && templateText.TemplateFormat.Name == TemplateFormat.Percent.Name) if (int.TryParse(parameter.ParameterName, out int parameterIndex1)) parameter.ParameterIndex = parameterIndex1 - 1;
            // Increment occurance
            parameter.OccuranceCount++;
            // Increment position sum
            foreach (ITemplatePart part in templateText.Breakdown.Parts)
            {
                // Increment position
                if (part is ITemplateTextPart text) parameter.PositionSum += text.Unescaped.Length;
                // Break
                if (part is ITemplatePlaceholderPart placeholderPart && placeholderPart.Parameter != null && placeholderPart.Parameter.Unescaped.AsString() == parameterName) break;
            }
        }

        // Assert there are no duplicate assignments to any of the parameter names
        if (pluralAssignments != null)
            for (int i = 0; i < pluralAssignments.Length; i++)
                for (int j = 0; j < i; j++)
                    if (pluralAssignments[j].parameterName == pluralAssignments[i].parameterName)
                    {
                        //
                        (MarkedText, MarkedText) x = default;
                        // 
                        if (PluralsTexts != null) PluralsTexts.TryGetValue(pluralAssignments ?? Array.Empty<PluralAssignment>(), out x);
                        // 
                        Errors.Add(new LocalizationError { Code = LocalizationMessageIds.PluralsParameterDuplicateAssignment, Key = Key, Culture = Culture, Message = $"Duplicate Plurals=\"{pluralAssignments![i]}\" and Plurals=\"{pluralAssignments[j]}\" assignments on {pluralAssignments[i].parameterName}.", Text = x.Item1.HasValue ? x.Item1 : default });
                    }

        // Add parameter names from plural assignments
        //foreach (PluralAssignment pluralAssignment in pluralAssignments ?? KEY_DEFAULT) if (!Parameters.TryGetValue(pluralAssignment.parameterName, out Parameter? parameter)) Parameters[pluralAssignment.parameterName] = new Parameter { ParameterName = pluralAssignment.parameterName };

        // Assign parameter count for this line
        if (parameterCount > line.ParameterCount) line.ParameterCount = parameterCount;

        // Calculate sorting score
        /*
        {
            // 
            long score = 0;
            // 
            for(int i = 0; i < (pluralAssignments ?? KEY_DEFAULT).Length; i++)
            {
                // Get parameter assignment
                var pa = pluralAssignments![i];
                // Is required
                bool isRequired = true;
                // Estimate optionality
                if (PluralRuleProvider != null && RuleSet != null && Culture != null)
                {
                    //
                    PluralRuleInfo ruleInfo = new PluralRuleInfo(RuleSet, pa.category, Culture, pa.@case, null);
                    // Get rule
                    if (PluralRuleProvider.TryGetValue(ruleInfo, out IPluralRule[] rules) && rules.Length>0)
                    {
                        foreach (IPluralRule rule in rules) if (rule.Info.Required.HasValue && !rule.Info.Required.Value) { isRequired = false; break; }
                    }
                    // Failed to find rule
                    else
                    {
                        //
                        (MarkedText, MarkedText) x = default;
                        // 
                        if (PluralsTexts != null) PluralsTexts.TryGetValue(pluralAssignments ?? KEY_DEFAULT, out x);
                        // 
                        Errors.Add(new LocalizationError { Code = LocalizationMessageIds.PluralRulesNotFound, Key = Key, Culture = Culture, Message = $"Rule=[{ruleInfo}] not found", Text = x.Item2.HasValue ? x.Item2 : x.Item1.HasValue ? x.Item1 : ruleInfo.ToString() });
                    }
                }
                // 
                score += int.MaxValue;
                score += i;
                if (pa.@case == "other") score += 0x4000;
                if (isRequired) score += 0x8000;
            }
            //
            line.SortingScore = score;
        }*/


    }

    /// <summary>Analyse and assign <see cref="ParameterNames"/>, <see cref="Parameters"/>, and <see cref="DefaultLine"/>.</summary>
    public void Analyze()
    {
        // Place here best candidate that has all parameters
        DefaultLine = null!;
        Line bestPluralsLine = null!;
        // Best score
        long bestScore = long.MinValue;

        // Iterate all supplied plural permutation
        foreach (var line in Lines)
        {
            // 
            PluralAssignment[] pluralAssignments = line.Key;
            // Number of parameters not assigned in this line
            int missingParameterCount = Parameters.Count - line.Value.ParameterCount;
            // 
            long score = long.MaxValue - missingParameterCount * 65536;
            // Decrement score for every non-"other" case
            foreach (var pa in pluralAssignments) if (pa.@case != "Other") score--;
            // Update best plurals case
            if (pluralAssignments.Length > 0 && score > bestScore) { bestPluralsLine = line.Value; bestScore = score; }
            // Assign default line
            if (DefaultLine == null && line.Key == Array.Empty<PluralAssignment>()) DefaultLine = line.Value;
        }
        // Best line for parameter reference
        Line bestReferenceLine = (bestPluralsLine != null && bestPluralsLine.ParameterCount == Parameters.Count ? bestPluralsLine : null) ?? DefaultLine ?? bestPluralsLine!;
        // Assign default line
        if (DefaultLine == null) DefaultLine = bestPluralsLine!;

        // Assign parameter indices from best reference line
        if (bestReferenceLine != null && bestReferenceLine.ParameterCount == Parameters.Count)
        {
            // Assign parameter indices
            foreach (var parameterPart in bestReferenceLine.Text.Breakdown.Parameters)
                if (parameterPart != null && Parameters.TryGetValue(parameterPart.Unescaped.AsString(), out Parameter? parameter0) && parameter0.ParameterIndex < 0)
                    parameter0.ParameterIndex = parameterPart.ParameterIndex;
        }
        else
        {
            // Add error "Missing required plurality cases"
            Errors.Add(new LocalizationError { Code = LocalizationMessageIds.PluralsMissingCases, Key = Key, Culture = Culture, Message = $"Missing required plurality cases.", Text = KeyText });
        }

        // Still got unassigned parameter index
        if (!Parameters.AllTrue(p => p.Value.ParameterIndex >= 0))
        {
            // Already assigned parameter indices
            Dictionary<int, Parameter> assignedParameters = new(Parameters.Where(p => p.Value.ParameterIndex >= 0).Select(p => new KeyValuePair<int, Parameter>(p.Value.ParameterIndex, p.Value)));
            // Unassigned parameters (parameter index is unassigned)
            IEnumerable<Parameter> unassignedParameters = Parameters.Select(p => p.Value).Where(p => p.ParameterIndex < 0).OrderBy(p => p.AverageIndex);
            // Assign parameter indices
            foreach (Parameter parameter in unassignedParameters)
            {
                // Find first free parameter index
                for (int i = 0; i < int.MaxValue; i++)
                {
                    // Index is used
                    if (assignedParameters.ContainsKey(i)) continue;
                    // Assign on this index
                    parameter.ParameterIndex = i;
                    assignedParameters[i] = parameter;
                    break;
                }
            }
        }

        // Validate that plural assignments correspond to parameter names
        foreach (var kv in Lines)
            foreach (PluralAssignment pluralAssignment in kv.Key)
                if (!Parameters.ContainsKey(pluralAssignment.parameterName))
                {
                    MarkedText pluralsText = default;
                    if (PluralsTexts != null) if (PluralsTexts.TryGetValue(kv.Key, out (MarkedText, MarkedText) x)) pluralsText = x.Item1;
                    Errors.Add(new LocalizationError { Code = LocalizationMessageIds.PluralsParameterNotFound, Key = Key, Culture = Culture, Message = $"Parameter \"{pluralAssignment.parameterName}\" in Plurals=\"{pluralAssignment}\" is not found.", Text = pluralsText });
                }

        // Assign parameter names array
        ParameterNames = TemplateBreakdownExtensions_.AssignByParameterIndex<Parameter, string>(Parameters.Values, p => p.ParameterIndex, p => p.ParameterName);
        // Update parameter names and parameter indices on template texts
        foreach (Line line in Lines.Values)
        {
            if (!TryAssignParameterNamesAndIndices(line.Text))
            {
                ITemplateBreakdown clone = line.Text.Clone();
                TryAssignParameterNamesAndIndices(clone);
                line.Text = clone;
            }
        }

        // Put Plural assignments to array and sort them.
        Plurals = Lines.Where(kv => kv.Key != Array.Empty<PluralAssignment>() && kv.Value.PluralAssignment != null)/*.OrderBy(kv=>kv.Value.SortingScore)*/.Select(kv => (kv.Value.PluralAssignment!, kv.Value.Text)).ToArray();
        PluralizedParameterCount = Parameters.Where(p => p.Value.IsPluralized).Count();
    }

    /// <summary>Reset</summary>
    public void Clear()
    {
        Culture = null!;
        Key = null!;
        DefaultLine = null!;
        Parameters.Clear();
        ParameterNames = null!;
        Lines.Clear();
        Errors.Clear();
        PluralizedParameterCount = 0;
        //PluralRuleProvider = null!;
        Plurals = null!;

        PluralsTexts = null!;
        KeyText = null!;
    }

    /// <summary>Try assign parameter names and indices to <paramref name="text"/>.</summary>
    public bool TryAssignParameterNamesAndIndices(ITemplateText text)
    {
        // Is writable?
        bool writable = text is IReadOnly @readonly && !@readonly.ReadOnly;
        try
        {
            // Assign ParameterNames
            if (!ArrayEqualityComparer<string?>.Instance.Equals(text.ParameterNames, this.ParameterNames)) { if (!writable) return false; text.ParameterNames = this.ParameterNames; }
            // Assign ParameterIndex
            foreach (ITemplatePart part in text.Breakdown.Parts)
                if (part is ITemplatePlaceholderPart placeholderPart)
                    if (placeholderPart.Parameter != null && this.Parameters.TryGetValue(placeholderPart.Parameter.Unescaped.AsString(), out Parameter? paramter))
                    {
                        // No need for change
                        if (placeholderPart.Parameter.ParameterIndex == paramter.ParameterIndex) continue;
                        // Not writable
                        if (!writable) return false;
                        // Assign value
                        placeholderPart.Parameter.ParameterIndex = paramter.ParameterIndex;
                    }

            // Done
            return true;
        }
        catch (Exception)
        {
            // Failed to write
            return false;
        }
    }
}


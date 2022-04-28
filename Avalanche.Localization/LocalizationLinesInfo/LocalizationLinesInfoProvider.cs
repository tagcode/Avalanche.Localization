// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Localization.Internal;
using Avalanche.Localization.Pluralization;
using Avalanche.Template;
using Avalanche.Tokenizer;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Converts localization lines into <see cref="ILocalizationLinesInfo"/>.</summary>
public class LocalizationLinesInfoProvider : ProviderBase<IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>, IEnumerable<ILocalizationLinesInfo>>
{
    /// <summary>Parses lines into line info that is required for <see cref="LocalizedText"/>.</summary>
    static LocalizationLinesInfoProvider instance = new LocalizationLinesInfoProvider();
    /// <summary>Parses lines into line info that is required for <see cref="LocalizedText"/>.</summary>
    static readonly IProvider<IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>, IEnumerable<ILocalizationLinesInfo>> cached = instance.AsReadOnly().WeakCached();
    /// <summary>Parses lines into line info that is required for <see cref="LocalizedText"/>.</summary>
    public static LocalizationLinesInfoProvider Instance => instance;
    /// <summary>Parses lines into line info that is required for <see cref="LocalizedText"/>.</summary>
    public static IProvider<IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>>, IEnumerable<ILocalizationLinesInfo>> Cached => cached;

    /// <summary>Template format provider</summary>
    protected IProvider<string, ITemplateFormat> templateFormatProvider;
    /// <summary>Parses "Plurals" values info <see cref="PluralsInfo"/>.</summary>
    protected IProvider<string, PluralsInfo> pluralsInfoProvider;
    /// <summary>Provides <see cref="IPluralRulesEvaluator"/></summary>
    protected IProvider<PluralRuleInfo, PluralRulesEvaluator> pluralRuleEvaluatorProvider;
    /// <summary>Queries and caches rules</summary>
    protected IProvider<PluralRuleInfo, IPluralRule[]> pluralRuleProvider;
    /// <summary>Handle localization error</summary>
    protected Action<ILocalizationError>? errorHandler;

    /// <summary>Template format provider</summary>
    public virtual IProvider<string, ITemplateFormat> TemplateFormatProvider => templateFormatProvider;
    /// <summary>Parses "Plurals" values info <see cref="PluralsInfo"/>.</summary>
    public virtual IProvider<string, PluralsInfo> PluralsInfoProvider => pluralsInfoProvider;
    /// <summary>Queries and caches rules</summary>
    public virtual IProvider<PluralRuleInfo, IPluralRule[]> PluralRuleProvider => pluralRuleProvider;
    /// <summary>Handle localization error</summary>
    public virtual Action<ILocalizationError>? ErrorHandler => errorHandler;

    /// <summary>Line info</summary>
    /// <param name="templateFormatProvider">Template format provider</param>
    /// <param name="pluralsInfoProvider">Parses "Plurals" values info <see cref="PluralsInfo"/>.</param>
    /// <param name="pluralRuleProvider"></param>
    /// <param name="errorHandler"></param>
    public LocalizationLinesInfoProvider(
        IProvider<string, ITemplateFormat>? templateFormatProvider = default,
        IProvider<string, PluralsInfo>? pluralsInfoProvider = default,
        IProvider<PluralRuleInfo, IPluralRule[]>? pluralRuleProvider = default,
        Action<ILocalizationError>? errorHandler = default)
    {
        this.templateFormatProvider = templateFormatProvider ?? TemplateFormats.All.ByName;
        this.pluralsInfoProvider = pluralsInfoProvider ?? PluralsInfo.Create;
        this.pluralRuleEvaluatorProvider = Providers.Func<PluralRuleInfo, PluralRulesEvaluator>(TryGetPluralRulesEvaluator).ValueResultCaptured().Cached().ValueResultOpened();
        this.pluralRuleProvider = pluralRuleProvider ?? PluralRulesProvider.Instance;
        this.errorHandler = errorHandler;
    }

    /// <summary>Parses plural rules</summary>
    protected virtual bool TryGetPluralRulesEvaluator(PluralRuleInfo pluralRuleInfo, out PluralRulesEvaluator pluralRuleEvaluator)
    {
        // Try get rules
        if (this.pluralRuleProvider.TryGetValue(pluralRuleInfo, out IPluralRule[] rules0)) { pluralRuleEvaluator = new PluralRulesEvaluator(rules0); return true; }
        // Try parse rules
        if (!string.IsNullOrEmpty(pluralRuleInfo.RuleSet) && PluralRuleExpressionParsers.RuleParser.TryGetValue(pluralRuleInfo.RuleSet.AsMemory(), out IPluralRule[] rules1)) { pluralRuleEvaluator = new PluralRulesEvaluator(rules1); return true; }
        //
        pluralRuleEvaluator = null!;
        return false;
    }

    /// <summary>Process error</summary>
    /// <param name="omitIfExists">If false, omits error if error of same type has already been processed.</param>
    void HandleError(ILocalizationLinesInfo lineInfo, bool omitIfExists, string? key, string? culture, int code, string message, MarkedText text)
    {
        // Omit because code already exists
        if (omitIfExists && lineInfo.Errors.ContainsCode(code)) return;
        // Create error
        ILocalizationError error = new LocalizationError { Key = key!, Culture = culture!, Code = code, Message = message, Text = text };
        // Add to errors
        lineInfo.GetOrCreateErrors().Add(error);
        // Process error
        try { var _errorHandler = ErrorHandler; if (_errorHandler != null) _errorHandler(error); } catch (Exception) { }
    }

    /// <summary></summary>
    public override bool TryGetValue(IEnumerable<IEnumerable<KeyValuePair<string, MarkedText>>> lines, out IEnumerable<ILocalizationLinesInfo> resultLines)
    {
        // Init
        var result = new Dictionary<(string culture, string key), LocalizationLinesInfo>();
        //
        foreach (var line in lines)
        {
            // Read args
            line.ReadValues(out MarkedText pluralRulesText, out MarkedText cultureText, out MarkedText key, out MarkedText pluralsText, out MarkedText templateFormatText, out MarkedText text);
            // No key
            if (!key.HasValue) continue;
            // Create line key
            (string culture, string key) lineKey = (cultureText.AsString, key.AsString);
            // Get-or-create line info
            if (!result.TryGetValue(lineKey, out LocalizationLinesInfo? lineInfo)) result[lineKey] = lineInfo = new LocalizationLinesInfo { Culture = lineKey.culture, Key = lineKey.key, Parameters = new List<ILocalizationLinesParameter>() };

            try
            {
                //
                lineInfo.KeyText = key;
                // No text
                if (!text.HasValue) { text = ""; HandleError(lineInfo, true, key, cultureText, LocalizationMessageIds.NoText, "No \"Text\"", text.HasPosition ? text : key); }
                // No culture
                if (!cultureText.HasValue) { cultureText = ""; HandleError(lineInfo, true, key, cultureText, LocalizationMessageIds.NoCulture, "No \"Culture\". Fallback to invariant culture \"\".", cultureText.HasPosition ? cultureText : key); }
                // Add plural rules
                string pluralRules = pluralRulesText.AsString;

                // No template format
                if (!templateFormatText.HasValue) {
                    // No template format
                    HandleError(lineInfo, true, key, cultureText, LocalizationMessageIds.NoTemplateFormat, $"No \"TemplateFormat\"", templateFormatText.HasPosition ? templateFormatText : key);
                    // 
                    templateFormatText = "Detect"; 
                }
                // 
                ITemplateFormat? templateFormat = null!;
                // Get template format
                if (templateFormatText.HasValue && !templateFormatProvider.TryGetValue(templateFormatText.AsString, out templateFormat)) 
                {
                    // Template format not found
                    HandleError(lineInfo, true, key, cultureText, LocalizationMessageIds.TemplateFormatNotFound, $"\"TemplateFormat\"=\"{templateFormatText.AsString}\" not found", templateFormatText);
                    // Assign parameterless for fallback
                    if (!templateFormatProvider.TryGetValue("Detect", out templateFormat)) templateFormat = TemplateFormat.Parameterless;
                }
                // No template format in localization file, fallback to parameterless
                if (templateFormat == null) { HandleError(lineInfo, true, key, cultureText, LocalizationMessageIds.NoTemplateFormat, $"No \"TemplateFormat\"", templateFormatText.HasPosition ? templateFormatText : key); templateFormat = TemplateFormat.Parameterless; }

                // Place here template text
                ITemplateBreakdown templateText;
                // Parse template text
                if (templateFormat.Breakdown.TryGetValue(text, out templateText))
                {
                    // Convert errors
                    foreach ((int index, int length, ReadOnlyMemory<char>? malformed) error in templateText.Breakdown.MalformedParts())
                    {
                        if (!error.malformed.HasValue) continue;

                        MarkedText tokenText = text.Slice(error.index, error.length);

                        HandleError(lineInfo, false, key, cultureText, LocalizationMessageIds.TextMalformed, $"Malformed Text=\"{text.AsString}\"", tokenText);
                    }
                    // Add parameters
                    foreach (var parameter in templateText.Parameters)
                    {
                        // 
                        if (parameter == null) continue;
                        // Get parameter name
                        string parameterName = parameter.Unescaped.AsString();
                        // Intern short name '0' .. '9'
                        if (parameterName.Length == 1) parameterName = String.Intern(parameterName);
                        // Get-or-create parameter info
                        if (!lineInfo.Parameters.TryGetValue(parameterName, out ILocalizationLinesParameter? parameterInfo))
                            lineInfo.Parameters.Add(parameterInfo = new LocalizationLinesParameter { Name = parameterName, PluralRuleInfos = new List<PluralRuleInfo>() });

                        // Assign format
                        foreach (ITemplatePlaceholderPart placeholder in templateText.Placeholders)
                            if (placeholder.Parameter == parameter) { parameterInfo.Format = placeholder.Formatting?.Unescaped.AsString(); break; }
                    }
                }
                // Parse failed
                else
                {
                    HandleError(lineInfo, false, key, cultureText, LocalizationMessageIds.TextParseFailed, $"Parse failed \"Text\"=\"{text.AsString}\"", text);
                    templateText = new ParameterlessText(text);
                }

                // Parse "Plurals=parameter:category:case[:culture],..."
                if (pluralsText.HasValue)
                {
                    // Parse plurals (Do not use cached info here, the new constructed 'plurals' will be modified)
                    if (PluralsInfo.Create.TryGetValue(pluralsText!, out PluralsInfo plurals))
                    {
                        // Add possible errors
                        if (plurals.Errors != null && plurals.Errors.Length > 0)
                            foreach (MalformedToken malformedToken in plurals.Errors)
                            {
                                MarkedText tokenText = pluralsText.Slice(malformedToken.Memory.Index(), malformedToken.Memory.Length);
                                HandleError(lineInfo, false, key, cultureText, LocalizationMessageIds.PluralsParseFailed, $"Malformed Plurals=\"{plurals}\"", pluralsText);
                            }
                    }
                    // Parsed
                    else
                    {
                        // Add error
                        HandleError(lineInfo, false, key, cultureText, LocalizationMessageIds.PluralsParseFailed, $"Malformed Plurals=\"{plurals}\"", pluralsText);
                        continue;
                    }

                    // Apply culture to plural assignments
                    for (int i = 0; i < plurals.Assignments.Length; i++)
                    {
                        // Get assignment
                        PluralAssignment pluralAssignment = plurals.Assignments[i];
                        // Already has culture
                        if (pluralAssignment.culture != null) continue;
                        // Replace
                        plurals.Assignments[i] = pluralAssignment.WithCulture(lineInfo.Culture);
                    }

                    // Add plural assignments
                    //if (templateText != null)
                    {
                        if (lineInfo.Plurals == null) lineInfo.Plurals = new List<(PluralAssignment[], ITemplateText)>(4);
                        lineInfo.Plurals.Add((plurals.Assignments, templateText));
                        if (lineInfo.PluralsTexts == null) lineInfo.PluralsTexts = new Dictionary<PluralAssignment[], (MarkedText,MarkedText)>(ArrayEqualityComparer<PluralAssignment>.Instance!);
                        lineInfo.PluralsTexts[plurals.Assignments] = (pluralsText, pluralRulesText);
                    }

                    // Process assignments
                    foreach (PluralAssignment assignment in plurals.Assignments)
                    {
                        // Get-or-create parameter info
                        if (!lineInfo.Parameters.TryGetValue(assignment.parameterName, out ILocalizationLinesParameter? parameterInfo))
                            lineInfo.Parameters.Add(parameterInfo = new LocalizationLinesParameter { Name = assignment.parameterName, PluralRuleInfos = new List<PluralRuleInfo>() });

                        // Put plurality info
                        if (parameterInfo!=null) parameterInfo.PluralRuleInfos.AddIfNew(new PluralRuleInfo(pluralRules, assignment.category, assignment.culture, assignment.@case, null));

                        // Assert rules exist
                        if (this.pluralRuleEvaluatorProvider != null && pluralRulesText.HasValue)
                        {
                            //
                            PluralRuleInfo ruleInfo = new PluralRuleInfo(pluralRules, assignment.category, assignment.culture, assignment.@case, null);
                            // Rule failed
                            if (!this.pluralRuleEvaluatorProvider.TryGetValue(ruleInfo, out PluralRulesEvaluator pluralRulesEvaluator) || !pluralRulesEvaluator.AllRules.TryQuery(ruleInfo.ChangeRuleSet(null), out IPluralRule[] _rules))
                                HandleError(lineInfo, true, lineInfo.Key, lineInfo.Culture, LocalizationMessageIds.PluralRulesNotFound, $"Rule=[{assignment}] not found", pluralsText);
                        }
                    }
                }
                else
                {
                    // Assign text
                    lineInfo.Default = templateText;
                }
            }
            catch (Exception e)
            {
                HandleError(lineInfo, false, key, cultureText, LocalizationMessageIds.Unexpected, e.Message, e.StackTrace??"");
            }
        }

        // Build here rules
        StructList8<PluralRuleInfo> list = new();
        StructList8<IPluralRulesEvaluator> evaluators = new();

        ParameterAnalysis parameterAnalysis = new ParameterAnalysis();

        // Finalize lines
        foreach (LocalizationLinesInfo lineInfo in result.Values)
        {
            // 
            parameterAnalysis.Clear();

            try
            {
                // Initialize analyser
                {

                    parameterAnalysis.Key = lineInfo.Key!;
                    parameterAnalysis.KeyText = lineInfo.KeyText;
                    parameterAnalysis.Culture = lineInfo.Culture!;
                    //parameterAnalysis.PluralRuleProvider = this.PluralRuleProvider;
                    parameterAnalysis.PluralsTexts = lineInfo.PluralsTexts;
                    if (lineInfo.Default != null)
                    {
                        parameterAnalysis.Add(null, lineInfo.Default);
                    }
                    if (lineInfo.Plurals != null && lineInfo.Plurals.Count > 0)
                        for (int i = 0; i < lineInfo.Plurals.Count; i++)
                        {
                            (PluralAssignment[] pluralAssignments, ITemplateText text) = lineInfo.Plurals[i];
                            parameterAnalysis.Add(pluralAssignments, text);
                        }
                    parameterAnalysis.Analyze();
                    lineInfo.PluralizedParameterCount = parameterAnalysis.PluralizedParameterCount;
                    lineInfo.Default = parameterAnalysis.DefaultLine?.Text!;
                    lineInfo.Plurals = parameterAnalysis.Plurals;
                    // Sort
                    if (lineInfo.Plurals.Count > 0 && lineInfo.Plurals is (PluralAssignment[], ITemplateText)[] array)
                    {
                        // Get all rulesets
                        string[] rulesets = lineInfo.RuleSets();
                        // Create sorter
                        PluralsSorter<(PluralAssignment[], ITemplateText)> sorter = new PluralsSorter<(PluralAssignment[], ITemplateText)>(rulesets.Length==0?null:rulesets[0], lineInfo.Culture, this.PluralRuleProvider, i => i.Item1);
                        // Sort array
                        Array.Sort(((PluralAssignment[], ITemplateText)[])lineInfo.Plurals, sorter);
                    }
                    // Apply errors
                    if (parameterAnalysis.Errors != null && parameterAnalysis.Errors.Count > 0)
                        foreach (var error in parameterAnalysis.Errors)
                        {
                            // Add to errors
                            lineInfo.GetOrCreateErrors().Add(error);
                            // Process error
                            try { var _errorHandler = ErrorHandler; if (_errorHandler != null) _errorHandler(error); } catch (Exception) { }
                        }
                    // Assign parameter indices
                    if (lineInfo.Parameters != null)
                        foreach (ILocalizationLinesParameter pi in lineInfo.Parameters)
                            if (parameterAnalysis.Parameters.TryGetValue(pi.Name, out ParameterAnalysis.Parameter? _parameter)) pi.Index = _parameter.ParameterIndex;
                }

                // Organize lineInfo.Parameters by parameter index
                lineInfo.Parameters = TemplateBreakdownExtensions_.AssignByParameterIndex<ILocalizationLinesParameter, ILocalizationLinesParameter>(lineInfo.Parameters!, p => p.Index, p => p);

                // Assign IPluralRulesEvaluator for each pluralized parameter
                if (pluralRuleEvaluatorProvider != null)
                {
                    // Get parameters
                    IList<ILocalizationLinesParameter> parameters = lineInfo.Parameters!;
                    // 
                    foreach (var parameterInfo in parameters)
                    {
                        // Get plural infos
                        var pluralRuleInfos = parameterInfo.PluralRuleInfos;
                        // No plural infos
                        if (pluralRuleInfos == null || pluralRuleInfos.Count == 0) { parameterInfo.PluralRuleEvaluators = null!; continue; }
                        // 
                        list.Clear();
                        evaluators.Clear();
                        // Add categories
                        foreach (PluralRuleInfo info in pluralRuleInfos)
                        {
                            // Get group evaluator
                            PluralRuleInfo info2 = new PluralRuleInfo(info.RuleSet, info.Category, info.Culture, null, null);
                            list.AddIfNew(info2);
                        }
                        // Add evaluator
                        foreach (PluralRuleInfo info in list)
                        {
                            // Find evaluator
                            if (pluralRuleEvaluatorProvider.TryGetValue(info, out PluralRulesEvaluator pluralRulesEvaluator))
                            {
                                evaluators.Add(pluralRulesEvaluator);
                            }
                            // Could not find evaluator
                            else
                            {
                                HandleError(lineInfo, false, lineInfo.Key, lineInfo.Culture, LocalizationMessageIds.PluralRulesNotFound, $"Plural rules not found [{info}]", info.ToString());
                            }
                        }
                        // Assign evaluators
                        parameterInfo.PluralRuleEvaluators = evaluators.ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                HandleError(lineInfo, false, lineInfo.Key!, lineInfo.Culture, LocalizationMessageIds.Unexpected, e.Message, e.StackTrace ?? "");
            }
            finally
            {
                lineInfo.PluralsTexts = null!;
                lineInfo.KeyText = null!;
            }
        }
        // Return
        ILocalizationLinesInfo[] _resultLines = new ILocalizationLinesInfo[result.Count];
        int ix = 0;
        foreach (LocalizationLinesInfo localizationText in result.Values) _resultLines[ix++] = localizationText;
        resultLines = _resultLines;
        return true;
    }

}


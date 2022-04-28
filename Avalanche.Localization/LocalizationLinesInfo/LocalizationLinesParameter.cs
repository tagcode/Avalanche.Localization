// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Localization.Pluralization;
using Avalanche.Template;
using Avalanche.Utilities;

/// <summary>A template format parameter with pluralization rules.</summary>
public class LocalizationLinesParameter : ReadOnlyAssignableClass, ILocalizationLinesParameter
{
    /// <summary>Create non-plural parameter info</summary>
    public static LocalizationLinesParameter CreateNonPlural(ITemplatePlaceholderPart placeholder)
    {
        // 
        LocalizationLinesParameter info = new LocalizationLinesParameter
        {
            PluralRuleInfos = Array.Empty<PluralRuleInfo>(),
            PluralRuleEvaluators = Array.Empty<IPluralRulesEvaluator>()
        };
        //
        if (placeholder.Parameter != null) { info.Name = placeholder.Parameter.Unescaped.AsString(); info.Index = placeholder.Parameter.ParameterIndex; }
        // 
        if (placeholder.Formatting != null) { info.Format = placeholder.Formatting.Unescaped.AsString(); }
        //
        return info;
    }
    /// <summary>Parameter name</summary>
    protected string name = null!;
    /// <summary>Parameter index</summary>
    protected int index = -1;
    /// <summary>Parameter format</summary>
    protected string? format;
    /// <summary>Plural infos</summary>
    protected IList<PluralRuleInfo> pluralRuleInfos = null!;
    /// <summary>Plural rule evaluators, should be 0 or 1 evaluator. May contain more if localization content is faulty and supplies to multiple categories, e.g. "cardinal", "ordinal". All will be evaluated until matching rule is detected.</summary>
    protected IList<IPluralRulesEvaluator> pluralRuleEvaluators = null!;

    /// <summary>Parameter name</summary>
    public string Name { get => name; set => this.AssertWritable().name = value; }
    /// <summary>Parameter index</summary>
    public int Index { get => index; set => this.AssertWritable().index = value; }
    /// <summary>Parameter format</summary>
    public string? Format { get => format; set => this.AssertWritable().format = value; }
    /// <summary>Plural infos</summary>
    public IList<PluralRuleInfo> PluralRuleInfos { get => pluralRuleInfos; set => this.AssertWritable().pluralRuleInfos = value; }
    /// <summary>Plural rule evaluators, should be 0 or 1 evaluator. May contain more if localization content is faulty and supplies to multiple categories, e.g. "cardinal", "ordinal". All will be evaluated until matching rule is detected.</summary>
    public IList<IPluralRulesEvaluator> PluralRuleEvaluators { get => pluralRuleEvaluators; set => this.AssertWritable().pluralRuleEvaluators = value; }

    /// <summary>Print info</summary>
    public override string ToString() => $"Name={name},PluralInfo={string.Join(',', pluralRuleInfos)}";
}


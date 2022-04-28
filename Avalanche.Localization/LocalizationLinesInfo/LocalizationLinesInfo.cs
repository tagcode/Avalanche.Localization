// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;
using Avalanche.Utilities;

/// <summary>Key specific info, such as parameters, parameter pluralization, parse errors.</summary>
public record LocalizationLinesInfo : ReadOnlyAssignableRecord, ILocalizationLinesInfo
{
    /// <summary>Adapt <paramref name="text"/> as non-pluralized lines info.</summary>
    public static LocalizationLinesInfo CreateNonPlural(ITemplateText text, string key, string culture)
    {
        // Create
        LocalizationLinesInfo info = new LocalizationLinesInfo()
        {
            Key = key,
            Default = text,
            Culture = culture,
            Errors = Array.Empty<ILocalizationError>(),
            Parameters = text.Breakdown.Placeholders.Select(LocalizationLinesParameter.CreateNonPlural).ToArray(),
            PluralizedParameterCount = 0,
            Plurals = Array.Empty<(PluralAssignment[], ITemplateText)>()
        };
        // Return
        return info;
    }

    /// <summary></summary>
    protected string key = null!;
    /// <summary>Culture with language and possibly region</summary>
    protected string culture = null!;
    /// <summary>Default template text, one where every pluralized parameter is "other" or with no pluralized assignments.</summary>
    protected ITemplateText @default = null!;
    /// <summary>Parameter infos</summary>
    protected IList<ILocalizationLinesParameter> parameters = null!;
    /// <summary>Parse errors</summary>
    protected IList<ILocalizationError> errors = null!;
    /// <summary>Number of pluralized parameters</summary>
    protected int pluralizedParameterCount;
    /// <summary>Plural permutations and texts</summary>
    protected IList<(PluralAssignment[], ITemplateText)>? plurals;

    /// <summary>Assign to read-only state</summary>
    protected override void setReadOnly()
    {
        if (this.parameters != null && this.parameters is not ILocalizationLinesParameter[]) this.parameters = this.parameters.Count == 0 ? Array.Empty<ILocalizationLinesParameter>() : this.parameters.ToArray();
        if (this.errors != null && this.errors is not ILocalizationError[]) this.errors = this.errors.Count == 0 ? Array.Empty<ILocalizationError>() : this.errors.ToArray();
        if (this.parameters != null)
            foreach (ILocalizationLinesParameter parameter in parameters)
                if (parameter is IReadOnly @readonly) @readonly.ReadOnly = true;
        base.setReadOnly();
    }

    /// <summary></summary>
    public string Key { get => key; set => this.AssertWritable().key = value; }
    /// <summary>Culture with language and possibly region</summary>
    public string Culture { get => culture; set => this.AssertWritable().culture = value; }
    /// <summary></summary>
    public IFormatProvider Format { get => null!; set { } }
    /// <summary>Default template text, one where every pluralized parameter is "other" or with no pluralized assignments.</summary>
    public ITemplateText Default { get => @default; set => this.AssertWritable().@default = value; }
    /// <summary>Parameter infos</summary>
    public IList<ILocalizationLinesParameter> Parameters { get => parameters; set => this.AssertWritable().parameters = value; }
    /// <summary>Parse errors</summary>
    public IList<ILocalizationError> Errors { get => errors; set => this.AssertWritable().errors = value; }
    /// <summary>Number of pluralized parameters</summary>
    public int PluralizedParameterCount { get => pluralizedParameterCount; set => this.AssertWritable().pluralizedParameterCount = value; }
    /// <summary>Plural permutations and texts</summary>
    public IList<(PluralAssignment[], ITemplateText)>? Plurals { get => plurals; set => this.AssertWritable().plurals = value; }

    /// <summary>key source text</summary>
    public MarkedText KeyText;
    /// <summary>Map indicating source texts for each plural case</summary>
    public Dictionary<PluralAssignment[], (MarkedText pluralsText, MarkedText ruleSetText)> PluralsTexts = null!;

    /// <summary>Print info</summary>
    public override string ToString() => String.Join(',', (IEnumerable<ILocalizationLinesParameter>)Parameters) + (Errors == null ? "" : $", Errors={string.Join(',', Errors)}");
}

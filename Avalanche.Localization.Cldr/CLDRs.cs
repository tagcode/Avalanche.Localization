// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Avalanche.Localization.Pluralization;
using Avalanche.Utilities;

/// <summary>
/// Unicode Common Locale Data Repository (CLDR) services.
/// 
/// <see href="https://unicode-org.github.io/cldr/ldml/tr35-numbers.html#Plural_rules_syntax"/>
/// <see href="https://www.unicode.org/cldr/charts/33/supplemental/language_plural_rules.html"/>
/// <see href="http://cldr.unicode.org/index/cldr-spec/plural-rules"/>
/// <see href="https://unicode.org/Public/cldr/35/cldr-common-35.0.zip"/>  
/// </summary>
public static class CLDRs
{
    /// <summary>Prefix</summary>
    public const string Key_CLDR = "Unicode.CLDR";

    /// <summary>All CLDR rules</summary>
    static readonly Lazy<IPluralRules> all = new Lazy<IPluralRules>(
        () => new PluralRules().SetAllRules(
            CLDR40PluralRules.Instance, CLDR41PluralRules.Instance
        ).SetReadOnly()
    );

    /// <summary>CLDR v40</summary>
    public static ICLDRPluralRules CLDR40 => CLDR40PluralRules.Instance;
    /// <summary>CLDR v41</summary>
    public static ICLDRPluralRules CLDR41 => CLDR41PluralRules.Instance;
    /// <summary>CLDR Latest code generated version.</summary>
    public static ICLDRPluralRules CLDR => CLDR41PluralRules.Instance;
    /// <summary>CLDR All code generated versions.</summary>
    public static IPluralRules All => all.Value;

    /// <summary>
    /// Create xml reader for plurality xml files at <paramref name="path"/>.
    /// 
    /// The expected filename is "pluralsXX.xml" and "ordinalsXX.xml".
    /// XX will be used as ruleset name "Unicode.CLDR40"
    /// </summary>
    /// <param name="path"></param>
    /// <param name="addOptionalZero">Add "zero" case to "cardinal" (as optional) if it didn't exist.</param>
    /// <param name="addOptionalOne">Add "one" case to "ordinal" (as optional) if it didn't exist.</param>
    /// <returns></returns>
    public static IEnumerable<IPluralRule> PluralReaders(string path, bool addOptionalZero = false, bool addOptionalOne = false)
    {
        // 
        List<IEnumerable<IPluralRule>> readers = new();
        // List files
        foreach (string filename in System.IO.Directory.GetFiles(path))
        {
            // Match to pluralsXX.xml
            Match match = PluralsFilePattern.Match(filename);
            if (match.Success)
            {
                // Get version
                string version = match.Groups[1].Value;
                // Derive ruleset name
                string ruleset = Key_CLDR + version;
                // Create reader
                IEnumerable<IPluralRule> xmlReader = PluralReader(filename, ruleset, addOptionalZero, addOptionalOne);
                // Add to result
                readers.Add(xmlReader);
            }
            // Match to ordinalsXX.xml
            match = OrdinalsFilePattern.Match(filename);
            if (match.Success)
            {
                // Get version
                string version = match.Groups[1].Value;
                // Derive ruleset name
                string ruleset = CLDRs.Key_CLDR + version;
                // Create reader
                IEnumerable<IPluralRule> xmlReader = PluralReader(filename, ruleset, addOptionalZero, addOptionalOne);
                // Add to result
                readers.Add(xmlReader);
            }
        }
        // No readers
        if (readers.Count == 0) return Array.Empty<IPluralRule>();
        // One readers
        if (readers.Count == 1) return readers[0];
        // Concat all readers
        IEnumerable<IPluralRule> concat = readers[0];
        for (int i = 1; i < readers.Count; i++) concat = concat.Concat(readers[i]);
        // Return readers
        return concat;
    }

    /// <summary>pluralsXX.xml filename pattern</summary>
    static readonly Regex plurals_file_pattern = new Regex("^.*plurals([\\d\\w]*).xml$", RegexOptions.Compiled);
    /// <summary>ordinalsXX.xml filename pattern</summary>
    static readonly Regex ordinals_file_pattern = new Regex("^.*ordinals([\\d\\w]*).xml$", RegexOptions.Compiled);
    /// <summary>pluralsXX.xml filename pattern</summary>
    public static Regex PluralsFilePattern => plurals_file_pattern;
    /// <summary>ordinalsXX.xml filename pattern</summary>
    public static Regex OrdinalsFilePattern => ordinals_file_pattern;

    /// <summary>Create ruleset reader</summary>
    /// <param name="ruleset">Ruleset name, e.g. "Unicode.CLDR40"</param>
    /// <param name="cardinalsXmlFileName">Path to "cldr-common-xx.zip/common/supplemental/plurals.xml"</param>
    /// <param name="ordinalsXmlFileName">Path to "cldr-common-xx.zip/common/supplemental/ordinals.xml"</param>
    /// <param name="addOptionalZero">Add "zero" case (as optional) if it didn't exist.</param>
    /// <param name="addOptionalOne">Add "one" case (as optional) if it didn't exist.</param>
    /// <returns>Rule reader</returns>
    /// <remarks>Call <see cref="System.Linq.Enumerable.ToArray"/> to begin reading</remarks>
    public static IEnumerable<IPluralRule> PluralReaders(string ruleset, string cardinalsXmlFileName, string ordinalsXmlFileName, bool addOptionalZero = false, bool addOptionalOne = false)
    {
        // 
        IEnumerable<IPluralRule> cardinalRules = PluralReader(ruleset, cardinalsXmlFileName, addOptionalZero, addOptionalOne);
        //
        IEnumerable<IPluralRule> ordinalRules = PluralReader(ruleset, ordinalsXmlFileName, addOptionalZero, addOptionalOne);
        //
        IEnumerable<IPluralRule> rules = cardinalRules.Concat(ordinalRules);
        //
        return rules;
    }

    /// <summary>Create <paramref name="xmlFileName"/> plurality rule reader</summary>
    /// <param name="xmlFileName">Path to ordinals.xml or plurals.xml</param>
    /// <param name="ruleset">Ruleset name, e.g. "Unicode.CLDR40"</param>
    /// <param name="addOptionalZero">Add "zero" case to "cardinal" (as optional) if it didn't exist.</param>
    /// <param name="addOptionalOne">Add "one" case to "ordinal" (as optional) if it didn't exist.</param>
    /// <returns></returns>
    public static IEnumerable<IPluralRule> PluralReader(string xmlFileName, string ruleset, bool addOptionalZero = false, bool addOptionalOne = false)
    {
        // Load document
        XDocument document = XDocument.Load(xmlFileName);
        // Get elements
        XElement rootElement = document.Root ?? throw new ArgumentException("No elements");
        // Create rule reader
        IEnumerable<IPluralRule> reader = PluralReader(rootElement, ruleset, addOptionalZero, addOptionalOne);
        // Return reader
        return reader;
    }

    /// <summary>Load rules from <paramref name="rootElement"/>.</summary>
    /// <param name="rootElement"></param>
    /// <param name="ruleset">The "RuleSet" parameter that is added to every instantiated <see cref="IPluralRule"/></param>
    /// <param name="addOptionalZero">Add "zero" case to "cardinal" (as optional) if it didn't exist.</param>
    /// <param name="addOptionalOne">Add "one" case to "ordinal" (as optional) if it didn't exist.</param>
    /// <returns></returns>
    public static IEnumerable<IPluralRule> PluralReader(XElement rootElement, string ruleset, bool addOptionalZero, bool addOptionalOne)
    {
        List<(string, IPluralRuleExpression)> list = new List<(string, IPluralRuleExpression)>();
        foreach (XElement pluralsElement in rootElement.Elements("plurals"))
        {
            string? category = pluralsElement.Attribute("type")?.Value;
            if (category == null) throw new ArgumentException($"Xml element {pluralsElement} does not have expected attribute \"type\".");
            foreach (XElement pluralRulesElement in pluralsElement.Elements("pluralRules"))
            {
                string? culturesText = pluralRulesElement.Attribute("locales")?.Value;
                if (culturesText == null) throw new ArgumentException($"Xml element {pluralRulesElement} does not have expected attribute \"locales\".");
                string[] cultures = culturesText.Split(' ').Select(l => l.Replace('_', '-')).ToArray();

                // Read expressions into list
                list.Clear();
                int otherCaseIx = -1, zeroCaseIx = -1, oneCaseIx = -1;
                foreach (XElement pluralRuleElement in pluralRulesElement.Elements("pluralRule"))
                {
                    string? @case = pluralRuleElement.Attribute("count")?.Value;
                    if (@case == null) throw new ArgumentException($"Xml element {pluralRuleElement} does not have expected attribute \"count\".");
                    string text = pluralRuleElement.Value;
                    if (text == null) throw new ArgumentException($"Xml element {pluralRuleElement} does not have expected text.");

                    IPluralRuleExpression[] exps = PluralRuleExpressionParsers.ConvertToRuleExpressions(text.AsMemory());
                    if (exps == null) throw new ArgumentException($"Could not parse expression \"{text}\"");
                    int expCountInText = 0;
                    foreach (IPluralRuleExpression exp in exps)
                    {
                        if (@case == "other") otherCaseIx = list.Count;
                        if (@case == "zero") zeroCaseIx = list.Count;
                        if (@case == "one") oneCaseIx = list.Count;
                        list.Add((@case, exp));
                        expCountInText++;
                    }
                    if (expCountInText == 0) list.Add((@case, null!));
                    if (expCountInText > 1) throw new ArgumentException($"Got more than one expression for case \"{@case}\"");
                }

                // Move "other" rule to as last index if it has no rules
                if (otherCaseIx >= 0)
                {
                    var otherCase = list[otherCaseIx];
                    if (otherCase.Item2.Rule == null)
                    {
                        /*
                        // Make 'other' not-required
                        if (!otherCase.Item2.Info.Required.HasValue || !otherCase.Item2.Info.Required.Value == true)
                        {
                            otherCase.Item2.Info = otherCase.Item2.Info.ChangeRequired(false);
                            list[otherCaseIx] = otherCase;
                        }
                        */
                        // Move to last
                        if (list.Count > 1)
                        {
                            list.RemoveAt(otherCaseIx);
                            list.Add(otherCase);
                        }
                    }
                }

                // Now instantiate rules for each culture
                foreach (string _culture in cultures)
                {
                    // C# uses "" as root culture
                    string culture = _culture == "root" ? "" : _culture;

                    // Add optional "zero" rule, if doesn't exist.
                    if (addOptionalZero && zeroCaseIx < 0 && category == "cardinal")
                    {
                        PluralRuleInfo info = new PluralRuleInfo(ruleset, category, culture, "zero", false);
                        yield return new PluralRule.Zero(info);
                    }
                    // Add optional "one" rule, if doesn't exist.
                    if (addOptionalOne && oneCaseIx < 0 && category == "ordinal")
                    {
                        PluralRuleInfo info = new PluralRuleInfo(ruleset, category, culture, "one", false);
                        yield return new PluralRule.One(info);
                    }
                    // Add mandatory rules
                    foreach ((string @case, IPluralRuleExpression exp) in list)
                    {
                        // Info
                        PluralRuleInfo info = new PluralRuleInfo(ruleset, category, culture, @case, exp.Info.Required.HasValue ? exp.Info.Required.Value : true);
                        // Return "other" case as a class that is easier to evaluate (always true)
                        if (exp.Rule == null && @case == "other") yield return new PluralRule.TrueWithExpression(info, exp.Rule!, exp.Samples!);
                        // Return rule that evaluates expression
                        else yield return new PluralRule.Expression(info, exp.Rule!, exp.Samples!);
                    }
                }
            }
        }
    }

}


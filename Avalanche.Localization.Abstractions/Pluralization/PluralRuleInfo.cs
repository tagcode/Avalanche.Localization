// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;

/// <summary>Request for <see cref="IPluralRule"/>. Also used as rule info.</summary>
public readonly struct PluralRuleInfo
{
    /// <summary>Key for requesting for default rule-set.</summary>
    public const string NEWEST = "Unicode.CLDR";

    /// <summary>No constraints, a request for all rules</summary>
    static readonly PluralRuleInfo noConstraints = new PluralRuleInfo(null, null, null, null, null);
    /// <summary>Request for rules of the newest rule-set.</summary>
    static readonly PluralRuleInfo newestRuleSet = new PluralRuleInfo(NEWEST, null, null, null, null);

    /// <summary>No constraints, a request for all rules</summary>
    public static PluralRuleInfo NoConstraints => noConstraints;
    /// <summary>Request for rules of the newest rule-set.</summary>
    public static PluralRuleInfo Newest => newestRuleSet;

    /// <summary>Name of ruleset. e.g. "Unicode.CLDR35".</summary>
    /// <remarks>If null, then info is used for querying for a set of rules, and RuleSet is left open.</remarks>
    public readonly string? RuleSet;

    /// <summary>Name of plurality category. E.g. "cardinal", "ordinal", "optional".</summary>
    /// <remarks>If null, then info is used for querying for a set of rules, and Category is left open.</remarks>
    public readonly string? Category;

    /// <summary>Name of culture. E.g. "fi"</summary>
    /// <remarks>If null, then info is used for querying for a set of rules, and Culture is left open.</remarks>
    public readonly string? Culture;

    /// <summary>Name of plurality case. E.g. "zero", "one", "few", "many", "other"</summary>
    /// <remarks>If null, then info is used for querying with case constraint.</remarks>
    public readonly string? Case;

    /// <summary>Policy whether rule is required.</summary>
    public readonly bool? Required;

    /// <summary>Implicit conversion from tuple.</summary>
    public static implicit operator PluralRuleInfo((string? ruleset, string? category, string? culture, string? @case, bool? required) tuple)  => new PluralRuleInfo(tuple.ruleset, tuple.category, tuple.culture, tuple.@case, tuple.required);
    /// <summary>Implicit conversion from tuple.</summary>
    public static implicit operator PluralRuleInfo((string? ruleset, string? category, string? culture, string? @case) tuple)  => new PluralRuleInfo(tuple.ruleset, tuple.category, tuple.culture, tuple.@case, null);
    /// <summary>Implicit conversion from tuple.</summary>
    public static implicit operator PluralRuleInfo((string? category, string? culture, string? @case) tuple)  => new PluralRuleInfo(NEWEST, tuple.category, tuple.culture, tuple.@case, null);
    /// <summary>Implicit conversion from tuple.</summary>
    public static implicit operator PluralRuleInfo((string? category, string? culture) tuple) => new PluralRuleInfo(NEWEST, tuple.category, tuple.culture, null, null);

    /// <summary>Create info from <paramref name="keyValuePairs"/>.</summary>
    public PluralRuleInfo(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
    {
        // Init
        RuleSet = null;
        Category = null;
        Culture = null;
        Case = null;
        Required = null;
        // Read key-values
        foreach(var kv in keyValuePairs)
        {
            if (StringComparer.InvariantCultureIgnoreCase.Equals(kv.Key, nameof(RuleSet))) RuleSet = kv.Value;
            else if (StringComparer.InvariantCultureIgnoreCase.Equals(kv.Key, nameof(Category))) Category = kv.Value;
            else if (StringComparer.InvariantCultureIgnoreCase.Equals(kv.Key, nameof(Culture))) Culture = kv.Value;
            else if (StringComparer.InvariantCultureIgnoreCase.Equals(kv.Key, nameof(Case))) Case = kv.Value;
            else if (StringComparer.InvariantCultureIgnoreCase.Equals(kv.Key, nameof(Required))) Required = string.IsNullOrEmpty(kv.Value)||kv.Value=="null"||kv.Value=="default" ? null: bool.Parse(kv.Value);
            else throw new NotSupportedException(kv.Key);
        }

        int hash = 0;
        if (RuleSet != null) hash ^= 3 * RuleSet.GetHashCode();
        if (Category != null) hash ^= 5 * Category.GetHashCode();
        if (Culture != null) hash ^= 7 * Culture.GetHashCode();
        if (Case != null) hash ^= 11 * Case.GetHashCode();
        if (Required == true) hash ^= 0x4234923; else if (Required == false) hash ^= 0xbbabab;
        this.hashcode = hash;
    }

    /// <summary>Create plural rule request</summary>
    /// <param name="ruleset">Rule set, e.g. "Unicode.CLDR35", "Unicode.CLDR", or null (all)</param>
    /// <param name="category">Category, one of: "cardinal", "ordinal", "optional", null (all)</param>
    /// <param name="culture">"Culture, e.g. "en", "fi"</param>
    /// <param name="case">Case, e.g. "zero", "one", "few", "many", "other", null (all)</param>
    /// <param name="required">Optionality</param>
    public PluralRuleInfo(string? ruleset, string? category, string? culture, string? @case, bool? required)
    {
        RuleSet = ruleset;
        Category = category;
        Culture = culture;
        Case = @case;
        Required = required;
        int hash = 0;
        if (ruleset != null) hash ^= 3 * ruleset.GetHashCode();
        if (category != null) hash ^= 5 * category.GetHashCode();
        if (culture != null) hash ^= 7 * culture.GetHashCode();
        if (@case != null) hash ^= 11 * @case.GetHashCode();
        if (Required == true) hash ^= 0x4234923; else if (Required == false) hash ^= 0xbbabab;
        this.hashcode = hash;
    }

    /// <summary>Compares filter info (this) against <paramref name="info"/>. If any of the local fields is null, then that value is not compared.</summary>
    public bool FilterMatch(PluralRuleInfo info)
    {
        if (RuleSet != null && RuleSet != info.RuleSet) return false;
        if (Category != null && Category != info.Category) return false;
        if (Culture != null && Culture != info.Culture) return false;
        if (Case != null && Case != info.Case) return false;
        if (Required.HasValue && Required.Value != info.Required) return false;
        return true;
    }

    /// <summary>Return new info with <paramref name="ruleSet"/>.</summary>
    public PluralRuleInfo ChangeRuleSet(string? ruleSet) => new PluralRuleInfo(ruleSet, Category, Culture, Case, Required);
    /// <summary>Return new info with <paramref name="required"/>.</summary>
    public PluralRuleInfo ChangeRequired(bool? required) => new PluralRuleInfo(RuleSet, Category, Culture, Case, required);

    /// <summary>Print information.</summary>
    public override string ToString() => $"{nameof(RuleSet)}={RuleSet},{nameof(Category)}={Category},{nameof(Culture)}={Culture},{nameof(Case)}={Case},{nameof(Required)}={Required}";

    /// <summary>Compare for equality</summary>
    public override bool Equals(object? obj)
    {
        // Cast;
        if (obj is not PluralRuleInfo other) return false;
        // Hashcode disqualification
        if (other.hashcode != hashcode) return false;
        // Compare
        bool equals = ((RuleSet == null && other.RuleSet == null) || (RuleSet == other.RuleSet)) &&
                ((Category == null && other.Category == null) || (Category == other.Category)) &&
                ((Culture == null && other.Culture == null) || (Culture == other.Culture)) &&
                ((Case == null && other.Case == null) || (Case == other.Case)) &&
                ((!Required.HasValue && !other.Required.HasValue) || (Required == other.Required));
        // Return
        return equals;
    }

    /// <summary>Cached hash code</summary>
    private readonly int hashcode;
    /// <summary>Get cached hashcode.</summary>
    public override int GetHashCode() => hashcode;

    /// <summary>Rule info comparer</summary>
    public class Comparer : IEqualityComparer<PluralRuleInfo>
    {
        /// <summary>Singleton</summary>
        private static readonly Comparer instance = new Comparer();
        /// <summary>Singleton</summary>
        public static Comparer Default => instance;

        /// <summary>Compare for equality</summary>
        public bool Equals(PluralRuleInfo x, PluralRuleInfo y)
        {
            // Hashcode disqualification
            if (y.hashcode != x.hashcode) return false;
            // Compare all
            bool equals = ((x.RuleSet == null && y.RuleSet == null) || (x.RuleSet == y.RuleSet)) &&
                ((x.Category == null && y.Category == null) || (x.Category == y.Category)) &&
                ((x.Culture == null && y.Culture == null) || (x.Culture == y.Culture)) &&
                ((x.Case == null && y.Case == null) || (x.Case == y.Case)) &&
                ((!x.Required.HasValue && !y.Required.HasValue) || (x.Required == y.Required));
            //
            return equals;
        }

        /// <summary>Get cached hashcode.</summary>
        public int GetHashCode(PluralRuleInfo obj) => obj.hashcode;
    }
}



// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Template;
using Avalanche.Utilities;

/// <summary>
/// File pattern represents localization files, e.g. "Resources/{Culture}/{Key}".
/// 
/// Folllowing placeholders can be used: "Key", "Culture", "Namespace", "Name".
/// 
/// A pattern without "Culture" placeholder is applied when searching for invariant culture "".
/// </summary>
public class LocalizationFilePatterns : ReadOnlyAssignableClass, ILocalizationFilePatterns
{
    /// <summary>Default patterns for "Resources/" folder</summary>
    static ILocalizationFilePatterns resourcesFolder = new LocalizationFilePatterns("Resources/{Key}", "Resources/{Culture}/{Key}").SetReadOnly();
    /// <summary>Default patterns for ".Resource." embedded path</summary>
    static ILocalizationFilePatterns resourcesEmbedded = new LocalizationFilePatterns("*/*.Resources.{Key}", "*/*.Resources.{Culture}.{Key}", "*/{Key}", "*/{Key}.{Culture}").SetReadOnly();
    /// <summary>Default patterns for "Resources/" folder</summary>
    public static ILocalizationFilePatterns ResourcesFolder => resourcesFolder;
    /// <summary>Default patterns for ".Resource." embedded path</summary>
    public static ILocalizationFilePatterns ResourcesEmbedded => resourcesEmbedded;

    /// <summary>Pattern, e.g. "Resources/{Culture}/{Key}".</summary>
    protected ITemplateFormatPrintable[] patterns = null!;
    /// <summary>Pattern, e.g. "Resources/{Culture}/{Key}".</summary>
    public ITemplateFormatPrintable[] Patterns { get => patterns; set => this.AssertWritable().patterns = value; }

    /// <summary></summary>
    public LocalizationFilePatterns() : base() { }
    /// <summary></summary>
    public LocalizationFilePatterns(params string[] patternTexts) : base() 
    {
        this.Patterns = patternTexts.Select(patternText => new TemplateText(patternText, TemplateFormat.BraceAlphaNumeric)).ToArray();
    }
    /// <summary></summary>
    public LocalizationFilePatterns(params ITemplateFormatPrintable[] patterns) : base() 
    {
        this.Patterns = patterns ?? throw new ArgumentNullException(nameof(patterns));
    }

    /// <summary>Get hash code</summary>
    public override int GetHashCode()
    {
        // Init
        int hash = 0x36254;
        // Hash-in with order insensitive xor
        foreach (ITemplateFormatPrintable pattern in Patterns) hash ^= pattern.GetHashCode();
        // Return
        return hash;
    }

    /// <summary></summary>
    public override bool Equals(object? obj)
    {
        // Cast
        if (obj is not ILocalizationFilePatterns other) return false;
        // Get references
        ITemplateFormatPrintable[] patterns1 = this.Patterns, patterns2 = other.Patterns;
        // Compare nulls
        if (patterns1 == null && patterns2 == null) return true;
        if (patterns1 == null || patterns2 == null) return false;
        if (patterns1.Length != patterns2.Length) return false;
        // Compare each
        foreach (ITemplateFormatPrintable _pattern in patterns1) if (!patterns2.Contains(_pattern)) return false;
        foreach (ITemplateFormatPrintable _pattern in patterns2) if (!patterns1.Contains(_pattern)) return false;
        // Equals
        return true;
    }

    /// <summary></summary>
    public override string ToString() => String.Join<ITemplateFormatPrintable>(",", Patterns);
}

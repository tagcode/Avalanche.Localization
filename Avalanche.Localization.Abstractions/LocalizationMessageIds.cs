// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using Avalanche.Utilities;

/// <summary>Localization message ids</summary>
public static class LocalizationMessageIds
{
    /// <summary></summary>
    const int bad = unchecked((int)0x8000_0000);
    /// <summary></summary>
    const int localization = CodeIds.Localization;

    /// <summary>No "Key=..." key-value in localization line (error may be omited)</summary>
    public const int NoKey = bad | localization | 0x01;
    /// <summary>No "Text=..." key-value in localization line</summary>
    public const int NoText = bad | localization | 0x02;
    /// <summary>No "TemplateFormat=..." key-value in localization line</summary>
    public const int NoTemplateFormat = bad | localization | 0x03;
    /// <summary>No "Culture=..." key-value in localization line</summary>
    public const int NoCulture = bad | localization | 0x04;
    /// <summary>Template format not found</summary>
    public const int TemplateFormatNotFound = bad | localization | 0x05;
    /// <summary>Text malformed</summary>
    public const int TextMalformed = bad | localization | 0x06;
    /// <summary>Template format could not parse text</summary>
    public const int TextParseFailed = bad | localization | 0x07;
    /// <summary>Plural rules not found.</summary>
    public const int PluralRulesNotFound = bad | localization | 0x08;
    /// <summary>Plurals failed to parse, "parameterName:category:case[:culture], ..." expected</summary>
    public const int PluralsParseFailed = bad | localization | 0x09;
    /// <summary>Plurals refered to parameter that is not found in any localization line for the key</summary>
    public const int PluralsParameterNotFound = bad | localization | 0x0A;
    /// <summary>Duplicate line with same "Plurals" value</summary>
    public const int PluralsDuplicateAssignment = bad | localization | 0x0B;
    /// <summary>Duplicate "Plurals" assignment on a specific parameter name</summary>
    public const int PluralsParameterDuplicateAssignment = bad | localization | 0x0C;
    /// <summary>Missing required plurality cases.</summary>
    public const int PluralsMissingCases = bad | localization | 0x0D;
    /// <summary>Searched Key and Culture was not found, nor fallback culture.</summary>
    public const int KeyNotFound = bad | localization | 0x0E;
    /// <summary>Unexpected exception</summary>
    public const int Unexpected = bad | localization | 0x0F;
}


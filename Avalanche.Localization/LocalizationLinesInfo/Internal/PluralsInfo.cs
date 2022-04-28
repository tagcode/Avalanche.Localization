// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Internal;
using Avalanche.Tokenizer;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>
/// Parses plurals strings key-values that are used in localization files: 'Plurals="parameter:category:case:culture[:culture],..."' 
/// </summary>
/// <example>
/// "0:cardinal:one,1:ordinal:other,parameter:cardinal:zero" is tokenized as following:
/// 
/// [0:54] CompositeToken: "0:cardinal:one,1:ordinal:other,parameter:cardinal:zero"
/// ├── [0:14] GroupToken: "0:cardinal:one"
/// │   ├── [0:1] ParameterToken: "0"
/// │   ├── [1:2] SeparatorToken: ":"
/// │   ├── [2:10] CategoryToken: "cardinal"
/// │   ├── [10:11] SeparatorToken: ":"
/// │   └── [11:14] CaseToken: "one"
/// ├── [14:30] GroupToken: ",1:ordinal:other"
/// │   ├── [14:15] GroupSeparatorToken: ","
/// │   ├── [15:16] ParameterToken: "1"
/// │   ├── [16:17] SeparatorToken: ":"
/// │   ├── [17:24] CategoryToken: "ordinal"
/// │   ├── [24:25] SeparatorToken: ":"
/// │   └── [25:30] CaseToken: "other"
/// └── [30:54] GroupToken: ",parameter:cardinal:zero"
///     ├── [30:31] GroupSeparatorToken: ","
///     ├── [31:40] ParameterToken: "parameter"
///     ├── [40:41] SeparatorToken: ":"
///     ├── [41:49] CategoryToken: "cardinal"
///     ├── [49:50] SeparatorToken: ":"
///     └── [50:54] CaseToken: "zero"
///     
/// 
/// Backslash can be used as escaping parameter names:
/// [0:17] CompositeToken: "0\\:0:cardinal:one"
/// └── [0:17] GroupToken: "0\\:0:cardinal:one"
///     ├── [0:4] ParameterToken: "0\\:0"
///     ├── [4:5] SeparatorToken: ":"
///     ├── [5:13] CategoryToken: "cardinal"
///     ├── [13:14] SeparatorToken: ":"
///     └── [14:17] CaseToken: "one"
///     
/// Usage: <![CDATA[Plurals.PluralAssignment[] plurals = Plurals.Cached["0:cardinal:one,1:ordinal:other,parameter:cardinal:zero"];]]>
/// </example>
public class PluralsInfo
{
    /// <summary>Parameter name escaper</summary>
    static IEscaper nameEscaper = Escaper.Create('\\', ',', ':', ' ');
    /// <summary>Parameter name escaper</summary>
    public static IEscaper NameEscaper => nameEscaper;

    /// <summary>Tokenizes ":"</summary>
    static ConstantTokenizer<SeparatorToken> colonTokenizer = new ConstantTokenizer<SeparatorToken>(":");
    /// <summary>Tokenizes ":"</summary>
    static ConstantTokenizer<GroupSeparatorToken> commaTokenizer = new ConstantTokenizer<GroupSeparatorToken>(",");
    /// <summary>Tokenizes "," or ":"</summary>
    static AnyTokenizer colonOrCommaTokenizer = new AnyTokenizer(colonTokenizer, commaTokenizer);
    /// <summary>Tokenizes parameter name</summary>
    static UntilTokenizer<ParameterToken> parameterTokenizer = new UntilTokenizer<ParameterToken>(colonOrCommaTokenizer, false, '\\');
    /// <summary>Tokenizes category</summary>
    static UntilTokenizer<CategoryToken> categoryTokenizer = new UntilTokenizer<CategoryToken>(colonOrCommaTokenizer, false, '\\');
    /// <summary>Tokenizes case</summary>
    static UntilTokenizer<CaseToken> caseTokenizer = new UntilTokenizer<CaseToken>(new AnyTokenizer(colonTokenizer, commaTokenizer, WhitespaceTokenizer.Any), false, '\\');
    /// <summary>Tokenizes culture</summary>
    static UntilTokenizer<CultureToken> cultureTokenizer = new UntilTokenizer<CultureToken>(new AnyTokenizer(commaTokenizer, WhitespaceTokenizer.Any), false, '\\');
    /// <summary>Tokenizes ":culture"</summary>
    static SequenceTokenizer<GroupToken> colonCultureTokenizer = new SequenceTokenizer<GroupToken>((WhitespaceTokenizer.Any, false), (colonTokenizer, true), (WhitespaceTokenizer.Any, false), (cultureTokenizer, true));
    /// <summary>Tokenizes one group of "parameterName:category:case[:culture]"</summary>
    static SequenceTokenizer<GroupToken> groupTokenizer = new SequenceTokenizer<GroupToken>((parameterTokenizer, true, false), (colonTokenizer, true, false), (categoryTokenizer, true, false), (colonTokenizer, true, false), (caseTokenizer, true, false), (colonCultureTokenizer, false, true));
    /// <summary>Tokenizes until "," or end</summary>
    static UntilTokenizer<MalformedToken> malformedTokenizer = new UntilTokenizer<MalformedToken>(commaTokenizer, false);
    /// <summary>Tokenizes into group "parameterName:category:case[:culture]" or malformed</summary>
    static AnyTokenizer groupOrMalformedTokenizer = new AnyTokenizer(groupTokenizer, malformedTokenizer);
    /// <summary>Tokenizes "parameterName:category:case[:culture],.... Value uses '\\' escaping for '\\', ',', ':' and control characters.</summary>
    static ITokenizer<CompositeToken> pluralsTokenizer =
        new SequenceTokenizer(
            (WhitespaceTokenizer.Any, false, false),
            (groupOrMalformedTokenizer, false, false),
            (WhitespaceTokenizer.Any, false, false),
            (new WhileTokenizer(new SequenceTokenizer<GroupToken>((WhitespaceTokenizer.Any, false, true), (commaTokenizer, true, true), (WhitespaceTokenizer.Any, false, true), (groupOrMalformedTokenizer, true, true), (WhitespaceTokenizer.Any, false, true))), false, true)
        );

    /// <summary>Tokenizes "parameterName:category:case[:culture],.... Value uses '\\' escaping for '\\', ',', ':' and control characters.</summary>
    public static ITokenizer<CompositeToken> Tokenizer => pluralsTokenizer;

    /// <summary>Try parse <paramref name="pluralsText"/> into <paramref name="plurals"/>.</summary>
    /// <param name="pluralsText">Value in format of "parameter:category:case[:culture],..."</param>
    /// <param name="plurals"></param>
    /// <returns>True if parse was successful.</returns>
    public static bool TryParse(string pluralsText, out PluralsInfo plurals)
    {
        // Tokenize
        if (!pluralsTokenizer.TryTake(pluralsText.AsMemory(), out CompositeToken compositeToken)) { plurals = null!; return false; }
        // Place here groups
        StructList4<PluralAssignment> list = new();
        // Place here errors
        StructList4<MalformedToken> errors = new();
        // Visit tokens
        foreach(IToken token in compositeToken.Children)
        {
            // Add error
            if (token is MalformedToken malformed) { errors.Add(malformed); continue; }
            // Not group (whitespace)
            if (token is not GroupToken groupToken) continue;
            // Place here
            string parameterName = null!, category = null!, @case = null!, culture = null!;
            // Visit tokens
            foreach (IToken token2 in groupToken.Children)
            {
                // Add error
                if (token2 is MalformedToken malformed2) { errors.Add(malformed2); continue; }
                else if (token2 is ParameterToken) { parameterName = NameEscaper.Unescape(token2.Memory); if (parameterName != null && parameterName.Length == 1) parameterName = string.Intern(parameterName); }
                else if (token2 is CategoryToken) category = string.Intern(NameEscaper.Unescape(token2.Memory));
                else if (token2 is CaseToken) @case = string.Intern(NameEscaper.Unescape(token2.Memory));
                else if (token2 is CultureToken) culture = string.Intern(NameEscaper.Unescape(token2.Memory));
            }
            // Add to result
            if (parameterName != null && category != null && @case != null) list.Add(new PluralAssignment(parameterName, category, @case, culture));
        }
        // Return empty singleton
        if (list.Count == 0 && errors.Count == 0) { plurals = Empty; return true; }
        // Return array
        plurals = new PluralsInfo(list.ToArray(), errors.ToArray());
        return true;
    }

    /// <summary>Plurals create provider</summary>
    static readonly IProvider<string, PluralsInfo> create = Providers.Func<string, PluralsInfo>(TryParse);
    /// <summary>Plurals cached provider</summary>
    static readonly IProvider<string, PluralsInfo> cached = create.WeakCached();
    /// <summary>Plurals create provider</summary>
    public static IProvider<string, PluralsInfo> Create => create;
    /// <summary>Plurals cached provider</summary>
    public static IProvider<string, PluralsInfo> Cached => cached;

    /// <summary>Empty singleton</summary>
    static PluralsInfo empty = new PluralsInfo(new PluralAssignment[0], MalformedToken.NO_TOKENS);
    /// <summary>Empty singleton</summary>
    public static PluralsInfo Empty => empty;

    /// <summary>Plural assignments</summary>
    public readonly PluralAssignment[] Assignments;
    /// <summary>Malformed parts</summary>
    public readonly MalformedToken[] Errors;

    /// <summary>Create plurals info.</summary>
    public PluralsInfo(PluralAssignment[] assignments, MalformedToken[] errors)
    {
        Assignments = assignments;
        Errors = errors;
    }
}

/// <summary>Indicates token ranges over variable name.</summary>
public struct ParameterToken : IToken
{
    /// <summary>Create token</summary>
    public ParameterToken() { }
    /// <summary>Text source</summary>
    public ReadOnlyMemory<char> Memory { get; set; } = default;
    /// <summary>Children of structural token. Each child must be contained in the range of this parent token.</summary>
    public IToken[] Children { get; set; } = Array.Empty<IToken>();
    /// <summary>Accept visitor.</summary>
    public bool Accept(ITokenVisitor visitor) { if (visitor is ITokenVisitor<ParameterToken> c) { c.Visit(this); return true; } else return false; }
    /// <summary>Print token as string</summary>
    public override string ToString() { int index = Memory.Index(); return $"[{index}:{index + Memory.Length}] {GetType().Name} \"{Memory}\""; }
}

/// <summary>Text token represents key.</summary>
public struct CategoryToken : IToken
{
    /// <summary>Create token</summary>
    public CategoryToken() { }
    /// <summary>Text source</summary>
    public ReadOnlyMemory<char> Memory { get; set; } = default;
    /// <summary>Children of structural token. Each child must be contained in the range of this parent token.</summary>
    public IToken[] Children { get; set; } = Array.Empty<IToken>();
    /// <summary>Accept visitor.</summary>
    public bool Accept(ITokenVisitor visitor) { if (visitor is ITokenVisitor<CategoryToken> c) { c.Visit(this); return true; } else return false; }
    /// <summary>Print token as string</summary>
    public override string ToString() { int index = Memory.Index(); return $"[{index}:{index + Memory.Length}] {GetType().Name} \"{Memory}\""; }
}

/// <summary>Text token represents value.</summary>
public struct CaseToken : IToken
{
    /// <summary>Create token</summary>
    public CaseToken() { }
    /// <summary>Text source</summary>
    public ReadOnlyMemory<char> Memory { get; set; } = default;
    /// <summary>Children of structural token. Each child must be contained in the range of this parent token.</summary>
    public IToken[] Children { get; set; } = Array.Empty<IToken>();
    /// <summary>Accept visitor.</summary>
    public bool Accept(ITokenVisitor visitor) { if (visitor is ITokenVisitor<CaseToken> c) { c.Visit(this); return true; } else return false; }
    /// <summary>Print token as string</summary>
    public override string ToString() { int index = Memory.Index(); return $"[{index}:{index + Memory.Length}] {GetType().Name} \"{Memory}\""; }
}

/// <summary>Indicates name span.</summary>
public struct CultureToken : IToken
{
    /// <summary>Create token</summary>
    public CultureToken() { }
    /// <summary>Text source</summary>
    public ReadOnlyMemory<char> Memory { get; set; } = default;
    /// <summary>Children of structural token. Each child must be contained in the range of this parent token.</summary>
    public IToken[] Children { get; set; } = Array.Empty<IToken>();
    /// <summary>Accept visitor.</summary>
    public bool Accept(ITokenVisitor visitor) { if (visitor is ITokenVisitor<CultureToken> c) { c.Visit(this); return true; } else return false; }
    /// <summary>Print token as string</summary>
    public override string ToString() { int index = Memory.Index(); return $"[{index}:{index + Memory.Length}] {GetType().Name} \"{Memory}\""; }
}


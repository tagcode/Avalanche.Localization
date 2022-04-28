// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using Avalanche.Tokenizer;
using Avalanche.Utilities;

/// <summary>Plural rule tokenizers</summary>
public static class PluralRuleTokenizers
{
    /// <summary>Tokenizes a Key=Value</summary>
    static readonly SequenceTokenizer<KeyValueToken> pluralRuleInfoKeyValueTokenizer =
        new(
            (WhitespaceTokenizer.Any, false),
            (new CharTokenizer<KeyToken>(m => char.IsLetter(m.Span[0])), true),
            (WhitespaceTokenizer.Any, false),
            (new ConstantTokenizer<OperandToken>("="), true),
            (WhitespaceTokenizer.Any, false),
            (new UntilTokenizer<ValueToken>(new AnyTokenizer(new ConstantTokenizer("]"), new ConstantTokenizer(",")), false, null), false)
        );
    /// <summary>Tokenizes Key=Value</summary>
    public static SequenceTokenizer<KeyValueToken> PluralRuleInfoKeyValueTokenizer => pluralRuleInfoKeyValueTokenizer;

    /// <summary>Tokenizes plural rule info, e.g. "[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one]"</summary>
    static readonly SequenceTokenizer<PluralRuleInfoToken> pluralRuleInfoTokenizer =
        new(
            (Tokenizers.Whitespace, false, false),
            (new ConstantTokenizer<ParenthesisToken>("["), true, false),
            (Tokenizers.Whitespace, false, false),
            (pluralRuleInfoKeyValueTokenizer, false, false),
            (Tokenizers.Whitespace, false, false),
            (new WhileTokenizer(
                new SequenceTokenizer(
                    (Tokenizers.Whitespace, false, false),
                    (new ConstantTokenizer<ParenthesisToken>(","), true, false),
                    (Tokenizers.Whitespace, false, false),
                    (pluralRuleInfoKeyValueTokenizer, true, false)
                ), true), false, true),
            (new ConstantTokenizer<ParenthesisToken>("]"), true, false),
            (Tokenizers.Whitespace, false, false)
        );
    /// <summary>Tokenizes plural rule info, e.g. "[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one]"</summary>
    public static SequenceTokenizer<PluralRuleInfoToken> PluralRuleInfoTokenizer => pluralRuleInfoTokenizer;

    /// <summary>Tokenizes a sample range "0.0~1.5"</summary>
    static readonly SequenceTokenizer<RangeToken> sampleRangeTokenizer =
        new(
            RealTokenizer<DecimalToken>.WithoutGroupSeparator,
            new ConstantTokenizer<SeparatorToken>("~"),
            RealTokenizer<DecimalToken>.WithoutGroupSeparator
        );
    /// <summary>Tokenizes a sample range "0.0~1.5"</summary>
    public static SequenceTokenizer<RangeToken> SampleRangeTokenizer => sampleRangeTokenizer;

    /// <summary>Tokenizes a sample: <see cref="DecimalToken"/>, <see cref="RangeToken"/> or '…' as <see cref="OperandToken"/>.</summary>
    static readonly AnyTokenizer sampleTokenizer =
        new(
            sampleRangeTokenizer,
            RealTokenizer<DecimalToken>.WithoutGroupSeparator,
            new ConstantTokenizer<OperandToken>("…")
        );
    /// <summary>Tokenizes a sample: <see cref="DecimalToken"/>, <see cref="RangeToken"/> or '…' as <see cref="OperandToken"/>.</summary>
    public static AnyTokenizer SampleTokenizer => sampleTokenizer;

    /// <summary>Tokenizes a sample segment "@name value/range, ..."</summary>
    static readonly SequenceTokenizer<SampleSegmentToken> sampleSegmentTokenizer =
        new(
            (Tokenizers.Whitespace, false, false),
            (new ConstantTokenizer<SampleToken>("@"), true, false),
            (new RegexTokenizer<NameToken>("^[a-zA-Z]{2,}"), true, false),
            (Tokenizers.Whitespace, true, false),
            (sampleTokenizer, false, false),
            (Tokenizers.Whitespace, false, false),
            (new WhileTokenizer(
                new SequenceTokenizer(
                    (Tokenizers.Whitespace, false),
                    (new ConstantTokenizer<SeparatorToken>(","), true),
                    (Tokenizers.Whitespace, false),
                    (sampleTokenizer, true),
                    (Tokenizers.Whitespace, false))
                , true), false, true)
        );
    /// <summary>Tokenizes a sample segment "@name value/range, ..."</summary>
    public static SequenceTokenizer<SampleSegmentToken> SampleSegmentTokenizer => sampleSegmentTokenizer;

    /// <summary>Expression tokenizer: or and = !=</summary>
    /// <remarks>"and" binds more tightly than "or". So X or Y and Z is interpreted as (X or (Y and Z)).</remarks>
    static readonly AnyTokenizer<BooleanExpressionToken> booleanExpressionTokenizer = new();
    /// <summary>Expression tokenizer: and = !=</summary>
    static readonly AnyTokenizer<BooleanExpressionToken> booleanExpressionTokenizer2 = new AnyTokenizer<BooleanExpressionToken>();
    /// <summary>Expression tokenizer: = !=</summary>
    static readonly AnyTokenizer<BooleanExpressionToken> booleanExpressionTokenizer3 = new AnyTokenizer<BooleanExpressionToken>();
    /// <summary>Expression tokenizer: "v", "0", "%"</summary>
    static readonly AnyTokenizer<ExpressionToken> expressionTokenizer = new AnyTokenizer<ExpressionToken>();
    /// <summary>Expression tokenizer: "v", "0"</summary>
    static readonly AnyTokenizer<ExpressionToken> expressionTokenizer2 = new AnyTokenizer<ExpressionToken>();

    /// <summary>"true"</summary>
    static readonly SequenceTokenizer<BooleanExpressionToken> trueTokenizer = new((WhitespaceTokenizer.Any, false), (new ConstantTokenizer<OperandToken>("true"), true), (WhitespaceTokenizer.Any, false));
    /// <summary>"false"</summary>
    static readonly SequenceTokenizer<BooleanExpressionToken> falseTokenizer = new((WhitespaceTokenizer.Any, false), (new ConstantTokenizer<OperandToken>("false"), true), (WhitespaceTokenizer.Any, false));
    /// <summary>"not"</summary>
    static readonly SequenceTokenizer<BooleanExpressionToken> notTokenizer = new((WhitespaceTokenizer.Any, false), (new ConstantTokenizer<OperandToken>("not"), true), (WhitespaceTokenizer.Any, false), (booleanExpressionTokenizer, true), (WhitespaceTokenizer.Any, false));
    /// <summary>"not"</summary>
    static readonly SequenceTokenizer<BooleanExpressionToken> booleanParenthesisTokenizer = new((new ConstantTokenizer<ParenthesisToken>("("), true), (WhitespaceTokenizer.Any, false), (booleanExpressionTokenizer, true), (WhitespaceTokenizer.Any, false), (new ConstantTokenizer<ParenthesisToken>(")"), true));

    /// <summary>Expression tokenizer: or and = !=</summary>
    /// <remarks>"and" binds more tightly than "or". So X or Y and Z is interpreted as (X or (Y and Z)).</remarks>
    public static AnyTokenizer<BooleanExpressionToken> BooleanExpressionTokenizer => booleanExpressionTokenizer;
    /// <summary>Expression tokenizer: "v", "0", "%"</summary>
    public static AnyTokenizer<ExpressionToken> ExpressionTokenizer => expressionTokenizer;

    /// <summary>Expression tokenizer: expression "and" expression</summary>
    /// <remarks>"and" binds more tightly than "or". So X or Y and Z is interpreted as (X or (Y and Z)).</remarks>
    static readonly SequenceTokenizer<BooleanExpressionToken> andTokenizer =
        new(
            (WhitespaceTokenizer.Any, false),
            (booleanExpressionTokenizer3, true),
            (WhitespaceTokenizer.Any, false),
            (new ConstantTokenizer<OperandToken>("and"), true),
            (WhitespaceTokenizer.Any, false),
            (booleanExpressionTokenizer2, true),
            (WhitespaceTokenizer.Any, false)
        );

    /// <summary>Expression tokenizer: expression "and" expression</summary>
    /// <remarks>"and" binds more tightly than "or". So X or Y and Z is interpreted as (X or (Y and Z)).</remarks>
    public static SequenceTokenizer<BooleanExpressionToken> AndTokenizer => andTokenizer;

    /// <summary>Expression tokenizer: expression "or" expression</summary>
    /// <remarks>"and" binds more tightly than "or". So X or Y and Z is interpreted as (X or (Y and Z)).</remarks>
    static readonly SequenceTokenizer<BooleanExpressionToken> orTokenizer =
        new(
            (WhitespaceTokenizer.Any, false),
            (booleanExpressionTokenizer2, true),
            (WhitespaceTokenizer.Any, false),
            (new ConstantTokenizer<OperandToken>("or"), true),
            (WhitespaceTokenizer.Any, false),
            (booleanExpressionTokenizer, true),
            (WhitespaceTokenizer.Any, false)
        );

    /// <summary>Expression tokenizer: expression "or" expression</summary>
    /// <remarks>"and" binds more tightly than "or". So X or Y and Z is interpreted as (X or (Y and Z)).</remarks>
    public static SequenceTokenizer<BooleanExpressionToken> OrTokenizer => orTokenizer;

    /// <summary>Expression tokenizer: expression "=" expression</summary>
    static readonly SequenceTokenizer<BooleanExpressionToken> equalityTokenizer =
        new(
            (WhitespaceTokenizer.Any, false),
            (expressionTokenizer, true),
            (WhitespaceTokenizer.Any, false),
            (new AnyTokenizer<OperandToken>(
                new ConstantTokenizer<OperandToken>("!="),
                new ConstantTokenizer<OperandToken>("="),
                new ConstantTokenizer<OperandToken>("<="),
                new ConstantTokenizer<OperandToken>(">="),
                new ConstantTokenizer<OperandToken>("<"),
                new ConstantTokenizer<OperandToken>(">")), true),
            (WhitespaceTokenizer.Any, false),
            (expressionTokenizer, true),
            (WhitespaceTokenizer.Any, false)
        );
    /// <summary>Expression tokenizer: expression "=" expression</summary>
    public static SequenceTokenizer<BooleanExpressionToken> EqualityTokenizer => equalityTokenizer;

    /// <summary>Expression tokenizer: "i"</summary>
    static readonly RegexTokenizer<VariableToken> variableTokenizer = new RegexTokenizer<VariableToken>("^[a-zA-Z]");
    /// <summary>Expression tokenizer: "i"</summary>
    static readonly SequenceTokenizer<ExpressionToken> variableExpressionTokenizer = new(variableTokenizer);
    /// <summary>Expression tokenizer: "0"</summary>
    static readonly IntegerTokenizer<DecimalToken> integerTokenizer = IntegerTokenizer<DecimalToken>.Instance;

    /// <summary>Expression tokenizer: "%"</summary>
    static readonly SequenceTokenizer<ExpressionToken> moduloTokenizer =
        new(
            (WhitespaceTokenizer.Any, false),
            (expressionTokenizer2, true),
            (WhitespaceTokenizer.Any, false),
            (new ConstantTokenizer<OperandToken>("%"), true),
            (WhitespaceTokenizer.Any, false),
            (integerTokenizer, true),
            (WhitespaceTokenizer.Any, false)
        );
    /// <summary>Expression tokenizer: "%"</summary>
    public static SequenceTokenizer<ExpressionToken> ModuloTokenizer => moduloTokenizer;

    /// <summary>Expression tokenizer: "0..10"</summary>
    static readonly SequenceTokenizer<ExpressionToken> rangeTokenizer =
        new(
            (WhitespaceTokenizer.Any, false, false),
            (integerTokenizer, true, false),
            (WhitespaceTokenizer.Any, false, false),
            (new ConstantTokenizer<OperandToken>(".."), true, false),
            (WhitespaceTokenizer.Any, false, false),
            (integerTokenizer, true, false),
            (WhitespaceTokenizer.Any, false, false)
        );
    /// <summary>Expression tokenizer: "0..10"</summary>
    public static SequenceTokenizer<ExpressionToken> RangeTokenizer => rangeTokenizer;

    /// <summary>Expression tokenizer: "0,1,2,10..12,20..22"</summary>
    static readonly SequenceTokenizer<ExpressionToken> valueTokenizer =
        new(
            (WhitespaceTokenizer.Any, false, false),
            (new AnyTokenizer(rangeTokenizer, integerTokenizer), true, false),
            (WhitespaceTokenizer.Any, false, false),
            (new WhileTokenizer<ExpressionToken>(
                new SequenceTokenizer<ExpressionToken>(
                    (Tokenizers.Whitespace, false, false),
                    (new ConstantTokenizer<SeparatorToken>(","), true, false),
                    (Tokenizers.Whitespace, false, false),
                    (new AnyTokenizer(rangeTokenizer, integerTokenizer), true, true),
                    (Tokenizers.Whitespace, false, false))
                ), false, true)
        );

    /// <summary>Expression tokenizer: "0,1,2,10..12,20..22"</summary>
    public static SequenceTokenizer<ExpressionToken> ValueTokenizer => valueTokenizer;

    /// <summary>"not"</summary>
    static readonly SequenceTokenizer<ExpressionToken> parenthesisTokenizer = new SequenceTokenizer<ExpressionToken>((new ConstantTokenizer<ParenthesisToken>("("), true), (WhitespaceTokenizer.Any, false), (expressionTokenizer, true), (WhitespaceTokenizer.Any, false), (new ConstantTokenizer<ParenthesisToken>(")"), true));

    /// <summary>Expression segment tokenizer: "name expression"</summary>
    static readonly SequenceTokenizer<ExpressionSegmentToken> expressionSegmentTokenizer =
        new(
            (new RegexTokenizer<NameToken>("^[a-zA-Z]{2,}"), false),
            (Tokenizers.Whitespace, false),
            (booleanExpressionTokenizer, true)
        );
    /// <summary>Expression segment tokenizer: "name expression"</summary>
    public static SequenceTokenizer<ExpressionSegmentToken> ExpressionSegmentTokenizer => expressionSegmentTokenizer;

    /// <summary>Plural rule line tokenizer: [ruleInfo]? case expression @sampleGroup samples</summary>
    static readonly SequenceTokenizer<CompositeToken> pluralRuleTokenizer = new(
        (pluralRuleInfoTokenizer, false, false), 
        (expressionSegmentTokenizer, false, false),
        (new WhileTokenizer<SampleSegmentToken>(sampleSegmentTokenizer), false, true)
    );
    /// <summary>Plural rule line tokenizer: [ruleInfo]? case expression @sampleGroup samples</summary>
    static WhileTokenizer<CompositeToken> pluralRulesTokenizer = new WhileTokenizer<CompositeToken>(pluralRuleTokenizer);
    /// <summary>Plural rule line tokenizer: [ruleInfo]? case expression @sampleGroup samples</summary>
    public static SequenceTokenizer<CompositeToken> PluralRuleTokenizer => pluralRuleTokenizer;
    /// <summary>Plural rule line tokenizer: [ruleInfo]? case expression @sampleGroup samples</summary>
    public static WhileTokenizer<CompositeToken> PluralRulesTokenizer => pluralRulesTokenizer;

    static PluralRuleTokenizers()
    {
        // "and" binds more tightly than "or". So X or Y and Z is interpreted as (X or (Y and Z)).
        booleanExpressionTokenizer.Add(orTokenizer);
        booleanExpressionTokenizer.Add(andTokenizer);
        booleanExpressionTokenizer.Add(equalityTokenizer);
        booleanExpressionTokenizer.Add(falseTokenizer);
        booleanExpressionTokenizer.Add(trueTokenizer);
        booleanExpressionTokenizer.Add(notTokenizer);
        booleanExpressionTokenizer.Add(booleanParenthesisTokenizer);
        booleanExpressionTokenizer.SetReadOnly();

        booleanExpressionTokenizer2.Add(andTokenizer);
        booleanExpressionTokenizer2.Add(equalityTokenizer);
        booleanExpressionTokenizer2.Add(falseTokenizer);
        booleanExpressionTokenizer2.Add(trueTokenizer);
        booleanExpressionTokenizer2.Add(notTokenizer);
        booleanExpressionTokenizer2.Add(booleanParenthesisTokenizer);
        booleanExpressionTokenizer2.SetReadOnly();

        booleanExpressionTokenizer3.Add(equalityTokenizer);
        booleanExpressionTokenizer3.Add(falseTokenizer);
        booleanExpressionTokenizer3.Add(trueTokenizer);
        booleanExpressionTokenizer3.Add(notTokenizer);
        booleanExpressionTokenizer3.Add(booleanParenthesisTokenizer);
        booleanExpressionTokenizer3.SetReadOnly();

        expressionTokenizer.Add(valueTokenizer);
        expressionTokenizer.Add(moduloTokenizer);
        expressionTokenizer.Add(variableExpressionTokenizer);
        expressionTokenizer.Add(parenthesisTokenizer);
        expressionTokenizer.SetReadOnly();
        expressionTokenizer2.Add(valueTokenizer);
        expressionTokenizer2.Add(variableExpressionTokenizer);
        expressionTokenizer2.Add(parenthesisTokenizer);
        expressionTokenizer2.SetReadOnly();
    }

    /// <summary>Test run</summary>
    public static void Run()
    {
        //PluralRulesTokenizer.TryTake("@integer 0~15, 100, 1000, 10000, 100000, 1000000, … @decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …", out CompositeToken composite);      
        /*
        PluralRulesTokenizer.TryTake("[Category=cardinal,Culture=,Case=zero,Required=False] n = 0 [Category = cardinal, Culture =, Case = one, Required = True] i = 1 and v = 0 [Category = cardinal, Culture =, Case = other, Required = True]", out CompositeToken composite);
        PluralRulesTokenizer.TryTake("[Category=cardinal,Culture=,Case=zero,Required=False] n = 0 [Category=cardinal,Culture=,Case=zero,Required=False] n = 0 [Category=cardinal,Culture=,Case=zero,Required=False] n = 0 ", out composite);

        var _token0 = PluralRuleInfoTokenizer.TakeAll<PluralRuleInfoToken>("[]");
        var _token1 = PluralRuleInfoTokenizer.TakeAll<PluralRuleInfoToken>("[RuleSet=Unicode.CLDR35]");
        var _token2 = PluralRuleInfoTokenizer.TakeAll<PluralRuleInfoToken>("[RuleSet=Unicode.CLDR35,Category=,Culture=fi,Case=one]");

        var token1 = ExpressionTokenizer.TakeAll<ExpressionToken>("0");
        var token2 = ExpressionTokenizer.TakeAll<ExpressionToken>("0..1");
        var token3 = ExpressionTokenizer.TakeAll<ExpressionToken>("i");
        var token4 = ExpressionTokenizer.TakeAll<ExpressionToken>("i%10");

        var token5 = BooleanExpressionTokenizer.TakeAll<BooleanExpressionToken>("i%10 = 1");
        var token6 = BooleanExpressionTokenizer.TakeAll<BooleanExpressionToken>("v = 0");
        var token7 = BooleanExpressionTokenizer.TakeAll<BooleanExpressionToken>("i % 10 = 0..1,5..6,10..11");
        var token8 = BooleanExpressionTokenizer.TakeAll<BooleanExpressionToken>("n % 10 = 2..4 and n % 100 != 12..14");
        var token9 = BooleanExpressionTokenizer.TakeAll<BooleanExpressionToken>("v = 0 and i != 1 and i % 10 = 0..1 or v = 0 and i % 10 = 5..9 or v = 0 and i % 100 = 12..14");
        var token10 = BooleanExpressionTokenizer.TakeAll<BooleanExpressionToken>("v != 0 or n = 0 or n % 100 = 2..19 or v = 0 and i != 1 and i % 10 = 0..1 or v = 0 and i % 10 = 5..9 or v = 0 and i % 100 = 12..14");

        var token40 = ExpressionSegmentTokenizer.TakeAll<ExpressionSegmentToken>("one i % 10 = 0..1");
        var token41 = ExpressionSegmentTokenizer.TakeAll<ExpressionSegmentToken>("many i % 10 = 0..1");

        var token51 = PluralRuleInfoTokenizer.TakeAll<PluralRuleInfoToken>("[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one]");

        var token16 = SampleTokenizer.TakeAll<IToken>("0.0");
        var token17 = SampleTokenizer.TakeAll<IToken>("0.0~1.5");
        var token18 = SampleTokenizer.TakeAll<IToken>("…");

        var token11 = SampleSegmentTokenizer.TakeAll<SampleSegmentToken>("@integer 0, 1, 2, 3, …");
        var token12 = SampleSegmentTokenizer.TakeAll<SampleSegmentToken>("@decimal 0.0~1.5, 10.0, …");
        var token13 = PluralLineTokenizer.TakeAll<CompositeToken>("@decimal 0.0~1.5, 10.0, …");

        var token20 = PluralLineTokenizer.TakeAll<IToken>("one v = 0 and i % 10 = 1 @integer 0, 1, 2, 3, … @decimal 0.0~1.5, 10.0, …");
        var token21 = PluralLineTokenizer.TakeAll<IToken>("v = 0 and i != 1 and i % 10 = 0..1 or v = 0 and i % 10 = 5..9 or v = 0 and i % 100 = 12..14 @integer 0, 5~19, 100, 1000, 10000, 100000, 1000000, …");
        var token22 = PluralLineTokenizer.TakeAll<IToken>("n % 10 = 2..4 and n % 100 != 12..14 @integer 2~4, 22~24, 32~34, 42~44, 52~54, 62, 102, 1002, … @decimal 2.0, 3.0, 4.0, 22.0, 23.0, 24.0, 32.0, 33.0, 102.0, 1002.0, …");
        var token23 = PluralLineTokenizer.TakeAll<IToken>(" @integer 0, 2~16, 100, 1000, 10000, 100000, 1c3, 2c3, 3c3, 4c3, 5c3, 6c3, … @decimal 0.0~0.9, 1.1~1.6, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, 1.0001c3, 1.1c3, 2.0001c3, 2.1c3, 3.0001c3, 3.1c3, …");
        var token24 = PluralLineTokenizer.TakeAll<IToken>("v != 0 or n = 0 or n % 100 = 2..19 @integer 0, 2~16, 102, 1002, … @decimal 0.0~1.5, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, …");
        var token25 = PluralLineTokenizer.TakeAll<IToken>("n = 3..10,13..19,30..31 @integer 3~10, 13~19 @decimal 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 19.0, 3.00");
        var token26 = PluralLineTokenizer.TakeAll<IToken>("n % 100 = 11..99 @integer 11~26, 111, 1011, … @decimal 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 111.0, 1011.0, …");
        var token27 = PluralLineTokenizer.TakeAll<IToken>("one v = 0 and i % 10 = 1 @integer 0, 1, 2, 3, … @decimal 0.0~1.5, 10.0, …");
        var token29 = PluralRuleTokenizer.TakeAll<IToken>("[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one] @integer 0, 1, 2, 3, … @decimal 0.0~1.5, 10.0, …");
        var token31 = PluralRuleTokenizer.TakeAll<IToken>("[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one] v = 0 and i % 10 = 1");
        var token32 = PluralRuleTokenizer.TakeAll<IToken>("[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one] one not (v = 0..9) and (i % 10) = 1,2,3 and true @integer 0, 1, 2, 3, … @decimal 0.0~1.5, 10.0, …");
        var token33 = PluralRuleTokenizer.TakeAll<IToken>("[RuleSet=Unicode.CLDR40, Category=ordinal, Culture=af, Case=, Optional=0] @integer 0~15, 100, 1000, 10000, 100000, 1000000, …");


        System.Console.WriteLine(token7.PrintTree(format: TokenPrintTreeExtensions.PrintFormat.DefaultLong));
        //System.Console.WriteLine(token6.PrintTree(format: TokenPrintTreeExtensions.PrintFormat.DefaultLong));
        System.Console.WriteLine(token32.PrintTree(format: TokenPrintTreeExtensions.PrintFormat.DefaultLong));
        //System.Console.WriteLine(token32.PrintTree(1, TokenPrintTreeExtensions.PrintFormat.DefaultLong));
        //System.Console.WriteLine(PluralRuleLineTokenizer);
        */
    }

}

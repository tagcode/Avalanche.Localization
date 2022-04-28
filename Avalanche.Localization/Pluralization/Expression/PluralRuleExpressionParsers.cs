// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using Avalanche.Tokenizer;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary>Methods that adapt strings and tokens into expressions.</summary>
public static class PluralRuleExpressionParsers
{
    /// <summary>Provider that parses rule text, e.g. "[Category=cardinal,Culture=,Case=one,Required=True] i=1 and v=0"</summary>
    static readonly IProvider<ReadOnlyMemory<char>, IPluralRule[]> ruleParser = Providers.Func<ReadOnlyMemory<char>, IPluralRule[]>(TryConvertToRules);
    /// <summary>Provider that parses rule text, e.g. "[Category=cardinal,Culture=,Case=one,Required=True] i=1 and v=0"</summary>
    public static IProvider<ReadOnlyMemory<char>, IPluralRule[]> RuleParser => ruleParser;

    /// <summary>Converts rule line. </summary>
    /// <param name="text">
    /// Rule line.
    /// 
    /// E.g. "[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one] one v = 0 and i % 10 = 1 @integer 0, 1, 2, 3, … @decimal 0.0~1.5, 10.0, …".
    /// 
    /// RuleInfo, expression and samples parts are all optional.
    /// </param>
    /// <returns>Expression</returns>
    /// <exception cref="InvalidOperationException">If could not tokenize all </exception>
    public static bool TryConvertToRules(ReadOnlyMemory<char> text, out IPluralRule[] rules)
    {
        //
        if (!TryConvertToRuleExpressions(text, out IPluralRuleExpression[] ruleExpressions)) { rules = null!; return false; }
        // 
        StructList6<IPluralRule> list = new();
        // 
        foreach(IPluralRuleExpression ruleExpression in ruleExpressions)
        {
            list.Add(new PluralRule.Expression(ruleExpression.Info, ruleExpression.Rule, ruleExpression.Samples));
        }
        // Return as array
        rules = list.ToArray();
        return true;
    }


    /// <summary>Converts rule line. </summary>
    /// <param name="text">
    /// Rule line.
    /// 
    /// E.g. "[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one] one v = 0 and i % 10 = 1 @integer 0, 1, 2, 3, … @decimal 0.0~1.5, 10.0, …".
    /// 
    /// RuleInfo, expression and samples parts are all optional.
    /// </param>
    /// <returns>Expression</returns>
    /// <exception cref="InvalidOperationException">If could not tokenize all </exception>
    public static IPluralRuleExpression[] ConvertToRuleExpressions(ReadOnlyMemory<char> text)
    {
        // Result list here
        StructList6<IPluralRuleExpression> list = new();
        // Tokenize
        if (!PluralRuleTokenizers.PluralRulesTokenizer.TryTake(text, out CompositeToken composite)) throw new InvalidOperationException($"Could not tokenize {text}");
        // 
        foreach (IToken token in composite.Children)
        {
            // Convert
            IPluralRuleExpression pluralRuleExpression = ConvertRule(token.Children);
            // Add to result
            list.Add(pluralRuleExpression);
        }
        // Return as array
        return list.ToArray();
    }

    /// <summary>Converts rule line. </summary>
    /// <param name="text">
    /// Rule line.
    /// 
    /// E.g. "[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one] one v = 0 and i % 10 = 1 @integer 0, 1, 2, 3, … @decimal 0.0~1.5, 10.0, …".
    /// 
    /// RuleInfo, expression and samples parts are all optional.
    /// </param>
    /// <returns>Expression</returns>
    /// <exception cref="InvalidOperationException">If could not tokenize all </exception>
    public static bool TryConvertToRuleExpressions(ReadOnlyMemory<char> text, out IPluralRuleExpression[] ruleExpressions)
    {
        // Result list here
        StructList6<IPluralRuleExpression> list = new();
        // Tokenize
        if (!PluralRuleTokenizers.PluralRulesTokenizer.TryTake(text, out CompositeToken composite)) { ruleExpressions = null!; return false; }
        // 
        foreach(IToken token in composite.Children)
        {
            // Convert
            IPluralRuleExpression pluralRuleExpression = ConvertRule(token.Children);
            // Add to result
            list.Add(pluralRuleExpression);
        }
        // Return as array
        ruleExpressions = list.ToArray();
        return true;
    }

    /// <summary>Converts rule line. </summary>
    /// <param name="text">
    /// Rule line.
    /// 
    /// E.g. "[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one] one v = 0 and i % 10 = 1 @integer 0, 1, 2, 3, … @decimal 0.0~1.5, 10.0, …".
    /// 
    /// RuleInfo, expression and samples parts are all optional.
    /// </param>
    /// <returns>Expression</returns>
    /// <exception cref="InvalidOperationException">If could not tokenize all </exception>
    public static IPluralRuleExpression ConvertRule(ReadOnlyMemory<char> text)
    {
        // Tokenize
        PluralRuleTokenizers.PluralRulesTokenizer.TryTake(text, out CompositeToken composite);
        //
        IToken[] children = composite.Children.Length == 0 ? Array.Empty<IToken>() : composite.Children[0].Children;
        // Convert
        IPluralRuleExpression pluralRuleExpression = ConvertRule(children);
        // Return
        return pluralRuleExpression;
    }

    /// <summary>Converts single rule from <paramref name="tokens"/>. e.g. "[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one] one v = 0 and i % 10 = 1 @integer 0, 1, 2, 3, … @decimal 0.0~1.5, 10.0, …".</summary>
    /// <returns>Expression</returns>
    /// <example>
    /// [0:136] CompositeToken:  "[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one] one v = 0 and i % 10 = 1 @integer 0, 1, 2, 3, . @decimal 0.0~1.5, 10.0, ."
    /// ├── [0:63] PluralRuleInfoToken:  "[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one] "
    /// ├── [63:88] ExpressionSegmentToken:  "one v = 0 and i % 10 = 1 "
    /// ├── [88:111] SampleSegmentToken:  "@integer 0, 1, 2, 3, . "
    /// └── [111:136] SampleSegmentToken:  "@decimal 0.0~1.5, 10.0, ."
    /// </example>
    public static IPluralRuleExpression ConvertRule(IEnumerable<IToken> tokens)
    {
        // Place here result
        IExpression? rule = null!;
        StructList6<ISamplesExpression> samples = new();
        KeyValuePair<string, string>[]? infos = null;
        string @case = null!;
        // Iterate tokens
        foreach (IToken token in tokens)
        {
            // PluralRuleInfo
            if (token is PluralRuleInfoToken pluralRuleInfoToken)
            {
                // Already has info
                if (infos != null) throw new InvalidOperationException($"Got second {nameof(PluralRuleInfoToken)} {pluralRuleInfoToken}");
                // Convert info
                infos = ConvertRuleInfos(pluralRuleInfoToken);
            }
            // ExpressionSegmentToken
            else if (token is ExpressionSegmentToken expressionSegmentToken)
            {
                // Already has rule
                if (rule != null) throw new InvalidOperationException($"Got second {nameof(ExpressionSegmentToken)} {expressionSegmentToken}");
                // Convert info
                (string _case, IExpression ruleExpression) = ConvertExpressionSegment(expressionSegmentToken);
                //
                if (ruleExpression != null)
                {
                    rule = ruleExpression;
                    @case = _case;
                }
            }
            // SampleSegmentToken
            else if (token is SampleSegmentToken sampleSegmentToken)
            {
                // Convert sample segment
                ISamplesExpression samplesExpression = ConvertSampleSegment(sampleSegmentToken);
                // Add
                samples.Add(samplesExpression);
            }
        }
        // Create rule inf
        PluralRuleInfo info = infos == null || infos.Length == 0 ? new PluralRuleInfo(null, null, null, @case, null) : new PluralRuleInfo(infos);
        // Create result
        PluralRuleExpression result = new PluralRuleExpression(info, rule, samples.ToArray());
        // Return result
        return result;
    }

    /// <summary>Convert rule info <paramref name="token"/>, e.g. "[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one]"</summary>
    /// <example>
    /// [0:63] PluralRuleInfoToken:  "[RuleSet=Unicode.CLDR35,Category=cardinal,Culture=fi,Case=one] "
    /// ├── [0:1] TextToken:  "["
    /// ├── [1:24] KeyValueToken:  "RuleSet=Unicode.CLDR35,"
    /// │   ├── [1:8] KeyToken:  "RuleSet"
    /// │   ├── [8:9] OperandToken:  "="
    /// │   ├── [9:23] ValueToken:  "Unicode.CLDR35"
    /// │   └── [23:24] SeparatorToken:  ","
    /// ├── [24:42] KeyValueToken:  "Category=cardinal,"
    /// │   ├── [24:32] KeyToken:  "Category"
    /// │   ├── [32:33] OperandToken:  "="
    /// │   ├── [33:41] ValueToken:  "cardinal"
    /// │   └── [41:42] SeparatorToken:  ","
    /// ├── [42:53] KeyValueToken:  "Culture=fi,"
    /// │   ├── [42:49] KeyToken:  "Culture"
    /// │   ├── [49:50] OperandToken:  "="
    /// │   ├── [50:52] ValueToken:  "fi"
    /// │   └── [52:53] SeparatorToken:  ","
    /// ├── [53:61] KeyValueToken:  "Case=one"
    /// │   ├── [53:57] KeyToken:  "Case"
    /// │   ├── [57:58] OperandToken:  "="
    /// │   └── [58:61] ValueToken:  "one"
    /// ├── [61:62] TextToken:  "]"
    /// └── [62:63] WhitespaceToken:  " "
    /// </example>
    public static KeyValuePair<string, string>[] ConvertRuleInfos(PluralRuleInfoToken token)
    {
        // Initialize
        StructList6<KeyValuePair<string, string>> infos = new();
        // Iterate tokens
        foreach (IToken token_ in token.Children)
        {
            // Not key-value
            if (token_ is not KeyValueToken keyValueToken) continue;
            // Convert
            KeyValuePair<string, string>? keyValue = ConvertRuleArgument(keyValueToken);
            // Add
            if (keyValue != null) infos.Add(keyValue.Value);
        }
        // Return arguments
        return infos.ToArray();
    }

    /// <summary>Convert <paramref name="token"/> to rule argument. e.g. "RuleSet=Unicode.CLDR35"</summary>
    /// <example>
    /// [1:24] KeyValueToken:  "RuleSet=Unicode.CLDR35,"
    /// ├── [1:8] KeyToken:  "RuleSet"
    /// ├── [8:9] OperandToken:  "="
    /// ├── [9:23] ValueToken:  "Unicode.CLDR35"
    /// └── [23:24] SeparatorToken:  ","
    /// </example>
    public static KeyValuePair<string, string>? ConvertRuleArgument(KeyValueToken token)
    {
        // Place here key and value
        string? key = null, value = null;
        // Find key-value
        foreach (IToken t in token.Children)
        {
            if (t is KeyToken) key = t.Text();
            else if (t is ValueToken) value = t.Text();
        }
        // Got key and value
        if (!string.IsNullOrEmpty(key)) return new KeyValuePair<string, string>(key, value ?? "");
        // No value
        return null;
    }

    /// <summary>Converts <paramref name="token"/> containing expression segment. e.g. one v = 0 and i % 10 = 1 "</summary>
    /// <returns>Expression</returns>
    /// <example>
    /// ├── [63:88] ExpressionSegmentToken:  "one v = 0 and i % 10 = 1 "
    /// │   ├── [63:66] NameToken:  "one"
    /// │   ├── [66:67] WhitespaceToken:  " "
    /// │   └── [67:88] BooleanExpressionToken:  "v = 0 and i % 10 = 1 "
    /// │       ├── [67:73] BooleanExpressionToken:  "v = 0 "
    /// │       │   ├── [67:68] ExpressionToken:  "v"
    /// │       │   ├── [68:69] WhitespaceToken:  " "
    /// │       │   ├── [69:70] OperandToken:  "="
    /// │       │   ├── [70:71] WhitespaceToken:  " "
    /// │       │   ├── [71:72] ExpressionToken:  "0"
    /// │       │   │   └── [71:72] ExpressionToken:  "0"
    /// │       │   └── [72:73] WhitespaceToken:  " "
    /// │       ├── [73:76] OperandToken:  "and"
    /// │       ├── [76:77] WhitespaceToken:  " "
    /// │       └── [77:88] BooleanExpressionToken:  "i % 10 = 1 "
    /// │           ├── [77:84] ExpressionToken:  "i % 10 "
    /// │           │   ├── [77:78] ExpressionToken:  "i"
    /// │           │   ├── [78:79] WhitespaceToken:  " "
    /// │           │   ├── [79:80] OperandToken:  "%"
    /// │           │   ├── [80:81] WhitespaceToken:  " "
    /// │           │   ├── [81:83] ExpressionToken:  "10"
    /// │           │   └── [83:84] WhitespaceToken:  " "
    /// │           ├── [84:85] OperandToken:  "="
    /// │           ├── [85:86] WhitespaceToken:  " "
    /// │           ├── [86:87] ExpressionToken:  "1"
    /// │           │   └── [86:87] ExpressionToken:  "1"
    /// │           └── [87:88] WhitespaceToken:  " "
    /// </example>
    public static (string @case, IExpression expression) ConvertExpressionSegment(ExpressionSegmentToken token)
    {
        //
        string? @case = null;
        IExpression? expression = null!;
        //
        foreach (IToken t in token.Children)
        {
            // Case name
            if (t is NameToken nameToken)
            {
                // Already has name
                if (!string.IsNullOrEmpty(@case)) throw new InvalidOperationException($"Got second case {nameToken}");
                // Assign
                @case = nameToken.Text();
            }
            // Expression
            if (t is BooleanExpressionToken booleanExpressionToken)
            {
                // Already has name
                if (expression != null) throw new InvalidOperationException($"Got second expression {booleanExpressionToken}");
                // Assign
                expression = ConvertBooleanExpression(booleanExpressionToken);
            }
        }
        // Return
        return (@case!, expression!);
    }

    /// <summary>Converts <paramref name="token"/> into expression. e.g. "i % 10 = 1".</summary>
    /// <example>
    /// └── [77:88] BooleanExpressionToken:  "i % 10 = 1 "
    ///     ├── [77:84] ExpressionToken:  "i % 10 "
    ///     │   ├── [77:78] ExpressionToken:  "i"
    ///     │   ├── [78:79] WhitespaceToken:  " "
    ///     │   ├── [79:80] OperandToken:  "%"
    ///     │   ├── [80:81] WhitespaceToken:  " "
    ///     │   ├── [81:83] ExpressionToken:  "10"
    ///     │   └── [83:84] WhitespaceToken:  " "
    ///     ├── [84:85] OperandToken:  "="
    ///     ├── [85:86] WhitespaceToken:  " "
    ///     ├── [86:87] ExpressionToken:  "1"
    ///     │   └── [86:87] ExpressionToken:  "1"
    ///     └── [87:88] WhitespaceToken:  " "
    /// </example>
    public static IExpression ConvertBooleanExpression(BooleanExpressionToken token)
    {
        // Place relevant parts here
        string? operand = null;
        StructList3<IExpression> booleanExpressions = new();
        StructList3<IExpression> expressions = new();
        // Catch relevant parts
        foreach (IToken t in token.Children)
        {
            // Operand
            if (t is OperandToken operandToken)
            {
                if (!string.IsNullOrEmpty(operand)) throw new InvalidOperationException($"Unexpected second operand {t}");
                operand = operandToken.Text();
                continue;
            }
            // Boolean expression
            if (t is BooleanExpressionToken bet) { booleanExpressions.Add(ConvertBooleanExpression(bet)); continue; }
            // Expression
            if (t is ExpressionToken et) { expressions.Add(ConvertExpression(et)); continue; }
        }
        // Recurse
        if (operand == null && expressions.Count == 0 && booleanExpressions.Count == 1) return booleanExpressions[0];
        // "true"
        if (operand == "true" && expressions.Count == 0 && booleanExpressions.Count == 0) return new ConstantExpression(true);
        // "false"
        if (operand == "true" && expressions.Count == 0 && booleanExpressions.Count == 0) return new ConstantExpression(false);
        // "not"
        if (operand == "not" && expressions.Count == 0 && booleanExpressions.Count == 1) return new UnaryOpExpression(UnaryOp.Not, booleanExpressions[0]);
        // "and"
        if (operand == "and" && expressions.Count == 0 && booleanExpressions.Count == 2) return new BinaryOpExpression(BinaryOp.LogicalAnd, booleanExpressions[0], booleanExpressions[1]);
        // "or"
        if (operand == "or" && expressions.Count == 0 && booleanExpressions.Count == 2) return new BinaryOpExpression(BinaryOp.LogicalOr, booleanExpressions[0], booleanExpressions[1]);

        // "="
        if (operand == "=" && expressions.Count == 2 && booleanExpressions.Count == 0) return new BinaryOpExpression(BinaryOp.Equal, expressions[0], expressions[1]);
        // "!="
        if (operand == "!=" && expressions.Count == 2 && booleanExpressions.Count == 0) return new BinaryOpExpression(BinaryOp.NotEqual, expressions[0], expressions[1]);
        // "<"
        if (operand == "<" && expressions.Count == 2 && booleanExpressions.Count == 0) return new BinaryOpExpression(BinaryOp.LessThan, expressions[0], expressions[1]);
        // "<="
        if (operand == "<=" && expressions.Count == 2 && booleanExpressions.Count == 0) return new BinaryOpExpression(BinaryOp.LessThanOrEqual, expressions[0], expressions[1]);
        // ">"
        if (operand == ">" && expressions.Count == 2 && booleanExpressions.Count == 0) return new BinaryOpExpression(BinaryOp.GreaterThan, expressions[0], expressions[1]);
        // ">="
        if (operand == ">=" && expressions.Count == 2 && booleanExpressions.Count == 0) return new BinaryOpExpression(BinaryOp.GreaterThanOrEqual, expressions[0], expressions[1]);

        // 
        throw new InvalidOperationException($"Unhandled token {token}");
    }

    /// <summary>Converts expression <paramref name="token"/> into value expression. e.g. "i % 10".</summary>
    /// <example>
    /// [83:91] ExpressionToken:  "(i % 10)"
    /// ├── [83:84] ParenthesisToken:  "("
    /// ├── [84:90] ExpressionToken:  "i % 10"
    /// │   ├── [84:85] ExpressionToken:  "i"
    /// │   │   └── [84:85] VariableToken:  "i"
    /// │   ├── [85:86] WhitespaceToken:  " "
    /// │   ├── [86:87] OperandToken:  "%"
    /// │   ├── [87:88] WhitespaceToken:  " "
    /// │   └── [88:90] DecimalToken:  "10"
    /// └── [90:91] ParenthesisToken:  ")"
    /// </example>
    public static IExpression ConvertExpression(ExpressionToken token)
    {
        // Place relevant parts here
        string? operand = null;
        StructList3<IExpression> expressions = new();
        // Catch relevant parts
        foreach (IToken t in token.Children)
        {
            // Operand
            if (t is OperandToken operandToken)
            {
                if (!string.IsNullOrEmpty(operand)) throw new InvalidOperationException($"Unexpected second operand {t}");
                operand = operandToken.Text();
                continue;
            }
            // Expression
            if (t is ExpressionToken et) { expressions.Add(ConvertExpression(et)); continue; }
            // Variable
            if (t is VariableToken vt) { expressions.Add(new ArgumentNameExpression(vt.Text())); continue; }
            // Constant
            if (t is DecimalToken nt) { expressions.Add(new ConstantExpression(ConvertNumber(nt.Memory))); continue; }
        }
        // Recurse
        if (operand == null && expressions.Count == 1) return expressions[0];
        // Group
        if (operand == null && expressions.Count > 1) return new GroupExpression(expressions.ToArray());
        // "%"
        if (operand == "%" && expressions.Count == 2) return new BinaryOpExpression(BinaryOp.Modulo, expressions[0], expressions[1]);
        // ".."
        if (operand == ".." && expressions.Count == 2) return new RangeExpression(expressions[0], expressions[1]);
        // 
        throw new InvalidOperationException($"Unhandled token {token}");
    }

    /// <summary>Converts sample <paramref name="token"/> into expression. e.g. "@decimal 0.0~1.5, 10.0, …".</summary>
    /// <example>
    /// [135:160] SampleSegmentToken:  "@decimal 0.0~1.5, 10.0, …"
    /// ├── [135:136] SampleToken:  "@"
    /// ├── [136:143] NameToken:  "decimal"
    /// ├── [143:144] WhitespaceToken:  " "
    /// ├── [144:151] RangeToken:  "0.0~1.5"
    /// │   ├── [144:147] DecimalToken:  "0.0"
    /// │   ├── [147:148] SeparatorToken:  "~"
    /// │   └── [148:151] DecimalToken:  "1.5"
    /// ├── [151:152] SeparatorToken:  ","
    /// ├── [152:153] WhitespaceToken:  " "
    /// ├── [153:157] DecimalToken:  "10.0"
    /// ├── [157:158] SeparatorToken:  ","
    /// ├── [158:159] WhitespaceToken:  " "
    /// └── [159:160] OperandToken:  "…"
    /// </example>
    public static ISamplesExpression ConvertSampleSegment(SampleSegmentToken token)
    {
        // Init
        string? name = null!;
        StructList5<IExpression> samples = new();
        // Iterate
        foreach (IToken t in token.Children)
        {
            // Name
            if (t is NameToken nameToken)
            {
                // Already has name
                if (!string.IsNullOrEmpty(name)) throw new InvalidOperationException($"Got second sample name {nameToken}");
                // Assign
                name = nameToken.Text();
            }
            // Sample
            if (t is RangeToken || t is DecimalToken || t is OperandToken)
            {
                // Convert 
                IExpression sampleExpression = ConvertSample(t);
                // Add
                samples.Add(sampleExpression);
            }
        }
        // Return
        return new SamplesExpression(name, samples.ToArray());
    }

    /// <summary>Converts sample <paramref name="token"/> into expression. e.g. "0.0~1.5".</summary>
    /// <example>
    /// ├── [120:127] RangeToken:  "0.0~1.5"
    /// │   ├── [120:123] DecimalToken:  "0.0"
    /// │   ├── [123:124] SeparatorToken:  "~"
    /// │   └── [124:127] DecimalToken:  "1.5"
    /// ├── [127:128] SeparatorToken:  ","
    /// └── [128:129] WhitespaceToken:  " "
    /// </example>
    public static IExpression ConvertSample(IToken token)
    {
        // a~b
        if (token is RangeToken rangeToken)
        {
            //
            IExpression min = null!, max = null!;
            // Iterate
            foreach (IToken t in rangeToken.Children)
            {
                if (t is DecimalToken || t is OperandToken)
                {
                    // As expression
                    IExpression value = ConvertSample(t);
                    // Assign min
                    if (min == null) { min = value; continue; }
                    // Assign max
                    if (min != null && max == null) { max = value; continue; }
                    // ?
                    throw new NotSupportedException($"Unsupported {t}");
                }
            }
            // Assert
            if (min == null || max == null) throw new NotSupportedException($"Expected min and max value {rangeToken}");
            // Return
            return new RangeExpression(min, max);
        }
        // 0.0
        if (token is DecimalToken numberToken) return new ConstantExpression(ConvertNumber(numberToken.Memory));
        // '…'
        if (token is OperandToken textToken && textToken.Memory.Length == 1 && textToken.Memory.Span[0] == '…') return new InfiniteExpression();
        // Unsupported
        throw new NotSupportedException($"Unsupported {token}");
    }

    /// <summary>Convert number chars to number </summary>
    public static IDecimalNumber ConvertNumber(ReadOnlyMemory<char> numberChars)
    {
        // Place here text
        string text;
        /*
        // Rent buffer
        char[] buf = ArrayPool<char>.Shared.Rent(numberChars.Length);
        try
        {
            //
            ReadOnlySpan<char> span = numberChars.Span;
            //
            for (int i=0; i<numberChars.Length; i++)
            {
                // Get char
                char ch = span[i];
                // 'c' -> 'e'
                if (ch == 'c') ch = 'e'; else if (ch == 'C') ch = 'E';
                // 
                buf[i] = ch;
            }
            // Print to string object. We don't want to cache the whole rule text as it may be a full xml document.
            string text = new string(buf, 0, numberChars.Length);
        } finally
        {
            ArrayPool<char>.Shared.Return(buf);
        }*/
        //
        text = numberChars.AsString();
        // Wrap to text number
        IDecimalNumber number = new TextNumber(text);
        //
        return number;
    }

}


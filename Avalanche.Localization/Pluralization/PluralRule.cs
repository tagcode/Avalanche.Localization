// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization.Pluralization;
using Avalanche.Utilities;

/// <summary>Basic plural rule info.</summary>
public class PluralRule : ReadOnlyAssignableClass, IPluralRule
{
    /// <summary>Plural rule info</summary>
    protected PluralRuleInfo info;
    /// <summary>Plural rule info</summary>
    public PluralRuleInfo Info { get => info; set => this.AssertWritable().info = value; }

    /// <summary>Create rule.</summary>
    /// <param name="info">info</param>
    public PluralRule(PluralRuleInfo info)
    {
        this.Info = info;
    }

    /// <summary>Evaluate number to the rule.</summary>
    public virtual bool Evaluate<N>(N number) where N : IPluralNumber => false;

    /// <summary>Print debug info</summary>
    public override string ToString() => $"{GetType().Name}[{Info}]";

    /// <summary>Zero case that matches when number is 0.</summary>
    public class Zero : PluralRule
    {
        /// <summary>Create rule that compares to zero value.</summary>
        public Zero(PluralRuleInfo info) : base(info) { }

        /// <summary>Compare to zero.</summary>
        public override bool Evaluate<N>(N number) => number != null && number.Sign == 0;

        /// <summary>Print information</summary>
        public override string ToString() => "[" + Info + "] n=0";
    }

    /// <summary>Case that matches when number is 1.</summary>
    public class One : PluralRule
    {
        /// <summary>Singleton</summary>
        static IPluralNumber instance = new LongNumber(1L);
        /// <summary>Singleton</summary>
        public static IPluralNumber Instance => instance;

        /// <summary>Create rule that compares to zero value.</summary>
        public One(PluralRuleInfo info) : base(info) { }

        /// <summary>Compare to zero.</summary>
        public override bool Evaluate<N>(N number) => PluralNumberComparer.Default.Equals(number, instance);

        /// <summary>Print information</summary>
        public override string ToString() => "[" + Info + "] n=1";
    }

    /// <summary>Null case that matches when number is null or empty.</summary>
    public class Empty : PluralRule
    {
        /// <summary>Create rule</summary>
        public Empty(PluralRuleInfo info) : base(info) { }
        /// <summary>Compare to null.</summary>
        public override bool Evaluate<N>(N number) => number == null || (number.I_Digits == 0 && number.F_Digits == 0 && number.E_Digits == 0 && number.Sign == 0);
        /// <summary>Print information</summary>
        public override string ToString() => "[" + Info + "] n=null";
    }

    /// <summary>Case that always evaluates to true value. Used for fallback case "other".</summary>
    public class True : PluralRule
    {
        /// <summary>Create rule that always evaluates to true.</summary>
        public True(PluralRuleInfo info) : base(info) { }

        /// <summary>Always true</summary>
        public override bool Evaluate<N>(N number) => true;

        /// <summary>Print information</summary>
        public override string ToString() => "[" + Info + "] true";
    }

    /// <summary>Case that always evaluates to true value. Used for fallback case "other".</summary>
    public class TrueWithExpression : PluralRule, IPluralRuleExpression
    {
        /// <summary></summary>
        protected IExpression? rule;
        /// <summary></summary>
        protected ISamplesExpression[]? samples;
        /// <summary></summary>
        public IExpression? Rule { get => rule; set => this.AssertWritable().rule = value; }
        /// <summary></summary>
        public ISamplesExpression[]? Samples { get => samples; set => this.AssertWritable().samples = value; }
        /// <summary></summary>
        public int ComponentCount => Samples == null ? 0 : Samples.Length;
        /// <summary></summary>
        public IExpression? GetComponent(int ix) => Samples![ix];

        /// <summary>Create rule that always evaluates to true.</summary>
        public TrueWithExpression(PluralRuleInfo info, IExpression? ruleExp, params ISamplesExpression[] samplesExps) : base(info)
        {
            this.Rule = ruleExp;
            this.Samples = samplesExps;
        }

        /// <summary>Always true</summary>
        public override bool Evaluate<N>(N number) => true;

        /// <summary>Print rule expression</summary>
        public override string ToString()
        {
            var printer = new PluralRuleExpressionStringPrinter();
            printer.Append('[').Append(Info.ToString()).Append("]");
            if (Rule != null) printer.Append(" ");
            if (Samples != null) printer.Append(' ').Append(Samples, " ");
            return printer.ToString();
        }
    }

    /// <summary>Rule that evaluates against an expression</summary>
    public class Expression : PluralRule, IPluralRuleExpression
    {
        /// <summary>Rule expression that can evaluate a number</summary>
        protected IExpression? rule;
        /// <summary>Samples</summary>
        protected ISamplesExpression[]? samples;
        /// <summary>Rule expression that can evaluate a number</summary>
        public IExpression? Rule { get => rule; set => this.AssertWritable().rule = value; }
        /// <summary>Samples</summary>
        public ISamplesExpression[]? Samples { get => samples; set => this.AssertWritable().samples = value; }
        /// <summary>No samples</summary>
        public static ISamplesExpression[] NO_SAMPLES = new ISamplesExpression[0];

        /// <summary></summary>
        public int ComponentCount => Samples == null ? 0 : Samples.Length;
        /// <summary></summary>
        public IExpression? GetComponent(int ix) => Samples![ix];

        /// <summary>Create rule that evaluates with <paramref name="ruleExpression"/>. Extracts <see cref="PluralRuleInfo"/> from <paramref name="info"/> expression.</summary>
        public Expression(PluralRuleInfo info, IExpression? ruleExpression, params ISamplesExpression[]? samplesExpression) : base(info)
        {
            this.Rule = ruleExpression;
            this.Samples = samplesExpression ?? NO_SAMPLES;
        }

        /// <summary>Evaluate <paramref name="number"/> against <see cref="Rule"/>.</summary>
        public override bool Evaluate<N>(N number)
        {
            // No rule
            if (Rule == null) return true;
            // Create evaluator
            var eval = new PluralRuleExpressionEvaluator(number);
            // Evaluation
            bool match = eval.EvaluateBoolean(Rule);
            // Return match
            return match;
        }

        /// <summary>Print rule expression</summary>
        public override string ToString()
        {
            var printer = new PluralRuleExpressionStringPrinter();
            printer.Append(this);
            return printer.ToString();
        }


    }

}



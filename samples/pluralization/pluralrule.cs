using System.Globalization;
using Avalanche.Localization;
using Avalanche.Localization.Pluralization;

class pluralrule
{
    public static void Run()
    {
        {
            PluralRuleInfo info = new PluralRuleInfo("PluralRules", "cardinal", "fi", "one", true);
            IExpression expression = new BinaryOpExpression(BinaryOp.Equal, new ArgumentNameExpression("i"), new ConstantExpression(new TextNumber("0", CultureInfo.InvariantCulture)));
            IPluralRule rule = new PluralRule.Expression(info, expression);
        }
        {
            PluralRuleInfo info = new PluralRuleInfo("PluralRules", "cardinal", "fi", "zero", true);
            IPluralRule rule = new PluralRule.Zero(info);
        }
        {
            PluralRuleInfo info = new PluralRuleInfo("PluralRules", "cardinal", "fi", "other", true);
            IPluralRule rule = new PluralRule.True(info);
        }
        {
            PluralRuleInfo info = new PluralRuleInfo("PluralRules", "cardinal", "fi", "", true);
            IPluralRule rule = new PluralRule.Empty(info);
        }
    }
}

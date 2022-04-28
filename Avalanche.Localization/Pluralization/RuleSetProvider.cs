namespace Avalanche.Localization.Pluralization;
using System;
using System.Reflection;
using Avalanche.Utilities.Provider;

/// <summary>Loads rulesets.</summary>
public class RuleSetProvider : ProviderBase<string, IPluralRules>
{
    /// <summary>Prefix</summary>
    public const string Key_CLDR = "Unicode.CLDR";
    /// <summary>Singleton</summary>
    static readonly IProvider<string, IPluralRules> instance = new RuleSetProvider();
    /// <summary>Singleton</summary>
    static readonly IProvider<string, IPluralRules> cached = instance.ResultCaptured().Cached().ResultOpened();
    /// <summary>Singleton</summary>
    public static IProvider<string, IPluralRules> Instance => instance;
    /// <summary>Singleton</summary>
    public static IProvider<string, IPluralRules> Cached => cached;

    /// <summary>Load plural rules by <paramref name="ruleset"/></summary>
    public override bool TryGetValue(string ruleset, out IPluralRules pluralRules)
    {
        // Null
        if (string.IsNullOrEmpty(ruleset)) { pluralRules = null!; return false; }


        // Try loading specific type
        if (ruleset.StartsWith(Key_CLDR))
        {
            // "Avalanche.Localization.Pluralization.CLDR__PluralRules" 
            string typeName = $"Avalanche.Localization.Pluralization.CLDR{ruleset.Substring(Key_CLDR.Length)}PluralRules, Avalanche.Localization.Cldr";
            // Try load
            if (TryLoadRules(typeName, true, out pluralRules)) return true;
        }

        // Search for type
        if (TryLoadRules(ruleset, false, out pluralRules)) return true;
        // Not found
        pluralRules = null!;
        return false;
    }

    /// <summary>Try load rules from <paramref name="typeName"/>.</summary>
    protected virtual bool TryLoadRules(string typeName, bool throwOnError, out IPluralRules pluralRules)
    {
        // Load type
        Type? type = Type.GetType(typeName, throwOnError);
        // No type
        if (type == null) { pluralRules = null!; return false; }
        // Assert implements IPlrualRules
        if (!type.IsAssignableTo(typeof(IPluralRules))) { pluralRules = null!; return false; }
        // Find "Instance" property
        MethodInfo? getter = type.GetProperty("Instance")?.GetGetMethod();
        // Get
        if (getter != null && getter.IsStatic && getter.ReturnType.IsAssignableTo(typeof(IPluralRules)) && getter.GetParameters().Length == 0)
        {
            // Invoke
            pluralRules = (getter.Invoke(null, null) as IPluralRules)!;
            // Got instance
            if (pluralRules != null) return true;
        }
        // Instantiate
        pluralRules = (Activator.CreateInstance(type) as IPluralRules)!;
        // Got instance
        return pluralRules != null;
    }

}

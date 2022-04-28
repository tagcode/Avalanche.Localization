// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Localization;
using System.Globalization;
using Avalanche.Utilities;
using Avalanche.Utilities.Provider;

/// <summary></summary>
public class CultureProvider : ReadOnlyAssignableClass, ICultureProvider
{
    /// <summary>Non-cached typeName constructor.</summary>
    static readonly IProvider<string, ICultureProvider> byTypeName = Providers.Func<string, ICultureProvider>(TryCreateByTypeName);
    /// <summary>Non-cached typeName constructor.</summary>
    static readonly IProvider<string, ICultureProvider> byTypeNameCached = byTypeName.AsReadOnly().ValueResultCaptured().Cached().ValueResultOpened();
    /// <summary>Non-cached type constructor.</summary>
    static readonly IProvider<Type, ICultureProvider> byType = Providers.Func<Type, ICultureProvider>(TryCreateByType);
    /// <summary>Non-cached type constructor.</summary>
    static readonly IProvider<Type, ICultureProvider> byTypeCached = byType.AsReadOnly().ValueResultCaptured().Cached().ValueResultOpened();
    /// <summary>Non-cached typeName constructor.</summary>
    public static IProvider<string, ICultureProvider> ByTypeName => byTypeName;
    /// <summary>Non-cached typeName constructor.</summary>
    public static IProvider<string, ICultureProvider> ByTypeNameCached => byTypeNameCached;
    /// <summary>Non-cached type constructor.</summary>
    public static IProvider<Type, ICultureProvider> ByType => byType;
    /// <summary>Non-cached type constructor.</summary>
    public static IProvider<Type, ICultureProvider> ByTypeCached => byTypeCached;

    /// <summary>Return default culture provider (CurrentCulture.CurrentUICulture)</summary>
    public static ICultureProvider Default => CultureProvider.CurrentCulture.Instance;

    /// <summary>Language-Region</summary>
    protected string culture;
    /// <summary>Format</summary>
    protected IFormatProvider format;
    /// <summary>Culture</summary>
    public string Culture { get => culture; set => this.AssertWritable().culture = value; }
    /// <summary>Format</summary>
    public IFormatProvider Format { get => format; set => this.AssertWritable().format = value; }

    /// <summary>Try create instance of <paramref name="typeName"/>. Must have parameterless constructor.</summary>
    static bool TryCreateByTypeName(string typeName, out ICultureProvider cultureProvider)
    {
        // Get type
        if (!TypeProvider.Instance.TryGetValue(typeName, out Type[] types) || types == null || types.Length != 1) { cultureProvider = null!; return false; }
        // Create instance
        cultureProvider = (Activator.CreateInstance(types[0]) as ICultureProvider)!;
        return cultureProvider != null;
    }

    /// <summary>Try create instance of <paramref name="type"/>. Must have parameterless constructor.</summary>
    static bool TryCreateByType(Type type, out ICultureProvider cultureProvider)
    {
        // No type
        if (type == null) { cultureProvider = null!; return false; }
        // Create instance
        cultureProvider = (Activator.CreateInstance(type) as ICultureProvider)!;
        return cultureProvider != null;
    }

    /// <summary></summary>
    public CultureProvider()
    {
        culture = null!;
        format = null!;
    }

    /// <summary></summary>
    public CultureProvider(CultureInfo culture)
    {
        this.culture = culture.Name;
        this.format = culture;
    }

    /// <summary></summary>
    public CultureProvider(CultureInfo language, IFormatProvider format)
    {
        this.culture = language.Name;
        this.format = format;
    }

    /// <summary></summary>
    public CultureProvider(string culture)
    {
        this.culture = culture;
        this.format = CultureInfo.GetCultureInfo(culture);
    }

    /// <summary></summary>
    public CultureProvider(string language, string format)
    {
        this.culture = language;
        this.format = CultureInfo.GetCultureInfo(format);
    }

    /// <summary></summary>
    public CultureProvider(string language, IFormatProvider format)
    {
        this.culture = language;
        this.format = format;
    }

    /// <summary></summary>
    public override string ToString() => $"({Culture},{Format})";

    /// <summary>Provides invariant culture ""</summary>
    public class InvariantCulture : CultureProvider
    {
        /// <summary></summary>
        static ICultureProvider? instance;
        /// <summary></summary>
        public static ICultureProvider Instance => instance ??= ByTypeCached[typeof(InvariantCulture)];
        /// <summary></summary>
        public InvariantCulture() : base(CultureInfo.InvariantCulture) { }
        /// <summary></summary>
        public override string ToString() => "InvariantCulture";
    }

    /// <summary>Provides english language "en", but no region. </summary>
    public class En : CultureProvider
    {
        /// <summary></summary>
        static ICultureProvider? instance;
        /// <summary></summary>
        public static ICultureProvider Instance => instance ??= ByTypeCached[typeof(En)];
        /// <summary></summary>
        public En() : base("en") { }
        /// <summary></summary>
        public override string ToString() => "en";
    }

    /// <summary></summary>
    public class Func : ICultureProvider
    {
        /// <summary></summary>
        public string Culture { get => func().Culture; set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public IFormatProvider Format { get => func().Format; set => throw new InvalidOperationException(); }
        /// <summary></summary>
        protected Func<(string Culture, IFormatProvider Format)> func;
        /// <summary></summary>
        public Func(Func<CultureInfo> func1)
        {
            // Assert not null
            if (func1 == null) throw new ArgumentNullException(nameof(func1));
            // Adapt func
            this.func = () => { CultureInfo ci = func1(); return (ci.Name, ci); };
        }
        /// <summary></summary>
        public Func(Func<string> func2)
        {
            // Assert not null
            if (func2 == null) throw new ArgumentNullException(nameof(func2));
            // Adapt func
            this.func = () => { CultureInfo ci = CultureInfo.GetCultureInfo(func2()); return (ci.Name, ci); };
        }
        /// <summary></summary>
        public Func(Func<(string Language, IFormatProvider Format)> func)
        {
            this.func = func ?? throw new ArgumentNullException(nameof(func));
        }
        /// <summary></summary>
        public override string ToString() => $"({Culture},{Format})";
    }

    /// <summary>Provides <![CDATA[((CultureInfo.CurrentUICulture.Name, CultureInfo.CurrentCulture))]]></summary>
    public class CurrentCulture : ICultureProvider
    {
        /// <summary></summary>
        static ICultureProvider? instance;
        /// <summary></summary>
        public static ICultureProvider Instance => instance ??= ByTypeCached[typeof(CurrentCulture)];
        /// <summary></summary>
        public string Culture { get => CultureInfo.CurrentUICulture.Name; set => CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(value); }
        /// <summary></summary>
        public IFormatProvider Format { get => CultureInfo.CurrentCulture; set => CultureInfo.CurrentCulture = (CultureInfo)value; }
        /// <summary></summary>
        public override string ToString() => $"CurrentCulture({Culture},{Format})";
    }

    /// <summary>Provides <![CDATA[(Thread.CurrentThread.CurrentUICulture.Name, Thread.CurrentThread.CurrentCulture)]]></summary>
    public class CurrentThread : ICultureProvider
    {
        /// <summary></summary>
        static ICultureProvider? instance;
        /// <summary></summary>
        public static ICultureProvider Instance => instance ??= ByTypeCached[typeof(CurrentThread)];
        /// <summary></summary>
        public string Culture { get => Thread.CurrentThread.CurrentUICulture.Name; set => Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(value); }
        /// <summary></summary>
        public IFormatProvider Format { get => Thread.CurrentThread.CurrentCulture; set => Thread.CurrentThread.CurrentCulture = (CultureInfo)value; }
        /// <summary></summary>
        public override string ToString() => $"CurrentThread({Culture},{Format})";
    }

    /// <summary>Provides </summary>
    public class DefaultThread : ICultureProvider
    {
        /// <summary></summary>
        static ICultureProvider? instance;
        /// <summary></summary>
        public static ICultureProvider Instance => instance ??= ByTypeCached[typeof(DefaultThread)];
        /// <summary></summary>
        public string Culture { get => (CultureInfo.DefaultThreadCurrentUICulture ?? CultureInfo.CurrentUICulture).Name; set => CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(value); }
        /// <summary></summary>
        public IFormatProvider Format { get => CultureInfo.DefaultThreadCurrentCulture ?? CultureInfo.CurrentCulture; set => CultureInfo.DefaultThreadCurrentCulture = (CultureInfo) value; }
        /// <summary></summary>
        public override string ToString() => $"DefaultThread({Culture},{Format})";
    }

    /// <summary></summary>
    public class InstalledCulture : ICultureProvider
    {
        /// <summary></summary>
        static ICultureProvider? instance;
        /// <summary></summary>
        public static ICultureProvider Instance => instance ??= ByTypeCached[typeof(InstalledCulture)];
        /// <summary></summary>
        public string Culture { get => CultureInfo.InstalledUICulture.Name; set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public IFormatProvider Format { get => CultureInfo.InvariantCulture; set => throw new InvalidOperationException(); }
        /// <summary></summary>
        public override string ToString() => $"InstalledCulture({Culture},{Format})";
    }
}
